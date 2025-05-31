using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameState
{
    public const int MAX_ROUNDS = 10;
    public int Round {  get; private set; }

    List<Character> playerArmy;
    List<Character> enemyArmy;
    List<CharacterAction> cards;

    bool isPlayerTurn = true;
    bool isChoosingCard = false;

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

    public void StartFight(List<Character> playerArmy, List<Character> enemyArmy, List<CharacterAction> cards)
    {
        this.playerArmy = playerArmy;
        this.enemyArmy = enemyArmy;
        this.cards = cards;

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
        return isPlayerTurn && !isChoosingCard;
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
            OnEnemyTurnEnd?.Invoke();

            isPlayerTurn = true;
            isChoosingCard = true;
            ShowRandomCards(2);

            Round += 1;
        }

        GameObject.Find("TurnsLeftText").GetComponent<TextMeshProUGUI>().text = (MAX_ROUNDS - Round) + " ходов осталось";
        CheckForWinLoseConditions();
    }

    void CheckForWinLoseConditions()
    {
        float allyHealth = 0;
        float enemyHealth = 0;
        foreach(Character c in playerArmy)
        {
            allyHealth += c.Health;
        }
        foreach (Character c in enemyArmy)
        {
            enemyHealth += c.Health;
        }

        if(allyHealth <= 0)
        {
            EndGame("Противник победил!");
        }
        if(enemyHealth <= 0)
        {
            EndGame("Вы победили!");
        }

        if(Round >= MAX_ROUNDS)
        {
            if(allyHealth > enemyHealth)
            {
                EndGame("Вы победили!");
            } 
            else if(enemyHealth > allyHealth)
            {
                EndGame("Противник победил!");
            } 
            else
            {
                EndGame("Ничья!");
            }
        }
    }

    void EndGame(string endText)
    {
        GameObject.Find("TurnsLeftText").GetComponent<TextMeshProUGUI>().text = endText;
        GameObject.Find("EndTurnButton").GetComponent<Button>().enabled = false;
        isPlayerTurn = false;
    }

    public void CloseCharacterActions()
    {
        // bad bad very bad but im low on time
        GameObject.Find("ActionPanel").GetComponent<ActionPanel>().ClearButtons();
    }

    public void ShowCharacterActions(List<CharacterAction> actions)
    {
        if (playerArmy.Count == 0) return;
        whoseCharacterActions = playerArmy[0];

        currentCharacterActions = actions;
        GameObject.Find("ActionPanel").GetComponent<ActionPanel>().CreateButtonsForCharacterActions(currentCharacterActions);
    }
    public void ShowCharacterActions(Character c)
    {
        whoseCharacterActions = c;
        currentCharacterActions = c.actionSet;

        // bad bad very bad but im low on time
        GameObject.Find("ActionPanel").GetComponent<ActionPanel>().CreateButtonsForCharacterActions(currentCharacterActions);
    }

    public void ExecuteCharacterAction(CharacterAction action)
    {
        if (!whoseCharacterActions.CanUseAction(action)) return;

        tempCharacterAction = action;
        // bad bad very bad but im low on time
        GameObject.Find("ActionPanel").GetComponent<ActionPanel>().ClearButtons();
        switch (action.actionType)
        {
            case CharacterActionType.ENEMY_TARGET:
                whoseCharacterActions.DrawAccessableEnemies();
                break;

            case CharacterActionType.CELL:
                whoseCharacterActions.DrawAccessableCells();
                break;

            case CharacterActionType.ALL_ALLIES:
                ExecuteSavedCharacterActionOnTargets(whoseCharacterActions.isPlayerUnit ? playerArmy : enemyArmy);
                break;

            case CharacterActionType.ALL_ENEMIES:
                ExecuteSavedCharacterActionOnTargets(whoseCharacterActions.isPlayerUnit ? enemyArmy : playerArmy);
                break;

            case CharacterActionType.ALL_UNITS:
                ExecuteSavedCharacterActionOnTargets(enemyArmy.Union(playerArmy).ToList());
                break;
        }

        TextMeshProUGUI text = GameObject.Find("UnitStatsText").GetComponent<TextMeshProUGUI>();
        text.text = action.actionDescription;

        isChoosingCard = false;
    }

    public void ExecuteSavedCharacterActionOnTarget(Character target)
    {
        Debug.Assert(tempCharacterAction != null);
        whoseCharacterActions.SpendManaAndAPOnAction(tempCharacterAction);

        tempCharacterAction.PerformTarget(whoseCharacterActions, target);
        tempCharacterAction = null;
    }
    public void ExecuteSavedCharacterActionOnTargets(List<Character> targets)
    {
        Debug.Assert(tempCharacterAction != null);
        whoseCharacterActions.SpendManaAndAPOnAction(tempCharacterAction);

        foreach (Character target in targets)
        {
            tempCharacterAction.PerformTarget(whoseCharacterActions, target);
        }
        tempCharacterAction = null;
    }
    public void ExecuteSavedCharacterActionOnCell(MapCell cell)
    {
        Debug.Assert(tempCharacterAction != null);
        whoseCharacterActions.SpendManaAndAPOnAction(tempCharacterAction);

        tempCharacterAction.PerformCell(cell, whoseCharacterActions);
        tempCharacterAction = null;
    }

    public void DisplayCharacterActionDescription(CharacterAction action)
    {
        TextMeshProUGUI text = GameObject.Find("UnitStatsText").GetComponent<TextMeshProUGUI>();
        text.text = action.actionDescription;
    }
    public void StopDisplayingCharacterActionDescription()
    {
        if (whoseCharacterActions) SetUnitStatsText(whoseCharacterActions);
        else ClearUnitStatsText();
    }

    public void ShowRandomCards(int amount)
    {
        amount = Mathf.Min(amount, cards.Count);

        List<CharacterAction> selectedCards = new List<CharacterAction>();
        for (int i = 0; i < amount; i++)
        {
            selectedCards.Add(cards[Random.Range(0, cards.Count)]);
        }

        ShowCharacterActions(selectedCards);
        TextMeshProUGUI text = GameObject.Find("UnitStatsText").GetComponent<TextMeshProUGUI>();

        text.text = "Выберите одну из карт:";
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
