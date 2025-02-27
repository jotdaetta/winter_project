using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelController : MonoBehaviour
{
    [SerializeField] UILevel[] uiLevels;

    void Awake()
    {
        for (int i = 0; i < Levels.MaxLevel; ++i)
        {
            var levelData = Levels.LoadLevelData(i + 1);
            uiLevels[i].SetLevel(levelData);
        }
    }

    [ContextMenu("Reset Date")]
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt($"{Levels.LevelUnLock}1", 1);
        LoadingController.LoadScene(SceneManager.GetActiveScene().name);
    }
}
