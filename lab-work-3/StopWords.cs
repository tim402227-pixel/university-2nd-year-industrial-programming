using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextProcessing
{
    /// <summary>
    /// Загрузка списков стоп-слов из файлов (одно слово на строку).
    /// </summary>
    public static class StopWords
    {
        /// <summary>Читает один или несколько файлов в один набор (регистр не учитывается).</summary>
        public static HashSet<string> Load(params string[] paths)
        {
            HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string path in paths)
            {
                foreach (string line in File.ReadLines(path, Encoding.UTF8))
                {
                    string word = line.Trim();
                    if (word.Length > 0)
                        set.Add(Normalize(word));
                }
            }

            return set;
        }

        /// <summary>Приводит типографский апостроф ’ к обычному ', чтобы "don’t" совпадало с "don't".</summary>
        public static string Normalize(string word)
        {
            return word.Replace(Constants.TypographicApostrophe, Constants.Apostrophe);
        }
    }
}
