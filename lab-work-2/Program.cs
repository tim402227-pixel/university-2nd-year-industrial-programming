using System;
using System.IO;

namespace CatAndMouse
{
    /// <summary>
    /// Клиентский класс: запускает игру на тестовых файлах и сверяет результат с эталоном.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
            string outDir = Path.Combine(AppContext.BaseDirectory, "Output");
            Directory.CreateDirectory(outDir);

            // тесты 1-3 - из задания, тест 4 - пример из условия (поле на 26 клеток)
            for (int test = 1; test <= 4; test++)
            {
                // через статические поля класса Game задаём пути к входному и выходному файлам
                Game.InputFile = Path.Combine(dataDir, test + ".ChaseData.txt");
                Game.OutFile = Path.Combine(outDir, test + ".PursuitLog.txt");

                Game game = new Game(Game.ReadFieldSize(Game.InputFile));
                game.Run(); // запуск игры и вывод результатов

                Console.WriteLine("===== Тест {0} (файл {1}) =====", test, Path.GetFileName(Game.OutFile));
                Console.WriteLine(File.ReadAllText(Game.OutFile));

                string expected = Path.Combine(dataDir, test + ".PursuitLog.txt");
                if (File.Exists(expected))
                {
                    bool same = File.ReadAllText(expected).Replace("\r\n", "\n").TrimEnd()
                             == File.ReadAllText(Game.OutFile).Replace("\r\n", "\n").TrimEnd();
                    Console.WriteLine(same ? ">>> СОВПАДАЕТ с эталоном" : ">>> НЕ СОВПАДАЕТ с эталоном");
                    Console.WriteLine();
                }
            }
        }
    }
}
