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

        private void Awake()
        {
            LoadPrefab();
        }

        private void LoadPrefab()
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Assets/Level/Prefabs/Building.prefab");
            handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _buildingPrefab = handle.Result;
            } else {
                Debug.LogError("Das Laden des Prefabs ist fehlgeschlagen.");
                return;
            }
        }

        public override void PlaceTroops(Vector3 position)
        {
            // Platzierung der Verteidigertruppen
            Instantiate(_buildingPrefab, position, Quaternion.identity);
        }
    }
}