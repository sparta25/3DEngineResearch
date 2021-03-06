﻿using System.Configuration;

namespace PlaneGenerator.Configuration
{
    public class GridSizeProperty : ConfigurationElement
    {
        private const string WidthPropertyName = "width";
        private const string HeightPropertyName = "height";

        [ConfigurationProperty(WidthPropertyName, DefaultValue = 10)]
        public int Width
        {
            get { return (int) this[WidthPropertyName]; }
            set { this[WidthPropertyName] = value; }
        }

        [ConfigurationProperty(HeightPropertyName, DefaultValue = 10)]
        public int Height
        {
            get { return (int) this[HeightPropertyName]; }
            set { this[HeightPropertyName] = value; }
        }
    }
}