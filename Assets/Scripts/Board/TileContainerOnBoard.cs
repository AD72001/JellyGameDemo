using UnityEngine;
using UnityEngine.EventSystems;

public class TileContainerOnBoard : MonoBehaviour, IDropHandler
{
    private Tile tile;
    [SerializeField] private AudioClip selectSound;

    private void Start() {
        tile = GetComponentInChildren<Tile>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (!tile.available) return;
        
        Debug.Log("OnDropToBoard");
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;

            Board.Instance.DragTileToEmpty(eventData.pointerDrag.GetComponent<Tile>(), tile);
            SoundManager.Instance.PlaySound(selectSound);
        }
    }
}
