using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TextProcessing
{
    /// <summary>
    /// Экспорт текста в XML и обратный импорт (System.Xml.Serialization).
    /// </summary>
    public static class TextXmlExporter
    {
        public static void Export(Text text, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Text));
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                serializer.Serialize(writer, text);
            }
        }

        public static Text Import(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Text));

            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                return (Text)serializer.Deserialize(reader);
            }
        }
    }
}
