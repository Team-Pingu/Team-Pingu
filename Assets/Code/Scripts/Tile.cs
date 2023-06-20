using System;
using Code.Scripts.Pathfinding;
using Code.Scripts.Player;
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
            Debug.Log("Mouse Clicked");
            if(IsServer) return;

            Debug.Log(IsServer);
            SpawnEntityServerRpc((int) NetworkManager.Singleton.LocalClientId);

        }

        [ServerRpc]
        private void SpawnEntityServerRpc(int playerID) {
            if(playerID > 2) return;

            Debug.Log("player ID: " + playerID);
            Debug.Log("Spawning Entity Server Rpc called");

            GameObject spawnedEntity = null;

            if(playerID == 1) {
                if (_defenderPlayerController == null)
                {
                    _defenderPlayerController = FindObjectOfType<DefenderPlayerController>();
                }

                if (isPlaceable && _defenderPlayerController != null)
                {
                    // Instantiate(buildingPrefab, transform.position, Quaternion.identity);
                    // isPlaceable = false;
                    // _gridManager.BlockNode(_coordinates);
                    
                    spawnedEntity = _defenderPlayerController.PlaceTroops(transform.position);

                    if (spawnedEntity == null) return;
                    
                    isPlaceable = false;
                    _gridManager.BlockNode(_coordinates);
                
                }
                spawnedEntity.GetComponent<NetworkObject>().Spawn();
                return;
            }
            
            
            if (_attackerPlayerController == null)
            {
                _attackerPlayerController = FindObjectOfType<AttackerPlayerController>();
            }
            
            if (isWalkable && _attackerPlayerController != null)
            {
                spawnedEntity = _attackerPlayerController.PlaceTroops(transform.position);
                // nothing for now
            }
            spawnedEntity.GetComponent<NetworkObject>().Spawn();

        }
    }
}
