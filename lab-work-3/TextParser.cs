using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TextProcessing
{
    /// <summary>
    /// Парсер текста (вариант "Парсер 2"): разбивает текст на токены регулярным выражением
    /// и собирает токены в предложения, а предложения - в текст.
    /// Работает с русским и английским текстом.
    /// </summary>
    public class TextParser
    {
        // шаблон (число | слово | завершающие знаки | другой символ) лежит в Constants
        private static readonly Regex TokenRegex = new Regex(Constants.TokenPattern, RegexOptions.Compiled);

        public Text Parse(string content)
        {
            Text text = new Text();
            Sentence current = new Sentence();
            int quotes = 0; // сколько прямых кавычек " встретилось в текущем предложении

            MatchCollection matches = TokenRegex.Matches(content ?? "");

            for (int i = 0; i < matches.Count; i++)
            {
                Match m = matches[i];

                if (m.Groups[Constants.NumberGroup].Success || m.Groups[Constants.WordGroup].Success)
                {
                    current.Tokens.Add(new Word(m.Value));
                    continue;
                }

                Punctuation p = new Punctuation(m.Value);
                current.Tokens.Add(p);
                if (p.IsStraightQuote)
                    quotes++;

                if (p.IsTerminal)
                {
                    // закрывающие кавычки и скобки сразу после точки относятся к этому же предложению
                    while (i + 1 < matches.Count && IsCloser(matches[i + 1].Value, quotes))
                    {
                        Punctuation closer = new Punctuation(matches[i + 1].Value);
                        if (closer.IsStraightQuote)
                            quotes++;
                        current.Tokens.Add(closer);
                        i++;
                    }

                    Flush(text, ref current, ref quotes);
                }
            }

            Flush(text, ref current, ref quotes); // последнее предложение без точки
            return text;
        }

        public Text ParseFile(string path)
        {
            return Parse(File.ReadAllText(path, Encoding.UTF8));
        }

        private static bool IsCloser(string value, int quotes)
        {
            if (value == Constants.StraightQuote)
                return quotes % 2 == 1;
            return value.Length == 1 && Constants.ClosingChars.IndexOf(value[0]) >= 0;
        }

        /// <summary>Добавляет собранное предложение в текст (если в нём есть слова).</summary>
        private static void Flush(Text text, ref Sentence current, ref int quotes)
        {
            if (current.WordCount > 0)
                text.Sentences.Add(current);

            current = new Sentence();
            quotes = 0;
        }
    }
}
