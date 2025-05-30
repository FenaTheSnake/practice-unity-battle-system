using UnityEngine;

public class EnemyAI
{
    GameState _gameState;
    Map _map;

    EnemyAI(GameState gameState, Map map)
    {
        _gameState = gameState;
        _map = map;

        _gameState.OnPlayerTurnEnd += OnPlayerTurnEnd;
    }

    public void OnPlayerTurnEnd()
    {
        
    }

    public void Think()
    {

    }
}
