using Code.Scripts.Player.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Code.Scripts
{
    public class Tower : MonoBehaviour
    {
        public int Health;
        public int AttackDamage;
        public float AttackRange;
        public Transform Weapon;
        public int CurrencyReward = 50;
        public float AttackInterval = 5000; // 5s
        public bool IsAttacking { get; private set; } = false;

        private DefenderPlayerController _defenderPlayerController;
        private UpgradeManager _upgradeManager;
        private Bank _bank;
        private Minion _lockedAttackTarget;
        private int _killedTargets = 0;
        private float _previousAttackTime = 0;

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

            _bank.Deposit((int)(CurrencyReward * _upgradeManager.MoneyBonusMultiplier));
        }

        private void Update()
        {
            if (Health <= 0) KillSelf();
        }

        public void FixedUpdate()
        {
            // TODO: Getters for all values multiplied with the upgrademanager!!!
            if (_lockedAttackTarget == null || _lockedAttackTarget.Health <= 0)
            {
                _lockedAttackTarget = GetNextAttackTarget();
            }
            else
            {
                AimWeapon();
                if (Time.time * 1000f - _previousAttackTime > AttackInterval)
                {
                    Attack(_lockedAttackTarget);
                }

                float targetDistance = Vector3.Distance(transform.position, _lockedAttackTarget.transform.position);
                // release too far away target
                if (targetDistance > AttackRange) _lockedAttackTarget = null;
            }
        }

        public void Attack(Minion target)
        {
            int damage = (int)(AttackDamage * _upgradeManager.AttackDamageMultiplier);
            int newTargetHealth = target.Health - damage;
            bool isKillingHit = newTargetHealth <= 0;

            target.Health = newTargetHealth;
            if (isKillingHit) _killedTargets++;

            print("ATTACK");

            _previousAttackTime = Time.time * 1000f;
        }

        private Minion GetNextAttackTarget()
        {
            Minion[] minions = FindObjectsOfType<Minion>();

            Minion closestTarget = null;
            float maxDistance = Mathf.Infinity;

            foreach (Minion minion in minions)
            {
                float targetDistance = Vector3.Distance(transform.position, minion.transform.position);

                if (targetDistance < maxDistance && targetDistance <= AttackRange)
                {
                    closestTarget = minion;
                    maxDistance = targetDistance;
                }
            }

            return closestTarget;
        }

        private void AimWeapon()
        {
            // if there is no target return out of the method
            if (!_lockedAttackTarget)
            {
                if (IsAttacking)
                {
                    IsAttacking = false;
                }

                return;
            }

            float targetDistance = Vector3.Distance(transform.position, _lockedAttackTarget.transform.position);
            Vector3 lookAtPosition = new Vector3(
                _lockedAttackTarget.transform.position.x,
                Weapon.transform.position.y,
                _lockedAttackTarget.transform.position.z
            );
            Weapon.LookAt(lookAtPosition);

            if (targetDistance < AttackRange)
            {
                /* attack */
                IsAttacking = true;
            }
            else
            {
                /* don't attack */
                IsAttacking = false;
            }
        }

        public void KillSelf()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
