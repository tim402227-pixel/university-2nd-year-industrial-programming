using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextProcessing
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // сначала программа сама проверяет себя
            bool testsOk = TestText.RunAll();

            string dataDir = Path.Combine(AppContext.BaseDirectory, Constants.DataFolder);
            string outDir = Path.Combine(AppContext.BaseDirectory, Constants.OutputFolder);
            Directory.CreateDirectory(outDir);

            HashSet<string> stopWords = StopWords.Load(
                Path.Combine(dataDir, Constants.StopWordsRu),
                Path.Combine(dataDir, Constants.StopWordsEn));

            // можно передать свои файлы: dotnet run -- мой_текст.txt
            // иначе обрабатываются два примера (длина слова для английского - 4, для русского - 3)
            List<KeyValuePair<string, int>> files = new List<KeyValuePair<string, int>>();
            if (args.Length > 0)
            {
                foreach (string arg in args)
                    files.Add(new KeyValuePair<string, int>(arg, Constants.DefaultWordLength));
            }
            else
            {
                files.Add(new KeyValuePair<string, int>(Path.Combine(dataDir, Constants.SampleEn), Constants.DefaultWordLength));
                files.Add(new KeyValuePair<string, int>(Path.Combine(dataDir, Constants.SampleRu), Constants.SampleRuWordLength));
            }

            foreach (KeyValuePair<string, int> file in files)
                Demo(file.Key, file.Value, stopWords, outDir);

            return testsOk ? 0 : 1;
        }

        static void Demo(string path, int length, HashSet<string> stopWords, string outDir)
        {
            TextParser parser = new TextParser();

            Console.WriteLine("################ {0} (длина слова: {1}) ################", Path.GetFileName(path), length);
            Text text = parser.ParseFile(path);
            Console.WriteLine("Предложений: {0}", text.Sentences.Count);
            Console.WriteLine();

            Console.WriteLine("1. По возрастанию количества слов:");
            foreach (Sentence s in text.GetSentencesOrderedByWordCount())
                Console.WriteLine("   [{0} сл.] {1}", s.WordCount, s);

            Console.WriteLine("2. По возрастанию длины предложения:");
            foreach (Sentence s in text.GetSentencesOrderedByLength())
                Console.WriteLine("   [{0} симв.] {1}", s.Length, s);

            Console.WriteLine("3. Слова длины {0} в вопросительных предложениях:", length);
            Console.WriteLine("   " + string.Join(", ", text.FindWordsInQuestions(length)));

            // следующие операции меняют текст, поэтому каждый раз читаем файл заново
            Text t4 = parser.ParseFile(path);
            int removed = t4.RemoveWordsStartingWithConsonant(length);
            Console.WriteLine("4. После удаления слов длины {0} на согласную (удалено {1}):", length, removed);
            Console.WriteLine("   " + t4);

            Text t5 = parser.ParseFile(path);
            int replaced = t5.ReplaceWordsInSentence(Constants.ReplaceSentenceIndex, length, Constants.ReplacementMark);
            Console.WriteLine("5. Замена слов длины {0} во 2-м предложении на [...] (замен: {1}):", length, replaced);
            Console.WriteLine("   " + t5);

            Text t6 = parser.ParseFile(path);
            int stopRemoved = t6.RemoveStopWords(stopWords);
            Console.WriteLine("6. После удаления стоп-слов (удалено {0}):", stopRemoved);
            Console.WriteLine("   " + t6);

            string xmlPath = Path.Combine(outDir, Path.GetFileNameWithoutExtension(path) + Constants.XmlExtension);
            TextXmlExporter.Export(text, xmlPath);
            Console.WriteLine("7. XML сохранён: {0}", xmlPath);
            Console.WriteLine();
        }
    }
}
