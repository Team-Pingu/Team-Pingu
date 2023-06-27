using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Scripts.Player
{
    // Enum für die verschiedenen Rollen des Spielers
    public enum PlayerRole
    {
        Attacker,
        Defender
    }

    public class Player : MonoBehaviour
    {
        [SerializeField]
        public PlayerRole Role;
        
        public PlayerController PlayerController;

        private void OnEnable()
        {
            if (Role == PlayerRole.Defender)
            {
                PlayerController = gameObject.AddComponent<DefenderPlayerController>();
            } else
            {
                PlayerController = gameObject.AddComponent<AttackerPlayerController>();
            }
        }

        private void Update()
        {
        }
    }
}