using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ZipParser.Model;
using ZipParser.Utility;

namespace ZipParserUnitTests.Utility
{
  /// <summary>
  /// Summary description for BinaryUtilitiesUnitTests
  /// </summary>
  [TestClass]
  public class BinaryUtilitiesUnitTests
  {
    public BinaryUtilitiesUnitTests()
    {
      //
      // TODO: Add constructor logic here
      //
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
    public void IsBitSet()
    {
      Assert.IsTrue(BinaryUtilities.IsBitSet(0x08, 3));
      Assert.IsFalse(BinaryUtilities.IsBitSet(0x08, 2));
      Assert.IsFalse(BinaryUtilities.IsBitSet(0x08, 1));
      Assert.IsFalse(BinaryUtilities.IsBitSet(0x08, 0));

      var bytes = new byte[4] {
        0xFF, 0xFF, 0xFF, 0xFF,
      };

      for (int i = 0; i < 32; i++)
      {
        Assert.IsTrue(BinaryUtilities.IsBitSet(bytes, i));
      }

      bytes = new byte[4]
      {
        0x00, 0x00, 0x00, 0x00,
      };

      for (int i = 0; i < 32; i++)
      {
        Assert.IsFalse(BinaryUtilities.IsBitSet(bytes, i));
      }


      bytes = new byte[4]
      {
        0x00, 0x08, 0x00, 0x10,
      };

      Assert.IsTrue(BinaryUtilities.IsBitSet(bytes, 11)); // bytes[2] position 3
      Assert.IsTrue(BinaryUtilities.IsBitSet(bytes, 28)); // bytes[3] position 4
    }

    [TestMethod]
    public void AreByteArraysEqual()
    {
      var a = new byte[] { 1, 2, 3, 4 };
      var b = new byte[] { 1, 2, 3, 4 };
      var c = new byte[] { 1, 2, 3 };
      var d = new byte[] { 1, 2, 3 };
      var e = LocalFileHeader.Signature;
      var f = CentralDirectoryFileHeader.Signature;


      Assert.IsTrue(BinaryUtilities.AreByteArraysEqual(a, b));
      Assert.IsTrue(BinaryUtilities.AreByteArraysEqual(c, d));
      Assert.IsFalse(BinaryUtilities.AreByteArraysEqual(a, c));
      Assert.IsFalse(BinaryUtilities.AreByteArraysEqual(b, d));
      Assert.IsFalse(BinaryUtilities.AreByteArraysEqual(e, f));
    }

    [TestMethod]
    public void BitArrayToAndFromInt()
    {
      for (int i = 0; i < int.MaxValue; i++)
      {
        var bitArray = BinaryUtilities.BitArrayFromInt(i);
        var control = BinaryUtilities.BitArrayToInt(bitArray);
        Assert.AreEqual(control, i);
      }
    }

  }
}
