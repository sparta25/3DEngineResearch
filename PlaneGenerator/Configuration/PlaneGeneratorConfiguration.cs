using System.Configuration;

namespace PlaneGenerator.Configuration
{
    public class PlaneGeneratorConfiguration : ConfigurationSection
    {
        const string QuadrilateralCountPropertyName = "quadrilateralCount";
        const string GridSizePropertyName = "gridSize";
        const string QuadrilateralSizePropertyName = "quadrilateralSize";
        const string BoundaryBoxSizePropertyName = "boundaryBoxSize";

        [ConfigurationProperty(QuadrilateralCountPropertyName, DefaultValue = 100)]
        public int QuadrilateralCount
        {
            get { return (int)this[QuadrilateralCountPropertyName]; }
            set { this[QuadrilateralCountPropertyName] = value; }
        }

        [ConfigurationProperty(GridSizePropertyName)]
        public GridSizeProperty GridSize
        {
            get { return (GridSizeProperty)this[GridSizePropertyName]; }
            set { this[GridSizePropertyName] = value; }
        }

        [ConfigurationProperty(QuadrilateralSizePropertyName)]
        public QuadrilateralSizeProperty QuadrilateralSize
        {
            get { return (QuadrilateralSizeProperty)this[QuadrilateralSizePropertyName]; }
            set { this[QuadrilateralSizePropertyName] = value; }
        }

        [ConfigurationProperty(BoundaryBoxSizePropertyName)]
        public BoundaryBoxSizeProperty BoundaryBoxSize
        {
            get { return (BoundaryBoxSizeProperty)this[BoundaryBoxSizePropertyName]; }
            set { this[BoundaryBoxSizePropertyName] = value; }
        }
    }
}
