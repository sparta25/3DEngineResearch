using System.Configuration;

namespace PlaneGenerator.Configuration
{
    public class PlaneGeneratorConfiguration : ConfigurationSection
    {
        private const string QuadrilateralCountPropertyName = "quadrilateralCount";
        private const string GridSizePropertyName = "gridSize";
        private const string QuadrilateralSizePropertyName = "quadrilateralSize";
        private const string BoundaryBoxSizePropertyName = "boundaryBoxSize";

        [ConfigurationProperty(QuadrilateralCountPropertyName, DefaultValue = 100)]
        public int QuadrilateralCount
        {
            get { return (int) this[QuadrilateralCountPropertyName]; }
            set { this[QuadrilateralCountPropertyName] = value; }
        }

        [ConfigurationProperty(GridSizePropertyName)]
        public GridSizeProperty GridSize
        {
            get { return (GridSizeProperty) this[GridSizePropertyName]; }
            set { this[GridSizePropertyName] = value; }
        }

        [ConfigurationProperty(QuadrilateralSizePropertyName)]
        public QuadrilateralSizeProperty QuadrilateralSize
        {
            get { return (QuadrilateralSizeProperty) this[QuadrilateralSizePropertyName]; }
            set { this[QuadrilateralSizePropertyName] = value; }
        }

        [ConfigurationProperty(BoundaryBoxSizePropertyName)]
        public BoundaryBoxSizeProperty BoundaryBoxSize
        {
            get { return (BoundaryBoxSizeProperty) this[BoundaryBoxSizePropertyName]; }
            set { this[BoundaryBoxSizePropertyName] = value; }
        }
    }
}