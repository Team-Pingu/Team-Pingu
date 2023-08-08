using Code.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlackoutAbility : MonoBehaviour, IAbility
{
    public float Range = 20f;
    public float Duration = 5;
    public GameObject PreSelection;
    public GameObject AbilityContent;
    public Color PreSelectColor = Color.red;

    private List<Tower> _selectedObjects;
    private bool _isInSelection = true;
    private readonly float _rangeMultiplier = 0.2f;
    private float _abilityStartTime = 0;
    private Color _resetPreSelectColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    public static BlackoutAbility ActiveInstance = null;
    public event Action OnAbilityApplied;

    public bool HasActiveInstance()
    {
        return ActiveInstance != null;
    }

    private void OnEnable()
    {
        Debug.Log("ActiveInstance SET");
        ActiveInstance?.AbortAbility();
        ActiveInstance = this;
    }

    private void OnDisable()
    {
        Debug.Log("ActiveInstance RESET");
        ActiveInstance = null;
    }

    void Start()
    {
        _selectedObjects = new List<Tower>();
        PreSelection.transform.localScale = new Vector3(Range * _rangeMultiplier, 1, Range * _rangeMultiplier);
        GameObjectStateTransition();
    }

    void Update()
    {
        if (_isInSelection)
        {
            CheckObjectsInRadius();
            MoveSelfWithMousePosition();
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                AbortAbility();
            }

            if (Input.GetMouseButtonDown(0))
            {
                ApplyAbility();
            }
        }
        else
        {
            float timePassedSinceStart = Time.time - _abilityStartTime;
            if (timePassedSinceStart > Duration)
            {
                FinishAbility();
            }
        }
    }

    private void GameObjectStateTransition()
    {
        if (_isInSelection)
        {
            PreSelection?.SetActive(true);
            AbilityContent?.SetActive(false);
        }
        else
        {
            PreSelection?.SetActive(false);
            AbilityContent?.SetActive(true);
        }
    }

    private void CheckObjectsInRadius()
    {
        var allObjects = FindObjectsByType<Tower>(FindObjectsSortMode.InstanceID);
        _selectedObjects.Clear();
        foreach (var obj in allObjects)
        {
            if (Vector3.Distance(obj.gameObject.transform.position, this.transform.position) <= Range)
            {
                // TODO: highlight objects here
                Debug.Log(obj.name);
                _selectedObjects.Add(obj);
                ColorGameObject(obj.gameObject, PreSelectColor);
            } else
            {
                //if (_selectedObjects.Contains(obj)) continue;
                ColorGameObject(obj.gameObject, _resetPreSelectColor);
            }
        }
    }

    private void MoveSelfWithMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitData))
        {
            Tile tile = hitData.collider.gameObject.GetComponent<Tile>();
            if (tile == null) return;
            //if (tile.isWalkable) return;

            //this.transform.position = hitData.point;
            this.transform.position = new Vector3(tile.transform.position.x, 1, tile.transform.position.z);
        }
    }

    private void ColorGameObject(GameObject go, Color color)
    {
        go = go.transform.Find("Mesh")?.gameObject ?? go;
        var renderers = go.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            // reset to original colo
            renderer.material.color = color;
        }
    }

    public void AbortAbility()
    {
        FinishAbility();
        // TODO: sell ability back?
    }

    public void FinishAbility()
    {
        GameObject.Destroy(gameObject);
        foreach (var obj in _selectedObjects)
        {
            ColorGameObject(obj.gameObject, _resetPreSelectColor);
            obj.EnableAttack(true);
        }
    }

    public void ApplyAbility()
    {
        _isInSelection = false;
        _abilityStartTime = Time.time;
        GameObjectStateTransition();
        foreach (var obj in _selectedObjects)
        {
            obj.EnableAttack(false);
        }
        ActiveInstance = null;
        OnAbilityApplied?.Invoke();
    }
}
