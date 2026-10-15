using UnityEngine;
using Zenject;

public class ChipHolderSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip startDragSound;
    [SerializeField] AudioClip endDragSound;

    public void PlayDragStartSound() 
    {
        audioSource.PlayOneShot(startDragSound);
    }
    public void PlayDragEndSound()
    {
        audioSource.PlayOneShot(endDragSound);
    }
}
