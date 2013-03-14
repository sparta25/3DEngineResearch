using System.Configuration;

namespace PlaneGenerator.Configuration
{
    public class QuadrilateralSizeProperty : ConfigurationElement
    {
        private const string MinPropertyName = "min";
        private const string MaxPropertyName = "max";

        [ConfigurationProperty(MinPropertyName, DefaultValue = 30f)]
        public float Min
        {
            get { return (float) this[MinPropertyName]; }
            set { this[MinPropertyName] = value; }
        }

        [ConfigurationProperty(MaxPropertyName, DefaultValue = 5f)]
        public float Max
        {
            get { return (float) this[MaxPropertyName]; }
            set { this[MaxPropertyName] = value; }
        }
    }
}