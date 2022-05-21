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
    static void Main(string[] args)
    {
      var cwd = AppContext.BaseDirectory;
      var filename = Path.Combine(cwd, args[0]);

      if (!File.Exists(filename))
        Console.WriteLine($"Unable to locate '{filename}'");

      using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      using (var binaryReader = new BinaryReader(fileStream))
      {
        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
        {
          var signature = binaryReader.ReadBytes(4);

          if (BinaryUtilities.AreByteArraysEqual(signature, LocalFileHeader.Signature))
          {
            var localFileHeader = new LocalFileHeader(binaryReader);
            var compressedSize = BitConverter.ToInt32(localFileHeader.CompressedSize, 0);
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
            Console.WriteLine($"{centralDirectoryFileHeader.FileName}\t{(centralDirectoryFileHeader.IsDirectory ? "True" : "False")}\t{centralDirectoryFileHeader.UncompressedSize}\t{centralDirectoryFileHeader.LastModifiedDateTime.ToString("yyyy-MM-ddThh:mm:ss")}\t{centralDirectoryFileHeader.FileComment}");
          }
          else if (BinaryUtilities.AreByteArraysEqual(signature, EndOfCentralDirectoryLocator.Signature))
          {
            _ = new EndOfCentralDirectoryLocator(binaryReader);
          }
          else if (BinaryUtilities.AreByteArraysEqual(signature, EndOfCentralDirectoryRecord.Signature))
          {
            _ = new EndOfCentralDirectoryRecord(binaryReader);
          }
        }
      }

      Console.WriteLine("Press any key to exit");
      Console.ReadKey();
    }
  }
}
