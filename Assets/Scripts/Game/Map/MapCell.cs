using System;
using UnityEngine;

public enum MapCellState : UInt16
{
    ACCESSABLE  = 1,    // the cell is noted as accessable by currently selected unit
    ENEMY       = 2,    // the cell is noted as containing enemy that can be attacked by selected unit
    HOVERED     = 4,    // the cell currently is hovered by mouse
    CLICKED     = 8,    // the cell is selected
}

public class MapCell : MonoBehaviour
{
    [SerializeField] Material passive;
    [SerializeField] Material hovered;
    [SerializeField] Material clicked;
    [SerializeField] Material accessable;
    [SerializeField] Material enemy;

    [HideInInspector] public MapEntity entity;
    public Vector2Int mapPosition;

    // A bitmap
    UInt16 cellState = 0;

    MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void AddState(MapCellState state)
    {
        cellState |= (UInt16)state;
        UpdateVisual();
    }
    public void RemoveState(MapCellState state)
    {
        cellState &= (UInt16)~state;
        UpdateVisual();
    }
    public bool GetState(MapCellState state)
    {
        return (cellState & (UInt16)state) != 0;
    }

    void UpdateVisual()
    {
        Material choosenMaterial = passive;
        if (GetState(MapCellState.ACCESSABLE))
        {
            choosenMaterial = accessable;
        }
        if (GetState(MapCellState.ENEMY))
        {
            choosenMaterial = enemy;
        }
        if (GetState(MapCellState.HOVERED))
        {
            choosenMaterial = hovered;
        }
        if (GetState(MapCellState.CLICKED))
        {
            choosenMaterial = clicked;
        }

        if (_meshRenderer.material != choosenMaterial)
        {
            _meshRenderer.material = choosenMaterial;
        }
    }
}
