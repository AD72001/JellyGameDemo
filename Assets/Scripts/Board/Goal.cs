using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private TMP_Text yellowText;
    [SerializeField] private TMP_Text blueText;
    [SerializeField] private TMP_Text greenText;
    [SerializeField] private TMP_Text redText;
    [SerializeField] private TMP_Text pinkText;

    private void Start() {
        
    }

    private void Update() {
        yellowText.text = Board.Instance.goal[0].ToString();
        greenText.text = Board.Instance.goal[1].ToString();
        blueText.text = Board.Instance.goal[2].ToString();
        redText.text = Board.Instance.goal[3].ToString();
        pinkText.text = Board.Instance.goal[4].ToString();
    }
}
