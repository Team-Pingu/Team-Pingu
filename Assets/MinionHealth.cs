using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHealth : MonoBehaviour
{

    [SerializeField] public int maxHitPoints = 5;
    private int _currentHitPoints = 0;
    
    void Start()
    {
        _currentHitPoints = maxHitPoints;
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
        }
    }
}
