using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubTile : MonoBehaviour
{
    public int x;
    public int y;

    public int presetColor = -1;

    public Image icon;

    public Item item;

    public Item Item {
        get { return item; }

        set {
            if (item == value) return;

            item = value;

            if (value != null)
            {
                icon.sprite = item.sprite;
            }
        }
    }

    public Tile parent;

    
    public SubTile Left => x > 0 ? Board.Instance.SubTiles[x - 1, y]: null;
    public SubTile Top => y > 0 ? Board.Instance.SubTiles[x, y - 1]: null;
    public SubTile Right => x < Board.Instance.Width*2 - 1 ? Board.Instance.SubTiles[x + 1, y]: null;
    public SubTile Bottom => y < Board.Instance.Height*2 - 1 ? Board.Instance.SubTiles[x, y + 1]: null;

    public SubTile[] Neighbors => new[] {
        Left, 
        Right,
        Top,
        Bottom
    };

    private void Start() {
        parent = transform.parent.parent.GetComponent<Tile>();
    }

    public List<SubTile> GetConnectedTiles(List<SubTile> exclude = null)
    {
        var result = new List<SubTile> {this,};

        if (exclude == null) {
            exclude = new List<SubTile> {this,};
        }
        else {
            exclude.Add(this);
        }

        foreach (var tile in Neighbors) {
            if (tile == null
            || tile.Item.name == "Z_Empty"
            || !tile.parent.available
            || exclude.Contains(tile)
            || Item != tile.Item) continue;

            result.AddRange(tile.GetConnectedTiles(exclude));
        }

        return result;
    }

    public List<SubTile> GetConnectedTilesDifferentParent(List<SubTile> exclude = null)
    {
        var result = new List<SubTile> {this,};

        if (exclude == null) {
            exclude = new List<SubTile> {this,};
        }
        else {
            exclude.Add(this);
        }

        foreach (var tile in Neighbors) {
            if (!tile
            || tile.Item.name == "Z_Empty"
            || !tile.parent.available
            || tile.parent == parent
            || exclude.Contains(tile)
            || Item != tile.Item) continue;

            result.AddRange(tile.GetConnectedTilesDifferentParent(exclude));
        }

        return result;
    }

    public void ReplaceTile()
    {
        foreach (var tile in Neighbors)
        {
            if (tile == null || tile.Item.name == "Z_Empty" || tile.parent != parent)
            {
                continue;
            }
            else {
                Item = tile.Item;
                return;
            }
        }
    }
}
