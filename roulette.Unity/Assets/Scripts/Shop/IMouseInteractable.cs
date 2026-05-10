public interface IMouseInteractable
{
    public bool IsInteractable { get; }
    public void OnMouseEnter();
    public void OnMouseExit();
    public void OnMouseDown();
    public void OnMouseUp();
}
