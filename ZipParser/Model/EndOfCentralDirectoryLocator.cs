using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipParser.Model
{
  internal class EndOfCentralDirectoryLocator
  {
    internal static readonly byte[] Signature = BitConverter.GetBytes(0x07064b50);
    internal int NumberOfDisksWithStartOfEndOfCentralDirecotry { get; private set; }
    internal long RelativeOffsetOfEndOfCentralDirectoryRecord { get; private set; }
    internal int TotalNumberOfDisks { get; private set; }

    internal EndOfCentralDirectoryLocator(BinaryReader reader)
    {
      ReadFromStream(reader);
    }

    internal EndOfCentralDirectoryLocator()
    {
    }

    internal bool ReadFromStream(BinaryReader reader)
    {
      var success = true;

      try
      {
        var data = reader.ReadBytes(16);
        NumberOfDisksWithStartOfEndOfCentralDirecotry = BitConverter.ToInt32(data, 0);
        RelativeOffsetOfEndOfCentralDirectoryRecord = BitConverter.ToInt64(data, 4);
        TotalNumberOfDisks = BitConverter.ToInt32(data, 12);
      }
      catch (Exception e)
      {
        success = false;
        Console.WriteLine($"Unable to read EndOfCentralDirectoryLocator: {e.Message}");
      }

      return success;
    }
  }
}
