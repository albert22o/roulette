using UnityEngine;
using Zenject;

public class PlaquesShopSounds : MonoBehaviour
{
    private SFXManager sfxManager;

    [Inject] private void Construct(SFXManager sFXManager) { sfxManager = sFXManager; }

    public void PlayServoSound()
    {
        sfxManager.Play("PlaqueShopMoving");
    }

    public void PlayPlaqueFellSound()
    {
        sfxManager.Play("PlaqueFall");
    }

    public void PlayInsertBossChipSound()
    {
        sfxManager.Play("ChipInsert");
    }
}
