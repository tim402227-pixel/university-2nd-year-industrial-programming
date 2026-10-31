using System.Linq;
using System.Xml.Serialization;

namespace TextProcessing
{
    /// <summary>
    /// Знак препинания (один символ или группа вроде "...", "?!").
    /// </summary>
    public class Punctuation : Token
    {
        public Punctuation() { }

        public Punctuation(string value) : base(value) { }

        /// <summary>Знак, которым заканчивается предложение: . ! ? … и их сочетания.</summary>
        [XmlIgnore]
        public bool IsTerminal
        {
            get { return Value.Length > 0 && Value.All(c => Constants.TerminalChars.IndexOf(c) >= 0); }
        }

        /// <summary>Открывающая скобка или кавычка.</summary>
        [XmlIgnore]
        public bool IsOpening
        {
            get { return Value.Length == 1 && Constants.OpeningChars.IndexOf(Value[0]) >= 0; }
        }

        /// <summary>Закрывающая скобка или кавычка.</summary>
        [XmlIgnore]
        public bool IsClosing
        {
            get { return Value.Length == 1 && Constants.ClosingChars.IndexOf(Value[0]) >= 0; }
        }

        /// <summary>Прямая кавычка " (открывающая или закрывающая - зависит от позиции).</summary>
        [XmlIgnore]
        public bool IsStraightQuote
        {
            get { return Value == Constants.StraightQuote; }
        }

        /// <summary>Тире или дефис, стоящие отдельно (с пробелами с обеих сторон).</summary>
        [XmlIgnore]
        public bool IsDash
        {
            get { return Value == Constants.EmDash || Value == Constants.EnDash || Value == Constants.Hyphen; }
        }

        /// <summary>Запятая, точка с запятой, двоеточие.</summary>
        [XmlIgnore]
        public bool IsSeparator
        {
            get { return Value == Constants.Comma || Value == Constants.Semicolon || Value == Constants.Colon; }
        }
    }
}
