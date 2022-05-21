using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZipParser.Utility;

[assembly: InternalsVisibleTo("ZipparserUnitTests")]
namespace ZipParser.Model
{
  /// <summary>
  /// Not necessary for the final implementation, but was used in the naive implementation
  /// </summary>
  internal class LocalFileHeader
  {
    internal enum Flags
    {
      EncryptedFile = 0,
      CompressionOption1,
      CompressionOption2,
      DataDescriptor,
      EnhancedDeflation,
      CompressedPatchedData,
      StrongEncryption,
      Unused1,
      Unused2,
      Unused3,
      Unused4,
      LanguageEncoding,
      Reserved1,
      MaskHeaderValues,
      Reserved2,
      Reserved3,
    }

    internal enum Compression
    {
      NoCompression = 0,
      Shrunk,
      ReducedCompressionFactor1,
      ReducedCompressionFactor2,
      ReducedCompressionFactor3,
      ReducedCompressionFactor4,
      Imploded,
      Reserved1,
      Deflated,
      EnhancedDeflated,
      PKWareDCLImploded,
      Reserved2,
      CompressedBZIP2,
      Reserved3,
      LZMA,
      Reserved4,
      Reserved5,
      Reserved6,
      CompressedIMBTERSE,
      IMBLZ77z,
      PPMdvIR1 = 98,
    }

    static internal readonly byte[] Signature = BitConverter.GetBytes(0x04034b50);

    internal short VersionNeededToExtract { get; private set; }
    internal short GeneralPurposeBitFlag { get; private set; }
    internal short CompressionMethod { get; private set; }
    internal short LastModFileTime { get; private set; }
    internal short LastModFileDate { get; private set; }
    internal int CRC32 { get; private set; }
    internal int CompressedSize { get; private set; }
    internal int UncompressedSize { get; private set; }
    internal short FileNameLength { get; private set; }
    internal short ExtraFieldLength { get; private set; }
    internal string FileName { get; private set; }
    internal string ExtraField { get; private set; }

    internal bool HasDataDescriptor { get { return BinaryUtilities.IsBitSet(BitConverter.GetBytes(GeneralPurposeBitFlag), (int)Flags.DataDescriptor); } }

    /// <summary>
    /// Constructs an instance by invoking ReadFromStream using the binaryReader
    /// </summary>
    /// <param name="binaryReader">The reader used to perform ReadFromStream</param>
    internal LocalFileHeader(BinaryReader reader)
    {
      ReadFromStream(reader);
    }

    internal LocalFileHeader()
    {
    }

    /// <summary>
    /// Reads the record (starting after the signature) from the stream.
    /// </summary>
    /// <param name="binaryReader">The binary reader used to read the underlying stream</param>
    /// <returns>true if the operation is successful</returns>
    internal bool ReadFromStream(BinaryReader reader)
    {
      var success = false;

      try
      {
        var data = reader.ReadBytes(26);
        VersionNeededToExtract = BitConverter.ToInt16(data, 0);
        GeneralPurposeBitFlag = BitConverter.ToInt16(data, 2);
        CompressionMethod = BitConverter.ToInt16(data, 4);
        LastModFileTime = BitConverter.ToInt16(data, 6);
        LastModFileDate = BitConverter.ToInt16(data, 8);
        CRC32 = BitConverter.ToInt32(data, 10);
        CompressedSize = BitConverter.ToInt32(data, 14);
        UncompressedSize = BitConverter.ToInt32(data, 18);
        FileNameLength = BitConverter.ToInt16(data, 22);
        ExtraFieldLength = BitConverter.ToInt16(data, 24);

        FileName = Encoding.UTF8.GetString(reader.ReadBytes(FileNameLength));
        if (ExtraFieldLength > 0)
          ExtraField = Encoding.UTF8.GetString(reader.ReadBytes(ExtraFieldLength));

        success = true;
      }
      catch (Exception e)
      {
        Console.WriteLine($"Unable to read zip local file header: {e.Message}");
        Console.WriteLine(e.StackTrace);
      }

      return success;
    }
  }
}
