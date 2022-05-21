using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZipParser.Model;

namespace ZipParserUnitTests.Model
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class LocalFileHeaderTests
  {
    public LocalFileHeaderTests()
    {
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void ReadFromStream()
    {
      var localFileHeader = new LocalFileHeader();
      bool success = false;
      using (var memoryStream = new MemoryStream(Properties.Resources.work_sample_exercise))
      using (var binaryReader = new BinaryReader(memoryStream))
      {
        success = localFileHeader.ReadFromStream(binaryReader);
      }

      Assert.IsTrue(!string.IsNullOrWhiteSpace(localFileHeader.GetFilename()));
      Assert.IsFalse(localFileHeader.HasDataDescriptor);

      Assert.IsTrue(success);
    }
  }
}
