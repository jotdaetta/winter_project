using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    [SerializeField] RectTransform scrollViewContents;
    [SerializeField] RectTransform mapRect;
    [SerializeField] RectTransform arrowRect;
    [SerializeField] GameObject settingsPannel;
    [SerializeField] Animator arrowAnimator;
    [SerializeField] Transform mapSceneTransform;
    [SerializeField] Image transitionImg;
    [SerializeField] Text pointName;
    [SerializeField] float transitionTime;
    [SerializeField] float transitionDelay;


    delegate void myFunc();
    bool onTransitionAction = false;
    bool onLevelChoiceAction = false;
    int lastClickedLevel = -1;
    Vector2 LastPos = Vector2.zero;
    Image lastEnterImage;

    private void Start()
    {
        SetScrollViewContentsHeight();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Settings();
        }
    }

    #region Private Methods
    void SetScrollViewContentsHeight()
    {
        Vector2 sizeDelta = scrollViewContents.sizeDelta;
        int children = scrollViewContents.childCount;
        sizeDelta.y = 140 * children;
        scrollViewContents.sizeDelta = sizeDelta;
    }

    IEnumerator FadeIO(bool isIn, myFunc func = null)
    {
        float elapsedTime = 0f;
        Color color = transitionImg.color;
        int cur = isIn ? 0 : 1;
        int des = isIn ? 1 : 0;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;
            color.a = Mathf.Lerp(cur, des, t);
            transitionImg.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(transitionDelay);
        if (isIn)
        {
            func();
            StartCoroutine(FadeIO(false));
        }
        else onTransitionAction = false;
    }
    IEnumerator MapFadeIO(bool isIn, Vector2 startPos, Vector2 endPos, myFunc func = null)
    {
        if (lastClickedLevel == -1)
        {
            arrowAnimator.SetTrigger("out");
            yield return new WaitForSeconds(1f);
        }
        float elapsedTime = 0f;
        float duration = 0.5f;

        Vector3 startScale = isIn ? new Vector3(1920, 1080, 1) : new Vector3(7680, 4320, 1);
        Vector3 endScale = isIn ? new Vector3(7680, 4320, 1) : new Vector3(1920, 1080, 1);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            mapRect.sizeDelta = Vector3.Lerp(startScale, endScale, t);
            mapRect.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        mapRect.sizeDelta = endScale;
        mapRect.localPosition = endPos;

        LastPos = endPos;

        if (lastClickedLevel != -1)
        {
            arrowAnimator.SetTrigger("in");
            yield return new WaitForSeconds(1f);
        }
        if (func == null)
        {
            onLevelChoiceAction = false;
            yield break;
        }
        func();
    }

    #endregion

    #region Public Methods
    Vector2[] endPos = {
        new Vector2(-2300, -790),
        new Vector2(-1300, -190),
    };
    Vector2[] arrowPos = {
        new Vector2(2590, 845),
        new Vector2(1500, 150),
    };
    string[] posName = {
        "KR_GoyangCity",
        "Unknown",
    };

    public void LevelClicked(int level)
    {
        if (onLevelChoiceAction) return;
        onLevelChoiceAction = true;
        if (lastClickedLevel == level)
        {
            lastClickedLevel = -1;
            StartCoroutine(MapFadeIO(false, LastPos, Vector2.zero));
            return;
        }
        else if (lastClickedLevel == -1)
        {
            lastClickedLevel = level;
            arrowRect.localPosition = arrowPos[level];
            pointName.text = posName[level];
            StartCoroutine(MapFadeIO(true, LastPos, endPos[level]));
            return;
        }
        lastClickedLevel = -1;
        StartCoroutine(MapFadeIO(false, LastPos, Vector2.zero, () =>
        {
            lastClickedLevel = level;
            arrowRect.localPosition = arrowPos[level];
            pointName.text = posName[level];
            StartCoroutine(MapFadeIO(true, LastPos, endPos[level]));
        }));
    }

    public void OpenMap()
    {
        if (onTransitionAction) return;
        onTransitionAction = true;
        StartCoroutine(FadeIO(true, () => mapSceneTransform.position = Vector2.zero));
    }

    public void CloseMap()
    {
        if (onTransitionAction) return;
        onTransitionAction = true;
        StartCoroutine(FadeIO(true, () => mapSceneTransform.position = new Vector2(2770, 0)));
    }

    public void Settings()
    {
        settingsPannel.SetActive(!settingsPannel.activeSelf);
        lastEnterImage.color = new Color(0, 0, 0, 0.8f);
    }

    public void OnButtonEnter(Image img)
    {
        lastEnterImage = img;
        Color color = new Color(1, 1, 1, 0.02f);
        img.color = color;
    }

    public void OnButtonExit(Image img)
    {
        Color color = new Color(0, 0, 0, 0.8f);
        img.color = color;
    }
    #endregion
}
