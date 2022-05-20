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

    internal byte[] Signature { get; private set; }
    internal byte[] VersionNeededToExtract { get; private set; }
    internal byte[] GeneralPurposeBitFlag { get; private set; }
    internal byte[] CompressionMethod { get; private set; }
    internal byte[] LastModFileTime { get; private set; }
    internal byte[] LastModFileDate { get; private set; }
    internal byte[] CRC32 { get; private set; }
    internal byte[] CompressedSize { get; private set; }
    internal byte[] UncompressedSize { get; private set; }
    internal byte[] FileNameLength { get; private set; }
    internal byte[] ExtraFieldLength { get; private set; }
    internal byte[] FileName { get; private set; }
    internal byte[] ExtraField { get; private set; }

    internal bool HasDataDescriptor { get { return BinaryUtilities.IsBitSet(GeneralPurposeBitFlag[0], (int)Flags.DataDescriptor); } }

    internal LocalFileHeader(BinaryReader reader)
    {
      ReadFromStream(reader);
    }

    internal LocalFileHeader()
    {
    }

    internal bool ReadFromStream(BinaryReader reader)
    {
      var success = false;

      try
      {
        Signature = reader.ReadBytes(4);
        VersionNeededToExtract = reader.ReadBytes(2);
        GeneralPurposeBitFlag = reader.ReadBytes(2);
        CompressionMethod = reader.ReadBytes(2);
        LastModFileTime = reader.ReadBytes(2);
        LastModFileDate = reader.ReadBytes(2);
        CRC32 = reader.ReadBytes(4);
        CompressedSize = reader.ReadBytes(4);
        UncompressedSize = reader.ReadBytes(4);
        FileNameLength = reader.ReadBytes(2);
        ExtraFieldLength = reader.ReadBytes(2);

        var fileNameLength = BitConverter.ToInt16(FileNameLength, 0);
        var extraFieldLength = BitConverter.ToInt16(ExtraFieldLength, 0);

        FileName = reader.ReadBytes(fileNameLength);
        ExtraField = reader.ReadBytes(extraFieldLength);

        success = true;
      }
      catch (Exception e)
      {
        Console.WriteLine($"Unable to read zip local file header: {e.Message}");
      }

      return success;
    }

    internal string GetFilename()
    {
      return Encoding.UTF8.GetString(FileName);
    }
  }
}
