using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] string sceneName;

    public void BackToMenu()
    {
        LoadingController.LoadScene(sceneName);
    }

    public void Clear()
    {
        Levels.LevelComplete(PlayerPrefs.GetInt(Levels.CurrentLevel));
        LoadingController.LoadScene(sceneName);
    }
}
