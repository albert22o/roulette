public interface IMouseInteractable
{
    public bool IsInteractable { get; }
}

public interface IHoverable
{
    public void OnHoverEnter();
    public void OnHoverExit();
}

public interface IClickable
{
    public void OnClick();
}

public interface IPressable
{

    public void Pressed();
    public void Released();
}
