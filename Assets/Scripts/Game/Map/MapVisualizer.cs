using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MapVisualizer
{
    //Vector2Int _mapSize;
    //List<GameObject> mapCells;

    //Map _map;

    //[Inject]
    //public void Construct(Map map)
    //{
    //    _map = map;

    //    mapCells = new List<GameObject>();
    //}

    //public void Init(Vector2Int mapSize)
    //{
    //    _map.Init(mapSize.x, mapSize.y);

    //    for (int i = 0; i < mapSize.x; i++)
    //    {
    //        for(int j = 0; j < mapSize.y; j++)
    //        {
    //            var cell = Instantiate(mapCellPrefab, new Vector3(i + 0.1f*i, 0, j + 0.1f*j), Quaternion.identity);
    //            cell.name = "Map Cell (" + i + "," + j + ")";
    //            cell.transform.parent = transform;
    //            cell.GetComponent<MapCell>().mapPosition = new Vector2Int(i, j);
    //            mapCells.Add(cell);
    //        }
    //    }
    //}
}
