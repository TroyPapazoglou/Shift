
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    [TextArea(5, 15)]
    public string layout;

    public int moveLimit = -1;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public TileType[] Tiles { get; private set; }

    [Header("Visuals")]
    [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
    public Color BackgroundColor => backgroundColor;


    public void ParseLayout()
    {
        string[] rows = layout.Split(
            new[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries
        );

        if (rows.Length == 0)
            throw new Exception("Level layout is empty!");

        List<string[]> parsedRows = new List<string[]>();

        Width = 0;
        Height = rows.Length;

        
        foreach (string row in rows)
        {
            string[] cells = row.Split(
                (char[])null,
                StringSplitOptions.RemoveEmptyEntries
            );

            parsedRows.Add(cells);
            Width = Mathf.Max(Width, cells.Length);
        }

        List<TileType> parsedTiles = new List<TileType>();

        for (int y = 0; y < Height; y++)
        {
            string[] cells = parsedRows[y];
                     
            foreach (string cell in cells)
            {
                TileType type = cell.ToLower() switch
                {
                    "e" => TileType.Empty,
                    "p" => TileType.Player,
                    "w" => TileType.Wall,
                    "v" => TileType.Void,
                    "x" => TileType.Exit,

                    _ => throw new Exception(
                        $"Invalid tile '{cell}' at row {y + 1}."
                    )
                };

                parsedTiles.Add(type);
            }
                       
            for (int x = cells.Length; x < Width; x++)
            {
                parsedTiles.Add(TileType.Void);
            }
        }

        Tiles = parsedTiles.ToArray();
    }

}
