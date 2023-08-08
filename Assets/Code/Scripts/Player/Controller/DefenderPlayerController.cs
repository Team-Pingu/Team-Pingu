using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Controller
{
    public class DefenderPlayerController : PlayerController
    {
        private Dictionary<string, GameObject> _prefabCollection;

        private void Awake()
        {
            _bank = GetComponent<Bank>();
            _prefabCollection = new Dictionary<string, GameObject>();
            LoadPrefab();

        }

        private void LoadPrefab()
        {
            var thunderCoil = new GameResource("Assets/Level/Prefabs/Towers/ThunderCoil.prefab", "ThunderCoil", GameResourceType.Tower);
            var core = new GameResource("Assets/Level/Prefabs/Towers/Core.prefab", "Core", GameResourceType.Tower);
            var boltThrower = new GameResource("Assets/Level/Prefabs/Towers/BoltThrower.prefab", "BoltThrower", GameResourceType.Tower);
            var electricFence = new GameResource("Assets/Level/Prefabs/Barricades/ElectricFence.prefab", "ElectricFence", GameResourceType.Barricade);

            this._prefabCollection.Add(thunderCoil.ResourceID, thunderCoil.LoadRessource<GameObject>());
            this._prefabCollection.Add(core.ResourceID, core.LoadRessource<GameObject>()); 
            this._prefabCollection.Add(boltThrower.ResourceID, boltThrower.LoadRessource<GameObject>());
            this._prefabCollection.Add(electricFence.ResourceID, electricFence.LoadRessource<GameObject>());
        }

        public Dictionary<String, GameObject> GetPrefabCollection() {
            return this._prefabCollection;
        }

        override
        public GameObject PlacePlaceholderUnit(Vector3 position)
        {
            return Instantiate(this._prefabCollection["ThunderCoil"], position, Quaternion.identity);
        }

        override
        public GameObject PlaceUnit(String prefabName, Vector3 position) {
            return Instantiate(this._prefabCollection[prefabName], position, Quaternion.identity);
        }
    }
}