using System;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts
{
    public class Minion : MonoBehaviour
    {
        [SerializeField] public int goldReward = 25;
        [SerializeField] public int goldPenalty = 25;

        private AttackerPlayerController _attackerPlayerController;
        private Bank _bank;

        private void Start()
        {
            _attackerPlayerController = FindObjectOfType<AttackerPlayerController>();
            
            if(_attackerPlayerController == null) return;

            _bank = _attackerPlayerController.GetBank();
        }

        public void RewardGold()
        {
            if (_bank == null) return;
            
            _bank.Deposit(goldReward);
        }

        public void StealGold()
        {
            if (_bank == null) return;
            
            _bank.Withdraw(goldPenalty);
        }
    }
}