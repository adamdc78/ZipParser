using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipParser.Model;

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

      using (var fileStream = new FileStream(filename, FileMode.Open))
      using (var binaryReader = new BinaryReader(fileStream))
      {
        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
        {
          var localFileHeader = new LocalFileHeader(binaryReader);
          Console.WriteLine($"{localFileHeader.GetFilename()}\t{string.Empty}\t{BitConverter.ToInt32(localFileHeader.UncompressedSize, 0)}\t{string.Empty}\t{string.Empty}");
          var compressedSize = BitConverter.ToInt32(localFileHeader.CompressedSize, 0);
          if (compressedSize > 0)
          {
            binaryReader.ReadBytes(compressedSize);
          }
        }
      }

      Console.WriteLine("Press any key to exit");
      Console.ReadKey();
    }
  }
}
