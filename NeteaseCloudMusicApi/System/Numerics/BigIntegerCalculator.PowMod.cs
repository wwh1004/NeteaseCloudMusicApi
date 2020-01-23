// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Numerics {
	internal static partial class BigIntegerCalculator {
		// Executes different exponentiation algorithms, which are
		// based on the classic square-and-multiply method.

		// https://en.wikipedia.org/wiki/Exponentiation_by_squaring

		public static uint Pow(uint value, uint power, uint modulus) {
			// The 32-bit modulus pow method for a 32-bit integer
			// raised by a 32-bit integer...

			return PowCore(power, modulus, value, 1);
		}

		public static uint Pow(uint[] value, uint power, uint modulus) {
			Debug.Assert(value != null);

			// The 32-bit modulus pow method for a big integer
			// raised by a 32-bit integer...

			uint v = Remainder(value, modulus);
			return PowCore(power, modulus, v, 1);
		}

		public static uint Pow(uint value, uint[] power, uint modulus) {
			Debug.Assert(power != null);

			// The 32-bit modulus pow method for a 32-bit integer
			// raised by a big integer...

			return PowCore(power, modulus, value, 1);
		}

		public static uint Pow(uint[] value, uint[] power, uint modulus) {
			Debug.Assert(value != null);
			Debug.Assert(power != null);

			// The 32-bit modulus pow method for a big integer
			// raised by a big integer...

			uint v = Remainder(value, modulus);
			return PowCore(power, modulus, v, 1);
		}

		private static uint PowCore(uint[] power, uint modulus,
									ulong value, ulong result) {
			// The 32-bit modulus pow algorithm for all but
			// the last power limb using square-and-multiply.

			for (int i = 0; i < power.Length - 1; i++) {
				uint p = power[i];
				for (int j = 0; j < 32; j++) {
					if ((p & 1) == 1)
						result = result * value % modulus;
					value = value * value % modulus;
					p = p >> 1;
				}
			}

			return PowCore(power[power.Length - 1], modulus, value, result);
		}

		private static uint PowCore(uint power, uint modulus,
									ulong value, ulong result) {
			// The 32-bit modulus pow algorithm for the last or
			// the only power limb using square-and-multiply.

			while (power != 0) {
				if ((power & 1) == 1)
					result = result * value % modulus;
				if (power != 1)
					value = value * value % modulus;
				power = power >> 1;
			}

			return (uint)(result % modulus);
		}

		public static uint[] Pow(uint value, uint power, uint[] modulus) {
			Debug.Assert(modulus != null);

			// The big modulus pow method for a 32-bit integer
			// raised by a 32-bit integer...

			int size = modulus.Length + modulus.Length;
			BitsBuffer v = new BitsBuffer(size, value);
			return PowCore(power, modulus, ref v);
		}

		public static uint[] Pow(uint[] value, uint power, uint[] modulus) {
			Debug.Assert(value != null);
			Debug.Assert(modulus != null);

			// The big modulus pow method for a big integer
			// raised by a 32-bit integer...

			if (value.Length > modulus.Length)
				value = Remainder(value, modulus);

			int size = modulus.Length + modulus.Length;
			BitsBuffer v = new BitsBuffer(size, value);
			return PowCore(power, modulus, ref v);
		}

		public static uint[] Pow(uint value, uint[] power, uint[] modulus) {
			Debug.Assert(power != null);
			Debug.Assert(modulus != null);

			// The big modulus pow method for a 32-bit integer
			// raised by a big integer...

			int size = modulus.Length + modulus.Length;
			BitsBuffer v = new BitsBuffer(size, value);
			return PowCore(power, modulus, ref v);
		}

		public static uint[] Pow(uint[] value, uint[] power, uint[] modulus) {
			Debug.Assert(value != null);
			Debug.Assert(power != null);
			Debug.Assert(modulus != null);

			// The big modulus pow method for a big integer
			// raised by a big integer...

			if (value.Length > modulus.Length)
				value = Remainder(value, modulus);

			int size = modulus.Length + modulus.Length;
			BitsBuffer v = new BitsBuffer(size, value);
			return PowCore(power, modulus, ref v);
		}

		// Mutable for unit testing...
		private static readonly int ReducerThreshold = 32;

		private static uint[] PowCore(uint[] power, uint[] modulus,
									  ref BitsBuffer value) {
			// Executes the big pow algorithm.

			int size = value.GetSize();

			BitsBuffer temp = new BitsBuffer(size, 0);
			BitsBuffer result = new BitsBuffer(size, 1);

			if (modulus.Length < ReducerThreshold) {
				PowCore(power, modulus, ref value, ref result, ref temp);
			}
			else {
				FastReducer reducer = new FastReducer(modulus);
				PowCore(power, ref reducer, ref value, ref result, ref temp);
			}

			return result.GetBits();
		}

		private static uint[] PowCore(uint power, uint[] modulus,
									  ref BitsBuffer value) {
			// Executes the big pow algorithm.

			int size = value.GetSize();

			BitsBuffer temp = new BitsBuffer(size, 0);
			BitsBuffer result = new BitsBuffer(size, 1);

			if (modulus.Length < ReducerThreshold) {
				PowCore(power, modulus, ref value, ref result, ref temp);
			}
			else {
				FastReducer reducer = new FastReducer(modulus);
				PowCore(power, ref reducer, ref value, ref result, ref temp);
			}

			return result.GetBits();
		}

		private static void PowCore(uint[] power, uint[] modulus,
									ref BitsBuffer value, ref BitsBuffer result,
									ref BitsBuffer temp) {
			// The big modulus pow algorithm for all but
			// the last power limb using square-and-multiply.

			// NOTE: we're using an ordinary remainder here,
			// since the reducer overhead doesn't pay off.

			for (int i = 0; i < power.Length - 1; i++) {
				uint p = power[i];
				for (int j = 0; j < 32; j++) {
					if ((p & 1) == 1) {
						result.MultiplySelf(ref value, ref temp);
						result.Reduce(modulus);
					}
					value.SquareSelf(ref temp);
					value.Reduce(modulus);
					p = p >> 1;
				}
			}

			PowCore(power[power.Length - 1], modulus, ref value, ref result,
				ref temp);
		}

		private static void PowCore(uint power, uint[] modulus,
									ref BitsBuffer value, ref BitsBuffer result,
									ref BitsBuffer temp) {
			// The big modulus pow algorithm for the last or
			// the only power limb using square-and-multiply.

			// NOTE: we're using an ordinary remainder here,
			// since the reducer overhead doesn't pay off.

			while (power != 0) {
				if ((power & 1) == 1) {
					result.MultiplySelf(ref value, ref temp);
					result.Reduce(modulus);
				}
				if (power != 1) {
					value.SquareSelf(ref temp);
					value.Reduce(modulus);
				}
				power = power >> 1;
			}
		}

		private static void PowCore(uint[] power, ref FastReducer reducer,
									ref BitsBuffer value, ref BitsBuffer result,
									ref BitsBuffer temp) {
			// The big modulus pow algorithm for all but
			// the last power limb using square-and-multiply.

			// NOTE: we're using a special reducer here,
			// since it's additional overhead does pay off.

			for (int i = 0; i < power.Length - 1; i++) {
				uint p = power[i];
				for (int j = 0; j < 32; j++) {
					if ((p & 1) == 1) {
						result.MultiplySelf(ref value, ref temp);
						result.Reduce(ref reducer);
					}
					value.SquareSelf(ref temp);
					value.Reduce(ref reducer);
					p = p >> 1;
				}
			}

			PowCore(power[power.Length - 1], ref reducer, ref value, ref result,
				ref temp);
		}

		private static void PowCore(uint power, ref FastReducer reducer,
									ref BitsBuffer value, ref BitsBuffer result,
									ref BitsBuffer temp) {
			// The big modulus pow algorithm for the last or
			// the only power limb using square-and-multiply.

			// NOTE: we're using a special reducer here,
			// since it's additional overhead does pay off.

			while (power != 0) {
				if ((power & 1) == 1) {
					result.MultiplySelf(ref value, ref temp);
					result.Reduce(ref reducer);
				}
				if (power != 1) {
					value.SquareSelf(ref temp);
					value.Reduce(ref reducer);
				}
				power = power >> 1;
			}
		}

		private static int ActualLength(uint[] value) {
			// Since we're reusing memory here, the actual length
			// of a given value may be less then the array's length

			return ActualLength(value, value.Length);
		}

		private static int ActualLength(uint[] value, int length) {
			Debug.Assert(value != null);
			Debug.Assert(length <= value.Length);

			while (length > 0 && value[length - 1] == 0)
				--length;
			return length;
		}
	}
}
