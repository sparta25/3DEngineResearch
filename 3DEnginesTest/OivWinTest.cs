using System;
using ConvexHelper;
using NUnit.Framework;
using OVIConvexTest;

namespace _3DEnginesTest
{
  

    [TestFixture]
    public class OivWinTest
    {
        private const string SettingsFile = @"C:\Temp\Dump.txt";
        private IndexedFaceSet _indexedFaceSet;

        [SetUp]
        [STAThread]
        public void InitTest()
        {
            var settings = SerializationProvider.LoadFromXml<ConvexSettings>(SettingsFile);
            _indexedFaceSet = new IndexedFaceSet(settings);
            _indexedFaceSet.Show();
        }

        [Test]
        public void RenderTest()
        {
            var testHelper = new TestHelper(_indexedFaceSet);
            testHelper.CreateScene();
        }

        [Test]
        public void RotateTest()
        {
            var testHelper = new TestHelper(_indexedFaceSet);
            testHelper.Rotate();
        }

    }
}
