using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts
{
    [Serializable]
    public abstract class EntityBase
    {
        public int Health;
        public int AttackDamage;
        public float AttackRange;
        private Bank _bank;

        public int CurrencyReward = 50;
        //public int CurrencyPenalty;

        public abstract void Attack();

        public abstract GameObject GetNextAttackTarget();
    }
}
