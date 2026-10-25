using UnityEngine;
using Zenject;

public class ChipHolderSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip startDragSound;
    [SerializeField] AudioClip endDragSound;
    [SerializeField] AudioClip chipActiveSound;

    [Inject]
    public void Construct(ChipHolder chipHolder)
    {
        chipHolder.ChipAnimated += PlayChipActivatedSound;
    }

    public void PlayDragStartSound() 
    {
        audioSource.PlayOneShot(startDragSound);
    }
    public void PlayDragEndSound()
    {
        audioSource.PlayOneShot(endDragSound);
    }
    private void PlayChipActivatedSound(int index)
    {

        audioSource.PlayOneShot(chipActiveSound);
    }
}
