using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Scripts.Player
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
            var resource = new GameResource("Assets/Level/Prefabs/Minion.prefab", "", GameResourceType.AutoMinion);
            _minionPrefab = resource.LoadRessource<GameObject>();
        }

        public override GameObject PlaceTroops(Vector3 position)
        {
            // Platzierung der Verteidigertruppen

            if (GetBank().CurrentBalance >= _cost)
            {
                GetBank().Withdraw(_cost);
                return Instantiate(_minionPrefab, position, Quaternion.identity);
            }

            return null;
        }
    }
}