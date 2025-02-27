using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UILevel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject lockObj;
    [SerializeField] string levelName = "Chapter";
    [SerializeField] int levelNumber = 1;

    float loadDelayTime = 0.2f;
    bool isClicked = false;
    bool isUnlock = false;

    public void SetLevel(bool isUnlock)
    {
        this.isUnlock = isUnlock;
        lockObj.SetActive(!isUnlock);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUnlock == false)
        {
            print($"레벨{levelNumber} 해금 필요");
            return;
        }
        if (isClicked) return;
        isClicked = true;

        // StartCoroutine(nameof(LoadLevel));
    }

    private IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(loadDelayTime);
        PlayerPrefs.SetInt(Levels.CurrentLevel, levelNumber);
        LoadingController.LoadScene($"{levelName}{levelNumber}");
    }
}
