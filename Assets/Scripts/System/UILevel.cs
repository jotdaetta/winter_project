using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UILevel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] string levelName = "Chapter ";
    [SerializeField] int levelNumber = 1;

    float loadDelayTime = 0.2f;
    bool isClicked = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked) return;
        isClicked = true;

        StartCoroutine(nameof(LoadLevel));
    }

    private IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(loadDelayTime);
        SceneManager.LoadScene($"{levelName}{levelNumber}");
    }
}
