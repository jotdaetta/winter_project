using UnityEngine;
using UnityEngine.UI;

public class InGameTimer : MonoBehaviour
{
    [SerializeField] Text timerText;
    bool timerRunning;
    [SerializeField] float curTime;
    void FixedUpdate()
    {
        if (!timerRunning) return;
        curTime += Time.deltaTime;
    }
    void Update()
    {
        string hour = curTime >= 3600 ? $"{Mathf.FloorToInt(curTime / 3600)}:" : "";
        string min = (curTime % 3600 / 60 >= 10) ? $"{Mathf.FloorToInt(curTime % 3600 / 60)}:" : (curTime >= 3600 ? "0" : "" + $"{Mathf.FloorToInt(curTime % 3600 / 60)}:");
        string under = curTime % 60 >= 10 ? (curTime % 60).ToString("F2") : $"0{(curTime % 60).ToString("F2")}";
        timerText.text = $"{hour}{min}{under}";
    }
    public void TimerActive(bool activity)
    {
        timerRunning = activity;
    }

    public void ResetTimer()
    {
        curTime = 0;
    }
    public void EvaluateRecord()
    {
        string curLevelKey = $"{TimerController.TIMERECORD}{Levels.CurrentLevel}";
        if (PlayerPrefs.HasKey(curLevelKey))
        {
            if (PlayerPrefs.GetFloat(curLevelKey) > curTime)
            {
                PlayerPrefs.SetFloat(curLevelKey, curTime);
            }
        }
        else
        {
            PlayerPrefs.SetFloat(curLevelKey, curTime);
        }
    }
}
