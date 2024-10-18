using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextStageButton : MonoBehaviour
{
    public Button button;

    public int nextLevel;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(LoadNextStage);
    }

    private void LoadNextStage()
    {
        NextStage(nextLevel);
    }

    private void NextStage(int level)
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath($"Level_{level}");

        if (buildIndex == -1)
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene($"Level_{level}");
    }
}
