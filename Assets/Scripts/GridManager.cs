using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private PlayerInputHandler playerInput;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private TMP_Text bestMovesText;
    [SerializeField] private TMP_Text currentMovesText;

    [Header("Level Loading")]
    [FormerlySerializedAs("startingLevelIndex")]
    [SerializeField] private int initialLevelIndex;

    [Header("Level Select")]
    [SerializeField] private float slotPadding = 2f;        
    [SerializeField] private float gameCameraPadding = 1f;
    [SerializeField] private float overviewCameraPadding = 1f;
    [SerializeField] private float cameraPanDuration = 0.6f;
    [SerializeField] private float previewPopDuration = 0.35f;

    [Header("Spawn Animation")]
    [SerializeField] private float totalSpawnDuration = 2f;
    [SerializeField] private float tileAnimationDuration = 0.3f;
    [SerializeField] private float playerSpawnDuration = 1f;

    [Header("Exit Animation")]
    [SerializeField] private float totalTsunamiDuration = 2f;
    [SerializeField] private float tsunamiCellDuration = 0.3f;
    [SerializeField] private float tsunamiScaleMultiplier = 1.4f;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.2f;

    [Header("Background / Intro")]
    [SerializeField] private UIIntro uiIntro;
    [SerializeField] private Color introStartColor = Color.black;
    [SerializeField] private float introBackgroundDuration = 1.5f;
    [SerializeField] private float levelBackgroundDuration = 0.8f;

    [Header("Wall Feedback")]
    [SerializeField] private float bumpDistance = 0.18f;   
    [SerializeField] private float bumpDuration = 0.18f;

    private LevelData[] Levels => levelDatabase != null ? levelDatabase.Levels : null;
    private float TileSpacing => levelDatabase.TileSpacing;

    private LevelBoard activeBoard;
    private readonly List<LevelBoard> previewBoards = new List<LevelBoard>();

    private int currentLevelIndex;
    private float maxLevelExtent;  

    private bool isMoving;
    private bool isLoading;
    private bool isInOverview;

    private int movesMade;
    private Vector3 playerVisualOriginalScale;  

    private void Awake()
    {
        playerVisualOriginalScale = playerVisual.transform.localScale;
    }

    private void Start()
    {
        if (Levels == null || Levels.Length == 0)
        {
            Debug.LogError("No levels have been assigned!");
            return;
        }

        CacheLevelLayouts();

        currentLevelIndex = Mathf.Clamp(initialLevelIndex, 0, Levels.Length - 1);

        cameraController.SetBackgroundColor(introStartColor);

        if (uiIntro != null)
            uiIntro.Play();

        StartCoroutine(LoadLevelRoutine(currentLevelIndex, false, introBackgroundDuration));
    }

    private void OnEnable()
    {
        if (playerInput != null)
            playerInput.MoveRequested += HandleMoveRequested;
    }

    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.MoveRequested -= HandleMoveRequested;
    } 
   
    public void ResetLevel()
    {
        if (isLoading || isMoving || isInOverview)
            return;

        StartCoroutine(LoadLevelRoutine(currentLevelIndex, false));
    }

    public void OpenLevelSelect()
    {
        if (isLoading || isMoving || isInOverview || activeBoard == null)
            return;

        StartCoroutine(OpenOverviewRoutine());
    }

    public void CloseLevelSelect()
    {
        SelectLevel(currentLevelIndex);
    }
 
    public void ToggleLevelSelect()
    {
        if (isInOverview)
            CloseLevelSelect();
        else
            OpenLevelSelect();
    }

    private void CacheLevelLayouts()
    {
        maxLevelExtent = 0f;

        foreach (LevelData level in Levels)
        {
            if (level == null)
                continue;

            level.ParseLayout();
            float extent = Mathf.Max(level.Width, level.Height) * TileSpacing;
            maxLevelExtent = Mathf.Max(maxLevelExtent, extent);
        }
    }

    private void GetSlotGridSize(out int cols, out int rows)
    {
        int count = Levels.Length;
        cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        rows = Mathf.CeilToInt(count / (float)cols);
    }

    private Vector3 GetSlotCenter(int index)
    {
        GetSlotGridSize(out int cols, out int rows);

        float cell = maxLevelExtent + slotPadding;
        float totalWidth = (cols - 1) * cell;
        float totalHeight = (rows - 1) * cell;

        int col = index % cols;
        int row = index / cols;

        return transform.position + new Vector3(
            col * cell - totalWidth / 2f,
            -row * cell + totalHeight / 2f,
            0f);
    }
      
    private Vector3 GetBoardOrigin(int index, LevelData data)
    {
        return GetSlotCenter(index) + new Vector3(
            -(data.Width - 1) * TileSpacing / 2f,
            (data.Height - 1) * TileSpacing / 2f,
            0f);
    }

    private Bounds GetBoardBounds(LevelBoard board)
    {
        return new Bounds(
            GetSlotCenter(board.Index),
            new Vector3(board.Data.Width * TileSpacing, board.Data.Height * TileSpacing, 0f));
    }

    private Bounds GetOverviewBounds()
    {
        Vector3 size = new Vector3(maxLevelExtent, maxLevelExtent, 0f);
        Bounds bounds = new Bounds(GetSlotCenter(0), size);

        for (int i = 1; i < Levels.Length; i++)
            bounds.Encapsulate(new Bounds(GetSlotCenter(i), size));

        return bounds;
    }
    
    private Vector3 GridToWorld(LevelBoard board, Vector2Int position)
    {
        return board.Root.position + new Vector3(
            position.x * TileSpacing,
            -position.y * TileSpacing,
            0f);
    }

    private Vector3 GridToWorld(Vector2Int position) => GridToWorld(activeBoard, position); 

    private LevelBoard CreateBoard(int index)
    {
        LevelData data = Levels[index];
        data.ParseLayout();

        int width = data.Width;
        int height = data.Height;

        GameObject pivotObject = new GameObject($"Board_{index}");
        pivotObject.transform.SetParent(transform, false);
        pivotObject.transform.position = GetSlotCenter(index);

        GameObject rootObject = new GameObject("Tiles");
        rootObject.transform.SetParent(pivotObject.transform, false);
        rootObject.transform.position = GetBoardOrigin(index, data);

        BoxCollider2D box = rootObject.AddComponent<BoxCollider2D>();
        box.size = new Vector2(width * TileSpacing, height * TileSpacing);
        box.offset = new Vector2(
            (width - 1) * TileSpacing / 2f,
            -(height - 1) * TileSpacing / 2f);

        rootObject.AddComponent<LevelSlotClickTarget>().Initialize(index, SelectLevel);

        LevelBoard board = new LevelBoard
        {
            Index = index,
            Data = data,
            Pivot = pivotObject.transform,
            Root = rootObject.transform,
            Grid = new TileType[width, height],
            Tiles = new Tile[width, height]
        };

        bool foundPlayer = false;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileType type = data.Tiles[x + y * width];
                board.Grid[x, y] = type;

                if (type == TileType.Player)
                {
                    board.PlayerPosition = new Vector2Int(x, y);
                    foundPlayer = true;
                }
            }
        }

        if (!foundPlayer)
            Debug.LogError($"No player starting position found in level {index}!");

        return board;
    }

    private Tile CreateTile(LevelBoard board, int x, int y, float popDuration)
    {
        TileType type = board.Grid[x, y];

        GameObject tileObject = Instantiate(levelDatabase.TilePrefab, board.Root);
        Tile tile = tileObject.GetComponent<Tile>();

        tile.Initialize(new Vector2Int(x, y), type, GetSprite(type), TileSpacing);
        board.Tiles[x, y] = tile;

        if (popDuration > 0f)
        {
            Vector3 targetScale = tile.transform.localScale;
            tile.transform.localScale = Vector3.zero;

            Tween.Scale(tile.transform, targetScale, popDuration, Ease.OutBack);
        }

        return tile;
    }
      
    private IEnumerator SpawnTilesStaggered(LevelBoard board)
    {
        int width = board.Data.Width;
        int height = board.Data.Height;

        int totalTiles = 0;

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (board.Grid[x, y] != TileType.Void)
                    totalTiles++;

        float cellInterval = totalTiles > 1
            ? (totalSpawnDuration - tileAnimationDuration) / (totalTiles - 1)
            : 0f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board.Grid[x, y] == TileType.Void)
                    continue;

                CreateTile(board, x, y, tileAnimationDuration);

                yield return new WaitForSeconds(cellInterval);
            }
        }
    }
       
    private void SpawnTilesTogether(LevelBoard board, float popDuration)
    {
        for (int y = 0; y < board.Data.Height; y++)
            for (int x = 0; x < board.Data.Width; x++)
                if (board.Grid[x, y] != TileType.Void)
                    CreateTile(board, x, y, 0f);

        board.Pivot.localScale = Vector3.zero;
        Tween.Scale(board.Pivot, Vector3.one, popDuration, Ease.OutBack);
    }

    private void DestroyBoard(LevelBoard board)
    {
        if (board == null)
            return;

        if (board.Pivot != null)
            Tween.StopAll(board.Pivot);

        if (board.Tiles != null)
        {
            foreach (Tile tile in board.Tiles)
                if (tile != null)
                    Tween.StopAll(tile.transform);
        }

        if (board.Pivot != null)
            Destroy(board.Pivot.gameObject);
    }

    private void ShrinkBoard(LevelBoard board, float duration)
    {
        Tween.StopAll(board.Pivot);
        Tween.Scale(board.Pivot, Vector3.zero, duration, Ease.InBack);
    }

    private void ClearPreviews()
    {
        foreach (LevelBoard board in previewBoards)
            DestroyBoard(board);

        previewBoards.Clear();
    }

    private IEnumerator LoadLevelRoutine(int index, bool panCamera, float backgroundDuration = -1f)
    {
        isLoading = true;
        isMoving = false;
        movesMade = 0;

        DestroyBoard(activeBoard);
        activeBoard = null;
        ClearPreviews();

        if (Levels == null || Levels.Length == 0)
        {
            Debug.LogError("No levels have been assigned!");
            isLoading = false;
            yield break;
        }

        index = Mathf.Clamp(index, 0, Levels.Length - 1);

        if (Levels[index] == null)
        {
            Debug.LogError("Selected level is null!");
            isLoading = false;
            yield break;
        }

        currentLevelIndex = index;
        float bgDuration = backgroundDuration >= 0f ? backgroundDuration : levelBackgroundDuration;
        cameraController.FadeBackgroundColor(Levels[index].BackgroundColor, bgDuration);
        UpdateMoveUI();

        activeBoard = CreateBoard(index);
             
        playerVisual.transform.localScale = Vector3.zero;

        Bounds bounds = GetBoardBounds(activeBoard);

        if (panCamera)
        {
            cameraController.FocusBounds(bounds, gameCameraPadding, cameraPanDuration);
            yield return new WaitForSeconds(cameraPanDuration);
        }
        else
        {
            cameraController.SnapToBounds(bounds, gameCameraPadding);
        }

        yield return StartCoroutine(SpawnTilesStaggered(activeBoard));
        yield return StartCoroutine(RevealPlayer());

        isLoading = false;
    }

    private IEnumerator RevealPlayer()
    {
        playerVisual.Initialize(GridToWorld(activeBoard.PlayerPosition));

        Tween.Scale(
            playerVisual.transform,
            playerVisualOriginalScale,
            playerSpawnDuration,
            Ease.OutBack);

        yield return new WaitForSeconds(playerSpawnDuration);
    }


    private IEnumerator OpenOverviewRoutine()
    {
        isLoading = true;
        isInOverview = true;

        float startTime = Time.time;

        cameraController.FocusBounds(GetOverviewBounds(), overviewCameraPadding, cameraPanDuration);

        for (int i = 0; i < Levels.Length; i++)
        {
            if (i == currentLevelIndex || Levels[i] == null)
                continue;

            LevelBoard board = CreateBoard(i);
            SpawnTilesTogether(board, previewPopDuration);
            previewBoards.Add(board);

            yield return null;  
        }

        float remaining = cameraPanDuration - (Time.time - startTime);

        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);

        isLoading = false;
    }

    private void SelectLevel(int index)
    {
        if (!isInOverview || isLoading)
            return;

        StartCoroutine(SelectLevelRoutine(index));
    }

    private IEnumerator SelectLevelRoutine(int index)
    {
        isLoading = true;
              
        if (index == currentLevelIndex)
        {
            foreach (LevelBoard board in previewBoards)
                ShrinkBoard(board, cameraPanDuration);

            cameraController.FocusBounds(GetBoardBounds(activeBoard), gameCameraPadding, cameraPanDuration);
            yield return new WaitForSeconds(cameraPanDuration);

            ClearPreviews();
            isInOverview = false;
            isLoading = false;
            yield break;
        }
              
        LevelBoard target = previewBoards.Find(b => b.Index == index);

        if (target == null)
        {
            isLoading = false;
            yield break;
        }

        Tween.Scale(playerVisual.transform, Vector3.zero, 0.2f, Ease.InBack);

        foreach (LevelBoard board in previewBoards)
            if (board != target)
                ShrinkBoard(board, cameraPanDuration);

        ShrinkBoard(activeBoard, cameraPanDuration);

        cameraController.FadeBackgroundColor(target.Data.BackgroundColor, levelBackgroundDuration);
        cameraController.FocusBounds(GetBoardBounds(target), gameCameraPadding, cameraPanDuration);
        yield return new WaitForSeconds(cameraPanDuration);

        previewBoards.Remove(target);

        Tween.StopAll(target.Pivot);
        target.Pivot.localScale = Vector3.one;

        ClearPreviews();
        DestroyBoard(activeBoard);

        activeBoard = target;
        currentLevelIndex = index;
        movesMade = 0;
        UpdateMoveUI();

        yield return StartCoroutine(RevealPlayer());

        isInOverview = false;
        isLoading = false;
    }     

    private void HandleMoveRequested(Vector2Int direction)
    {
        if (isMoving || isLoading || isInOverview || activeBoard == null)
            return;

        playerVisual.CancelBump(GridToWorld(activeBoard.PlayerPosition));

        if (direction.x != 0)
            ShiftRow(activeBoard.PlayerPosition.y, direction.x);
        else if (direction.y != 0)
            ShiftColumn(activeBoard.PlayerPosition.x, direction.y);
    }

    private void PlayBlockedFeedback(int direction, bool isRow)
    {       
        Vector2 worldDirection = isRow
            ? new Vector2(direction, 0f)
            : new Vector2(0f, -direction);

        Vector3 rest = GridToWorld(activeBoard.PlayerPosition);

        playerVisual.Bump(rest, worldDirection, bumpDistance * TileSpacing, bumpDuration);
              
    }

    private static bool IsShiftable(TileType type)
    {
        return type != TileType.Void &&
               type != TileType.Exit &&
               type != TileType.Wall;
    }

    private void ShiftRow(int row, int direction)
    {
        List<Vector2Int> positions = new();

        for (int x = 0; x < activeBoard.Data.Width; x++)
            if (IsShiftable(activeBoard.Grid[x, row]))
                positions.Add(new Vector2Int(x, row));

        ShiftLine(positions, direction, true);
    }

    private void ShiftColumn(int column, int direction)
    {
        List<Vector2Int> positions = new();

        for (int y = 0; y < activeBoard.Data.Height; y++)
            if (IsShiftable(activeBoard.Grid[column, y]))
                positions.Add(new Vector2Int(column, y));

        ShiftLine(positions, direction, false);
    }
       
    private void GetWrapPoints(LevelBoard board, Vector2Int cell, int direction, bool isRow, out Vector3 wrapExit, out Vector3 wrapEntry)
    {
        int width = board.Data.Width;
        int height = board.Data.Height;

        Vector2Int pastEnd = isRow ? new Vector2Int(width, cell.y) : new Vector2Int(cell.x, height);
        Vector2Int beforeStart = isRow ? new Vector2Int(-1, cell.y) : new Vector2Int(cell.x, -1);

        if (direction > 0)
        {
            wrapExit = GridToWorld(board, pastEnd);
            wrapEntry = GridToWorld(board, beforeStart);
        }
        else
        {
            wrapExit = GridToWorld(board, beforeStart);
            wrapEntry = GridToWorld(board, pastEnd);
        }
    }

    private void ShiftLine(List<Vector2Int> positions, int direction, bool isRow)
    {
        LevelBoard board = activeBoard;
        TileType[,] grid = board.Grid;
        Tile[,] tileViews = board.Tiles;
        int width = board.Data.Width;
        int height = board.Data.Height;      

        direction = direction > 0 ? 1 : -1;

        int count = positions.Count;

        if (count <= 1)
        {
            PlayBlockedFeedback(direction, isRow);
            return;
        }           

        TileType[] newTypes = new TileType[count];
        Tile[] newTiles = new Tile[count];
        TileType[] oldTypes = new TileType[count];
        Tile[] oldTiles = new Tile[count];

        for (int i = 0; i < count; i++)
        {
            Vector2Int source = positions[i];
            oldTypes[i] = grid[source.x, source.y];
            oldTiles[i] = tileViews[source.x, source.y];
        }

        Vector2Int targetPosition = board.PlayerPosition;

        if (isRow)
            targetPosition.x = (board.PlayerPosition.x + direction + width) % width;
        else
            targetPosition.y = (board.PlayerPosition.y + direction + height) % height;

        TileType targetType = grid[targetPosition.x, targetPosition.y];

        if (targetType == TileType.Wall)
        {
            PlayBlockedFeedback(direction, isRow);
            return;
        }

        if (targetType == TileType.Exit)
        {
            RegisterMove();

            isMoving = true;
            StartCoroutine(CompleteLevel(targetPosition, direction, isRow));
            return;
        }
               
        for (int i = 0; i < count; i++)
        {
            int destinationIndex = (i + direction + count) % count;

            newTypes[destinationIndex] = oldTypes[i];
            newTiles[destinationIndex] = oldTiles[i];
        }
             
        for (int i = 0; i < count; i++)
        {
            Vector2Int position = positions[i];

            grid[position.x, position.y] = newTypes[i];
            tileViews[position.x, position.y] = newTiles[i];
        }

        isMoving = true;
        RegisterMove();

        int completedAnimations = 0;

        for (int i = 0; i < count; i++)
        {
            int destinationIndex = (i + direction + count) % count;

            Tile tile = oldTiles[i];
            Vector2Int destinationPosition = positions[destinationIndex];

            tile.SetGridPosition(destinationPosition);

            if (oldTypes[i] == TileType.Player)
                board.PlayerPosition = destinationPosition;

            Vector3 destination = GridToWorld(board, destinationPosition);

            Vector3? wrapExit = null;
            Vector3? wrapEntry = null;

            bool isWrapping =
                (direction > 0 && i == count - 1) ||
                (direction < 0 && i == 0);

            if (isWrapping)
            {
                GetWrapPoints(board, destinationPosition, direction, isRow,
                    out Vector3 exitPoint, out Vector3 entryPoint);

                wrapExit = exitPoint;
                wrapEntry = entryPoint;
            }
                      
            if (oldTypes[i] == TileType.Player)
            {
                playerVisual.AnimateTo(destination, moveDuration, wrapExit, wrapEntry);
            }

            tile.AnimateTo(
                destination,
                moveDuration,
                wrapExit,
                wrapEntry,
                () =>
                {
                    completedAnimations++;

                    if (completedAnimations == count)
                        isMoving = false;
                });
        }
    }
  

    private void RegisterMove()
    {
        movesMade++;
        UpdateMoveUI();
    }

    private void UpdateMoveUI()
    {
        if (currentMovesText != null)
            currentMovesText.text = $"MOVES: {movesMade}";

        if (bestMovesText != null)
        {
            int bestMoves = PlayerPrefs.GetInt(GetBestMovesKey(), -1);

            bestMovesText.text = bestMoves < 0
                ? "BEST: --"
                : $"BEST: {bestMoves}";
        }
    }

    private string GetBestMovesKey()
    {
        return $"GridPuzzle_BestMoves_Level_{currentLevelIndex}";
    }

    private void SaveBestMoves()
    {
        string key = GetBestMovesKey();
        int bestMoves = PlayerPrefs.GetInt(key, -1);

        if (bestMoves < 0 || movesMade < bestMoves)
        {
            PlayerPrefs.SetInt(key, movesMade);
            PlayerPrefs.Save();
        }

        UpdateMoveUI();
    }
    

    private IEnumerator CompleteLevel(Vector2Int exitPosition, int direction, bool isRow)
    {
        LevelBoard board = activeBoard;
        Vector2Int playerPos = board.PlayerPosition;

        bool isWrapping = isRow
            ? (direction > 0 && playerPos.x == board.Data.Width - 1) ||
              (direction < 0 && playerPos.x == 0)
            : (direction > 0 && playerPos.y == board.Data.Height - 1) ||
              (direction < 0 && playerPos.y == 0);

        Vector3? wrapExit = null;
        Vector3? wrapEntry = null;

        if (isWrapping)
        {
            GetWrapPoints(board, exitPosition, direction, isRow,
                out Vector3 exitPoint, out Vector3 entryPoint);

            wrapExit = exitPoint;
            wrapEntry = entryPoint;
        }

        playerVisual.AnimateTo(
            GridToWorld(board, exitPosition),
            moveDuration,
            wrapExit,
            wrapEntry);

        yield return new WaitForSeconds(moveDuration);

        yield return StartCoroutine(PlayTsunamiEffect(board, exitPosition));
        yield return StartCoroutine(RemoveFinalTiles(board, exitPosition));

        Debug.Log($"Level {currentLevelIndex + 1} completed in {movesMade} moves!");

        SaveBestMoves();

        if (currentLevelIndex < Levels.Length - 1)
        {         
            yield return StartCoroutine(LoadLevelRoutine(currentLevelIndex + 1, true));
        }
        else
        {
            yield return StartCoroutine(LoadLevelRoutine(currentLevelIndex, true));
            Debug.Log("All levels completed!");           
        }
    }

    private IEnumerator PlayTsunamiEffect(LevelBoard board, Vector2Int exitPosition)
    {
        List<Tile> waveTiles = new List<Tile>();

        for (int y = 0; y < board.Data.Height; y++)
        {
            for (int x = 0; x < board.Data.Width; x++)
            {
                if (new Vector2Int(x, y) == exitPosition)
                    continue;

                Tile tile = board.Tiles[x, y];

                if (tile != null)
                    waveTiles.Add(tile);
            }
        }

        if (waveTiles.Count == 0)
            yield break;

        waveTiles.Sort((a, b) =>
        {
            int distanceA =
                Mathf.Abs(a.GridPosition.x - exitPosition.x) +
                Mathf.Abs(a.GridPosition.y - exitPosition.y);

            int distanceB =
                Mathf.Abs(b.GridPosition.x - exitPosition.x) +
                Mathf.Abs(b.GridPosition.y - exitPosition.y);

            return distanceA.CompareTo(distanceB);
        });

        float cellInterval = waveTiles.Count > 1
            ? Mathf.Max(0f, totalTsunamiDuration - tsunamiCellDuration) / (waveTiles.Count - 1)
            : 0f;

        foreach (Tile tile in waveTiles)
        {
            Vector3 originalScale = tile.transform.localScale;

            Tween.Scale(
                tile.transform,
                originalScale * tsunamiScaleMultiplier,
                tsunamiCellDuration * 0.5f,
                Ease.OutBack
            ).OnComplete(() =>
            {
                Tween.Scale(
                    tile.transform,
                    Vector3.zero,
                    tsunamiCellDuration * 0.5f,
                    Ease.InBack);
            });

            yield return new WaitForSeconds(cellInterval);
        }

        yield return new WaitForSeconds(tsunamiCellDuration);
    }

    private IEnumerator RemoveFinalTiles(LevelBoard board, Vector2Int exitPosition)
    {
        Tile exitTile = board.Tiles[exitPosition.x, exitPosition.y];

        List<Transform> finalObjects = new List<Transform>();

        if (exitTile != null)
            finalObjects.Add(exitTile.transform);

        finalObjects.Add(playerVisual.transform);

        foreach (Transform item in finalObjects)
        {
            Tween.Scale(
                item,
                item.localScale * tsunamiScaleMultiplier,
                tsunamiCellDuration * 0.5f,
                Ease.OutBack
            ).OnComplete(() =>
            {
                Tween.Scale(
                    item,
                    Vector3.zero,
                    tsunamiCellDuration * 0.5f,
                    Ease.InBack);
            });
        }

        yield return new WaitForSeconds(tsunamiCellDuration);
    }

    private Sprite GetSprite(TileType type)
    {
        return type switch
        {
            TileType.Empty => levelDatabase.TileVisuals.empty,
            TileType.Wall => levelDatabase.TileVisuals.wall,
            TileType.Player => levelDatabase.TileVisuals.player,
            TileType.Exit => levelDatabase.TileVisuals.exit,
            _ => null
        };
    }
}