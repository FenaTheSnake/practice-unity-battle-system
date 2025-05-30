using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public Dictionary<Vector2Int, MapCell> map;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public void Init(int w, int h)
    {
        map = new Dictionary<Vector2Int, MapCell>();

        Width = w;
        Height = h;
        for (int i = 0; i < w; i++)
        {
            for(int j = 0; j < h; j++)
            {
                map[new Vector2Int(i, j)] = null;
            }
        }
    }

    public Vector3 MapPositionToWorldPosition(Vector2Int position)
    {
        return new Vector3(position.x + 0.1f * position.x, 0, position.y + 0.1f * position.y);
    }

    //public MapEntity GetMapEntity(Vector2Int pos)
    //{
    //    return map[pos];
    //}
}
