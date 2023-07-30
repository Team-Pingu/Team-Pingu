using System;
using Code.Scripts.Pathfinding;
using Code.Scripts.Player.Controller;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

namespace Code.Scripts
{
    public class Tile : NetworkBehaviour
    {
        public bool isPlaceable;
        public bool isWalkable;
        public bool IsSelectable { get; private set; } = false;
        public GameObject HighlightObject;
        public float HightlightObjectScale = 1f;

        private GridManager _gridManager;
        private Vector2Int _coordinates = new Vector2Int();
        private GameObject _objectSpawner;
        private Code.Scripts.Player.Controller.Player _player;
        private TileHighlightManager _tileHighlightManager;
        private Outline _outline;

        private void Awake()
        {
            _gridManager = FindObjectOfType<GridManager>();
            _outline = gameObject.AddComponent<Outline>();

            _outline.OutlineMode = Outline.Mode.OutlineAll;
            _outline.OutlineColor = Color.yellow;
            _outline.OutlineWidth = 10f;

            //_meshRenderer = transform.Find("Mesh")?.GetComponent<Renderer>();
            _tileHighlightManager = GameObject.FindFirstObjectByType<TileHighlightManager>();
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
            DisableOutline();
        }

        public void SetIsSelectable(bool selectable)
        {
            IsSelectable = selectable;
        }

        private void OnMouseEnter()
        {
            if (!IsSelectable) return;
            //_meshRenderer.material.color = Color.green;
            EnableOutline();
        }

        private void OnMouseExit()
        {
            if (!IsSelectable) return;
            //_meshRenderer.material.color = _initialMaterialColor;
            DisableOutline();
        }

        private void OnMouseDown()
        {
            if (!IsSelectable) return;

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
                _tileHighlightManager.ResetMarkTiles();
                return;
            }

            if(this._player.Role == PlayerRole.Defender && isPlaceable && activeEntities.Count > 0) {
                this._objectSpawner.GetComponent<ObjectSpawner>().SpawnDefenderUnitServerRpc(activeEntities[0], this.transform.position);
                isPlaceable = false;
                this._gridManager.BlockNode(this._coordinates);
                this._player.ClearActiveEntity();
                _tileHighlightManager.ResetMarkTiles();
                return;
            }
        }

        public void EnableOutline()
        {
            _outline.enabled = true;
            //_outline.OutlineMode = Outline.Mode.OutlineAll;
        }

        public void DisableOutline()
        {
            _outline.enabled = false;
            //_outline.OutlineMode = Outline.Mode.OutlineHidden;
        }

        private GameObject _markedTileGo = null;

        public void MarkTile()
        {
            if (HighlightObject == null) return;
            _markedTileGo = Instantiate(HighlightObject, transform);
            _markedTileGo.transform.position = new Vector3(
                _markedTileGo.transform.position.x,
                _markedTileGo.transform.position.y + 0.5f,
                _markedTileGo.transform.position.z
            );
            _markedTileGo.transform.localScale = new Vector3(HightlightObjectScale, HightlightObjectScale, HightlightObjectScale);
        }

        public void UnmarkTile()
        {
            if (_markedTileGo == null) return;
            Destroy(_markedTileGo);
        }
    }
}
