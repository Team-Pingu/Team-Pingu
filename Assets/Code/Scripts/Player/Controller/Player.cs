using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Object = UnityEngine.Object;

namespace Code.Scripts.Player.Controller
{
    public class Player : NetworkBehaviour
    {

        private Button _attackerButton;
        private Button _defenderButton;
        private GameObject _chooseRoleUIInstance;
        
        private PlayerController _playerController;

        // private void Awake()
        // {
        //     if (chooseRoleUI == null) return;

        //     _chooseRoleUIInstance = Instantiate(chooseRoleUI);
        //     _chooseRoleUIInstance.SetActive(true);
            
        //     _attackerButton = _chooseRoleUIInstance.transform.Find("Canvas/AttackerButton").GetComponent<Button>();
        //     _defenderButton = _chooseRoleUIInstance.transform.Find("Canvas/DefenderButton").GetComponent<Button>();
            
        //     _attackerButton.onClick.AddListener(() =>
        //     {
        //         _playerController.role = PlayerController.Role.Attacker;
        //         _playerController = gameObject.AddComponent<AttackerPlayerController>();
        //         DeactivateUI();
        //     });
            
        //     _defenderButton.onClick.AddListener(() =>
        //     {
        //         _playerController = gameObject.AddComponent<DefenderPlayerController>();
        //         _playerController.role = PlayerController.Role.Defender;
        //         DeactivateUI();
        //     });
        // }

        private void Start()
        {
            if(IsServer) return;
            
            if(NetworkManager.Singleton.LocalClientId == 1) {
                _playerController = gameObject.AddComponent<DefenderPlayerController>();
                _playerController.role = PlayerController.Role.Defender;
                return;
            }
            
            if(NetworkManager.Singleton.LocalClientId == 2) {
                _playerController = gameObject.AddComponent<AttackerPlayerController>();
                _playerController.role = PlayerController.Role.Attacker;
                return;
            }
            
            _playerController.role = PlayerController.Role.Spectator;
        }

        // private void Update()
        // {
        //     if (_playerController == null)
        //     {
        //         ActivateUI();
        //     }
        // }

        // private void ActivateUI()
        // {
        //     _chooseRoleUIInstance.SetActive(true);
        // }
        
        // private void DeactivateUI()
        // {
        //     _chooseRoleUIInstance.SetActive(false);
        // }
    }
}