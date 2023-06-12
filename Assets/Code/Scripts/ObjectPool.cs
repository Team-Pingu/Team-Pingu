using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] public GameObject minionPrefab;
        [SerializeField][Range(0, 50)] public int poolSize = 5;
        [SerializeField][Range(0.1f, 30f)] public float spawnTimer = 1f;

        private GameObject[] _pool;

        private void Awake()
        {
            PopulatePool();
        }

        private void Start()
        {
            StartCoroutine(SpawnMinion());
        }

        private void PopulatePool()
        {
            _pool = new GameObject[poolSize];

            for (int i = 0; i < _pool.Length; i++)
            {
                _pool[i] = Instantiate(minionPrefab, transform);
                _pool[i].SetActive(false);
            }
        }

        private void EnableObjectInPool()
        {
            for(int i = 0; i < _pool.Length; i++)
            {
                if(!_pool[i].activeInHierarchy)
                {
                    _pool[i].SetActive(true);
                    return;
                }
            }
        }

        IEnumerator SpawnMinion()
        {
            while (true)
            {
                EnableObjectInPool();
                yield return new WaitForSeconds(spawnTimer);
            }
        }
    }
}
