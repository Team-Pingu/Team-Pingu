using System;
using Code.Scripts.Player.Controller;
using Microlight.MicroBar;
using UnityEngine;

namespace Code.Scripts
{
    public class Minion : MonoBehaviour
    {
        public int Health;
        private MicroBar _healthBar;
        //public GameObject HitParticleSystem;
        public GameObject DeathParticleSystem;
        private Collider _collider;

        private AttackerPlayerController _attackerPlayerController;
        private Bank _bank;
        private UpgradeManager _upgradeManager;

        private void Awake()
        {
            _attackerPlayerController = FindObjectOfType<AttackerPlayerController>();
            _upgradeManager = FindObjectOfType<UpgradeManager>();

            if (_attackerPlayerController == null) return;

            _bank = _attackerPlayerController.GetBank();

            _healthBar = transform.GetComponentInChildren<MicroBar>();
            _healthBar?.Initialize(Health);

            _collider = GetComponent<Collider>();
        }

        public void KillSelf()
        {
            GameObject.Destroy(gameObject);
        }

        public bool DamageSelf(int damage, GameObject hitParticle = null)
        {
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
                if (DeathParticleSystem != null)
                    GameObject.Instantiate(
                        DeathParticleSystem,
                        new Vector3(transform.position.x, transform.position.y, transform.position.z),
                        Quaternion.identity
                    );
                return true;
            }
            return false;
        }
    }
}