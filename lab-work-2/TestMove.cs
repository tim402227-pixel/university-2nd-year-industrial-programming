using System;
using System.IO;

namespace CatAndMouse
{
    static class TestMove
    {
        public static bool RunAll()
        {
            Console.WriteLine("===== РЕЗУЛЬТАТ ТЕСТА =====");

            // --- Player напрямую (поле на 12 клеток) ---
            Player mouse = new Player("Mouse", 12);
            bool t1 = Check("До старта позиция -1", mouse.location, -1);
            mouse.Move(9);
            bool t2 = Check("Старт 9: позиция 9", mouse.location, 9)
                   && Check("Старт 9: пройдено 0", mouse.distanceTraveled, 0);
            mouse.Move(-4);
            bool t3 = Check("Ход -4: позиция 5", mouse.location, 5)
                   && Check("Ход -4: пройдено 4", mouse.distanceTraveled, 4);
            mouse.Move(15);
            bool t4 = Check("Ход 15 через конец поля: позиция 8", mouse.location, 8)
                   && Check("Ход 15: пройдено 19", mouse.distanceTraveled, 19);
            mouse.Move(-10);
            bool t5 = Check("Ход -10 через начало поля: позиция 10", mouse.location, 10)
                   && Check("Ход -10: пройдено 29", mouse.distanceTraveled, 29);

            Player cat = new Player("Cat", 12);
            cat.Move(0);
            bool t6 = Check("Старт 0 превращается в клетку 12", cat.location, 12);
            cat.Move(13);
            bool t7 = Check("Ход 13 с клетки 12: позиция 1", cat.location, 1)
                   && Check("Ход 13: пройдено 13", cat.distanceTraveled, 13);

            // --- Game: Run() и GetDistance() через лог ---
            bool t8 = CheckLog("Кот поймал мышь", "10",
                new[] { "M 5", "C 2", "C 3" }, "Mouse caught at:  5");
            bool t9 = CheckLog("Мышь убежала", "10",
                new[] { "M 5", "C 2", "M 3" }, "Mouse evaded Cat");
            bool t10 = CheckLog("Расстояние кот-мышь (GetDistance)", "10",
                new[] { "M 5", "C 2", "P" }, "  2     5         3");
            bool t11 = CheckLog("До старта кота выводится ??", "10",
                new[] { "M 4", "P" }, " ??     4");

            bool ok = t1 && t2 && t3 && t4 && t5 && t6 && t7 && t8 && t9 && t10 && t11;
            Console.WriteLine();
            Console.WriteLine(ok ? ">>> Программа работает ПРАВИЛЬНО"
                                 : ">>> Программа работает НЕПРАВИЛЬНО");
            Console.WriteLine();
            return ok;
        }

        static bool Check<T>(string name, T actual, T expected)
        {
            bool ok = Equals(actual, expected);
            Console.WriteLine("{0}: {1}", ok ? "[ OK ]" : "[FAIL]", name);
            if (!ok)
                Console.WriteLine("   получено: {0}, ожидалось: {1}", actual, expected);
            return ok;
        }

        static bool CheckLog(string name, string size, string[] commands, string expected)
        {
            string log = Play(size, commands);
            bool ok = log.Contains(expected);
            Console.WriteLine("{0}: {1}", ok ? "[ OK ]" : "[FAIL]", name);
            if (!ok)
                Console.WriteLine("   ожидалось в логе: \"" + expected + "\" - НЕ найдено");
            return ok;
        }

        static string Play(string size, string[] commands)
        {
            string oldIn = Game.InputFile, oldOut = Game.OutFile;
            Game.InputFile = Path.GetTempFileName();
            Game.OutFile = Path.GetTempFileName();
            File.WriteAllText(Game.InputFile,
                size + Environment.NewLine + string.Join(Environment.NewLine, commands));
            new Game(int.Parse(size)).Run();
            string result = File.ReadAllText(Game.OutFile);
            File.Delete(Game.InputFile);
            File.Delete(Game.OutFile);
            Game.InputFile = oldIn;
            Game.OutFile = oldOut;
            return result;
        }
    }
}