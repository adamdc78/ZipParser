using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipParser.Utility
{
  internal class BinaryUtilities
  {
    internal static bool IsBitSet(byte b, short position)
    {
      return (b & (1 << position)) != 0;
    }
  }
}
