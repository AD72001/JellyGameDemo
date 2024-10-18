using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    // Type: -1 (Null) 0 (OnBoard) 1 (OffBoard)
    public int type;

    public bool preset = false;
    public int presetColor;

    public bool available = true;

    public Button button;

    public SubRow[] subRows;

    public SubTile[] subTiles {get; set;}

    private void Start() {
        button.onClick.AddListener(() => Board.Instance.SelectTile(this));

        button.interactable = available;
    }

    public bool isEmpty()
    {
        for (int y=0; y < 2; y++)
        {
            for (int x=0; x < 2; x++)
            {
                if (!subRows[y].subTiles[x].parent.available)
                    continue;

                if (subRows[y].subTiles[x].Item.name != "Z_Empty")
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool isFull()
    {
        for (int y=0; y < 2; y++)
        {
            for (int x=0; x < 2; x++)
            {
                if (subRows[y].subTiles[x].Item.name == "Z_Empty")
                {
                    return false;
                }
            }
        }

        return true;
    }

}
