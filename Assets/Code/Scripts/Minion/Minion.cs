﻿using System;
using Code.Scripts.Player.Controller;
using Microlight.MicroBar;
using UnityEngine;

namespace Code.Scripts
{
    public class Minion : MonoBehaviour
    {
        public bool IsInvincible = false;
        public int Health;
        public int AttackDamage = 50;
        public float AttackInterval = 1000;
        private MicroBar _healthBar;
        //public GameObject HitParticleSystem;
        public GameObject DeathParticleSystem;
        private Collider _collider;

        private AttackerPlayerController _attackerPlayerController;
        private Bank _bank;
        private UpgradeManager _upgradeManager;
        private CoreTower _coreTower;
        private Collider _coreTowerCollider;
        private float _previousAttackTime = 0;

        private void Awake()
        {
            _attackerPlayerController = FindObjectOfType<AttackerPlayerController>();
            _upgradeManager = FindObjectOfType<UpgradeManager>();

            if (_attackerPlayerController == null) return;

            _bank = _attackerPlayerController.GetBank();

            Health = (int)(Health * _upgradeManager.HealthMultiplier);
            _healthBar = transform.GetComponentInChildren<MicroBar>();
            _healthBar?.Initialize(Health);

            _collider = GetComponent<Collider>();
            _coreTower = FindObjectOfType<CoreTower>();
            if (_coreTower == null) Debug.LogError("Minion cannot find Core Tower on Map");
            _coreTowerCollider = _coreTower.GetComponent<Collider>();
        }

        private void FixedUpdate()
        {
            bool canAttackCoreTower = _collider.bounds.Intersects(_coreTowerCollider.bounds);
            
            if (canAttackCoreTower)
            {
                // TODO: stop moving
                if (Time.time * 1000f - _previousAttackTime > AttackInterval)
                {
                    Attack();
                }

            } else
            {
                // TODO: move
            }
        }

        public void Attack()
        {
            int damage = (int)(AttackDamage * _upgradeManager.AttackDamageMultiplier);
            bool isKillingHit = _coreTower.DamageSelf(damage);

            _previousAttackTime = Time.time * 1000f;
        }

        public void KillSelf()
        {
            if (DeathParticleSystem != null)
                GameObject.Instantiate(
                    DeathParticleSystem,
                    new Vector3(transform.position.x, transform.position.y, transform.position.z),
                    Quaternion.identity
                );
            GameObject.Destroy(gameObject);
        }

        public bool DamageSelf(int damage, GameObject hitParticle = null)
        {
            if (IsInvincible) return false;

            Health -= damage;
            _healthBar?.UpdateHealthBar(Health);
            if (hitParticle != null)
                GameObject.Instantiate(
                    hitParticle,
                    new Vector3(transform.position.x, transform.position.y + _collider.bounds.size.y, transform.position.z),
                    Quaternion.identity
                );

            if (Health <= 0)
            {
                KillSelf();
                return true;
            }
            return false;
        }
    }
}