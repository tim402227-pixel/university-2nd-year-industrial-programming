using System;

namespace CatAndMouse
{
    class Player
    {
        public string name;
        public int location;
        public State state = State.NotInGame;
        public int distanceTraveled = 0;
        private readonly int fieldSize; 

        public Player(string name, int fieldSize)
        {
            this.name = name;
            this.fieldSize = fieldSize;
            this.location = -1;
        }
        public void Move(int steps)
        {
            if (state == State.NotInGame)
            {
                location = Normalize(steps);
                state = State.Playing;
                return;
            }

            location = Normalize(location + steps);
            distanceTraveled += Math.Abs(steps);
        }
        private int Normalize(int position)
        {
            return ((position - 1) % fieldSize + fieldSize) % fieldSize + 1;
        }
    }
}
