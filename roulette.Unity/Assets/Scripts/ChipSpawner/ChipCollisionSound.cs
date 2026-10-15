using UnityEngine;

public class ChipCollisionSound : MonoBehaviour
{
    private ChipSoundPlayer player;
    private float spawnTime;
    private float lastPlayTime = -10f;

    public void Init(ChipSoundPlayer player)
    {
        this.player = player;
        spawnTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (player == null) return;

        var s = player.Settings;

        if (Time.time - spawnTime < s.spawnGraceTime) return;
        if (Time.time - lastPlayTime < s.perChipCooldown) return;

        float impact = collision.relativeVelocity.magnitude;
        if (impact < s.minImpactVelocity) return;

        var otherChip = collision.collider.GetComponentInParent<ChipCollisionSound>();

        if (otherChip != null)
        {
            // фишка о фишку: звук играет только одна из пары
            if (otherChip.GetInstanceID() > GetInstanceID()) return;

            if (player.Play(true, impact, collision.GetContact(0).point))
                lastPlayTime = Time.time;
        }
        else if ((s.floorMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            if (player.Play(false, impact, collision.GetContact(0).point))
                lastPlayTime = Time.time;
        }
    }
}