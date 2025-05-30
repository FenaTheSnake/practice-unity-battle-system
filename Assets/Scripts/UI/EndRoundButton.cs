using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EndRoundButton : MonoBehaviour
{
    Button _button;
    TextMeshProUGUI _text;

    GameState _gameState;

    [Inject]
    public void Construct(GameState gameState)
    {
        _gameState = gameState;
        _button = GetComponent<Button>();
        _text = GetComponentInChildren<TextMeshProUGUI>();

        _gameState.OnEnemyTurnEnd += OnEnemyTurnEnd;
    }

    private void OnDestroy()
    {
        _gameState.OnEnemyTurnEnd -= OnEnemyTurnEnd;
    }

    public void ButtonPressed()
    {
        _gameState.EndTurn();
        _button.interactable = false;
        _text.text = "Ожидайте...";
    }

    public void OnEnemyTurnEnd()
    {
        _button.interactable = true;
        _text.text = "Завершить ход";
    }
}
