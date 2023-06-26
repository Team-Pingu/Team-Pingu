using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Scripts.Player
{
    public class DefenderPlayerController : PlayerController
    {
        private GameObject _buildingPrefab;
        private int _cost = 75;

        private void Awake()
        {
            LoadPrefab();
        }

        private void LoadPrefab()
        {
            var resource = new GameResource("Assets/Level/Prefabs/Towers/ThunderCoil.prefab", "", GameResourceType.AutoMinion);
            _buildingPrefab = resource.LoadRessource<GameObject>();
        }

        public override GameObject PlaceTroops(Vector3 position)
        {
            // Platzierung der Verteidigertruppen
            if (GetBank().CurrentBalance >= _cost)
            {
                GetBank().Withdraw(_cost);
                return Instantiate(_buildingPrefab, position, Quaternion.identity);
            }

            return null;
        }
    }
}