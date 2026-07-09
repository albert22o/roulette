using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BossRewardInvoker : MonoBehaviour
{
    [SerializeField] private InputAction rewardKey = new InputAction(type: InputActionType.Button);

    private BossReward bossReward;

    [Inject]
    private void Construct(BossReward bossReward)
    {
        this.bossReward = bossReward;
    }

    private void OnEnable()
    {
        rewardKey.Enable();
        rewardKey.performed += HandleRewardInput;
    }

    private void OnDisable()
    {
        rewardKey.performed -= HandleRewardInput;
        rewardKey.Disable();
    }

    private void HandleRewardInput(InputAction.CallbackContext context)
    {
        bossReward.GiveReward();
    }
}
