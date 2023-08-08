using Code.Scripts.Player.Controller;
using Code.Scripts;
using Microlight.MicroBar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreTower : MonoBehaviour
{
    public int Health;
    private MicroBar _healthBar;
    public GameObject DeathParticleSystem;
    public float DeathParticleSystemScale = 1f;
    public GameObject HitParticleSystem;
    public float HitParticleSystemScale = 1f;
    private Collider _collider;
    public GameObject RotationPart;
    public Vector3 RotationPartRotation = new Vector3(1, 0, 0);
    private int _initialHealth;

    private void Awake()
    {
        _healthBar = transform.GetComponentInChildren<MicroBar>();
        _healthBar?.Initialize(Health);

        _collider = GetComponent<Collider>();

        _initialHealth = Health;
    }

    private void Update()
    {
        if (RotationPart == null) return;
        float rotationMultiplier = Health <= 0.25 * _initialHealth ? 10 : 1;
        RotationPart.transform.Rotate(RotationPartRotation * rotationMultiplier);
    }

    public void KillSelf()
    {
        //GameObject.Destroy(gameObject);
        Time.timeScale = 0.05f;
        // TODO: END GAME
        if (DeathParticleSystem != null)
        {
            var go = GameObject.Instantiate(
                DeathParticleSystem,
                new Vector3(transform.position.x, transform.position.y + _collider.bounds.size.y, transform.position.z),
                Quaternion.identity
            );
            go.transform.localScale = new Vector3(DeathParticleSystemScale, DeathParticleSystemScale, DeathParticleSystemScale);
        }

        foreach(var minion in FindObjectsByType<Minion>(FindObjectsSortMode.None))
        {
            minion.DamageSelf(9999, DeathParticleSystem);
        }
    }

    public bool DamageSelf(int damage)
    {
        Health -= damage;
        _healthBar?.UpdateHealthBar(Health);
        if (HitParticleSystem != null)
        {
            var go = GameObject.Instantiate(
                HitParticleSystem,
                new Vector3(transform.position.x, transform.position.y + _collider.bounds.size.y, transform.position.z),
                Quaternion.identity
            );
            go.transform.localScale = new Vector3(HitParticleSystemScale, HitParticleSystemScale, HitParticleSystemScale);
        }

        if (Health <= 0)
        {
            KillSelf();
            return true;
        }
        return false;
    }
}
