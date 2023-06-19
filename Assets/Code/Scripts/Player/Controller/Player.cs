using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] public GameObject chooseRoleUI;

        private Button _attackerButton;
        private Button _defenderButton;
        private GameObject _chooseRoleUIInstance;
        
        private PlayerController _playerController;

        private void Awake()
        {
            if (chooseRoleUI == null) return;

            _chooseRoleUIInstance = Instantiate(chooseRoleUI);
            _chooseRoleUIInstance.SetActive(true);
            
            _attackerButton = _chooseRoleUIInstance.transform.Find("Canvas/AttackerButton").GetComponent<Button>();
            _defenderButton = _chooseRoleUIInstance.transform.Find("Canvas/DefenderButton").GetComponent<Button>();
            
            _attackerButton.onClick.AddListener(() =>
            {
                _playerController = gameObject.AddComponent<AttackerPlayerController>();
                _playerController.role = PlayerController.Role.Attacker;
                DeactivateUI();
            });
            
            _defenderButton.onClick.AddListener(() =>
            {
                _playerController = gameObject.AddComponent<DefenderPlayerController>();
                _playerController.role = PlayerController.Role.Defender;
                DeactivateUI();
            });
        }

        private void Start()
        {
            ActivateUI();
        }

        private void Update()
        {
            if (_playerController == null)
            {
                ActivateUI();
            }
        }

        private void ActivateUI()
        {
            _chooseRoleUIInstance.SetActive(true);
        }
        
        private void DeactivateUI()
        {
            _chooseRoleUIInstance.SetActive(false);
        }
    }
}