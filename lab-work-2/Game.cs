using System;
using System.IO;

namespace CatAndMouse
{
    /// <summary>
    /// Игра "кошки-мышки": читает команды, управляет игроками, пишет протокол.
    /// </summary>
    class Game
    {
        // пути к входному и выходному файлам задаются снаружи (из Main)
        public static string InputFile = "ChaseData.txt";
        public static string OutFile = "PursuitLog.txt";

        public int size;            // размер игрового поля
        public Player cat;
        public Player mouse;
        public GameState state;

        public Game(int size)
        {
            this.size = size;
            // создаём игроков
            cat = new Player("Cat", size);
            mouse = new Player("Mouse", size);
            state = GameState.Start;
        }

        /// <summary>
        /// Читает первую строку файла - количество клеток.
        /// Нужна клиенту, чтобы создать Game(size) с правильным размером.
        /// </summary>
        public static int ReadFieldSize(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return int.Parse(reader.ReadLine().Trim());
            }
        }

        /// <summary>
        /// Запуск игры: обработка команд из InputFile и запись протокола в OutFile.
        /// </summary>
        public void Run()
        {
            using (StreamReader reader = new StreamReader(InputFile))
            using (StreamWriter writer = new StreamWriter(OutFile))
            {
                reader.ReadLine(); // первая строка - размер поля, он уже передан в конструктор

                WriteHeader(writer);

                while (state != GameState.End) // пока не конец игры
                {
                    // прочитать команду
                    string line = reader.ReadLine();

                    // если все команды исчерпаны, закончить игру
                    if (line == null)
                    {
                        state = GameState.End;
                        break;
                    }

                    line = line.Trim();
                    if (line.Length == 0)
                        continue; // пустые строки пропускаем

                    string[] parts = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                    char command = char.ToUpper(parts[0][0]);

                    // выполнить команду
                    switch (command)
                    {
                        case 'M':
                        case 'C':
                            DoMoveCommand(command, int.Parse(parts[1]));
                            break;
                        case 'P':
                            DoPrintCommand(writer);
                            break;
                    }

                    // если мышь поймана, закончить игру
                    if (IsMouseCaught())
                    {
                        mouse.state = State.Loser;
                        cat.state = State.Winner;
                        state = GameState.End;
                    }
                }

                // мышь убежала - кот проиграл, мышь победила
                if (mouse.state == State.Playing && cat.state == State.Playing)
                {
                    mouse.state = State.Winner;
                    cat.state = State.Loser;
                }

                WriteFooter(writer);
            }
        }

        private void DoMoveCommand(char command, int steps)
        {
            switch (command)
            {
                case 'M': mouse.Move(steps); break;
                case 'C': cat.Move(steps); break;
            }
        }

        private void DoPrintCommand(StreamWriter writer)
        {
            // вывод позиций игроков и расстояния между ними (если это возможно)
            string catText = cat.state == State.NotInGame ? "??" : cat.location.ToString();
            string mouseText = mouse.state == State.NotInGame ? "??" : mouse.location.ToString();

            string row = string.Format("{0,3}{1,6}", catText, mouseText);

            // расстояние выводим, только если оба в игре
            if (cat.state != State.NotInGame && mouse.state != State.NotInGame)
                row += string.Format("{0,10}", GetDistance());

            writer.WriteLine(row);
        }

        private int GetDistance()
        {
            // расстояние никогда не бывает отрицательным
            return Math.Abs(cat.location - mouse.location);
        }

        /// <summary>
        /// Мышь поймана, если оба в игре и стоят в одной клетке.
        /// </summary>
        private bool IsMouseCaught()
        {
            return cat.state != State.NotInGame
                && mouse.state != State.NotInGame
                && cat.location == mouse.location;
        }

        // ---------- вывод шапки и итогов ----------

        private void WriteHeader(StreamWriter writer)
        {
            writer.WriteLine("Cat and Mouse");
            writer.WriteLine();
            writer.WriteLine("Cat Mouse  Distance");
            writer.WriteLine(new string('-', 19));
        }

        private void WriteFooter(StreamWriter writer)
        {
            writer.WriteLine(new string('-', 19));
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine("Distance traveled:   Mouse    Cat");
            writer.WriteLine("{0,26}{1,7}", mouse.distanceTraveled, cat.distanceTraveled);
            writer.WriteLine();

            if (mouse.state == State.Loser)
                writer.WriteLine("Mouse caught at: {0,2}", mouse.location);
            else
                writer.WriteLine("Mouse evaded Cat");
        }
    }
}
