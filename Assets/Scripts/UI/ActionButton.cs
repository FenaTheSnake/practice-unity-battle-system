using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ActionButton : MonoBehaviour
{
    public CharacterAction myAction;

    GameState _gameState;
    Button _button;
    TextMeshProUGUI _text;

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

        _gameState = gameState;
    }

    public void OnClicked()
    {
        _gameState.ExecuteCharacterAction(myAction);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClicked);
    }
}
