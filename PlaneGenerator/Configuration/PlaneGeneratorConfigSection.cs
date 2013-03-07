using System.Configuration;

namespace PlaneGenerator
{
    class PlaneGeneratorConfiguration : ConfigurationSection
    {
        const string QuadrilateralCountPropertyName = "QuadrilateralCount";
        const string PartsPerWidthPropertyName = "PartsPerWidth";
        const string PartsPerHeightPropertyName = "PartsPerHeight";
        const string MinQuadrilateralSizePropertyName = "MinQuadrilateralSize";
        const string MaxQuadrilateralSizePropertyName = "MaxQuadrilateralSize";
        const string BoundaryBoxSizeXPropertyName = "BoundaryBoxSizeX";
        const string BoundaryBoxSizeYPropertyName = "BoundaryBoxSizeY";
        const string BoundaryBoxSizeZPropertyName = "BoundaryBoxSizeZ";

        [ConfigurationProperty(QuadrilateralCountPropertyName, DefaultValue = 100)]
        int QuadrilateralCount
        {
            get { return (int)this[QuadrilateralCountPropertyName]; }
            set { this[QuadrilateralCountPropertyName] = value; }
        }

        [ConfigurationProperty(PartsPerWidthPropertyName, DefaultValue = 10)]
        int PartsPerWidth
        {
            get { return (int)this[PartsPerWidthPropertyName]; }
            set { this[PartsPerWidthPropertyName] = value; }
        }

        [ConfigurationProperty(PartsPerHeightPropertyName, DefaultValue = 10)]
        int PartsPerHeight {
            get { return (int)this[PartsPerHeightPropertyName]; }
            set { this[PartsPerHeightPropertyName] = value; }
        }

        [ConfigurationProperty(MaxQuadrilateralSizePropertyName, DefaultValue = 5)]
        float MinQuadrilateralSize
        {
            get { return (float)this[MaxQuadrilateralSizePropertyName]; }
            set { this[MaxQuadrilateralSizePropertyName] = value; }
        }

        [ConfigurationProperty(MinQuadrilateralSizePropertyName, DefaultValue = 30)]
        float MaxQuadrilateralSize
        {
            get { return (float)this[MinQuadrilateralSizePropertyName]; }
            set { this[MinQuadrilateralSizePropertyName] = value; }
        }

        [ConfigurationProperty(BoundaryBoxSizeXPropertyName, DefaultValue = 100)]
        public float BoundaryBoxSizeX
        {
            get { return (float)this[BoundaryBoxSizeXPropertyName]; }
            set { this[BoundaryBoxSizeXPropertyName] = value; }
        }

        [ConfigurationProperty(BoundaryBoxSizeYPropertyName, DefaultValue = 100)]
        public float BoundaryBoxSizeY
        {
            get { return (float)this[BoundaryBoxSizeYPropertyName]; }
            set { this[BoundaryBoxSizeYPropertyName] = value; }
        }

        [ConfigurationProperty(BoundaryBoxSizeZPropertyName, DefaultValue = 100)]
        public float BoundaryBoxSizeZ
        {
            get { return (float)this[BoundaryBoxSizeZPropertyName]; }
            set { this[BoundaryBoxSizeZPropertyName] = value; }
        }
    }
}
