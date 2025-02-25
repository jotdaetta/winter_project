using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [SerializeField] string sceneName;

    public void BackToMenu()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Clear()
    {
        Levels.LevelComplete(PlayerPrefs.GetInt(Levels.CurrentLevel));
        SceneManager.LoadScene(sceneName);
    }
}
