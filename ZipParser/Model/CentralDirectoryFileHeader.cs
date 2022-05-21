using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipParser.Utility;

namespace ZipParser.Model
{
  [Flags]
  enum ExternalFilettributes
  {
    ReadOnly = 0,
    Hidden = 1,
    Directory = 4,
    Archive = 5,
    Normal = 7,
    Compressed = 11,
    Encrypted = 14,
  }

  internal class CentralDirectoryFileHeader
  {
    internal static readonly byte[] Signature = BitConverter.GetBytes(0x02014b50);

    internal short VersionMadeBy { get; private set; }
    internal short VersionNeededToExtract { get; private set; }
    internal short GeneralPurposeBitFlag { get; private set; }
    internal short CompressionMethod { get; private set; }
    internal byte[] LastModFileTime { get; private set; }
    internal byte[] LastModFileDate { get; private set; }
    internal int CRC32 { get; private set; }
    internal int CompressedSize { get; private set; }
    internal int UncompressedSize { get; private set; }
    internal short FileNameLength { get; private set; }
    internal short ExtraFieldLength { get; private set; }
    internal short FileCommentLength { get; private set; }
    internal short DiskNumberStart { get; private set; }
    internal short InternalFileAttributes { get; private set; }
    internal byte[] ExternalFileAttributes { get; private set; }
    internal int RelativeOffsetOfLocalHeader { get; private set; }
    internal string FileName { get; private set; }
    internal string ExtraField { get; private set; }
    internal string FileComment { get; private set; }
    internal bool IsDirectory { get { return BinaryUtilities.IsBitSet(ExternalFileAttributes, (int)ExternalFilettributes.Directory); } }
    internal DateTime LastModifiedDateTime
    {
      get
      {
        var dayBits = new BitArray(5);
        var monthBits = new BitArray(4);
        var yearBits = new BitArray(7);
        var secondBits = new BitArray(5);
        var minuteBits = new BitArray(6);
        var hourBits = new BitArray(5);

        for (int i = 0; i < 4; i++)
        {
          secondBits.Set(i, BinaryUtilities.IsBitSet(LastModFileTime, i));
          minuteBits.Set(i, BinaryUtilities.IsBitSet(LastModFileTime, (short)(i + 5)));
          hourBits.Set(i, BinaryUtilities.IsBitSet(LastModFileTime, (short)(i + 11)));
          dayBits.Set(i, BinaryUtilities.IsBitSet(LastModFileDate, i));
          monthBits.Set(i, BinaryUtilities.IsBitSet(LastModFileDate, (short)(i + 5)));
          yearBits.Set(i, BinaryUtilities.IsBitSet(LastModFileDate, (short)(i + 9)));
        }

        // Not worth the loop overhead for so few values
        secondBits.Set(4, BinaryUtilities.IsBitSet(LastModFileTime, 4));
        minuteBits.Set(4, BinaryUtilities.IsBitSet(LastModFileTime, 9));
        minuteBits.Set(5, BinaryUtilities.IsBitSet(LastModFileTime, 10));
        hourBits.Set(4, BinaryUtilities.IsBitSet(LastModFileTime, 15));
        dayBits.Set(4, BinaryUtilities.IsBitSet(LastModFileDate, 4));
        yearBits.Set(4, BinaryUtilities.IsBitSet(LastModFileDate, 13));
        yearBits.Set(5, BinaryUtilities.IsBitSet(LastModFileDate, 14));
        yearBits.Set(6, BinaryUtilities.IsBitSet(LastModFileDate, 15));

        var seconds = BinaryUtilities.BitArrayToInt(secondBits) * 2;
        var minutes = BinaryUtilities.BitArrayToInt(minuteBits);
        var hours = BinaryUtilities.BitArrayToInt(hourBits);
        var day = BinaryUtilities.BitArrayToInt(dayBits);
        var month = BinaryUtilities.BitArrayToInt(monthBits);
        var year = BinaryUtilities.BitArrayToInt(yearBits);

        return new DateTime(1980 + year, month, day, hours, minutes, seconds);
      }
    }

    internal CentralDirectoryFileHeader(BinaryReader reader)
    {
      ReadFromStream(reader);
    }

    internal CentralDirectoryFileHeader()
    {
    }

    internal bool ReadFromStream(BinaryReader reader)
    {
      var success = true;

      try
      {
        var data = reader.ReadBytes(42);
        VersionMadeBy = BitConverter.ToInt16(data, 0);
        VersionNeededToExtract = BitConverter.ToInt16(data, 2);
        GeneralPurposeBitFlag = BitConverter.ToInt16(data, 4);
        CompressionMethod = BitConverter.ToInt16(data, 6);

        LastModFileTime = new byte[2];
        Array.Copy(data, 8, LastModFileTime, 0, 2);

        LastModFileDate = new byte[2];
        Array.Copy(data, 10, LastModFileDate, 0, 2);

        CRC32 = BitConverter.ToInt32(data, 12);
        CompressedSize = BitConverter.ToInt32(data, 16);
        UncompressedSize = BitConverter.ToInt32(data, 20);
        FileNameLength = BitConverter.ToInt16(data, 24);
        ExtraFieldLength = BitConverter.ToInt16(data, 26);
        FileCommentLength = BitConverter.ToInt16(data, 28);
        DiskNumberStart = BitConverter.ToInt16(data, 30);
        InternalFileAttributes = BitConverter.ToInt16(data, 32);
        
        ExternalFileAttributes = new byte[2];
        Array.Copy(data, 34, ExternalFileAttributes, 0, 2);
        
        RelativeOffsetOfLocalHeader = BitConverter.ToInt32(data, 38);

        FileName = Encoding.UTF8.GetString(reader.ReadBytes(FileNameLength));
        ExtraField = Encoding.UTF8.GetString(reader.ReadBytes(ExtraFieldLength));
        FileComment = Encoding.UTF8.GetString(reader.ReadBytes(FileCommentLength));
      }
      catch (Exception e)
      {
        Console.WriteLine($"Unable to read CentralDirectoryFileHeader: {e.Message}");
        success = false;
      }

      return success;
    }
  }
}
