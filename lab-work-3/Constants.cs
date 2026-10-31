namespace TextProcessing
{
    /// <summary>
    /// Все константы проекта в одном месте: символы, шаблоны регулярного выражения,
    /// имена папок и файлов, параметры демонстрации.
    /// </summary>
    public static class Constants
    {
        // ---------- символы ----------

        public const char Space = ' ';
        public const char QuestionMark = '?';

        public const char Apostrophe = '\'';
        public const char TypographicApostrophe = '’';

        /// <summary>Знаки, завершающие предложение.</summary>
        public const string TerminalChars = ".!?…";

        /// <summary>Открывающие скобки и кавычки.</summary>
        public const string OpeningChars = "([{«“„";

        /// <summary>Закрывающие скобки и кавычки.</summary>
        public const string ClosingChars = ")]}»”";

        /// <summary>Прямая кавычка (одна и для открытия, и для закрытия).</summary>
        public const string StraightQuote = "\"";

        public const string Hyphen = "-";
        public const string EnDash = "–";
        public const string EmDash = "—";

        public const string Comma = ",";
        public const string Semicolon = ";";
        public const string Colon = ":";

        /// <summary>Гласные русского и английского алфавитов (остальные буквы - согласные).</summary>
        public const string Vowels = "aeiouаеёиоуыэюя";

        // ---------- регулярное выражение парсера ----------

        public const string NumberGroup = "num";
        public const string WordGroup = "word";
        public const string TerminalGroup = "term";
        public const string OtherGroup = "other";

        /// <summary>Число с дробной частью: 3.14, 2,5.</summary>
        public const string NumberPattern = @"\d+(?:[.,]\d+)+";

        /// <summary>Слово: буквы и цифры, внутри допустим дефис или апостроф (кто-то, don't).</summary>
        public const string WordPattern = @"[\p{L}\p{N}]+(?:['’\-][\p{L}\p{N}]+)*";

        /// <summary>Группа завершающих знаков: . ! ? … ... ?!</summary>
        public const string TerminalPattern = "[" + TerminalChars + "]+";

        /// <summary>Любой другой непробельный символ - знак препинания.</summary>
        public const string OtherPattern = @"\S";

        public const string TokenPattern =
            "(?<" + NumberGroup + ">" + NumberPattern + ")|" +
            "(?<" + WordGroup + ">" + WordPattern + ")|" +
            "(?<" + TerminalGroup + ">" + TerminalPattern + ")|" +
            "(?<" + OtherGroup + ">" + OtherPattern + ")";

        // ---------- папки и файлы ----------

        public const string DataFolder = "Data";
        public const string OutputFolder = "Output";
        public const string StopWordsRu = "stopwords_ru.txt";
        public const string StopWordsEn = "stopwords_en.txt";
        public const string SampleEn = "sample_en.txt";
        public const string SampleRu = "sample_ru.txt";
        public const string XmlExtension = ".xml";

        // ---------- параметры демонстрации в Program ----------

        public const int DefaultWordLength = 4;
        public const int SampleRuWordLength = 3;
        public const int ReplaceSentenceIndex = 1; // номер предложения с нуля (2-е предложение)
        public const string ReplacementMark = "[...]";
    }
}
