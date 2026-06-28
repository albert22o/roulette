using UnityEngine;
using Zenject;

public class StartBossChipsLoader : MonoBehaviour
{
        [SerializeField] private int initMoney;
        private BossChips bossChips;

        [Inject]
        public void Construct(BossChips bossChips)
        {
            this.bossChips = bossChips;
        }

        void Start()
        {
            bossChips.Add(initMoney);
        }
}
