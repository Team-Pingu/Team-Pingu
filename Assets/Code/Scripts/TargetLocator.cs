using System;
using UnityEngine;

namespace Code.Scripts
{
    public class TargetLocator : MonoBehaviour
    {
        [SerializeField] public Transform weapon;
        [SerializeField] public float range = 15f;
        public Transform target;

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
            if (!target) { return; }
            
            float targetDistance = Vector3.Distance(transform.position, target.position);
            
            weapon.LookAt(target);

            if (targetDistance < range) { /* attack */ } else { /* don't attack */ }
        }
    }
}
