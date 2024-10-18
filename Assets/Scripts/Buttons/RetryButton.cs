using UnityEngine;
using UnityEngine.UI;

public class RetryButton : MonoBehaviour
{
    public Button button;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(RetryStage);
    }

    public void RetryStage()
    {
        do {Board.Instance.Shuffle();} while (Board.Instance.CanPop());
        Board.Instance.SetGoal();
        TileContainer.Instance.ResetTile();

        Board.Instance.GameOverScreen.SetActive(false);
        Board.Instance.VictoryScreen.SetActive(false);

        Board.Instance.victory = false;
        Board.Instance.gameOver = false;
    }
}
