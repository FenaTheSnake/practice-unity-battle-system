using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class GameState
{
    public int Round {  get; private set; }

    List<Character> playerArmy;
    List<Character> enemyArmy;

    bool isPlayerTurn = true;

    Map _map;

    public delegate void PlayerTurnEnd();
    public event PlayerTurnEnd OnPlayerTurnEnd;

    public delegate void EnemyTurnEnd();
    public event EnemyTurnEnd OnEnemyTurnEnd;

    List<CharacterAction> currentCharacterActions;
    Character whoseCharacterActions;
    CharacterAction tempCharacterAction;

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
        return isPlayerTurn;
    }

    public void EndTurn()
    {
        if(isPlayerTurn)
        {
            foreach(Character c in enemyArmy)
            {
                c.PrepareForNewRound();
            }
            isPlayerTurn = false;
            OnPlayerTurnEnd?.Invoke();
        }
        else
        {
            foreach (Character c in playerArmy)
            {
                c.PrepareForNewRound();
            }
            isPlayerTurn = true;
            OnEnemyTurnEnd?.Invoke();
        }
    }

    public void CloseCharacterActions()
    {
        // bad bad very bad but im low on time
        GameObject.Find("ActionPanel").GetComponent<ActionPanel>().ClearButtons();
    }

    public void OpenCharacterActions(Character c)
    {
        whoseCharacterActions = c;
        currentCharacterActions = c.actionSet;

        // bad bad very bad but im low on time
        GameObject.Find("ActionPanel").GetComponent<ActionPanel>().CreateButtonsForCharacterActions(currentCharacterActions);
    }

    public void ExecuteCharacterAction(CharacterAction action)
    {
        if (!whoseCharacterActions.CanUseAction(action)) return;

        switch(action.actionType)
        {
            case CharacterActionType.ENEMY_TARGET:
                tempCharacterAction = action;
                // bad bad very bad but im low on time
                GameObject.Find("ActionPanel").GetComponent<ActionPanel>().ClearButtons();

                whoseCharacterActions.DrawAccessableEnemies();
                break;

            case CharacterActionType.CELL:
                tempCharacterAction = action;
                // bad bad very bad but im low on time
                GameObject.Find("ActionPanel").GetComponent<ActionPanel>().ClearButtons();

                whoseCharacterActions.DrawAccessableCells();
                break;
        }

        TextMeshProUGUI text = GameObject.Find("UnitStatsText").GetComponent<TextMeshProUGUI>();
        text.text = action.actionDescription;
    }

    public void ExecuteSavedCharacterActionOnTarget(Character target)
    {
        Debug.Assert(tempCharacterAction != null);
        whoseCharacterActions.SpendManaAndAPOnAction(tempCharacterAction);

        tempCharacterAction.PerformTarget(whoseCharacterActions, target);
        tempCharacterAction = null;
    }
    public void ExecuteSavedCharacterActionOnCell(MapCell cell)
    {
        Debug.Assert(tempCharacterAction != null);
        whoseCharacterActions.SpendManaAndAPOnAction(tempCharacterAction);

        tempCharacterAction.PerformCell(cell, whoseCharacterActions);
        tempCharacterAction = null;
    }

    public void SetUnitStatsText(Character c)
    {
        TextMeshProUGUI text = GameObject.Find("UnitStatsText").GetComponent<TextMeshProUGUI>();

        text.text = c.StatsAsText();
    }
    public void ClearUnitStatsText()
    {
        TextMeshProUGUI text = GameObject.Find("UnitStatsText").GetComponent<TextMeshProUGUI>();

        text.text = "";
    }
}
