using System.Xml.Serialization;

namespace TextProcessing
{
    /// <summary>
    /// Токен - элементарный блок текста (слово или знак препинания).
    /// Класс public и с пустым конструктором, чтобы работала XML-сериализация.
    /// </summary>
    public abstract class Token
    {
        [XmlAttribute("Value")]
        public string Value { get; set; } = "";

        protected Token() { }

        protected Token(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
