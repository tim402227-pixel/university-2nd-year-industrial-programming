using System.Xml.Serialization;

namespace TextProcessing
{
    /// <summary>
    /// Слово - неразрывная последовательность букв/цифр (может содержать дефис или апостроф).
    /// </summary>
    public class Word : Token
    {
        public Word() { }

        public Word(string value) : base(value) { }

        /// <summary>Длина слова в символах.</summary>
        [XmlIgnore]
        public int Length
        {
            get { return Value.Length; }
        }

        /// <summary>
        /// Слово начинается с согласной буквы (русской или английской).
        /// Если первый символ не буква (например, цифра) - false.
        /// </summary>
        [XmlIgnore]
        public bool StartsWithConsonant
        {
            get
            {
                if (Value.Length == 0)
                    return false;

                char first = char.ToLowerInvariant(Value[0]);
                return char.IsLetter(first) && Constants.Vowels.IndexOf(first) < 0;
            }
        }
    }
}
