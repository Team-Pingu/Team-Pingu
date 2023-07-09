using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Scripts.Player.Controller
{
    // Hauptklasse für den PlayerController
    public abstract class PlayerController : MonoBehaviour
    {
        private Bank _bank;

        private GameObject _placeholderPrefab;

        private void Awake()
        {
            _bank = GetComponent<Bank>();
            LoadPrefab();
        }

        private void LoadPrefab()
        {
            var resource = new GameResource("Assets/Level/Prefabs/Towers/ThunderCoil.prefab", "", GameResourceType.AutoMinion);
            _placeholderPrefab = resource.LoadRessource<GameObject>();
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
                    const float positionRange = 5f;
                    const float localScale = 0.75f;

                    for(int i = 0; i < prefabAmount; i++)
                    {
                        Vector3 positionAdjust = new Vector3(Random.Range(-positionRange, positionRange), 0, Random.Range(-positionRange, positionRange));
                        GameObject instantiatedGameObject = Instantiate(gameObject, position + positionAdjust, Quaternion.identity);
                        instantiatedGameObject.transform.localScale *= localScale;
                        outPrefabs.Add(instantiatedGameObject);
                    }
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

