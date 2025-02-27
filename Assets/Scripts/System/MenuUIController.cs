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
    [SerializeField] GameObject howtoPannel;
    [SerializeField] Animator arrowAnimator;
    [SerializeField] Transform mapSceneTransform;
    [SerializeField] Image transitionImg;
    [SerializeField] Image AttackStartFrame;
    [SerializeField] Text pointName;
    [SerializeField] float transitionTime;
    [SerializeField] float transitionDelay;


    delegate void myFunc();
    bool onTransitionAction = false;
    bool onLevelChoiceAction = false;
    [SerializeField] int lastClickedLevel = -1;
    Vector2 LastPos = Vector2.zero;
    Image lastEnterImage;

    private void Start()
    {
        SetScrollViewContentsHeight();

        SoundManager.Instance.Play("music.main");
    }

    private void Update()
    {
        // if (Input.GetKeyUp(KeyCode.Escape))
        // {
        //     Settings();
        // }
        if (lastClickedLevel == -1)
        {
            AttackStartFrame.color = Color.black;
        }
        else
        {
            AttackStartFrame.color = new Color(255, 229, 0);
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

    IEnumerator FadeIO(bool isIn, myFunc func = null, float funcBeforeDelay = 0, float funcAfterDelay = 0, bool noFadeOut = false)
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
            yield return new WaitForSeconds(funcBeforeDelay);
            func();
            yield return new WaitForSeconds(funcAfterDelay);
            if (!noFadeOut)
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
    public string levelName = "Level";
    public void AttackStart()
    {
        if (lastClickedLevel == -1) return;
        StartCoroutine(FadeIO(true, () =>
        {
            PlayerPrefs.SetInt(Levels.CurrentLevel, lastClickedLevel);
            LoadingController.LoadScene($"{levelName}{lastClickedLevel}");
        }, 0.4f, 0, true));
    }
    public void LevelClicked(int level)
    {
        SoundManager.Instance.Play("ui.click");
        
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
        StartCoroutine(FadeIO(true, () => {
            mapSceneTransform.position = Vector2.zero;
        }));

        SoundManager.Instance.Play("ui.click");
    }

    public void CloseMap()
    {
        if (onTransitionAction) return;
        onTransitionAction = true;
        StartCoroutine(FadeIO(true, () => {
                mapSceneTransform.position = new Vector2(2770, 0);
            }
        ));

        SoundManager.Instance.Play("ui.click");
    }

    public void Settings()
    {
        settingsPannel.SetActive(!settingsPannel.activeSelf);
        lastEnterImage.color = new Color(0, 0, 0, 0.8f);
    }
    public void HowTo(bool parm)
    {
        howtoPannel.SetActive(parm);
        lastEnterImage.color = new Color(0, 0, 0, 0.8f);

        SoundManager.Instance.Play("ui.click");
    }

    public void OnButtonEnter(Image img)
    {
        lastEnterImage = img;
        Color color = new Color(1, 1, 1, 0.2f);
        img.color = color;
    }

    public void OnButtonExit(Image img)
    {
        Color color = new Color(0, 0, 0, 0.8f);
        img.color = color;
    }
    #endregion
}
