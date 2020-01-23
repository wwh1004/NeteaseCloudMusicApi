// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Numerics {
	internal readonly struct BigInteger {
		private const uint kuMaskHighBit = unchecked((uint)int.MinValue);
		private const int kcbitUint = 32;

		internal readonly int _sign; // Do not rename (binary serialization)
		internal readonly uint[] _bits; // Do not rename (binary serialization)

		private static readonly BigInteger s_bnMinInt = new BigInteger(-1, new uint[] { kuMaskHighBit });
		private static readonly BigInteger s_bnZeroInt = new BigInteger(0);
		private static readonly BigInteger s_bnMinusOneInt = new BigInteger(-1);

		public BigInteger(int value) {
			if (value == int.MinValue)
				this = s_bnMinInt;
			else {
				_sign = value;
				_bits = null;
			}
			AssertValid();
		}

		public BigInteger(long value) {
			if (int.MinValue < value && value <= int.MaxValue) {
				_sign = (int)value;
				_bits = null;
			}
			else if (value == int.MinValue) {
				this = s_bnMinInt;
			}
			else {
				ulong x = 0;
				if (value < 0) {
					x = unchecked((ulong)-value);
					_sign = -1;
				}
				else {
					x = (ulong)value;
					_sign = +1;
				}

				if (x <= uint.MaxValue) {
					_bits = new uint[1];
					_bits[0] = (uint)x;
				}
				else {
					_bits = new uint[2];
					_bits[0] = unchecked((uint)x);
					_bits[1] = (uint)(x >> kcbitUint);
				}
			}

			AssertValid();
		}

		public BigInteger(ReadOnlySpan<byte> value, bool isUnsigned = false, bool isBigEndian = false) {
			int byteCount = value.Length;

			bool isNegative;
			if (byteCount > 0) {
				byte mostSignificantByte = isBigEndian ? value[0] : value[byteCount - 1];
				isNegative = (mostSignificantByte & 0x80) != 0 && !isUnsigned;

				if (mostSignificantByte == 0) {
					// Try to conserve space as much as possible by checking for wasted leading byte[] entries
					if (isBigEndian) {
						int offset = 1;

						while (offset < byteCount && value[offset] == 0) {
							offset++;
						}

						value = value.Slice(offset);
						byteCount = value.Length;
					}
					else {
						byteCount -= 2;

						while (byteCount >= 0 && value[byteCount] == 0) {
							byteCount--;
						}

						byteCount++;
					}
				}
			}
			else {
				isNegative = false;
			}

			if (byteCount == 0) {
				// BigInteger.Zero
				_sign = 0;
				_bits = null;
				AssertValid();
				return;
			}

			if (byteCount <= 4) {
				_sign = isNegative ? unchecked((int)0xffffffff) : 0;

				if (isBigEndian) {
					for (int i = 0; i < byteCount; i++) {
						_sign = (_sign << 8) | value[i];
					}
				}
				else {
					for (int i = byteCount - 1; i >= 0; i--) {
						_sign = (_sign << 8) | value[i];
					}
				}

				_bits = null;
				if (_sign < 0 && !isNegative) {
					// Int32 overflow
					// Example: Int64 value 2362232011 (0xCB, 0xCC, 0xCC, 0x8C, 0x0)
					// can be naively packed into 4 bytes (due to the leading 0x0)
					// it overflows into the int32 sign bit
					_bits = new uint[1] { unchecked((uint)_sign) };
					_sign = +1;
				}
				if (_sign == int.MinValue) {
					this = s_bnMinInt;
				}
			}
			else {
				int unalignedBytes = byteCount % 4;
				int dwordCount = byteCount / 4 + (unalignedBytes == 0 ? 0 : 1);
				uint[] val = new uint[dwordCount];
				int byteCountMinus1 = byteCount - 1;

				// Copy all dwords, except don't do the last one if it's not a full four bytes
				int curDword, curByte;

				if (isBigEndian) {
					curByte = byteCount - sizeof(int);
					for (curDword = 0; curDword < dwordCount - (unalignedBytes == 0 ? 0 : 1); curDword++) {
						for (int byteInDword = 0; byteInDword < 4; byteInDword++) {
							byte curByteValue = value[curByte];
							val[curDword] = (val[curDword] << 8) | curByteValue;
							curByte++;
						}

						curByte -= 8;
					}
				}
				else {
					curByte = sizeof(int) - 1;
					for (curDword = 0; curDword < dwordCount - (unalignedBytes == 0 ? 0 : 1); curDword++) {
						for (int byteInDword = 0; byteInDword < 4; byteInDword++) {
							byte curByteValue = value[curByte];
							val[curDword] = (val[curDword] << 8) | curByteValue;
							curByte--;
						}

						curByte += 8;
					}
				}

				// Copy the last dword specially if it's not aligned
				if (unalignedBytes != 0) {
					if (isNegative) {
						val[dwordCount - 1] = 0xffffffff;
					}

					if (isBigEndian) {
						for (curByte = 0; curByte < unalignedBytes; curByte++) {
							byte curByteValue = value[curByte];
							val[curDword] = (val[curDword] << 8) | curByteValue;
						}
					}
					else {
						for (curByte = byteCountMinus1; curByte >= byteCount - unalignedBytes; curByte--) {
							byte curByteValue = value[curByte];
							val[curDword] = (val[curDword] << 8) | curByteValue;
						}
					}
				}

				if (isNegative) {
					NumericsHelpers.DangerousMakeTwosComplement(val); // Mutates val

					// Pack _bits to remove any wasted space after the twos complement
					int len = val.Length - 1;
					while (len >= 0 && val[len] == 0) len--;
					len++;

					if (len == 1) {
						switch (val[0]) {
						case 1: // abs(-1)
							this = s_bnMinusOneInt;
							return;

						case kuMaskHighBit: // abs(Int32.MinValue)
							this = s_bnMinInt;
							return;

						default:
							if (unchecked((int)val[0]) > 0) {
								_sign = (-1) * ((int)val[0]);
								_bits = null;
								AssertValid();
								return;
							}

							break;
						}
					}

					if (len != val.Length) {
						_sign = -1;
						_bits = new uint[len];
						Array.Copy(val, 0, _bits, 0, len);
					}
					else {
						_sign = -1;
						_bits = val;
					}
				}
				else {
					_sign = +1;
					_bits = val;
				}
			}
			AssertValid();
		}

		internal BigInteger(int n, uint[] rgu) {
			_sign = n;
			_bits = rgu;
			AssertValid();
		}

		internal BigInteger(uint[] value, bool negative) {
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			int len;

			// Try to conserve space as much as possible by checking for wasted leading uint[] entries
			// sometimes the uint[] has leading zeros from bit manipulation operations & and ^
			for (len = value.Length; len > 0 && value[len - 1] == 0; len--) ;

			if (len == 0)
				this = s_bnZeroInt;
			// Values like (Int32.MaxValue+1) are stored as "0x80000000" and as such cannot be packed into _sign
			else if (len == 1 && value[0] < kuMaskHighBit) {
				_sign = negative ? -(int)value[0] : (int)value[0];
				_bits = null;
				// Although Int32.MinValue fits in _sign, we represent this case differently for negate
				if (_sign == int.MinValue)
					this = s_bnMinInt;
			}
			else {
				_sign = negative ? -1 : +1;
				_bits = new uint[len];
				Array.Copy(value, 0, _bits, 0, len);
			}
			AssertValid();
		}

		public bool IsEven { get { AssertValid(); return _bits == null ? (_sign & 1) == 0 : (_bits[0] & 1) == 0; } }

		public int Sign {
			get { AssertValid(); return (_sign >> (kcbitUint - 1)) - (-_sign >> (kcbitUint - 1)); }
		}

		public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus) {
			if (exponent.Sign < 0)
				throw new ArgumentOutOfRangeException(nameof(exponent));

			value.AssertValid();
			exponent.AssertValid();
			modulus.AssertValid();

			bool trivialValue = value._bits == null;
			bool trivialExponent = exponent._bits == null;
			bool trivialModulus = modulus._bits == null;

			if (trivialModulus) {
				uint bits = trivialValue && trivialExponent ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent._sign), NumericsHelpers.Abs(modulus._sign)) :
							trivialValue ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), exponent._bits, NumericsHelpers.Abs(modulus._sign)) :
							trivialExponent ? BigIntegerCalculator.Pow(value._bits, NumericsHelpers.Abs(exponent._sign), NumericsHelpers.Abs(modulus._sign)) :
							BigIntegerCalculator.Pow(value._bits, exponent._bits, NumericsHelpers.Abs(modulus._sign));

				return value._sign < 0 && !exponent.IsEven ? -1 * bits : bits;
			}
			else {
				uint[] bits = trivialValue && trivialExponent ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent._sign), modulus._bits) :
							  trivialValue ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), exponent._bits, modulus._bits) :
							  trivialExponent ? BigIntegerCalculator.Pow(value._bits, NumericsHelpers.Abs(exponent._sign), modulus._bits) :
							  BigIntegerCalculator.Pow(value._bits, exponent._bits, modulus._bits);

				return new BigInteger(bits, value._sign < 0 && !exponent.IsEven);
			}
		}

		public byte[] ToByteArray(bool isUnsigned = false, bool isBigEndian = false) {
			int ignored = 0;
			return TryGetBytes(GetBytesMode.AllocateArray, default, isUnsigned, isBigEndian, ref ignored);
		}

		private enum GetBytesMode { AllocateArray, Count, Span }

		private static readonly byte[] s_success = Array.Empty<byte>();

		private byte[] TryGetBytes(GetBytesMode mode, Span<byte> destination, bool isUnsigned, bool isBigEndian, ref int bytesWritten) {
			Debug.Assert(mode == GetBytesMode.AllocateArray || mode == GetBytesMode.Count || mode == GetBytesMode.Span, $"Unexpected mode {mode}.");
			Debug.Assert(mode == GetBytesMode.Span || destination.IsEmpty, $"If we're not in span mode, we shouldn't have been passed a destination.");

			int sign = _sign;
			if (sign == 0) {
				switch (mode) {
				case GetBytesMode.AllocateArray:
					return new byte[] { 0 };
				case GetBytesMode.Count:
					bytesWritten = 1;
					return null;
				default: // case GetBytesMode.Span:
					bytesWritten = 1;
					if (destination.Length != 0) {
						destination[0] = 0;
						return s_success;
					}
					return null;
				}
			}

			if (isUnsigned && sign < 0) {
				throw new OverflowException();
			}

			byte highByte;
			int nonZeroDwordIndex = 0;
			uint highDword;
			uint[] bits = _bits;
			if (bits == null) {
				highByte = (byte)((sign < 0) ? 0xff : 0x00);
				highDword = unchecked((uint)sign);
			}
			else if (sign == -1) {
				highByte = 0xff;

				// If sign is -1, we will need to two's complement bits.
				// Previously this was accomplished via NumericsHelpers.DangerousMakeTwosComplement(),
				// however, we can do the two's complement on the stack so as to avoid
				// creating a temporary copy of bits just to hold the two's complement.
				// One special case in DangerousMakeTwosComplement() is that if the array
				// is all zeros, then it would allocate a new array with the high-order
				// uint set to 1 (for the carry). In our usage, we will not hit this case
				// because a bits array of all zeros would represent 0, and this case
				// would be encoded as _bits = null and _sign = 0.
				Debug.Assert(bits.Length > 0);
				Debug.Assert(bits[bits.Length - 1] != 0);
				while (bits[nonZeroDwordIndex] == 0U) {
					nonZeroDwordIndex++;
				}

				highDword = ~bits[bits.Length - 1];
				if (bits.Length - 1 == nonZeroDwordIndex) {
					// This will not overflow because highDword is less than or equal to uint.MaxValue - 1.
					Debug.Assert(highDword <= uint.MaxValue - 1);
					highDword += 1U;
				}
			}
			else {
				Debug.Assert(sign == 1);
				highByte = 0x00;
				highDword = bits[bits.Length - 1];
			}

			byte msb;
			int msbIndex;
			if ((msb = unchecked((byte)(highDword >> 24))) != highByte) {
				msbIndex = 3;
			}
			else if ((msb = unchecked((byte)(highDword >> 16))) != highByte) {
				msbIndex = 2;
			}
			else if ((msb = unchecked((byte)(highDword >> 8))) != highByte) {
				msbIndex = 1;
			}
			else {
				msb = unchecked((byte)highDword);
				msbIndex = 0;
			}

			// Ensure high bit is 0 if positive, 1 if negative
			bool needExtraByte = (msb & 0x80) != (highByte & 0x80) && !isUnsigned;
			int length = msbIndex + 1 + (needExtraByte ? 1 : 0);
			if (bits != null) {
				length = checked(4 * (bits.Length - 1) + length);
			}

			byte[] array;
			switch (mode) {
			case GetBytesMode.AllocateArray:
				destination = array = new byte[length];
				break;
			case GetBytesMode.Count:
				bytesWritten = length;
				return null;
			default: // case GetBytesMode.Span:
				bytesWritten = length;
				if (destination.Length < length) {
					return null;
				}
				array = s_success;
				break;
			}

			int curByte = isBigEndian ? length - 1 : 0;
			int increment = isBigEndian ? -1 : 1;

			if (bits != null) {
				for (int i = 0; i < bits.Length - 1; i++) {
					uint dword = bits[i];

					if (sign == -1) {
						dword = ~dword;
						if (i <= nonZeroDwordIndex) {
							dword = unchecked(dword + 1U);
						}
					}

					destination[curByte] = unchecked((byte)dword);
					curByte += increment;
					destination[curByte] = unchecked((byte)(dword >> 8));
					curByte += increment;
					destination[curByte] = unchecked((byte)(dword >> 16));
					curByte += increment;
					destination[curByte] = unchecked((byte)(dword >> 24));
					curByte += increment;
				}
			}

			Debug.Assert(msbIndex >= 0 && msbIndex <= 3);
			destination[curByte] = unchecked((byte)highDword);
			if (msbIndex != 0) {
				curByte += increment;
				destination[curByte] = unchecked((byte)(highDword >> 8));
				if (msbIndex != 1) {
					curByte += increment;
					destination[curByte] = unchecked((byte)(highDword >> 16));
					if (msbIndex != 2) {
						curByte += increment;
						destination[curByte] = unchecked((byte)(highDword >> 24));
					}
				}
			}

			// Assert we're big endian, or little endian consistency holds.
			Debug.Assert(isBigEndian || (!needExtraByte && curByte == length - 1) || (needExtraByte && curByte == length - 2));
			// Assert we're little endian, or big endian consistency holds.
			Debug.Assert(!isBigEndian || (!needExtraByte && curByte == 0) || (needExtraByte && curByte == 1));

			if (needExtraByte) {
				curByte += increment;
				destination[curByte] = highByte;
			}

			return array;
		}

		public static implicit operator BigInteger(long value) {
			return new BigInteger(value);
		}

		[Conditional("DEBUG")]
		private void AssertValid() {
			if (_bits != null) {
				// _sign must be +1 or -1 when _bits is non-null
				Debug.Assert(_sign == 1 || _sign == -1);
				// _bits must contain at least 1 element or be null
				Debug.Assert(_bits.Length > 0);
				// Wasted space: _bits[0] could have been packed into _sign
				Debug.Assert(_bits.Length > 1 || _bits[0] >= kuMaskHighBit);
				// Wasted space: leading zeros could have been truncated
				Debug.Assert(_bits[_bits.Length - 1] != 0);
			}
			else {
				// Int32.MinValue should not be stored in the _sign field
				Debug.Assert(_sign > int.MinValue);
			}
		}
	}
}
