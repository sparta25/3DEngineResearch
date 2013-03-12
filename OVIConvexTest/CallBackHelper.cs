using TestFramework;
using OIV.Inventor;
using OIV.Inventor.Nodes;
using OIV.Inventor.Sensors;
using OIVCommon;

namespace OVIConvexTest
{
    public class CallBackHelper
    {
        private readonly object _param;
        private readonly int _changingPercent;
        private readonly IConvexSettings _settings;
        public CallBackHelper(object param, int changingPercent, IConvexSettings settings)
        {
            _param = param;
            _changingPercent = changingPercent;
            _settings = settings;
        }

        public void ColorSensorCallback(SoSensor sensor)
        {
            var property = _param as SoVertexProperty;
            if (property == null) return;

            var colours = ConvexGenerator.GetFaceColors(_settings.PartWidth, _settings.PartHeight).ToArrayOfVec3F(); //there is random function, so double calling make sense
            for (int k = 0; k < colours.Length * _changingPercent / 100; k++)
                property.orderedRGBA[k] = new SbColor(colours[k]).GetPackedValue();
        }
    }
}