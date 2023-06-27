using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Scripts.Player.Controller
{
    public class AttackerPlayerController : PlayerController
    {
        private GameObject _minionPrefab;
        private int _cost = 75;
        
        private void Awake()
        {
            LoadPrefab();
        }

        private void LoadPrefab()
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Assets/Level/Prefabs/PrototypeModels/Minion.prefab");
            handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _minionPrefab = handle.Result;
            } else {
                Debug.LogError("Das Laden des Prefabs ist fehlgeschlagen.");
                return;
            }
        }

        public override GameObject PlaceTroops(Vector3 position)
        {
            // Platzierung der Verteidigertruppen

            // if (GetBank().CurrentBalance >= _cost)
            // {
            //     GetBank().Withdraw(_cost);
                return Instantiate(_minionPrefab, position, Quaternion.identity);
            // }

            // return null;
        }
    }
}