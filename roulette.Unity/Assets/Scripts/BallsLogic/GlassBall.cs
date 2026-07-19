using UnityEngine;
public class GlassBall : BallInfo
{
    private int destroyChance;
    private int multiplier;
    public GlassBall(int multiplier, int destroyChance, string hash, BallInfoData data, BallDependencies deps) : base(hash, data, deps)
    {
        this.multiplier = multiplier;
        this.destroyChance = destroyChance;
    }

    public override void ApplySelectedEffect(HitInfo hitInfo)
    {
        Dependencies.BetCalculator.MuliplyGeneralModifier(multiplier);
    }

    public override void RemoveSelectedEffect()
    {
        Dependencies.BetCalculator.DivideGeneralModifier(multiplier);
        if (Random.Range(0, 100) <= destroyChance)
        {
            Dependencies.BallHolder.RemoveBall(Hash);
        }
    }
}
