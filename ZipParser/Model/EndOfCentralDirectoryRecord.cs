using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipParser.Model
{
  internal class EndOfCentralDirectoryRecord
  {
    static internal readonly byte[] Signature = BitConverter.GetBytes(0x06054b50);

    internal short DiskNumber { get; private set; }
    internal short DiskNumberWithCentralDirectory { get; private set; }
    internal short DiskEntries { get; private set; }
    internal short TotalEntries { get; private set; }
    internal int CentralDirectorySize { get; private set; }
    internal int OffsetOfCentralDirectoryOnStartingDisk { get; private set; }
    internal short CommentLength { get; private set; }
    internal string ZipFileComment { get; private set; }

    internal EndOfCentralDirectoryRecord(BinaryReader reader)
    {
      ReadFromStream(reader);
    }

    internal EndOfCentralDirectoryRecord()
    {
    }

    internal bool ReadFromStream(BinaryReader reader)
    {
      var success = true;

      try
      {
        var data = reader.ReadBytes(18);
        DiskNumber = BitConverter.ToInt16(data, 0);
        DiskNumberWithCentralDirectory = BitConverter.ToInt16(data, 2);
        DiskEntries = BitConverter.ToInt16(data, 4);
        TotalEntries = BitConverter.ToInt16(data, 6);
        CentralDirectorySize = BitConverter.ToInt32(data, 8);
        OffsetOfCentralDirectoryOnStartingDisk = BitConverter.ToInt32(data, 12);
        CommentLength = BitConverter.ToInt16(data, 16);
        ZipFileComment = CommentLength > 0 ? Encoding.UTF8.GetString(reader.ReadBytes(CommentLength)) : null;
      }
      catch (Exception e)
      {
        Console.WriteLine($"Unable to read EndOfCentralDirectoryRecord: {e.Message}");
      }

      return success;
    }
  }
}
