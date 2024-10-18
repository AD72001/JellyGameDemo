using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    public Row[] rows;

    public Tile[,] Tiles { get; private set; }

    public SubTile[,] SubTiles { get; private set; }

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private const float TweenDuration = 0.25f;

    // Selection
    private List<Tile> selectionTiles;

    // Goal
    public int[] goal { get; private set; }
    public int yellow, green, blue, red, pink;

    // Check for win condition
    public bool gameStart = false;
    public bool victory = false;
    public bool gameOver = false;
    public GameObject VictoryScreen;
    public GameObject GameOverScreen;

    // Audio
    [SerializeField] private AudioClip popSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip selectSound;

    private void Awake() {
        Instance = this;
    }

    private void Start() 
    {
        selectionTiles = new List<Tile>();

        SetGoal();

        // DOTween.SetTweensCapacity(7812, 780);

        do {Shuffle();} while (CanPop());

        gameStart = true;
    }

    public void SetGoal()
    {
        goal = new int[ItemDatabase.Items.Length - 1];

        goal[0] = yellow;
        goal[1] = green;
        goal[2] = blue;
        goal[3] = red;
        goal[4] = pink;
    }

    public void Shuffle()
    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];
        SubTiles = new SubTile[rows.Max(row => row.tiles.Length)*2, rows.Length*2];

        // Initialize the board randomly
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;

                if (!tile.available)
                {
                    tile.type = -1;
                    continue;
                }
                
                tile.type = 0; // On the board

                for (int srow = y; srow < y + 2; srow++)
                {
                    for (int scol = x; scol < x + 2; scol++)
                    {
                        var subtile = tile.subRows[srow - y].subTiles[scol - x];

                        subtile.x = scol + x;
                        subtile.y = srow + y;

                        subtile.parent = tile;

                        if (tile.preset == false)
                            subtile.Item = ItemDatabase.Items[UnityEngine.Random.Range(0, ItemDatabase.Items.Length - 1)];
                        else
                        {
                            subtile.Item = subtile.presetColor != -1 ? 
                                ItemDatabase.Items[subtile.presetColor] : ItemDatabase.Items[tile.presetColor];
                        }


                        SubTiles[scol + x, srow + y] = subtile;
                    }
                }

                Tiles[x, y] = tile;
            }
        }
    }

    private async void Update() {
        if (Array.TrueForAll(goal, x => x == 0) && !victory)
        {
            victory = true;
            SoundManager.Instance.PlaySound(victorySound);
            VictoryScreen.SetActive(true);
        }
        else if (CheckForLoseCondition() && !CanPop() && !ContainEmptySubtile() && !gameOver)
        {
            Debug.Log("Game Over");
            gameOver = true;
            SoundManager.Instance.PlaySound(gameOverSound);
            GameOverScreen.SetActive(true);
        }
        
        if (CanPop())
        {
            // SoundManager.Instance.PlaySound(popSound);
            await Pop();
            await ReplaceEmptyAsync();
        }
            
    }

    public bool CanPop() 
    {
        for (var y = 0; y < Height*2; y++) 
        {
            for (var x = 0; x < Width*2; x++) 
            {  
                if (
                !SubTiles[x, y]
                || !SubTiles[x, y].parent.available
                || SubTiles[x, y].Item.name == "Z_Empty") {
                    continue;
                }

                if (SubTiles[x, y].GetConnectedTilesDifferentParent().Skip(1).Count() >= 1)  
                {
                    return true;
                }
            }
        }

        return false;
    }

    private async Task Pop() 
    {
        for (var y = 0; y < Height*2; y++) 
        {
            for (var x = 0; x < Width*2; x++) 
            {
                var subTile = SubTiles[x, y];

                if (subTile == null  || subTile.Item == null) continue;

                var connectedTiles = subTile.GetConnectedTilesDifferentParent();

                if (connectedTiles.Skip(1).Count() < 1) continue;

                connectedTiles = subTile.GetConnectedTiles();

                // Start the popping, only pop if different tile
                bool differentParent = false;
                List<Tile> parents = new List<Tile>();

                Tile initialParent = connectedTiles[0].parent;

                parents.Add(initialParent);

                var deleteSequence = DOTween.Sequence();

                foreach (var tile in connectedTiles)
                {
                    if (initialParent != tile.parent)
                    {
                        if (!parents.Contains(tile.parent) && tile.Item == subTile.Item)
                        {
                            parents.Add(tile.parent);
                        }

                        differentParent = true;
                    }
                }

                if (!differentParent) 
                {
                    parents.Clear();
                    continue;
                }

                // Tiles are deleted and Empty tiles' sprite are created

                foreach (var tile in connectedTiles)
                {
                    deleteSequence.Join(tile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                }

                await deleteSequence.Play().AsyncWaitForCompletion();

                if (gameStart)
                {
                    goal[subTile.Item.value] -= parents.Count;

                    if (goal[subTile.Item.value] < 0)
                        goal[subTile.Item.value] = 0;
                }

                deleteSequence.Kill(true);
                    
                var emptySequence = DOTween.Sequence();

                foreach (var tile in connectedTiles) {
                    tile.Item = ItemDatabase.Items[ItemDatabase.Items.Length - 1];
                    emptySequence.Join(tile.icon.transform.DOScale(Vector3.one, TweenDuration));
                }

                await emptySequence.Play().AsyncWaitForCompletion();

                emptySequence.Kill(true);
            }
        }
    }

    private bool ContainEmptySubtile()
    {
        for (int y = 0; y < Width; y++)
        {
            for (int x = 0; x < Height; x++)
            {
                if (Tiles[x, y] && !Tiles[x, y].isFull())
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Replace empty tiles with adjacent tile's type of item
    public async Task ReplaceEmptyAsync()
    {
        var newSequence = DOTween.Sequence();

        for (int y = 0; y < Height*2; y++)
        {
            for (int x = 0; x < Width*2; x++)
            {
                if (SubTiles[x, y] && SubTiles[x, y].Item.name == "Z_Empty")
                {
                    SubTiles[x, y].ReplaceTile();
                    newSequence.Join(SubTiles[x, y].icon.transform.DOScale(Vector3.one, TweenDuration));
                }
            }
        }

        await newSequence.Play().AsyncWaitForCompletion();

        newSequence.Kill(true);
    }

    public void SelectTile(Tile tile)
    {
        if (tile.type == -1)
        {
            selectionTiles.Clear();
            return;
        }

        if (!selectionTiles.Contains(tile)) selectionTiles.Add(tile);

        SoundManager.Instance.PlaySound(selectSound);

        if (selectionTiles[0].type != 1)
        {
            Debug.Log("Check type [0]");
            selectionTiles.Clear();
            return;
        }

        if (selectionTiles.Count < 2) return;

        if (selectionTiles[1].type != 0 || !selectionTiles[1].isEmpty())
        {
            Debug.Log("Check type [1] and empty state");
            selectionTiles.Clear();
            return;
        }

        DragTileToEmpty(selectionTiles[0], selectionTiles[1]);

        selectionTiles.Clear();
    }

    public void DragTileToEmpty(Tile offboard, Tile onboard)
    {
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                onboard.subRows[y].subTiles[x].Item = offboard.subRows[y].subTiles[x].Item;
                offboard.subRows[y].subTiles[x].Item = ItemDatabase.Items[ItemDatabase.Items.Length -1];
            }
        }
    }

    private bool CheckForLoseCondition()
    {
        for (int y = 0; y < Width; y++)
        {
            for (int x = 0; x < Height; x++)
            {
                if (Tiles[x, y] && Tiles[x, y].isEmpty())
                {
                    return false;
                }
            }
        }

        return true;
    }

}
