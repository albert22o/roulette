using UnityEngine;

public class BossRewardAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private BossRewardView bossReward;

    private int throwBossChipHash = Animator.StringToHash("ThrowBossChip");

    private void OnEnable()
    {
        bossReward.RewardReceived += PlayThrowChipAnimation;
    }

    private void OnDisable()
    {
        bossReward.RewardReceived -= PlayThrowChipAnimation;
    }

    private void PlayThrowChipAnimation() => animator.SetTrigger(throwBossChipHash);
}
