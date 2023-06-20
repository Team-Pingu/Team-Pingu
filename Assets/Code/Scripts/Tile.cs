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
            if(IsServer) return;

            Debug.Log("Mouse Clicked");
            SpawnEntityServerRpc();

        }

        [ServerRpc]
        private void SpawnEntityServerRpc() {

            Debug.Log("Spawning Entity Server Rpc called");

            if (_defenderPlayerController == null)
            {
                _defenderPlayerController = FindObjectOfType<DefenderPlayerController>();
            }
            
            if (_attackerPlayerController == null)
            {
                _attackerPlayerController = FindObjectOfType<AttackerPlayerController>();
            }
            
            GameObject spawnedEntity = null;
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

            if (isWalkable && _attackerPlayerController != null)
            {
                spawnedEntity = _attackerPlayerController.PlaceTroops(transform.position);
                // nothing for now
            }

            spawnedEntity.GetComponent<NetworkObject>().Spawn();
        }
    }
}
