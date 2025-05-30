using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameState
{
    public int Round {  get; private set; }

    List<Character> playerArmy;
    List<Character> enemyArmy;

    bool isPlayerMove = true;   // player is doing the move rn.

    Map _map;

    public delegate void PlayerTurnEnd();
    public event PlayerTurnEnd OnPlayerTurnEnd;

    public delegate void EnemyTurnEnd();
    public event EnemyTurnEnd OnEnemyTurnEnd;

    [Inject]
    GameState(Map map)
    {
        _map = map;
    }

    public void StartFight(List<Character> playerArmy, List<Character> enemyArmy)
    {
        this.playerArmy = playerArmy;
        this.enemyArmy = enemyArmy;

        // Place armies on opposite sides of map.
        int y = Mathf.Max(_map.Height / (playerArmy.Count + 1) - 1, 1);
        foreach (Character c in playerArmy)
        {
            c.SetPosition(new Vector2Int(0, y));
            y += Mathf.Max(_map.Height / (playerArmy.Count + 1), 1);
            c.PrepareForNewRound();
        }

        y = _map.Height / (playerArmy.Count + 1);
        foreach (Character c in enemyArmy)
        {
            c.SetPosition(new Vector2Int(_map.Width - 1, y));
            y += Mathf.Max(_map.Height / (playerArmy.Count + 1), 1);
            c.PrepareForNewRound();
        }

        Round = 0;
    }

    public bool CanHoverAndClickCells()
    {
        return isPlayerMove;
    }

    public void EndTurn()
    {
        if(isPlayerMove)
        {
            foreach(Character c in enemyArmy)
            {
                c.PrepareForNewRound();
            }
            isPlayerMove = false;
            OnPlayerTurnEnd.Invoke();
        }
        else
        {
            foreach (Character c in playerArmy)
            {
                c.PrepareForNewRound();
            }
            isPlayerMove = true;
            OnEnemyTurnEnd.Invoke();
        }
    }
}
