using UnityEngine;

public class TileContainer : MonoBehaviour
{
    public static TileContainer Instance {get; private set;}
    public Tile tile;

    public bool simpleTile = false;
    public int initial;

    private void Start() {
        Instance = this;

        tile = GetComponentInChildren<Tile>();

        tile.x = -1;
        tile.y = -1;

        tile.type = 1;

        tile = Shuffle(tile);

        tile.preset = simpleTile;

        initial = tile.presetColor;
    }

    private Tile Shuffle(Tile tile)
    {
        SubTile[,] SubTiles = new SubTile[2, 2];

        for (int y=0; y<2; y++)
        {
            for (int x=0; x<2; x++)
            {
                var subtile = tile.subRows[y].subTiles[x];

                subtile.x = x;
                subtile.y = y;

                subtile.parent = tile;

                if (subtile.parent.preset == false)
                    subtile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length - 1)];
                else
                {
                    subtile.Item = subtile.presetColor != -1 ? 
                        ItemDatabase.Items[subtile.presetColor] : ItemDatabase.Items[tile.presetColor];
                }

                SubTiles[x, y] = subtile;
                tile.subRows[y].subTiles[x] = subtile;
            }
        }

        return tile;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S))
        {
            tile = Shuffle(tile);
        }

        if (tile.isEmpty())
        {
            tile.presetColor = Random.Range(0, ItemDatabase.Items.Length -1);
            tile = Shuffle(tile);
        }
    }

    public void ResetTile()
    {
        tile.presetColor = initial;
        tile.preset = true;
        tile = Shuffle(tile);
        tile.preset = simpleTile;
    }
}
