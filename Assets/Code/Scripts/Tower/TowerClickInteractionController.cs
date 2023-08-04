using Code.Scripts;
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
    private TextMeshPro _numKillsTextMP;
    private float _rangeMultiplier = 0.2f;

    void Start()
    {
        _tower = GetComponent<Tower>();
        _numKillsTextMP = NumKillsText?.GetComponent<TextMeshPro>();

        Menu.SetActive(IsMenuOpen);
    }

    void Update()
    {
        if (!IsMenuOpen) return;

        UpdateElements();
    }

    private void ToggleMenuVisibility()
    {
        IsMenuOpen = !IsMenuOpen;
        Menu.SetActive(IsMenuOpen);
    }

    private void OnMouseUp()
    {
        ToggleMenuVisibility();
    }

    private void UpdateElements()
    {
        _numKillsTextMP.text = $"{_tower.GetNumberOfKills()}";
        AttackRadiusCircle.transform.localScale = new Vector3(_tower.AttackRange * _rangeMultiplier, 1, _tower.AttackRange * _rangeMultiplier);
    }
}
