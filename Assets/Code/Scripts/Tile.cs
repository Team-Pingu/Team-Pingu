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
        private GameObject _objectSpawnManager;

        
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

            _objectSpawnManager = GameObject.Find("ServerObjectSpawner");
        }

        private void OnMouseDown()
        {
            int localId =  (int) NetworkManager.Singleton.LocalClientId;
            if(IsServer || localId > 2) return;

            // if((int) NetworkManager.Singleton.LocalClientId  == 1) 
            _objectSpawnManager.GetComponent<ObjectSpawnManager>().SpawnObjectServerRpc(localId, transform.position);
            Debug.Log("Mouse Clicked");
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnEntityServerRpc() {

            // this.PlayerController.GetComponent<PlayerObject>();

            // if (_defenderPlayerController == null)
            // {
            //     _defenderPlayerController = FindObjectOfType<DefenderPlayerController>();
            // }

            // if (_attackerPlayerController == null)
            // {
            //     _attackerPlayerController = FindObjectOfType<AttackerPlayerController>();
            // }

            // GameObject spawnedEntity = null;

            // if(isPlaceable && _defenderPlayerController != null) {
            //     Debug.Log("Placing defender troop");
            //     spawnedEntity = _defenderPlayerController.PlaceTroops(transform.position);

            //     if (spawnedEntity == null) return;
                
            //     isPlaceable = false;
            //     _gridManager.BlockNode(_coordinates);

            //     Debug.Log(spawnedEntity);
            //     spawnedEntity.GetComponent<NetworkObject>().Spawn();
            //     return;
            // }
                        
            // if (isWalkable && _attackerPlayerController != null)
            // {
            //     spawnedEntity = _attackerPlayerController.PlaceTroops(transform.position);
            //     // nothing for now
            //     spawnedEntity.GetComponent<NetworkObject>().Spawn();
            // }

        }
    }
}
