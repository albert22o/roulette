public interface IMouseInteractable
{
    public bool IsInteractable { get; }
}

public interface IHoverable
{
    public void HoverEnter();
    public void HoverExit();
}

public interface IClickable
{
    public void Clicked();
}

public interface IPressable
{

    public void Pressed();
    public void Released();
}
