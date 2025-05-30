using UnityEngine;
using Zenject;


// Any object occupying map's cell
public class MapEntity : MonoBehaviour
{
    Vector2Int m_MapPosition;
    public Vector2Int MapPosition { get => m_MapPosition; set { SetPosition(value); } }

    Map _map;

    [Inject]
    public void Construct(Map map)
    {
        _map = map;
    }

    // Moves an entity to target position.
    public void SetPosition(Vector2Int where)
    {
        if (where.x < 0 || where.y < 0 || where.x >= _map.Width || where.y >= _map.Height) return;
        if (_map.map[where].entity != null)
        {
            Debug.LogError("Attempt to move entity " + name + " to occupied cell: " + where + " (occupied by " + _map.map[where].entity.name + ")");
            return;
        }

        _map.map[MapPosition].entity = null;

        m_MapPosition = where;
        transform.position = _map.MapPositionToWorldPosition(where);
        _map.map[where].entity = this;
    }
}
