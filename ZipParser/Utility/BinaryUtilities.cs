using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipParser.Utility
{
  internal class BinaryUtilities
  {
    /// <summary>
    /// Compares two byte arrays for equality via brute force
    /// </summary>
    /// <param name="a">The first byte array</param>
    /// <param name="b">The second byte array</param>
    /// <returns></returns>
    internal static bool AreByteArraysEqual(byte[] a, byte[] b)
    {
      if (a == null && b == null) return true;
      if ((a == null && b != null) || (a != null && b == null)) return false;

      if (a.Length != b.Length) return false;

      for (int i = 0; i < a.Length; i++)
      {
        if (a[i] != b[i]) return false;
      }

      return true;
    }

    /// <summary>
    /// Shamelesly borrowed from https://codereview.stackexchange.com/questions/3796/converting-binary-value-from-bitarray-to-an-int-and-back-in-c/3797#3797
    /// 
    /// Converts a BitArray to a 32-bit Integer
    /// </summary>
    /// <param name="binary">The bit array to be converted to 32-bit int</param>
    /// <returns>The integer value represented by 'binary'</returns>
    /// <exception cref="ArgumentNullException">Thrown when 'binary' is null</exception>
    /// <exception cref="ArgumentException">Thrown when 'binary' exceeds 32-bits</exception>
    internal static int BitArrayToInt(BitArray binary)
    {
      if (binary == null)
        throw new ArgumentNullException("Cannot convert null.");

      if (binary.Length > 32)
        throw new ArgumentException("Cannot convert more than 32 bits to int");

      var result = new int[1];
      binary.CopyTo(result, 0);
      return result[0];
    }

    /// <summary>
    /// Shamelessly borrowed from https://codereview.stackexchange.com/questions/3796/converting-binary-value-from-bitarray-to-an-int-and-back-in-c/3797#3797
    /// 
    /// Converts a 32-bit Integer to a BitArray
    /// </summary>
    /// <param name="integer">The integer value to be converted to a BitArray</param>
    /// <returns>The BitArray representing 'integer'</returns>
    internal static BitArray BitArrayFromInt(int integer)
    {
      return new BitArray(new[] { integer });
    }

    /// <summary>
    /// Determines whether a bit is set within a byte array
    /// </summary>
    /// <param name="bytes">The array of bytes</param>
    /// <param name="position">The position within the array of bytes</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    internal static bool IsBitSet(byte[] bytes, int position)
    {
      if (bytes == null)
        throw new ArgumentNullException("Cannot check whether a bit is set in null.");

      var byteIndex = position / 8;
      var bytePosition = (short)(position % 8);

      if (byteIndex > bytes.Length)
        throw new ArgumentException($"Position out of bounds; position {position} for {bytes.Length} bytes.");

      return IsBitSet(bytes[byteIndex], bytePosition);
    }

    /// <summary>
    /// Shamelessly borrowed from https://stackoverflow.com/questions/2431732/checking-if-a-bit-is-set-or-not
    /// 
    /// Determines whehter a bit is set within a byte
    /// </summary>
    /// <param name="b"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static bool IsBitSet(byte b, short position)
    {
      return (b & (1 << position)) != 0;
    }
  }
}
