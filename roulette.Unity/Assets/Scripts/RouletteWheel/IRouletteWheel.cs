public interface IRouletteWheel
{
    /// <summary>
    /// Добавляет шар с именем ballName в рулетку
    /// </summary>
    /// <param name="ballName"> Имя шара, обязано быть уникальным</param>
    public void AddBall(string ballName);

    /// <summary>
    /// Крутит все шары в рулетке
    /// </summary>
    public void Spin();

    /// <summary>
    /// Крутит определенные шары
    /// </summary>
    /// <param name="ballNames"> Имена шаров, которые нужно перекрутить</param>
    public void Reroll(params string[] ballNames);

    /// <summary>
    /// Выводит все шары из рулетки
    /// </summary>
    public void ClearBalls();

    /// <summary>
    /// Получаем информацию о том, на каком поле остановился каждый шар после кручения
    /// </summary>
    /// <returns> Информация о шарах, или null, если они не были прокатаны</returns>
    public HitInfo[] GetHitInfo();
}