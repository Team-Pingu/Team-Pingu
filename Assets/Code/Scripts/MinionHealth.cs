
using System;
using UnityEngine;

namespace Code.Scripts
{
    public class MinionHealth : MonoBehaviour
    {

        [SerializeField] public int maxHitPoints = 5;
        private int _currentHitPoints = 0;

        private Minion _minion;
    
        private void OnEnable()
        {
            _currentHitPoints = maxHitPoints;
        }

        private void Start()
        {
            _minion = GetComponent<Minion>();
        }

        private void OnParticleCollision(GameObject other)
        {
            ProcessHit();
        }

        private void ProcessHit()
        {
            _currentHitPoints--;

            if (_currentHitPoints <= 0)
            {
                Destroy(gameObject);
                _minion.StealGold();
            }
        }
    }   
}