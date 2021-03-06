﻿using TestFramework;

namespace PlaneGenerator.Configuration
{
    public static class CreationHelper
    {
        public static ConvexSettings FromConfiguration(this PlaneGeneratorConfiguration configuration)
        {
            return new ConvexSettings
            {
                BoundaryBox = new BoundaryBox
                {
                    Height = configuration.BoundaryBoxSize.Z,
                    Length = configuration.BoundaryBoxSize.X,
                    Width = configuration.BoundaryBoxSize.Y
                },
                MaxFractureSize = configuration.QuadrilateralSize.Max,
                MinFractureSize = configuration.QuadrilateralSize.Min,
                NumberOfPlanes = configuration.QuadrilateralCount,
                PartHeight = configuration.GridSize.Height,
                PartWidth = configuration.GridSize.Height
            };
        }
    }
}
