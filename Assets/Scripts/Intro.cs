using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    private bool gameStart = false;
    [SerializeField] private GameObject menuScreen;

    void Update()
    {
        if (Input.anyKey || Input.touchCount > 0)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0 || gameStart)
                return;

            gameStart = true;

            menuScreen.SetActive(true);
        }
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene($"Level_{level}");
    }
}
