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
    public float HealthMultiplier = 1f;
    [SerializeField]
    public float MovementSpeedMultiplier = 1f;
    [SerializeField]
    public float AttackDamageMultiplier = 1f;
    [SerializeField]
    public float AttackKnockbackMultiplier = 0.1f;
    [SerializeField]
    public float AttackSpeedMultiplier = 1f;
    [SerializeField]
    public float MoneyBonusMultiplier = 1f;
    [SerializeField]
    public float AttackRangeMultiplier = 1f;

    [SerializeField]
    public float AttackAreaOfEffectMultiplier = 1f;

    [SerializeField]
    public bool MinionsCanOpenAlternativePath = false;

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
        HealthMultiplier *= updateDiff;
    }

    /// <summary>
    /// Updates the global movement speed multiplier for units
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global movement speed multiplier</param>
    public void UpdateMovementSpeedMultiplier(float updateDiff)
    {
        MovementSpeedMultiplier *= updateDiff;
    }

    /// <summary>
    /// Updates the global attack damage multiplier for units
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global attack damage multiplier</param>
    public void UpdateAttackDamageMultiplier(float updateDiff)
    {
        AttackDamageMultiplier *= updateDiff;
    }

    /// <summary>
    /// Updates the global attack knockback multiplier for units
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global attack knockback multiplier</param>
    public void UpdateAttackKnockbackMultiplier(float updateDiff)
    {
        AttackKnockbackMultiplier *= updateDiff;
    }

    /// <summary>
    /// Updates the global attack speed multiplier for units
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global attack speed multiplier</param>
    public void UpdateAttackSpeedMultiplier(float updateDiff)
    {
        AttackSpeedMultiplier *= updateDiff;
    }

    /// <summary>
    /// Updates the global money bonus multiplier
    /// </summary>
    /// <param name="updateDiff">negative or positive delta which is added to the global money bonus multiplier</param>
    public void UpdateMoneyBonusMultiplier(float updateDiff)
    {
        MoneyBonusMultiplier *= updateDiff;
    }

    public void UpdateAttackRangeMultiplier(float updateDiff)
    {
        AttackRangeMultiplier *= updateDiff;
    }

    public void UpdateAttackAreaOfEffectMultiplier(float updateDiff)
    {
        AttackAreaOfEffectMultiplier *= updateDiff;
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
