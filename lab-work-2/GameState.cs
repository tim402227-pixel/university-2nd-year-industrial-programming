namespace CatAndMouse
{
    /// <summary>
    /// Состояние самой игры.
    /// </summary>
    enum GameState
    {
        Start,  // игра идёт (обрабатываем команды)
        End     // игра закончена (мышь поймана или команды кончились)
    }
}
