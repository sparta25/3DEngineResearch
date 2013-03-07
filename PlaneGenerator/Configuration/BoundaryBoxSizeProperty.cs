﻿using System.Configuration;

namespace PlaneGenerator.Configuration
{
    public class BoundaryBoxSizeProperty : ConfigurationElement
    {
        const string XPropertyName = "x";
        const string YPropertyName = "y";
        const string ZPropertyName = "z";

        [ConfigurationProperty(XPropertyName, DefaultValue = 100f)]
        public float X
        {
            get { return (float)this[XPropertyName]; }
            set { this[XPropertyName] = value; }
        }

        [ConfigurationProperty(YPropertyName, DefaultValue = 100f)]
        public float Y
        {
            get { return (float)this[YPropertyName]; }
            set { this[YPropertyName] = value; }
        }

        [ConfigurationProperty(ZPropertyName, DefaultValue = 100f)]
        public float Z
        {
            get { return (float)this[ZPropertyName]; }
            set { this[ZPropertyName] = value; }
        }
    }
}
