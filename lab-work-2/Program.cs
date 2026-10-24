using System;
using System.IO;

namespace CatAndMouse
{
    class Program
    {
        static void Main(string[] args)
        {
            TestMove.RunAll();
            string dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
            string outDir = Path.Combine(AppContext.BaseDirectory, "Output");
            Directory.CreateDirectory(outDir);

            for (int test = 1; test <= 4; test++)
            {
                Game.InputFile = Path.Combine(dataDir, test + ".ChaseData.txt");
                Game.OutFile = Path.Combine(outDir, test + ".PursuitLog.txt");

                Game game = new Game(Game.ReadFieldSize(Game.InputFile));
                game.Run();

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
