using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class EnemyAI
{
    GameState _gameState;
    Map _map;

    [Inject]
    EnemyAI(GameState gameState, Map map)
    {
        _gameState = gameState;
        _map = map;

        _gameState.OnPlayerTurnEnd += OnPlayerTurnEnd;
    }

    public void OnPlayerTurnEnd()
    {
        Think();
    }

    async public void Think()
    {
        Debug.Log("[AI] Thinking...");
        await UniTask.Delay(2000);
        Debug.Log("[AI] Imma gonna skip this turn, bro.");
        _gameState.EndTurn();
    }
}
