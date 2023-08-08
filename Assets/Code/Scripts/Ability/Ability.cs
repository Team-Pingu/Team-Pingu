using System;

interface IAbility
{
    public event Action OnAbilityApplied;
    public bool HasActiveInstance();
    public void AbortAbility();
    public void FinishAbility();
    public void ApplyAbility();
}
