using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public Button button;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(Quit);
    }

    void Quit() {
        Application.Quit(); // Quits the application

        # if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode(); // Quit Playmode
        # endif
    }
}
