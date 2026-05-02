using System;

public interface IRouletteWheel
{
    /// <summary>
    /// Вызывается когда все шары остановились
    /// </summary>
    public event Action<HitInfo[]> AllBallsStopped;
    /// <summary>
    /// Добавляет шар с хешем в рулетку
    /// </summary>
    /// <param name="ballHash"> Хеш шара, обязан быть уникальным</param>
    public void AddBall(string ballHash, VisualComponent visualComponent);

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

    /// <summary>
    /// Устанавливает цвет поля
    /// </summary>
    /// <param name="fieldPosition">Фактическая позиция поля</param>
    /// <param name="color">Цвет поля</param>
    public void SetFieldColor(int fieldPosition, string color);

    /// <summary>
    /// Устанавливает номер поля
    /// </summary>
    /// <param name="fieldIndex">Фактическая позиция поля</param>
    /// <param name="number">Номер поля</param>
    public void SetFieldNumber(int fieldIndex, int number);
}