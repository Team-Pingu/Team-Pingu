﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Controller
{
    public class AttackerPlayerController : PlayerController
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
            var aetherShieldBearer = new GameResource("Assets/Level/Prefabs/Units/AetherShieldBearer.prefab", "AetherShieldBearer", GameResourceType.Minion);
            var clockworkScout = new GameResource("Assets/Level/Prefabs/Units/ClockworkScout.prefab", "ClockworkScout", GameResourceType.Minion);
            var gearheadSapper = new GameResource("Assets/Level/Prefabs/Units/GearheadSapper.prefab", "GearheadSapper", GameResourceType.Minion);
            var autoMinion = new GameResource("Assets/Level/Prefabs/Units/AutoMinion.prefab", "AutoMinion", GameResourceType.AutoMinion);
            var steamPoweredGoliath = new GameResource("Assets/Level/Prefabs/Units/SteamPoweredGoliath.prefab", "SteamPoweredGoliath", GameResourceType.Minion);

            this._prefabCollection.Add(aetherShieldBearer.ResourceID, aetherShieldBearer.LoadRessource<GameObject>());
            this._prefabCollection.Add(clockworkScout.ResourceID, clockworkScout.LoadRessource<GameObject>());
            this._prefabCollection.Add(gearheadSapper.ResourceID, gearheadSapper.LoadRessource<GameObject>());
            this._prefabCollection.Add(autoMinion.ResourceID, autoMinion.LoadRessource<GameObject>());
            this._prefabCollection.Add(steamPoweredGoliath.ResourceID, steamPoweredGoliath.LoadRessource<GameObject>());
            
        }
        
        public Dictionary<String, GameObject> GetPrefabCollection() {
            return this._prefabCollection;
        }

        override
        public GameObject PlacePlaceholderUnit(Vector3 position)
        {
            return Instantiate(this._prefabCollection["AetherShieldBearer"] , position, Quaternion.identity);
        }

        override
        public GameObject PlaceUnit(String prefabName, Vector3 position)
        {
            Debug.Log("Attacker Controller spawning " + prefabName);
            return Instantiate(this._prefabCollection[prefabName] , position, Quaternion.identity);
        }
    }
}