using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipParser.Utility;

namespace ZipParser.Model
{
  internal class EndOfCentralDirectoryRecord
  {
    static internal readonly byte[] Signature = BitConverter.GetBytes(0x06054b50);

    internal const int MinEndOfStreamOffset = 22;
    internal const int MaxEndOfStreamOffset = 278;

    internal short DiskNumber { get; private set; }
    internal short DiskNumberWithCentralDirectory { get; private set; }
    internal short DiskEntries { get; private set; }
    internal short TotalEntries { get; private set; }
    internal int CentralDirectorySize { get; private set; }
    internal int OffsetOfCentralDirectoryOnStartingDisk { get; private set; }
    internal short CommentLength { get; private set; }
    internal string ZipFileComment { get; private set; }

    /// <summary>
    /// Constructs an instance by invoking ReadFromStream using the binaryReader
    /// </summary>
    /// <param name="binaryReader">The reader used to perform ReadFromStream</param>
    /// <param name="seek">Whether to seek for the signature from the end of the stream</param>
    internal EndOfCentralDirectoryRecord(BinaryReader binaryReader, bool seek = true)
    {
      ReadFromStream(binaryReader, seek);
    }

    internal EndOfCentralDirectoryRecord()
    {
    }

    /// <summary>
    /// Reads the record (starting after the signature) from the stream.
    /// </summary>
    /// <param name="binaryReader">The binary reader used to read the underlying stream</param>
    /// <param name="seek">Whether to seek for the signature from the end of the stream</param>
    /// <returns>true if the operation is successful</returns>
    internal bool ReadFromStream(BinaryReader binaryReader, bool seek = true)
    {
      var success = true;

      try
      {
        if (seek)
          Seek(binaryReader);

        var data = binaryReader.ReadBytes(18);
        DiskNumber = BitConverter.ToInt16(data, 0);
        DiskNumberWithCentralDirectory = BitConverter.ToInt16(data, 2);
        DiskEntries = BitConverter.ToInt16(data, 4);
        TotalEntries = BitConverter.ToInt16(data, 6);
        CentralDirectorySize = BitConverter.ToInt32(data, 8);
        OffsetOfCentralDirectoryOnStartingDisk = BitConverter.ToInt32(data, 12);
        CommentLength = BitConverter.ToInt16(data, 16);
        ZipFileComment = CommentLength > 0 ? Encoding.UTF8.GetString(binaryReader.ReadBytes(CommentLength)) : null;
      }
      catch (Exception e)
      {
        Console.WriteLine($"Unable to read EndOfCentralDirectoryRecord: {e.Message}");
      }

      return success;
    }

    /// <summary>
    /// Searches for the signature from the end of the stream
    /// </summary>
    /// <param name="binaryReader">The binary reader which will do the seeking</param>
    /// <returns>The position in the stream at which the signature is found (note: the stream is left with Current at the end of the signature)</returns>
    long Seek(BinaryReader binaryReader)
    {
      var seekOffset = -1 * EndOfCentralDirectoryRecord.MinEndOfStreamOffset;

      try
      {
        while (seekOffset > -MaxEndOfStreamOffset)
        {
          binaryReader.BaseStream.Seek(seekOffset, SeekOrigin.End);

          var firstByte = binaryReader.ReadByte();
          switch (firstByte)
          {
            case 0x50:
              binaryReader.BaseStream.Seek(-1, SeekOrigin.Current);
              var signature = binaryReader.ReadBytes(4);
              if (BinaryUtilities.AreByteArraysEqual(signature, Signature))
              {
                return binaryReader.BaseStream.Position - 4;
              }
              else
              {
                seekOffset -= 8;
              }
              break;
            case 0x4b:
              seekOffset -= 1;
              break;
            case 0x05:
              seekOffset -= 2;
              break;
            case 0x06:
              seekOffset -= 3;
              break;
            default:
              seekOffset -= 4;
              break;
          }
        }

        throw new Exception("Unable to locate EndofCentralDirectoryRecord");
      }
      catch (Exception e)
      {
        Console.WriteLine($"Unexpected exception while seeking EndOfCentralDirectoryRecord: {e.Message}");
        throw;
      }
    }
  }
}
