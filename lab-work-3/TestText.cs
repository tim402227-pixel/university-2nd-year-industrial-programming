using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TextProcessing
{
    /// <summary>
    /// Самопроверяющийся тест: программа сама сравнивает результат с ожидаемым
    /// и сообщает, правильно ли она работает.
    /// </summary>
    static class TestText
    {
        private static readonly TextParser Parser = new TextParser();

        public static bool RunAll()
        {
            Console.WriteLine("===== РЕЗУЛЬТАТ ТЕСТА =====");

            bool ok = true;
            ok &= TestParser();
            ok &= TestOrdering();
            ok &= TestQuestionWords();
            ok &= TestRemoveConsonantWords();
            ok &= TestReplace();
            ok &= TestStopWords();
            ok &= TestXml();

            Console.WriteLine();
            Console.WriteLine(ok ? ">>> Программа работает ПРАВИЛЬНО"
                                 : ">>> Программа работает НЕПРАВИЛЬНО");
            Console.WriteLine();
            return ok;
        }

        // ---------- парсер ----------

        private static bool TestParser()
        {
            bool ok = true;

            Text t = Parser.Parse("Hello, world! How are you?");
            ok &= Check("Парсер: 3 слова в 2 предложениях, количество предложений", t.Sentences.Count, 2);
            ok &= Check("Парсер: в 1-м предложении 2 слова", t.Sentences[0].WordCount, 2);
            ok &= Check("Парсер: в 1-м предложении 4 токена (слово , слово !)", t.Sentences[0].Tokens.Count, 4);
            ok &= Check("Парсер: предложение собирается обратно", t.Sentences[0].ToString(), "Hello, world!");
            ok &= Check("Парсер: 1-е предложение не вопросительное", t.Sentences[0].IsQuestion, false);
            ok &= Check("Парсер: 2-е предложение вопросительное", t.Sentences[1].IsQuestion, true);

            t = Parser.Parse("Привет, мир! Как дела?");
            ok &= Check("Парсер: русский текст", t.ToString(), "Привет, мир! Как дела?");

            t = Parser.Parse("Wait... Go.");
            ok &= Check("Парсер: многоточие не делит предложение", t.Sentences.Count, 2);
            ok &= Check("Парсер: многоточие сохраняется", t.Sentences[0].ToString(), "Wait...");

            t = Parser.Parse("Don't stop-it now.");
            ok &= Check("Парсер: апостроф и дефис внутри слова (3 слова)", t.Sentences[0].WordCount, 3);

            t = Parser.Parse("Pi is 3.14 today.");
            ok &= Check("Парсер: дробное число - одно слово (1 предложение)", t.Sentences.Count, 1);
            ok &= Check("Парсер: дробное число - одно слово (4 слова)", t.Sentences[0].WordCount, 4);

            t = Parser.Parse("She asked \"Why?\" Then left.");
            ok &= Check("Парсер: кавычка после ? остаётся в предложении", t.Sentences[0].ToString(), "She asked \"Why?\"");
            ok &= Check("Парсер: вопрос в кавычках определяется", t.Sentences[0].IsQuestion, true);
            ok &= Check("Парсер: следующее предложение отдельно", t.Sentences[1].ToString(), "Then left.");

            t = Parser.Parse("No end here");
            ok &= Check("Парсер: последнее предложение без точки сохраняется", t.Sentences.Count, 1);

            return ok;
        }

        // ---------- 1 и 2. сортировки ----------

        private static bool TestOrdering()
        {
            bool ok = true;

            Text t = Parser.Parse("Aa bb cc dd. Ee ff. Gg hh ii.");
            string byWords = string.Join(" | ", t.GetSentencesOrderedByWordCount());
            ok &= Check("Сортировка по словам (2, 3, 4 слова)", byWords, "Ee ff. | Gg hh ii. | Aa bb cc dd.");

            // в этом тексте порядок по словам и по длине разный
            t = Parser.Parse("Extraordinarily long word. A b c d e.");
            ok &= Check("По количеству слов: сначала предложение из 3 слов",
                t.GetSentencesOrderedByWordCount()[0].ToString(), "Extraordinarily long word.");
            ok &= Check("По длине: сначала короткое предложение из 5 слов",
                t.GetSentencesOrderedByLength()[0].ToString(), "A b c d e.");
            ok &= Check("Длина предложения считается вместе с пробелами и точкой",
                t.Sentences[1].Length, 10);
            ok &= Check("Сортировка не меняет исходный текст",
                t.Sentences[0].ToString(), "Extraordinarily long word.");

            return ok;
        }

        // ---------- 3. слова в вопросительных предложениях ----------

        private static bool TestQuestionWords()
        {
            bool ok = true;

            Text t = Parser.Parse("Is this a test? Yes, this works. What is that? Who is there?");
            ok &= Check("Вопросы: слова длины 4 без повторов и без повествовательных",
                string.Join(",", t.FindWordsInQuestions(4)), "this,test,What,that");
            ok &= Check("Вопросы: повторы без учёта регистра (Is / is)",
                string.Join(",", t.FindWordsInQuestions(2)), "Is");
            ok &= Check("Вопросы: 'works' только в повествовательном - не выводится, 'there' выводится",
                string.Join(",", t.FindWordsInQuestions(5)), "there");

            return ok;
        }

        // ---------- 4. удаление слов заданной длины на согласную ----------

        private static bool TestRemoveConsonantWords()
        {
            bool ok = true;

            Text t = Parser.Parse("The big dog ate an apple.");
            int removed = t.RemoveWordsStartingWithConsonant(3);
            ok &= Check("Согласные (EN): удалено 3 слова", removed, 3);
            ok &= Check("Согласные (EN): слово на гласную 'ate' осталось", t.ToString(), "ate an apple.");

            t = Parser.Parse("Мама мыла окно.");
            t.RemoveWordsStartingWithConsonant(4);
            ok &= Check("Согласные (RU): 'окно' осталось, 'Мама' и 'мыла' удалены", t.ToString(), "окно.");

            t = Parser.Parse("Well, that works.");
            t.RemoveWordsStartingWithConsonant(4);
            ok &= Check("Согласные: запятая в начале после удаления убирается", t.ToString(), "works.");

            t = Parser.Parse("Hello, big dog.");
            t.RemoveWordsStartingWithConsonant(3);
            ok &= Check("Согласные: запятая перед точкой после удаления убирается", t.ToString(), "Hello.");

            t = Parser.Parse("Big dog. Apple tree.");
            t.RemoveWordsStartingWithConsonant(3);
            ok &= Check("Согласные: предложение без слов удаляется из текста", t.Sentences.Count, 1);

            return ok;
        }

        // ---------- 5. замена слов заданной длины ----------

        private static bool TestReplace()
        {
            bool ok = true;

            Text t = Parser.Parse("The cat sat on the mat.");
            int count = t.ReplaceWordsInSentence(0, 3, "[X]");
            ok &= Check("Замена: заменено 5 слов длины 3", count, 5);
            ok &= Check("Замена: результат", t.ToString(), "[X] [X] [X] on [X] [X].");

            t = Parser.Parse("The cat sat. The dog ran.");
            t.ReplaceWordsInSentence(1, 3, "a");
            ok &= Check("Замена: подстрока короче слова, меняется только указанное предложение",
                t.ToString(), "The cat sat. a a a.");

            t = Parser.Parse("Go now.");
            t.ReplaceWordsInSentence(0, 2, "elephant");
            ok &= Check("Замена: подстрока длиннее слова", t.ToString(), "elephant now.");

            bool thrown = false;
            try
            {
                t.ReplaceWordsInSentence(5, 3, "x");
            }
            catch (ArgumentOutOfRangeException)
            {
                thrown = true;
            }
            ok &= Check("Замена: неверный номер предложения - исключение", thrown, true);

            return ok;
        }

        // ---------- 6. стоп-слова ----------

        private static bool TestStopWords()
        {
            bool ok = true;
            string file = Path.GetTempFileName();

            try
            {
                File.WriteAllText(file, "the\r\nis\r\non\r\nit\r\nи\r\nв\r\n", new UTF8Encoding(false));
                var stop = StopWords.Load(file);
                ok &= Check("Стоп-слова: загружено 6 слов", stop.Count, 6);

                Text t = Parser.Parse("The cat is on the mat.");
                int removed = t.RemoveStopWords(stop);
                ok &= Check("Стоп-слова (EN): удалено 4 слова (регистр не важен)", removed, 4);
                ok &= Check("Стоп-слова (EN): результат", t.ToString(), "cat mat.");

                t = Parser.Parse("Кот и пёс в доме.");
                t.RemoveStopWords(stop);
                ok &= Check("Стоп-слова (RU): результат", t.ToString(), "Кот пёс доме.");

                t = Parser.Parse("It is. Cat sleeps.");
                t.RemoveStopWords(stop);
                ok &= Check("Стоп-слова: предложение из одних стоп-слов удаляется", t.Sentences.Count, 1);
            }
            finally
            {
                File.Delete(file);
            }

            return ok;
        }

        // ---------- 7. XML ----------

        private static bool TestXml()
        {
            bool ok = true;
            string file = Path.GetTempFileName();

            try
            {
                Text t = Parser.Parse("Hello, world! How are you?");
                TextXmlExporter.Export(t, file);
                string xml = File.ReadAllText(file, Encoding.UTF8);

                ok &= Check("XML: файл содержит слово", xml.Contains("<Word Value=\"Hello\""), true);
                ok &= Check("XML: файл содержит знак препинания", xml.Contains("<Punctuation Value=\"?\""), true);

                Text back = TextXmlExporter.Import(file);
                ok &= Check("XML: после импорта то же число предложений", back.Sentences.Count, 2);
                ok &= Check("XML: после импорта тот же текст", back.ToString(), t.ToString());
                ok &= Check("XML: после импорта вопрос определяется", back.Sentences[1].IsQuestion, true);
            }
            finally
            {
                File.Delete(file);
            }

            return ok;
        }

        // ---------- проверка ----------

        private static bool Check<T>(string name, T actual, T expected)
        {
            bool ok = Equals(actual, expected);
            Console.WriteLine("{0}: {1}", ok ? "[ OK ]" : "[FAIL]", name);
            if (!ok)
                Console.WriteLine("   получено: {0}, ожидалось: {1}", actual, expected);
            return ok;
        }
    }
}
