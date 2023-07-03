using Code.Scripts.Player.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] public int goldReward = 25;
        [SerializeField] public int goldPenalty = 25;

        private DefenderPlayerController _defenderPlayerController;
        private Bank _bank;
        private UpgradeManager _upgradeManager;

        private void Start()
        {
            _defenderPlayerController = FindObjectOfType<DefenderPlayerController>();
            _upgradeManager = FindObjectOfType<UpgradeManager>();

            if (_defenderPlayerController == null) return;

            _bank = _defenderPlayerController.GetBank();
        }

        public void RewardGold()
        {
            if (_bank == null) return;

            _bank.Deposit((int)(goldReward * _upgradeManager.MoneyBonusMultiplier));
        }

        public void StealGold()
        {
            if (_bank == null) return;

            _bank.Withdraw(goldPenalty);
        }
    }
}
