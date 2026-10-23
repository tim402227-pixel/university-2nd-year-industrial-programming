namespace CatAndMouse
{
    /// <summary>
    /// Состояние игрока.
    /// </summary>
    enum State
    {
        Winner,    // победитель
        Loser,     // проигравший
        Playing,   // играет
        NotInGame  // не в игре - позиция на поле ещё не задана
    }
}
