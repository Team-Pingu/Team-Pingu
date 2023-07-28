using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IAbility
{
    public event Action OnAbilityApplied;
    public bool HasActiveInstance();
    public void AbortAbility();
    public void FinishAbility();
    public void ApplyAbility();
}
