
using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelThumbnail : MonoBehaviour
{
    private int levelIndex;
    private Action<int> onSelected;

    public void Initialize(int index, LevelData levelData, GameObject tilePrefab, TileVisuals tileVisuals, float thumbnailScale, float tileSpacing, Action<int> selectionCallback)
    {
        levelIndex = index;
        onSelected = selectionCallback;

        levelData.ParseLayout();

        float spacing = tileSpacing * thumbnailScale;

        Vector3 offset = new Vector3(
            (levelData.Width - 1) * spacing * 0.5f,
            -(levelData.Height - 1) * spacing * 0.5f,
            0f
        );

        SpriteRenderer prefabRenderer =
            tilePrefab.GetComponent<SpriteRenderer>();

        for (int y = 0; y < levelData.Height; y++)
        {
            for (int x = 0; x < levelData.Width; x++)
            {
                TileType type = levelData.Tiles[x + y * levelData.Width];

                if (type == TileType.Void)
                    continue;

                Sprite sprite = GetSprite(type, tileVisuals);

                GameObject cell = new GameObject($"Cell {x},{y}");
                cell.transform.SetParent(transform);

                cell.transform.localPosition = new Vector3(
                    x * spacing,
                    -y * spacing,
                    0f
                ) - offset;

                cell.transform.localScale =
                    tilePrefab.transform.localScale * thumbnailScale;

                SpriteRenderer renderer =
                    cell.AddComponent<SpriteRenderer>();

                renderer.sprite = sprite;

                if (prefabRenderer != null)
                {
                    renderer.sortingLayerID =
                        prefabRenderer.sortingLayerID;

                    renderer.sortingOrder =
                        prefabRenderer.sortingOrder;

                    renderer.sharedMaterial =
                        prefabRenderer.sharedMaterial;

                    renderer.color = prefabRenderer.color;
                }
            }
        }

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        boxCollider.size = new Vector2(
            levelData.Width * spacing,
            levelData.Height * spacing
        );

        boxCollider.offset = Vector2.zero;
    }

    private Sprite GetSprite(TileType type, TileVisuals visuals)
    {
        return type switch
        {
            TileType.Empty => visuals.empty,
            TileType.Wall => visuals.wall,
            TileType.Player => visuals.player,
            TileType.Exit => visuals.exit,
            _ => null
        };
    }

    private void OnMouseDown()
    {
        onSelected?.Invoke(levelIndex);
    }
}
