using Code.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlackoutAbility : MonoBehaviour
{
    public float Range = 20f;
    public float Duration = 5;
    public GameObject PreSelection;
    public GameObject AbilityContent;

    private bool _isInSelection = true;
    private readonly float _rangeMultiplier = 0.2f;
    private float _abilityStartTime = 0;

    private void Awake()
    {

    }

    void Start()
    {
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
        foreach (var obj in allObjects)
        {
            if (Vector3.Distance(obj.gameObject.transform.position, this.transform.position) <= Range)
            {
                // TODO: highlight objects here
                Debug.Log(obj.name);
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

    private void AbortAbility()
    {
        GameObject.Destroy(gameObject);
        // TODO: sell ability back?
    }

    private void FinishAbility()
    {
        GameObject.Destroy(gameObject);
    }

    private void ApplyAbility()
    {
        _isInSelection = false;
        _abilityStartTime = Time.time;
        GameObjectStateTransition();
    }
}
