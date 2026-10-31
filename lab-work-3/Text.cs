using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextProcessing
{
    /// <summary>
    /// Текст - коллекция предложений и все операции над ним.
    /// </summary>
    public class Text
    {
        [XmlArray("Sentences")]
        [XmlArrayItem("Sentence")]
        public List<Sentence> Sentences { get; set; } = new List<Sentence>();

        public Text() { }

        public Text(IEnumerable<Sentence> sentences)
        {
            Sentences = sentences.ToList();
        }

        // ---------- 1. Сортировка по количеству слов ----------

        /// <summary>Предложения по возрастанию количества слов (порядок равных сохраняется).</summary>
        public List<Sentence> GetSentencesOrderedByWordCount()
        {
            return Sentences.OrderBy(s => s.WordCount).ToList();
        }

        // ---------- 2. Сортировка по длине предложения ----------

        /// <summary>Предложения по возрастанию длины в символах (порядок равных сохраняется).</summary>
        public List<Sentence> GetSentencesOrderedByLength()
        {
            return Sentences.OrderBy(s => s.Length).ToList();
        }

        // ---------- 3. Слова заданной длины в вопросительных предложениях ----------

        /// <summary>
        /// Слова длины length во всех вопросительных предложениях.
        /// Повторы (без учёта регистра) не выводятся.
        /// </summary>
        public List<string> FindWordsInQuestions(int length)
        {
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            List<string> result = new List<string>();

            foreach (Sentence sentence in Sentences.Where(s => s.IsQuestion))
            {
                foreach (Word word in sentence.Words.Where(w => w.Length == length))
                {
                    if (seen.Add(word.Value))
                        result.Add(word.Value);
                }
            }

            return result;
        }

        // ---------- 4. Удаление слов заданной длины, начинающихся с согласной ----------

        /// <summary>Возвращает количество удалённых слов.</summary>
        public int RemoveWordsStartingWithConsonant(int length)
        {
            return RemoveWords(w => w.Length == length && w.StartsWithConsonant);
        }

        // ---------- 5. Замена слов заданной длины в одном предложении ----------

        /// <summary>
        /// В предложении с номером sentenceIndex (с нуля) заменяет слова длины length
        /// на подстроку replacement. Длина подстроки может не совпадать с длиной слова.
        /// Возвращает количество замен.
        /// </summary>
        public int ReplaceWordsInSentence(int sentenceIndex, int length, string replacement)
        {
            if (replacement == null)
                throw new ArgumentNullException("replacement");
            if (sentenceIndex < 0 || sentenceIndex >= Sentences.Count)
                throw new ArgumentOutOfRangeException("sentenceIndex");

            Sentence sentence = Sentences[sentenceIndex];
            int count = 0;

            for (int i = 0; i < sentence.Tokens.Count; i++)
            {
                Word word = sentence.Tokens[i] as Word;
                if (word != null && word.Length == length)
                {
                    sentence.Tokens[i] = new Word(replacement);
                    count++;
                }
            }

            return count;
        }

        // ---------- 6. Удаление стоп-слов ----------

        /// <summary>Удаляет из текста стоп-слова (без учёта регистра). Возвращает количество удалённых слов.</summary>
        public int RemoveStopWords(ISet<string> stopWords)
        {
            if (stopWords == null)
                throw new ArgumentNullException("stopWords");

            return RemoveWords(w => stopWords.Contains(StopWords.Normalize(w.Value)));
        }

        // ---------- общая часть удаления ----------

        /// <summary>
        /// Удаляет слова, подходящие под условие, чистит оставшуюся пунктуацию
        /// и выбрасывает предложения, в которых не осталось слов.
        /// </summary>
        private int RemoveWords(Func<Word, bool> predicate)
        {
            int removed = 0;

            foreach (Sentence sentence in Sentences)
            {
                removed += sentence.Tokens.RemoveAll(t =>
                {
                    Word w = t as Word;
                    return w != null && predicate(w);
                });
                sentence.CleanPunctuation();
            }

            Sentences.RemoveAll(s => s.WordCount == 0);
            return removed;
        }

        public override string ToString()
        {
            return string.Join(" ", Sentences);
        }
    }
}
