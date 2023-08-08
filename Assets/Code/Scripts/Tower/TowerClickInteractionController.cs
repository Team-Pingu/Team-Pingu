using Code.Scripts;
using Code.Scripts.Player.Controller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerClickInteractionController : MonoBehaviour
{
    public GameObject Menu;
    public bool IsMenuOpen = false;
    public GameObject NumKillsText;
    public GameObject AttackRadiusCircle;

    private Tower _tower;
    private Barricade _barricade;
    private TextMeshPro _numKillsTextMP;
    private float _rangeMultiplier = 0.2f;
    private bool _canOpenMenu = true;

    private void Awake()
    {
        _tower = GetComponent<Tower>();
        _barricade = GetComponent<Barricade>();
        _numKillsTextMP = NumKillsText?.GetComponent<TextMeshPro>();
        Menu.SetActive(IsMenuOpen);
    }

    void Start()
    {
        _numKillsTextMP.text = "0";
        if (FindAnyObjectByType<Player>()?.Role != PlayerRole.Defender)
        {
            // remove script
            Destroy(this);
        }
    }

    void Update()
    {
        if (!_canOpenMenu) return;
        if (!IsMenuOpen) return;

        UpdateElements();

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            DisableMenuVisibility();
        }
    }

    private void ToggleMenuVisibility()
    {
        IsMenuOpen = !IsMenuOpen;
        Menu.SetActive(IsMenuOpen);
    }

    private void DisableMenuVisibility()
    {
        IsMenuOpen = false;
        Menu.SetActive(IsMenuOpen);
    }

    private void EnableMenuVisibility()
    {
        IsMenuOpen = true;
        Menu.SetActive(IsMenuOpen);
    }

    //private void OnMouseUp()
    //{
    //    ToggleMenuVisibility();
    //}

    private void OnMouseEnter()
    {
        EnableMenuVisibility();
    }

    private void OnMouseExit()
    {
        DisableMenuVisibility();
    }

    private void UpdateElements()
    {
        if (_tower != null)
        {
            _numKillsTextMP.text = $"{_tower.GetNumberOfKills()}";
        } else if (_barricade != null)
        {
            _numKillsTextMP.text = $"{_barricade.GetNumberOfKills()}";
        }
        if (AttackRadiusCircle != null) AttackRadiusCircle.transform.localScale = new Vector3(_tower.AttackRange * _rangeMultiplier, 1, _tower.AttackRange * _rangeMultiplier);
    }
}
