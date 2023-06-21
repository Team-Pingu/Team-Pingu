using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles global multipliers and specializations for units
/// Is handled only on client side
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    [SerializeField]
    private float HealthMultiplier = 1f;
    [SerializeField]
    private float MovementSpeedMultiplier = 1f;
    [SerializeField]
    private float AttackDamageMultiplier = 1f;
    [SerializeField]
    private float AttackKnockbackMultiplier = 1f;
    [SerializeField]
    private float AttackSpeedMultiplier = 1f;

    [SerializeField]
    private bool MinionsCanOpenAlternativePath = false;

    #region MonoBehaviour Methods
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Multiplier Update Methods
    /// <summary>
    /// Updates the global health multiplier for units
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global health multiplier</param>
    public void UpdateHealthMultiplier(float updateDiff)
    {
        HealthMultiplier += updateDiff;
    }

    /// <summary>
    /// Updates the global movement speed multiplier for units
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global movement speed multiplier</param>
    public void UpdateMovementSpeedMultiplier(float updateDiff)
    {
        MovementSpeedMultiplier += updateDiff;
    }

    /// <summary>
    /// Updates the global attack damage multiplier for units
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global attack damage multiplier</param>
    public void UpdateAttackDamageMultiplier(float updateDiff)
    {
        AttackDamageMultiplier += updateDiff;
    }

    /// <summary>
    /// Updates the global attack knockback multiplier for units
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global attack knockback multiplier</param>
    public void UpdateAttackKnockbackMultiplier(float updateDiff)
    {
        AttackKnockbackMultiplier += updateDiff;
    }

    /// <summary>
    /// Updates the global attack speed multiplier for units
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global attack speed multiplier</param>
    public void UpdateAttackSpeedMultiplier(float updateDiff)
    {
        AttackSpeedMultiplier += updateDiff;
    }
    #endregion

    #region Specialization Update Methods
    /// <summary>
    /// Sets the specialization for whether Minions can open alternative paths
    /// </summary>
    /// <param name="newState">new state for the specializations</param>
    public void UpdateCanMinionsOpenAlternativePath(bool newState)
    {
        MinionsCanOpenAlternativePath = newState;
    }
    #endregion
}
