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

    public class Player : MonoBehaviour
    {
        [SerializeField]
        public PlayerRole Role;
        
        public PlayerController PlayerController;

        private void Start()
        {  
            if(NetworkManager.Singleton != null) {
                Role = PlayerRole.Defender;
                if(NetworkManager.Singleton.LocalClientId == 2) Role = PlayerRole.Attacker;
            }

            Debug.Log(Role);
            
            if (Role == PlayerRole.Defender) PlayerController = gameObject.AddComponent<DefenderPlayerController>();
            else PlayerController = gameObject.AddComponent<AttackerPlayerController>();
        }

        private void Update()
        {
        }
    }
}