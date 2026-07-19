public abstract class BallInfo
{
    public string Hash { get; private set; }
    public BallInfoData BallData { get; private set; }
    public BallDependencies Dependencies { get; private set; }

    protected BallInfo(string ballHash, BallInfoData data, BallDependencies deps)
    {
        Hash = ballHash;
        BallData = data;
        Dependencies = deps;
    }

    public virtual void ApplySelectedEffect(HitInfo hitInfo) { }
    public virtual void RemoveSelectedEffect() { }
    public virtual void OnAddedToHolder() { }
    public virtual void OnRemovedFromHolder() { }
}
