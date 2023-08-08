using Code.Scripts.Player.Controller;
using Code.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Barricade : MonoBehaviour
{
    public int Health;
    public int AttackDamage;
    public int CurrencyReward = 50;
    public float AttackInterval = 5000; // 5s
    public bool IsAttacking { get; private set; } = false;
    public bool CanAttack { get; private set; } = true;
    public GameObject HitParticleSystem;
    public float speedDecrease = 0.5f;

    private DefenderPlayerController _defenderPlayerController;
    private UpgradeManager _upgradeManager;
    private Bank _bank;
    private Minion[] _lockedAttackTargets;
    private int _killedTargets = 0;
    private float _previousAttackTime = 0;
    private Collider _collider;

    private void Start()
    {
        _defenderPlayerController = FindObjectOfType<DefenderPlayerController>();
        _upgradeManager = FindObjectOfType<UpgradeManager>();
        _collider = GetComponent<Collider>();

        if (_defenderPlayerController == null) return;

        _bank = _defenderPlayerController.GetBank();
    }

    public void RewardGold()
    {
        if (_bank == null) return;

        _bank.Deposit((int)(CurrencyReward * _upgradeManager.MoneyBonusMultiplier));
    }

    public int GetNumberOfKills()
    {
        return _killedTargets;
    }

    public void FixedUpdate()
    {
        if (!CanAttack) return;

        _lockedAttackTargets = GetNextAttackTargets();
        if (Time.time * 1000f - _previousAttackTime > AttackInterval)
        {
            Attack(_lockedAttackTargets);
        }
    }

    public void Attack(Minion[] targets)
    {
        int damage = (int)(AttackDamage * _upgradeManager.AttackDamageMultiplier);
        foreach (Minion target in targets)
        {
            if (target == null) continue;
            // TODO: Slow down enemies
            MinionMover minionMover = target.GetComponent<MinionMover>();
            if (minionMover != null) minionMover.DecreaseSpeed(speedDecrease);

            bool isKillingHit = target.DamageSelf(damage, HitParticleSystem);
            if (isKillingHit)
            {
                _killedTargets++;
                RewardGold();
            }
        }

        _previousAttackTime = Time.time * 1000f;
    }

    private Minion[] GetNextAttackTargets()
    {
        Minion[] minions = FindObjectsOfType<Minion>();
        List<Minion> minionsInRange = new List<Minion>();

        foreach (Minion minion in minions)
        {
            // pre check to save calculation power
            float targetDistance = Vector3.Distance(transform.position, minion.transform.position);
            if (targetDistance > 15) continue;

            Collider minionCollider = minion.GetComponent<Collider>();
            if (minionCollider == null) continue;
            if (minionCollider.bounds.Intersects(_collider.bounds))
            {
                minionsInRange.Add(minion);
            } else
            {
                // TODO: Slow down enemies
                MinionMover minionMover = minion.GetComponent<MinionMover>();
                if (minionMover != null) minionMover.ResetSpeed();
            }
        }

        return minionsInRange.ToArray();
    }

    public void KillSelf()
    {
        GameObject.Destroy(gameObject);
    }

    public void EnableAttack(bool canAttack)
    {
        CanAttack = canAttack;
    }
}
