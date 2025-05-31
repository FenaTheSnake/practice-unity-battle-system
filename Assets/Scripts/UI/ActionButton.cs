using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using System.Linq;

public class ActionButton : MonoBehaviour
{
    public CharacterAction myAction;

    GameState _gameState;
    Button _button;
    TextMeshProUGUI _text;
    EventTrigger _eventTrigger;

    //[Inject]
    //public void Construct(GameState gameState)
    //{
    //    _gameState = gameState;
    //}

    public void Init(CharacterAction action, GameState gameState)
    {
        myAction = action;

        _button = GetComponent<Button>();
        _text = GetComponentInChildren<TextMeshProUGUI>();

        _text.text = myAction.actionName;
        _button.onClick.AddListener(OnClicked);

        _eventTrigger = GetComponent<EventTrigger>();
        _eventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerEnter).First().callback.AddListener(OnHover);
        _eventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerExit).First().callback.AddListener(OnUnhover);

        _gameState = gameState;
    }

    public void OnClicked()
    {
        _gameState.ExecuteCharacterAction(myAction);
    }

    public void OnHover(BaseEventData data)
    {
        _gameState.DisplayCharacterActionDescription(myAction);
    }
    public void OnUnhover(BaseEventData data)
    {
        _gameState.StopDisplayingCharacterActionDescription();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClicked);
    }
}
