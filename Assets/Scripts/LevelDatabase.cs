
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Scriptable Objects/Level Database")]
public class LevelDatabase : ScriptableObject
{
    [Header("Levels")]
    [SerializeField] private LevelData[] levels;

    [Header("Shared Visuals")]
    [SerializeField] private TileVisuals tileVisuals;
    [SerializeField] private GameObject tilePrefab;

    [Header("Grid Settings")]
    [SerializeField] private float tileSpacing = 1.1f;

    public LevelData[] Levels => levels;
    public TileVisuals TileVisuals => tileVisuals;
    public GameObject TilePrefab => tilePrefab;
    public float TileSpacing => tileSpacing;
}
