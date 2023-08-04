using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Scripts.Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

namespace Code.Scripts
{
    [RequireComponent(typeof(Minion))]
    public class MinionMover : NetworkBehaviour
    {
        [SerializeField][Range(0f, 5f)] private float initialSpeed = 1f;
        public float speed;

        private List<Node> _path = new List<Node>();
        private GridManager _gridManager;
        private Pathfinder _pathfinder;
        private Minion _minion;
        private UpgradeManager _upgradeManager;

        private Vector2Int _startCoordinate;

        private void Awake()
        {
            _gridManager = FindObjectOfType<GridManager>();
            _pathfinder = FindObjectOfType<Pathfinder>();
            _upgradeManager = FindObjectOfType<UpgradeManager>();
            speed = initialSpeed;
        }

        private void Start()
        {
            if(IsClient) return;

            FindPath();
            ReturnToStart();
            StartCoroutine(FollowPath()); 
            _minion = GetComponent<Minion>();
        }

        private void FindPath()
        {
            _path.Clear();

            _path = _pathfinder.GetNewPath().First(); // TODO: hier abändern, dass User Pfad wählen kann
            _startCoordinate = _path[0].coordinates;
        }

        /**
         * Gets the list of all paths.
         * The paths arte sorted by its lengths and first is the shortest.
         */
        private void FindShortestPath()
        {
            _path.Clear();

            _path = _pathfinder.GetNewPath().First();
            _startCoordinate = _path[0].coordinates;
        }

        private void ReturnToStart()
        {
            transform.position = _gridManager.GetPositionFromCoordinates(_startCoordinate);
        }

        private void FinishPath()
        {
            // for now disabling game object
            // gameObject.SetActive(false);
            
            //_minion.RewardGold();
            Destroy(gameObject);
        }

        IEnumerator FollowPath()
        {
            for(int i = 0; i < _path.Count; i++)
            {
                Vector3 startPosition = transform.position;
                Vector3 endPosition = _gridManager.GetPositionFromCoordinates(_path[i].coordinates);
                float travelPercent = 0f;
                
                transform.LookAt(endPosition);

                while (travelPercent < 1f)
                {
                    travelPercent += Time.deltaTime * speed * _upgradeManager.MovementSpeedMultiplier;
                    transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                    yield return new WaitForEndOfFrame();
                }
            }
            
            FinishPath();
        }

        public void DecreaseSpeed(float multiplier)
        {
            speed *= multiplier;
        }

        public void ResetSpeed()
        {
            speed = initialSpeed;
        }
    }
}