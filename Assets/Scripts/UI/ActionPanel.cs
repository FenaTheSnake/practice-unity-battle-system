using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ActionPanel : MonoBehaviour
{
    List<ActionButton> buttons;

    GameState _gameState;

    [Inject]
    public void Construct(GameState gameState)
    {
        _gameState = gameState;
    }

    private void Start()
    {
        buttons = new List<ActionButton>();
    }

    public void ClearButtons()
    {
        foreach (var button in buttons)
        {
            Destroy(button.gameObject);
        }
        buttons.Clear();
    }

    public void CreateButtonsForCharacterActions(List<CharacterAction> actions)
    {
        ClearButtons();

        foreach (CharacterAction action in actions)
        {
            var button = (Instantiate(Resources.Load("UI/ActionButton", typeof(GameObject))) as GameObject).GetComponent<ActionButton>();
            button.Init(action, _gameState);
            button.transform.SetParent(transform, false);

            buttons.Add(button);
        }
    }
}
