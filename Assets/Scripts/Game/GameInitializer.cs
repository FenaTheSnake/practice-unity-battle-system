using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] List<Character> playerArmy;
    [SerializeField] List<Character> enemyArmy;

    [SerializeField] GameObject mapCellPrefab;
    [SerializeField] Vector2Int mapSize = new Vector2Int(8, 8);

    [SerializeField] List<CharacterAction> cards;

    GameState _gameState;
    Map _map;

    [Inject]
    public void Construct(GameState gameState, Map map)
    {
        _gameState = gameState;
        _map = map;
    }

    private void Start()
    {
        BuildMap();
        _gameState.StartFight(playerArmy, enemyArmy, cards);

        Camera.main.GetComponent<GameCamera>().SetFollowing(_map.MapPositionToWorldPosition(new Vector2Int(_map.Width / 2, _map.Height / 2 - 2)));
    }

    public void BuildMap()
    {
        _map.Init(mapSize.x, mapSize.y);

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                var cell = Instantiate(mapCellPrefab, new Vector3(i + 0.1f * i, 0, j + 0.1f * j), Quaternion.identity);
                cell.name = "Map Cell (" + i + "," + j + ")";
                cell.transform.parent = transform;

                Vector2Int pos = new Vector2Int(i, j);
                MapCell mapCell = cell.GetComponent<MapCell>();
                mapCell.mapPosition = pos;
                _map.map[pos] = mapCell;
            }
        }
    }
}
