using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace EasyVideoScreensaver
{
    public class MySettings
    {
        public string VideoFilename { get; set; }
        public string StretchMode { get; set; }
        public double Volume { get; set; }
        public bool Mute { get; set; }
        public bool Resume { get; set; }
        [XmlIgnore]
        public TimeSpan Position { get; set; }
        // Support for XmlSerializer, as it does not support TimeSpan
        [Browsable(false)]
        [XmlElement(DataType = "duration", ElementName = "PositionString")]
        public string PositionString
        {
            get
            {
                return XmlConvert.ToString(Position);
            }
            set
            {
                Position = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }

        public void Save(string filename)
        {
            using (var sw = new StreamWriter(filename))
            {
                var xmls = new XmlSerializer(typeof(MySettings));
                xmls.Serialize(sw, this);
            }
        }

        public static MySettings Load(string filename)
        {
            MySettings settings;
            if (File.Exists(filename))
            {
                // Load settings
                using (var sw = new StreamReader(filename))
                {
                    var xmls = new XmlSerializer(typeof(MySettings));
                    settings = xmls.Deserialize(sw) as MySettings;
                }
            }
            else
            {
                // Get default settings
                return new MySettings
                {
                    VideoFilename = "",
                    StretchMode = "Fit",
                    Mute = false,
                    Volume = .5
                };
            }

            // Validate volume
            if (settings.Volume < 0)
                settings.Volume = 0;

            if (settings.Volume > 1)
                settings.Volume = 1;

            return settings;
        }
    }
}