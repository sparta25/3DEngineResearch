using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaneGenerator.Configuration;
using TestFramework;

namespace Framework.Tests
{
    [TestClass]
    public class ConvexSettingsTest
    {
        private const string settingsFile = "App.config";
        ConvexSettings target;

        [TestInitialize]
        public void Init()
        {
            var configMap = new ExeConfigurationFileMap() { ExeConfigFilename = settingsFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            var configuration = (PlaneGeneratorConfiguration)config.GetSection("planeGenerator");

            target = configuration.FromConfiguration();
            target.FillPlanes();
            target.FillIndices();
        }

        [TestMethod]
        public void GetQuadrilateralByPositionTest()
        {
            var lastX = target.PartWidth;
            var lastY = target.PartHeight;
            var plane = target.Planes[0];

            target.GetQuadrilateralByPosition(plane, lastX, lastY);
        }
    }
}
