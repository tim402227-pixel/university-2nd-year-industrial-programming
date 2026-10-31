using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TextProcessing
{
    /// <summary>
    /// Предложение - последовательность слов и знаков препинания.
    /// </summary>
    public class Sentence
    {
        [XmlArray("Tokens")]
        [XmlArrayItem("Word", typeof(Word))]
        [XmlArrayItem("Punctuation", typeof(Punctuation))]
        public List<Token> Tokens { get; set; } = new List<Token>();

        /// <summary>Только слова предложения.</summary>
        [XmlIgnore]
        public IEnumerable<Word> Words
        {
            get { return Tokens.OfType<Word>(); }
        }

        [XmlIgnore]
        public int WordCount
        {
            get { return Words.Count(); }
        }

        /// <summary>Длина предложения в символах (вместе с пробелами и знаками препинания).</summary>
        [XmlIgnore]
        public int Length
        {
            get { return ToString().Length; }
        }

        /// <summary>
        /// Вопросительное предложение: последний завершающий знак содержит '?'.
        /// Закрывающие кавычки и скобки после него пропускаются.
        /// </summary>
        [XmlIgnore]
        public bool IsQuestion
        {
            get
            {
                for (int i = Tokens.Count - 1; i >= 0; i--)
                {
                    Punctuation p = Tokens[i] as Punctuation;
                    if (p == null)
                        return false; // дошли до слова, завершающего знака нет
                    if (p.IsTerminal)
                        return p.Value.Contains(Constants.QuestionMark);
                }
                return false;
            }
        }

        /// <summary>
        /// Убирает "осиротевшие" запятые, точки с запятой и двоеточия,
        /// которые остаются после удаления слов (в начале предложения, подряд или перед точкой).
        /// </summary>
        internal void CleanPunctuation()
        {
            List<Token> result = new List<Token>();

            foreach (Token token in Tokens)
            {
                Punctuation p = token as Punctuation;

                if (p != null && p.IsSeparator)
                {
                    bool atStart = result.Count == 0;
                    Punctuation prev = result.Count > 0 ? result[result.Count - 1] as Punctuation : null;
                    if (atStart || (prev != null && prev.IsSeparator))
                        continue;
                }

                if (p != null && p.IsTerminal && result.Count > 0)
                {
                    Punctuation prev = result[result.Count - 1] as Punctuation;
                    if (prev != null && prev.IsSeparator)
                        result.RemoveAt(result.Count - 1);
                }

                result.Add(token);
            }

            Tokens = result;
        }

        /// <summary>
        /// Собирает предложение в строку: слова через пробел,
        /// знаки препинания "прилипают" к предыдущему слову.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool prevOpening = false; // предыдущий токен - открывающая скобка/кавычка
            bool quoteOpen = false;   // открыта ли прямая кавычка "

            foreach (Token token in Tokens)
            {
                bool space = sb.Length > 0;
                bool opening = false;

                Punctuation p = token as Punctuation;
                if (p != null)
                {
                    if (p.IsOpening)
                    {
                        opening = true;
                    }
                    else if (p.IsStraightQuote)
                    {
                        if (!quoteOpen)
                        {
                            quoteOpen = true;
                            opening = true;
                        }
                        else
                        {
                            quoteOpen = false;
                            space = false;
                        }
                    }
                    else if (!p.IsDash)
                    {
                        space = false; // . , ! ? ; : ) » и т.п. - без пробела перед ними
                    }
                }

                if (prevOpening)
                    space = false;

                if (space)
                    sb.Append(Constants.Space);
                sb.Append(token.Value);
                prevOpening = opening;
            }

            return sb.ToString();
        }
    }
}
