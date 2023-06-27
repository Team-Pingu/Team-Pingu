using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Scripts.Player
{
    // Hauptklasse für den PlayerController
    public abstract class PlayerController : MonoBehaviour
    {
        private Bank _bank;

        private GameObject _placeholderPrefab;

        private void Awake()
        {
            LoadPrefab();
        }

        private void LoadPrefab()
        {
            var resource = new GameResource("Assets/Level/Prefabs/Towers/ThunderCoil.prefab", "", GameResourceType.AutoMinion);
            _placeholderPrefab = resource.LoadRessource<GameObject>();
        }

        private void Start()
        {
            _bank = GetComponent<Bank>();
        }

        public Bank GetBank()
        {
            return _bank;
        }

        // Methode zum Platzieren der Truppen
        public GameObject[] PlaceUnits(Dictionary<GameResource, int> prefabs, Vector3 position)
        {
            List<GameObject> outPrefabs = new List<GameObject>();
            foreach (var unit in prefabs)
            {
                GameResource prefabResource = unit.Key;
                int prefabAmount = unit.Value;

                GameObject gameObject = prefabResource.LoadRessource<GameObject>();
                if (gameObject != null)
                {
                    // scale and position adjustments to spread GameObjects
                    Vector3 positionAdjust = new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f));
                    float localScale = 0.66f;
                    gameObject.transform.localScale *= localScale;

                    GameObject instantiatedGameObject = Instantiate(gameObject, position + positionAdjust, Quaternion.identity);
                    outPrefabs.AddRange(Enumerable.Repeat(instantiatedGameObject, prefabAmount));
                }
            }

            return outPrefabs.Count == 0 ? null : outPrefabs.ToArray();
        }

        public virtual GameObject PlacePlaceholderUnit(Vector3 position)
        {
            return Instantiate(_placeholderPrefab, position, Quaternion.identity);
        }
    }
}

