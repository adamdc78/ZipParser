using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipParser.Model;
using ZipParser.Utility;

namespace ZipParser
{
  internal class Program
  {
    /// <summary>
    /// Prints archive statistics about the specified zip file
    /// </summary>
    /// <param name="args">The only argument accepted is a path to a file</param>
    static void Main(string[] args)
    {
      var cwd = AppContext.BaseDirectory;
      if (args.Length == 0 || args[0] == null)
        Console.WriteLine("Usage: ZipParser.exe <path_to_zip>");

      var filename = Path.Combine(cwd, args[0]);

      if (!File.Exists(filename))
        Console.WriteLine($"Unable to locate '{filename}'");

      using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      using (var binaryReader = new BinaryReader(fileStream))
      {
        // My testing indicates a moderate performance increase for seeking the end of central directory record versus the full archive for this very small zip;
        // Larger archives should see larger performance gains, although I haven't been able to pull together large archives of public-github-safe content with which to test.
        ReadCentralDirectory(binaryReader);
        // ReadAll(binaryReader);
      }

#if DEBUG
      Console.WriteLine("Press any key to exit");
      Console.ReadKey();
#endif
    }

    /// <summary>
    /// Reads only the central directory (note: this implementation will not work with Central Directory Encryption without
    /// accounting for the 'archive decryption header' and 'archive extra data record' within the central directory, nor are
    /// spanned zips support (currently)
    /// </summary>
    /// <param name="binaryReader">The binary reader for the zip file containing the central directory</param>
    static void ReadCentralDirectory(BinaryReader binaryReader)
    {
      var endOfCentralDirectoryRecord = new EndOfCentralDirectoryRecord(binaryReader);

      if (endOfCentralDirectoryRecord.DiskNumber == endOfCentralDirectoryRecord.DiskNumberWithCentralDirectory)
      {
        binaryReader.BaseStream.Seek(endOfCentralDirectoryRecord.OffsetOfCentralDirectoryOnStartingDisk, SeekOrigin.Begin);
        // We're not looking at encrypted archives, so ignore the Archive Decryption Header and Archive Extra Data Records

        while (binaryReader.BaseStream.Position < (endOfCentralDirectoryRecord.OffsetOfCentralDirectoryOnStartingDisk + endOfCentralDirectoryRecord.CentralDirectorySize))
        {
          var signature = binaryReader.ReadBytes(4);
          if (BinaryUtilities.AreByteArraysEqual(signature, CentralDirectoryFileHeader.Signature))
          {
            var centralDirectoryFileHeader = new CentralDirectoryFileHeader(binaryReader);
            // Output the required data to the console
            Console.WriteLine($"{centralDirectoryFileHeader.FileName}\t{(centralDirectoryFileHeader.IsDirectory ? "True" : "False")}\t{centralDirectoryFileHeader.UncompressedSize}\t{centralDirectoryFileHeader.LastModifiedDateTime.ToString("yyyy-MM-ddThh:mm:ss")}\t{centralDirectoryFileHeader.FileComment}");
          }
        }
      }
    }

    /// <summary>
    /// Reads all records in the zip starting at byte 0. Since we're not using the compressed data, this seems excessive, but was my initial
    /// naive implementation
    /// </summary>
    /// <param name="binaryReader"></param>
    static void ReadAll(BinaryReader binaryReader)
    {
      while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
      {
        var signature = binaryReader.ReadBytes(4);

        // Would be nice to convert this to a switch, but that requires changing data types
        if (BinaryUtilities.AreByteArraysEqual(signature, LocalFileHeader.Signature))
        {
          var localFileHeader = new LocalFileHeader(binaryReader);
          var compressedSize = localFileHeader.CompressedSize;
          if (compressedSize > 0)
          {
            binaryReader.ReadBytes(compressedSize);
          }
          if (localFileHeader.HasDataDescriptor)
          {
            binaryReader.ReadBytes(12);
          }
        }
        else if (BinaryUtilities.AreByteArraysEqual(signature, CentralDirectoryFileHeader.Signature))
        {
          var centralDirectoryFileHeader = new CentralDirectoryFileHeader(binaryReader);
          // Output the required data to the console
          Console.WriteLine($"{centralDirectoryFileHeader.FileName}\t{(centralDirectoryFileHeader.IsDirectory ? "True" : "False")}\t{centralDirectoryFileHeader.UncompressedSize}\t{centralDirectoryFileHeader.LastModifiedDateTime.ToString("yyyy-MM-ddThh:mm:ss")}\t{centralDirectoryFileHeader.FileComment}");
        }
        else if (BinaryUtilities.AreByteArraysEqual(signature, EndOfCentralDirectoryLocator.Signature))
        {
          _ = new EndOfCentralDirectoryLocator(binaryReader, false);
        }
        else if (BinaryUtilities.AreByteArraysEqual(signature, EndOfCentralDirectoryRecord.Signature))
        {
          _ = new EndOfCentralDirectoryRecord(binaryReader, false);
        }
      }
    }
  }
}
