using UnityEngine;

public class LevelBoard
{
    public int Index;
    public LevelData Data;
    public Transform Root;       
    public TileType[,] Grid;
    public Tile[,] Tiles;
    public Vector2Int PlayerPosition;
    public Transform Pivot;
}