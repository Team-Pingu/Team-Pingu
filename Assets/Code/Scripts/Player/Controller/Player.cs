using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        // Enum für die verschiedenen Rollen des Spielers
        public enum Role
        {
            Attacker,
            Defender
        }

        [SerializeField]
        private Role _role;
        
        private PlayerController _playerController;

        private void Start()
        {
            if (_role == Role.Defender)
            {
                _playerController = gameObject.AddComponent<DefenderPlayerController>();
            } else
            {
                _playerController = gameObject.AddComponent<AttackerPlayerController>();
            }
        }

        private void Update()
        {
        }
    }
}