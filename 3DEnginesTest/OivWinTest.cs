using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ConvexHelper;
using NUnit.Framework;
using OVIConvexTest;

namespace _3DEnginesTest
{
  

    [TestFixture]
    public class OivWinTest
    {
        private readonly string _settingsFile = @"C:\Temp\Dump.txt";
        private IndexedFaceSet _indexedFaceSet;

        [SetUp]
        [STAThread]
        public void InitTest()
        {
            var settings = SerializationProvider.LoadFromXml<ConvexSettings>(_settingsFile);
            _indexedFaceSet = new IndexedFaceSet(settings);
            _indexedFaceSet.Show();
        }

        [Test]
        public void RenderTest()
        {
            var testHelper = new TestHelper(_indexedFaceSet);
            testHelper.Render();
        }

        [Test]
        public void RotateTest()
        {
            var testHelper = new TestHelper(_indexedFaceSet);
            testHelper.Rotate();
        }

    }
}
