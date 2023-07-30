using Code.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteAbility : MonoBehaviour, IAbility
{
    public float Range = 30f;
    public int Damage = 80;
    public float Duration = 3;
    public GameObject PreSelection;
    public GameObject PreSelectionGround;
    public GameObject AbilityContent;
    public GameObject ExplosionParticleSystem;
    public float ExplosionParticleSystemScale = 3;
    public Color PreSelectColor = Color.red;

    private List<Minion> _selectedObjects;
    private bool _isInSelection = true;
    private readonly float _rangeMultiplier = 0.2f;
    private float _abilityStartTime = 0;
    private Color _resetPreSelectColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private UpgradeManager _upgradeManager;

    public static DynamiteAbility ActiveInstance = null;
    public event Action OnAbilityApplied;

    public bool HasActiveInstance()
    {
        return ActiveInstance != null;
    }

    private void OnEnable()
    {
        ActiveInstance = this;
    }

    private void OnDisable()
    {
        ActiveInstance = null;
    }

    void Start()
    {
        _upgradeManager = GameObject.FindFirstObjectByType<UpgradeManager>();
        _selectedObjects = new List<Minion>();
        PreSelectionGround.transform.localScale = new Vector3(Range * _rangeMultiplier, 1, Range * _rangeMultiplier);
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
                Explode();
                FinishAbility();
            }
        }
    }

    private void Explode()
    {
        var go = GameObject.Instantiate(ExplosionParticleSystem, this.transform.position, Quaternion.identity);
        go.transform.localScale = new Vector3(ExplosionParticleSystemScale, ExplosionParticleSystemScale, ExplosionParticleSystemScale);
        
        var allObjects = FindObjectsByType<Minion>(FindObjectsSortMode.InstanceID);
        foreach (var obj in allObjects)
        {
            if (Vector3.Distance(obj.gameObject.transform.position, this.transform.position) <= Range)
            {
                // Damage minions with no fall-off in radius
                obj.DamageSelf((int)(Damage * _upgradeManager.AttackDamageMultiplier));
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
                // TODO: highlight objects here
                Debug.Log(obj.name);
                _selectedObjects.Add(obj);
                ColorGameObject(obj.gameObject, PreSelectColor);
            }
            else
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
            if (!tile.isWalkable) return;

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
        }
    }

    public void ApplyAbility()
    {
        _isInSelection = false;
        _abilityStartTime = Time.time;
        GameObjectStateTransition();
        ActiveInstance = null;
        OnAbilityApplied?.Invoke();
        foreach(var obj in _selectedObjects)
        {
            ColorGameObject(obj.gameObject, _resetPreSelectColor);
        }
    }
}
