using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Scripts.Player
{
    public class AttackerPlayerController : PlayerController
    {
        private GameObject _minionPrefab;

        private void Awake()
        {
            LoadPrefab();
        }

        private void LoadPrefab()
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Assets/Level/Prefabs/Minion.prefab");
            handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _minionPrefab = handle.Result;
            } else {
                Debug.LogError("Das Laden des Prefabs ist fehlgeschlagen.");
                return;
            }
        }

        public override void PlaceTroops(Vector3 position)
        {
            // Platzierung der Verteidigertruppen
            Instantiate(_minionPrefab, position, Quaternion.identity);
        }
    }
}