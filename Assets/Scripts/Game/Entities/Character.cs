using System.Collections.Generic;
using System;
using UnityEngine;
using Zenject;

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
    [SerializeField] float maxMana = 40.0f;     
    [SerializeField] float strength = 10.0f;
    [SerializeField] float defense = 0.05f;     // 0.05f = 5% of any incoming damage is blocked.
    [SerializeField] public AttackType attackType = AttackType.MELEE;

    [SerializeField] public List<CharacterAction> actionSet;

    private float m_Health = 0;
    private float m_Mana = 0;
    private float m_RemainingMovement = 0;

    public float Health { get => m_Health; private set { m_Health = Mathf.Min(Mathf.Max(value, 0), maxHealth); } }
    public float Mana { get => m_Mana; private set { m_Mana = Mathf.Min(Mathf.Max(value, 0), maxMana); } }
    public int AP { get; private set; }
    public float RemainingMovement { get => m_RemainingMovement; private set { m_RemainingMovement = Mathf.Min(Mathf.Max(value, 0), movementRange); } }

    HealthStatus _healthStatus;

    Map _map;

    [Inject]
    public void Construct(Map map)
    {
        _map = map;
    }

    private void Start()
    {
        Health = maxHealth;
        Mana = maxMana;
        AP = 0;

        _healthStatus = (Instantiate(Resources.Load("UI/HealthStatus", typeof(GameObject))) as GameObject).GetComponent<HealthStatus>();
        _healthStatus.anchor = transform;

        _healthStatus.Init();
        _healthStatus.UpdatePosition();
    }

    public void PrepareForNewRound()
    {
        RemainingMovement = movementRange;
        Mana = Mathf.Min(maxMana, Mana + 5);
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
        c.RecieveDamage(strength, true);
    }

    public void RecieveDamage(float damage, bool useDefense)
    {
        if (useDefense)
        {
            Health -= damage * (1.0f - defense);
        }else
        {
            Health -= damage;
        }

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

    public void DrawAccessableCells()
    {
        Vector2Int center = MapPosition;
        int radius = (int)RemainingMovement;

        for (int i = center.x - radius; i <= center.x + radius; i++)
        {
            for (int j = center.y - radius; j <= center.y + radius; j++)
            {
                if (i < 0 || j < 0 || i >= _map.Width || j >= _map.Height) continue;
                float d = Vector2Int.Distance(center, new Vector2Int(i, j));
                if (d > radius) continue;

                MapCell cell = _map.map[new Vector2Int(i, j)];
                if (cell.entity != null) continue;

                cell.AddState(MapCellState.ACCESSABLE);
            }
        }
    }

    public void DrawAccessableEnemies()
    {
        Vector2Int center = MapPosition;
        int radius = (int)RemainingMovement;

        for (int i = center.x - radius; i <= center.x + radius; i++)
        {
            for (int j = center.y - radius; j <= center.y + radius; j++)
            {
                if (i < 0 || j < 0 || i >= _map.Width || j >= _map.Height) continue;
                float d = Vector2Int.Distance(center, new Vector2Int(i, j));
                if (d > radius) continue;

                MapCell cell = _map.map[new Vector2Int(i, j)];
                if (cell.entity != null)
                {
                    if (cell.entity is Character c && !c.isPlayerUnit)
                    {
                        if (attackType == AttackType.RANGED || (d <= 1)) cell.AddState(MapCellState.ENEMY);
                    }
                    continue;
                }
            }
        }
    }

    public void DrawAccessableAllies()
    {
        Vector2Int center = MapPosition;
        int radius = (int)RemainingMovement;

        for (int i = center.x - radius; i <= center.x + radius; i++)
        {
            for (int j = center.y - radius; j <= center.y + radius; j++)
            {
                if (i < 0 || j < 0 || i >= _map.Width || j >= _map.Height) continue;
                float d = Vector2Int.Distance(center, new Vector2Int(i, j));
                if (d > radius) continue;

                MapCell cell = _map.map[new Vector2Int(i, j)];
                if (cell.entity != null)
                {
                    if (cell.entity is Character c && c.isPlayerUnit)
                    {
                        cell.AddState(MapCellState.ACCESSABLE);
                    }
                    continue;
                }
            }
        }
    }

    public string StatsAsText()
    {
        return  name + 
                "\nHP: " + Health + "/" + maxHealth + 
                "\nMP: " + Mana + "/" + maxMana + 
                "\nAP: " + AP + 
                "\nОчки действия: " + (int)RemainingMovement + "/" + movementRange + 
                "\nАтака: " + strength + " (" + (attackType == AttackType.MELEE ? "Ближ." : "Дальн.") + ")" +
                "\nЗащита:" + defense * 100 + "%";
    }

    public bool CanUseAction(CharacterAction action)
    {
        return Mana >= action.requiredMP && AP >= action.requiredAP && RemainingMovement >= action.requiredMovement;
    }
    public void SpendManaAndAPOnAction(CharacterAction action)
    {
        Mana -= action.requiredMP;
        AP -= action.requiredAP;
        RemainingMovement -= action.requiredMovement;
    }
    public void AddAP(int amount)
    {
        AP += amount;
    }
    public void Heal(float amount)
    {
        Health += amount;
        _healthStatus.SetHealthText(Health, maxHealth);
    }
}
