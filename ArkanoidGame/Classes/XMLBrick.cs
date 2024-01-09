using System;
using System.Xml.Serialization;

namespace ArkanoidGame
{
    public class XMLBrick
    {
        [XmlElement("Type")]
        public int Type { get; set; }

        [XmlElement("PosX")]
        public int PosX { get; set; }

        [XmlElement("PosY")]
        public int PosY { get; set; }

        [XmlElement("Value")]
        public int Value { get; set; }

        [XmlElement("Color")]
        public String Color { get; set; }

        [XmlElement("TimesToBreak")]
        public int TimesToBreak { get; set; }
    }
}