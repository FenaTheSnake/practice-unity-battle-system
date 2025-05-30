using System;
using UnityEngine;

public enum AttackType
{
    MELEE,  // can attack only when standing nearby
    RANGED  // can attack any enemy in movement range.
}

public class Character : MapEntity
{
    [SerializeField] public bool isPlayerUnit;
    [SerializeField] public float movementRange = 4;
    [SerializeField] float maxHealth = 100.0f;     
    [SerializeField] float strength = 10.0f;
    [SerializeField] float defense = 0.05f;     // 0.05f = 5% of any incoming damage is blocked.
    [SerializeField] public AttackType attackType = AttackType.MELEE;

    public float Health { get; private set; }
    public float RemainingMovement { get; private set; }

    HealthStatus _healthStatus;

    private void Start()
    {
        Health = maxHealth;

        _healthStatus = (Instantiate(Resources.Load("UI/HealthStatus", typeof(GameObject))) as GameObject).GetComponent<HealthStatus>();
        _healthStatus.anchor = transform;

        _healthStatus.Init();
        _healthStatus.UpdatePosition();
    }

    public void PrepareForNewRound()
    {
        RemainingMovement = movementRange;
    }

    public bool IsMovementPossible(Vector2Int where)
    {
        return Vector2Int.Distance(MapPosition, where) <= RemainingMovement;
    }

    public void Move(Vector2Int where)
    {
        if (!IsMovementPossible(where))
        {
            Debug.LogWarning("Movement is not allowed: By " + name + " from " + MapPosition + " to " + where + " with remainingMovement " + RemainingMovement);
            return;
        }

        RemainingMovement -= Vector2Int.Distance(MapPosition, where);
        SetPosition(where);

        _healthStatus.UpdatePosition();
    }

    public void Attack(Character c)
    {
        c.RecieveDamage(strength);
        RemainingMovement = 0;
    }

    public void RecieveDamage(float damage)
    {
        Health -= damage * (1.0f - defense);

        _healthStatus.SetHealthText(Health, maxHealth);

        if(Health <= 0)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        throw new NotImplementedException();
    }
}
