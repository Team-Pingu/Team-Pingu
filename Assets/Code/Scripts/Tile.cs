using System;
using Code.Scripts.Pathfinding;
using Code.Scripts.Player.Controller;
using UnityEngine;
using Unity.Netcode;

namespace Code.Scripts
{
    public class Tile : NetworkBehaviour
    {
        [SerializeField] public bool isPlaceable;
        [SerializeField] public bool isWalkable;

        private GridManager _gridManager;
        private Vector2Int _coordinates = new Vector2Int();

        private DefenderPlayerController _defenderPlayerController;
        private AttackerPlayerController _attackerPlayerController;

        private void Awake()
        {
            _gridManager = FindObjectOfType<GridManager>();
        }

        private void Start()
        {
            this._defenderPlayerController = FindObjectOfType<DefenderPlayerController>();
            this._attackerPlayerController = FindObjectOfType<AttackerPlayerController>();

            if (_gridManager != null)
            {
                _coordinates = _gridManager.GetCoordinatesFromPosition(transform.position);
                
                if (!isPlaceable && !isWalkable)
                {
                    _gridManager.BlockNode(_coordinates);
                }
            }
        }

        private void OnMouseDown()
        {
            int playerID = (int) NetworkManager.Singleton.LocalClientId;
            if(IsServer || playerID > 2) return;

            GameObject objectSpawner = GameObject.Find("ObjectSpawner");
            Code.Scripts.Player.Controller.Player player = GameObject.Find("Player").GetComponent<Code.Scripts.Player.Controller.Player>();

            if(player.Role == PlayerRole.Attacker) {
                objectSpawner.GetComponent<ObjectSpawner>().SpawnAttackerUnitServerRpc(this.transform.position);
                return;
            }

            if(player.Role == PlayerRole.Defender) objectSpawner.GetComponent<ObjectSpawner>().SpawnDefenderUnitServerRpc(this.transform.position);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnEntityServerRpc(int playerID) {

            Debug.Log("player ID: " + playerID);
            Debug.Log("Spawning Entity Server Rpc called");

            GameObject spawnedEntity = null;

            if(playerID == 1 && isPlaceable && _defenderPlayerController != null) {
                // Instantiate(buildingPrefab, transform.position, Quaternion.identity);
                // isPlaceable = false;
                // _gridManager.BlockNode(_coordinates);
                Debug.Log("Placing defender troop");
                spawnedEntity = _defenderPlayerController.PlacePlaceholderUnit(transform.position);

                if (spawnedEntity == null) return;

                isPlaceable = false;
                _gridManager.BlockNode(_coordinates);

                Debug.Log(spawnedEntity);
                spawnedEntity.GetComponent<NetworkObject>().Spawn();
                return;
            }
                        
            if (playerID == 2 && isWalkable && _attackerPlayerController != null)
            {
                spawnedEntity = _attackerPlayerController.PlacePlaceholderUnit(transform.position);
                // nothing for now
                spawnedEntity.GetComponent<NetworkObject>().Spawn();
            }

        }
    }
}
