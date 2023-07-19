using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Scripts.Player.Controller
{
    public class AttackerPlayerController : PlayerController
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
            var aetherShieldBearer = new GameResource("Assets/Level/Prefabs/Units/AetherShieldBearer.prefab", "", GameResourceType.Minion);
            var clockworkScout = new GameResource("Assets/Level/Prefabs/Units/ClockworkScout.prefab", "", GameResourceType.Minion);
            var gearheadSapper = new GameResource("Assets/Level/Prefabs/Units/GearheadSapper.prefab", "", GameResourceType.Minion);
            var autoMinion = new GameResource("Assets/Level/Prefabs/Units/AutoMinion.prefab", "", GameResourceType.AutoMinion);
            var steamPoweredGoliath = new GameResource("Assets/Level/Prefabs/Units/SteamPoweredGoliath.prefab", "", GameResourceType.Minion);

            prefabList = new GameObject[] {aetherShieldBearer.LoadRessource<GameObject>(),
            clockworkScout.LoadRessource<GameObject>(), 
            gearheadSapper.LoadRessource<GameObject>(),
            autoMinion.LoadRessource<GameObject>(),
            steamPoweredGoliath.LoadRessource<GameObject>(),
            };
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