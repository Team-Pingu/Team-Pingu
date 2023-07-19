using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Controller
{
    public class DefenderPlayerController : PlayerController
    {
        private Dictionary<String, GameObject> _prefabCollection;

        private void Awake()
        {
            _bank = GetComponent<Bank>();
            _prefabCollection = new Dictionary<string, GameObject>();
            LoadPrefab();

        }

        private void LoadPrefab()
        {
            var thunderCoil = new GameResource("Assets/Level/Prefabs/Towers/ThunderCoil.prefab", "", GameResourceType.Tower);
            var core = new GameResource("Assets/Level/Prefabs/Towers/Core.prefab", "", GameResourceType.Tower);
            var boltThrower = new GameResource("Assets/Level/Prefabs/Towers/BoltThrower.prefab", "", GameResourceType.Tower);

            this._prefabCollection.Add("thunderCoil", thunderCoil.LoadRessource<GameObject>());
            this._prefabCollection.Add("core", core.LoadRessource<GameObject>()); 
            this._prefabCollection.Add("boltThrower", boltThrower.LoadRessource<GameObject>());
        }

        public Dictionary<String, GameObject> GetPrefabCollection() {
            return this._prefabCollection;
        }

        override
        public GameObject PlacePlaceholderUnit(Vector3 position)
        {
            return Instantiate(this._prefabCollection["thunderCoil"], position, Quaternion.identity);
        }

        override
        public GameObject PlaceUnit(String prefabName, Vector3 position) {
            return Instantiate(this._prefabCollection[prefabName], position, Quaternion.identity);
        }
    }
}