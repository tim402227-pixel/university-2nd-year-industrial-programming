using System;

namespace CatAndMouse
{
    /// <summary>
    /// Игрок (кот или мышь).
    /// </summary>
    class Player
    {
        public string name;                        // имя игрока
        public int location;                       // позиция на поле (-1, пока игрок не в игре)
        public State state = State.NotInGame;      // состояние
        public int distanceTraveled = 0;           // пройденное расстояние
        private readonly int fieldSize;            // размер поля (нужен, чтобы замкнуть поле в круг)

        public Player(string name, int fieldSize)
        {
            this.name = name;
            this.fieldSize = fieldSize;
            this.location = -1; // не в игре
        }

        /// <summary>
        /// Первый вызов задаёт начальную позицию (это НЕ ход, расстояние не считается).
        /// Все следующие вызовы - обычные ходы на steps клеток (steps может быть < 0).
        /// </summary>
        public void Move(int steps)
        {
            if (state == State.NotInGame)
            {
                // начальная позиция
                location = Normalize(steps);
                state = State.Playing;
                return;
            }

            // обычный ход: пересчитываем позицию, считаем пройденный путь
            location = Normalize(location + steps);
            distanceTraveled += Math.Abs(steps);
        }

        /// <summary>
        /// Приводит позицию к диапазону 1..fieldSize, считая поле кругом:
        /// 0 -> N, -1 -> N-1, N+1 -> 1 и т.д.
        /// </summary>
        private int Normalize(int position)
        {
            return ((position - 1) % fieldSize + fieldSize) % fieldSize + 1;
        }
    }
}
