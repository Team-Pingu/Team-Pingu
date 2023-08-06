using Code.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockFieldTrapAbility : MonoBehaviour
{
    public float Range = 20f;
    public float Duration = 8;
    public int Damage = 50;
    public GameObject PreSelection;
    public GameObject AbilityContent;
    public Color PreSelectColor = Color.blue;

    private List<GameObject> _selectedObjects;
    private bool _isInSelection = true;
    private readonly float _rangeMultiplier = 0.2f;
    private float _abilityStartTime = 0;
    private Color _resetPreSelectColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private UpgradeManager _upgradeManager;

    public static ShockFieldTrapAbility ActiveInstance = null;
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

    private void Awake()
    {
        _upgradeManager = FindObjectOfType<UpgradeManager>();
    }

    void Start()
    {
        _selectedObjects = new List<GameObject>();
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
        var allObjects = FindObjectsByType<Minion>(FindObjectsSortMode.InstanceID);
        _selectedObjects.Clear();
        foreach (var obj in allObjects)
        {
            if (Vector3.Distance(obj.gameObject.transform.position, this.transform.position) <= Range)
            {
                _selectedObjects.Add(obj.gameObject);
                ColorGameObject(obj.gameObject, PreSelectColor);
            }
            else
            {
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
    }

    public void FinishAbility()
    {
        GameObject.Destroy(gameObject);
        foreach (var obj in _selectedObjects)
        {
            if (obj == null) continue;
            ColorGameObject(obj.gameObject, _resetPreSelectColor);
            obj.GetComponent<MinionMover>()?.ResetSpeed();
        }
    }

    public void ApplyAbility()
    {
        _isInSelection = false;
        _abilityStartTime = Time.time;
        GameObjectStateTransition();
        foreach (var obj in _selectedObjects)
        {
            if (obj == null) continue;
            obj.GetComponent<MinionMover>()?.DecreaseSpeed(0.1f);
            obj.GetComponent<Minion>()?.DamageSelf(Damage);
        }
        ActiveInstance = null;
        OnAbilityApplied?.Invoke();
    }
}
