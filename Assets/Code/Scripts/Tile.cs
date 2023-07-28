using System;
using Code.Scripts.Pathfinding;
using Code.Scripts.Player.Controller;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace Code.Scripts
{
    public class Tile : NetworkBehaviour
    {
        [SerializeField] public bool isPlaceable;
        [SerializeField] public bool isWalkable;

        private GridManager _gridManager;
        private Vector2Int _coordinates = new Vector2Int();
        private GameObject _objectSpawner;
        private Code.Scripts.Player.Controller.Player _player;

        // private DefenderPlayerController _defenderPlayerController;
        // private AttackerPlayerController _attackerPlayerController;

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

            this._objectSpawner = GameObject.Find("ObjectSpawner");
            this._player = GameObject.Find("Player").GetComponent<Code.Scripts.Player.Controller.Player>();
        }

        private void OnMouseDown()
        {
            #if !UNITY_EDITOR
            int playerID = (int) NetworkManager.Singleton.LocalClientId;
            if(IsServer || playerID > 2) return;
            #endif

            List<String> activeEntities = this._player.GetActiveEntity();
            if (activeEntities == null || activeEntities.Count == 0) return;

            if(_player.Role == PlayerRole.Attacker && isWalkable) {
                foreach (String prefabName in activeEntities)
                {
                    this._objectSpawner.GetComponent<ObjectSpawner>().SpawnAttackerUnitServerRpc(prefabName, this.transform.position);
                }
                this._player.ClearActiveEntity();
                return;
            }

            if(this._player.Role == PlayerRole.Defender && isPlaceable && activeEntities.Count > 0) {
                this._objectSpawner.GetComponent<ObjectSpawner>().SpawnDefenderUnitServerRpc(activeEntities[0], this.transform.position);
                isPlaceable = false;
                this._gridManager.BlockNode(this._coordinates);
                this._player.ClearActiveEntity();
            }
        }
    }
}
