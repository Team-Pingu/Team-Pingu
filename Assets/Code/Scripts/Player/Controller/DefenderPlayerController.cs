using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Scripts.Player.Controller
{
    public class DefenderPlayerController : PlayerController
    {
        private Bank _bank;

        private GameObject[] prefabList;

        private void Awake()
        {
            _bank = GetComponent<Bank>();
            LoadPrefab();
        }

        private void LoadPrefab()
        {
            var thunderCoil = new GameResource("Assets/Level/Prefabs/Towers/ThunderCoil.prefab", "", GameResourceType.Tower);
            var core = new GameResource("Assets/Level/Prefabs/Towers/Core.prefab", "", GameResourceType.Tower);
            var boltThrower = new GameResource("Assets/Level/Prefabs/Towers/BoltThrower.prefab", "", GameResourceType.Tower);

            prefabList = new GameObject[] {thunderCoil.LoadRessource<GameObject>(), core.LoadRessource<GameObject>(), boltThrower.LoadRessource<GameObject>()};
        }

        public GameObject[] getPrefabList() {
            return this.prefabList;
        }

        override
        public GameObject PlacePlaceholderUnit(Vector3 position)
        {
            return Instantiate(prefabList[0], position, Quaternion.identity);
        }
    }
}