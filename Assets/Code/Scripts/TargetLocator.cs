using System;
using UnityEngine;

namespace Code.Scripts
{
    public class TargetLocator : MonoBehaviour
    {
        [SerializeField] public Transform weapon;
        [SerializeField] public float range = 15f;
        [SerializeField] public ParticleSystem bullets;
        public Transform target;

        private void Start()
        {
            SetIsShooting(false);
        }

        private void Update()
        {
            FindClosestTarget();
            AimWeapon();
        }

        private void FindClosestTarget()
        {
            Minion[] minions = FindObjectsOfType<Minion>();

            Transform closestTarget = null;
            float maxDistance = Mathf.Infinity;

            foreach (Minion minion in minions)
            {
                float targetDistance = Vector3.Distance(transform.position, minion.transform.position);
                
                if (targetDistance < maxDistance)
                {
                    closestTarget = minion.transform;
                    maxDistance = targetDistance;
                }
            }

            target = closestTarget;
        }
        
        private void AimWeapon()
        {
            // if there is no target return out of the method
            if (!target)
            {
                if (IsShooting())
                {
                    SetIsShooting(false);
                }
                
                return;
            }
            
            print(target);
            
            float targetDistance = Vector3.Distance(transform.position, target.position);
            
            weapon.LookAt(target);

            if (targetDistance < range)
            {
                 /* attack */
                 SetIsShooting(true);
            }
            else
            {
                 /* don't attack */
                 SetIsShooting(false);
            }
        }

        private void SetIsShooting(bool isShooting)
        {
            var emission = bullets.emission;
            emission.enabled = isShooting;
        }

        private bool IsShooting()
        {
            return bullets.emission.enabled;
        }
    }
}
