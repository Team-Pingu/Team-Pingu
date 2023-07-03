using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Unity.Netcode;

namespace Code.Scripts.Player.Controller
{
    // Enum für die verschiedenen Rollen des Spielers
    public enum PlayerRole
    {
        Attacker,
        Defender
    }

    public class Player : NetworkBehaviour
    {
        [SerializeField]
        public PlayerRole Role;
        
        public PlayerController PlayerController;

        private void Start()
        {
            Role = PlayerRole.Defender;
            if(NetworkManager.Singleton.LocalClientId == 2) Role = PlayerRole.Attacker;
            
            if (Role == PlayerRole.Defender) PlayerController = gameObject.AddComponent<DefenderPlayerController>();
            else PlayerController = gameObject.AddComponent<AttackerPlayerController>();
        }

        private void Update()
        {
        }
    }
}