using System;
using UnityEngine;

public class Character : MapEntity
{
    [SerializeField] public bool isPlayerUnit;
    [SerializeField] public float movementRange = 4;
    [SerializeField] float health = 100.0f;     
    [SerializeField] float strength = 10.0f;
    [SerializeField] float defense = 0.05f;     // 0.05f = 5% of any incoming damage is blocked.

    public float Health { get; private set; }
    public float RemainingMovement { get; private set; }

    private void Start()
    {
        Health = health;
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
    }

    public void Attack(Character c)
    {
        c.RecieveDamage(strength);
        RemainingMovement = 0;
    }

    public void RecieveDamage(float damage)
    {
        Health -= damage * (1.0f - defense);
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
