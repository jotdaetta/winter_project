using UnityEngine;

public class Levels
{
    public static readonly int MaxLevel = 4;
    public static readonly string CurrentLevel = "CURRENTLEVEL";
    public static readonly string LevelUnLock = "LEVELUNLOCK_";

    static Levels()
    {
        PlayerPrefs.SetInt($"{LevelUnLock}1", 1);
    }

    public static bool LoadLevelData(int level)
    {
        return PlayerPrefs.GetInt($"{LevelUnLock}{level}", 0) == 1;
    }

    public static void LevelComplete(int level)
    {
        if (level + 1 <= MaxLevel)
        {
            PlayerPrefs.SetInt($"{LevelUnLock}{level + 1}", 1);
        }
        else
        {
            Debug.Log("모든 레벨 해금됨");
        }
    }
}
