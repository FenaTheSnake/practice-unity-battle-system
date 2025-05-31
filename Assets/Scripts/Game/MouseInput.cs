using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

public class MouseInput : MonoBehaviour
{
    MapCell _hoveredCell;
    MapCell _clickedCell;

    Camera _cam;
    InputAction _cursorMoved;
    InputAction _clicked;
    InputAction _action;

    LayerMask _mapMask;
    int UILayer;

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
        _cursorMoved = InputSystem.actions.FindAction("CursorMoved");
        _cursorMoved.started += OnCursorMoved;

        _clicked = InputSystem.actions.FindAction("Click");
        _clicked.started += OnCursorClicked;

        _action = InputSystem.actions.FindAction("Action");
        _action.started += OnAction;

        _cam = Camera.main;

        _mapMask = LayerMask.GetMask("Map");
        UILayer = LayerMask.NameToLayer("UI");
    }

    void Update()
    {
        
    }

    void OnCursorMoved(InputAction.CallbackContext context)
    {
        if (!_gameState.CanHoverAndClickCells()) return;

        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000.0f, _mapMask))
        {
            MapCell cell = hit.collider.transform.parent.GetComponent<MapCell>();
            Debug.Assert(cell != null);

            if (_hoveredCell == cell) return;
            if (_hoveredCell != null)
            {
                _hoveredCell.RemoveState(MapCellState.HOVERED);
            }
            _hoveredCell = cell;
            cell.AddState(MapCellState.HOVERED);
        }
        else
        {
            if (_hoveredCell != null)
            {
                _hoveredCell.RemoveState(MapCellState.HOVERED);
                _hoveredCell = null;
            }
        }
    }

    // LMB Clicked
    void OnCursorClicked(InputAction.CallbackContext context)
    {
        if (!_gameState.CanHoverAndClickCells()) return;
        if (IsPointerOverUIElement()) return;

        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000.0f, _mapMask))
        {
            MapCell cell = hit.collider.transform.parent.GetComponent<MapCell>();
            Debug.Assert(cell != null);

            if (_clickedCell != null)
            {
                _clickedCell.RemoveState(MapCellState.CLICKED);
                if (_clickedCell == cell)
                {
                    _clickedCell = null;
                    return;
                }
            }
            _clickedCell = cell;
            cell.AddState(MapCellState.CLICKED);
            CellClicked(cell);
        }
        else
        {
            if (_clickedCell != null)
            {
                ClearAccessableCells();
                _clickedCell.RemoveState(MapCellState.CLICKED);
                _clickedCell = null;
            }
        }
    }

    // RMB Clicked
    void OnAction(InputAction.CallbackContext context)
    {
        if (!_gameState.CanHoverAndClickCells()) return;

        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000.0f, _mapMask))
        {
            MapCell cell = hit.collider.transform.parent.GetComponent<MapCell>();
            Debug.Assert(cell != null);

            if (_clickedCell != null && _clickedCell.entity != null && _clickedCell.entity is Character c && c.isPlayerUnit)
            {
                if (cell.GetState(MapCellState.ACCESSABLE))
                {
                    c.Move(cell.mapPosition);

                    ClearAccessableCells();
                    _clickedCell.RemoveState(MapCellState.CLICKED);
                    _clickedCell = null;
                }
                else if (cell.GetState(MapCellState.ENEMY))
                {
                    var target = cell.entity as Character;
                    var source = _clickedCell.entity as Character;
                    source.Attack(target);

                    ClearAccessableCells();
                    _clickedCell.RemoveState(MapCellState.CLICKED);
                    _clickedCell = null;
                }
            }
        }
    }

    void CellClicked(MapCell cell)
    {
        if(cell.GetState(MapCellState.ENEMY))
        {
            _gameState.ExecuteSavedCharacterActionOnTarget(cell.entity as Character);
            ClearAccessableCells();
            _clickedCell.RemoveState(MapCellState.CLICKED);
            _clickedCell = null;
        }
        if(cell.GetState(MapCellState.ACCESSABLE))
        {
            _gameState.ExecuteSavedCharacterActionOnCell(cell);
            ClearAccessableCells();
            _clickedCell.RemoveState(MapCellState.CLICKED);
            _clickedCell = null;
        }
        else
        {
            ClearAccessableCells();
            MapEntity ent = _map.map[cell.mapPosition].entity;
            if (ent == null) return;

            if (ent is Character c)
            {
                if (c.isPlayerUnit)
                {
                    _gameState.ShowCharacterActions(c);
                }
                _gameState.SetUnitStatsText(c);
            }
        }
    }

    void DrawAccessableCells(Vector2Int center, int radius, bool isMelee)
    {
        for (int i = center.x - radius; i <= center.x + radius; i++)
        {
            for (int j = center.y - radius; j <= center.y + radius; j++)
            {
                if (i < 0 || j < 0 || i >= _map.Width || j >= _map.Height) continue;
                float d = Vector2Int.Distance(center, new Vector2Int(i, j));
                if (d > radius) continue;

                MapCell cell = _map.map[new Vector2Int(i, j)];
                //if (cell.entity != null)
                //{
                //    if(cell.entity is Character c && !c.isPlayerUnit)
                //    {
                //        if(!isMelee || (d <= 1)) cell.AddState(MapCellState.ENEMY);
                //    }
                //    continue;
                //} 

                cell.AddState(MapCellState.ACCESSABLE);
            }
        }
    }

    void ClearAccessableCells()
    {
        foreach (var cell in _map.map)
        {
            cell.Value.RemoveState(MapCellState.ACCESSABLE);
            cell.Value.RemoveState(MapCellState.ENEMY);
        }
        _gameState.CloseCharacterActions();
        _gameState.ClearUnitStatsText();
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
