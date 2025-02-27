using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameProcessManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; // 게임 오버 UI
    [SerializeField] private GameObject clearUI;
    [SerializeField] private GameObject reObject;
    [SerializeField] private float slowDownDuration = 2f; // X초 동안 게임을 느려지게 함
    [SerializeField] private float restartDelay = 3f; // Y초 뒤에 재시작
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] LevelController levelController; //나중에 클리어 할떄 씀
    [SerializeField] InGameFade inGameFade;
    [SerializeField] Transform EnemiesCount;
    [SerializeField] GameObject menuUI;


    Image lastEnterImage;
    public int enemyCount;

    private bool isGameOver = false;
    private bool reAble = false;

    void Start()
    {
        enemyCount = EnemiesCount.childCount;
        gameOverPanel.SetActive(false); // 게임 시작 시 UI 숨김

        SoundManager.Instance.Play("music.fight");
    }

    void Update()
    {
        if (reAble && Input.anyKeyDown)
        {
            reAble = false;
            RestartGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            Time.timeScale = 0;
            menuUI.SetActive(true);
        }
    }

    public void GameClear()
    {
        if (enemyCount > 0) return;
        // levelController.Clear();
        // levelController.BackToMenu();
        StartCoroutine(OnClear());
    }

    IEnumerator OnClear()
    {
        float elapsed = 0f;

        Time.timeScale = 0;
        while (elapsed < slowDownDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            playerRenderer.color = new Color(1, 1, 1, Mathf.Lerp(1, 0f, elapsed / slowDownDuration));
            yield return null;
        }

        clearUI.SetActive(true);

        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1.4f);
        inGameFade.Fade();
        yield return new WaitForSecondsRealtime(inGameFade.transitionTime + 0.2f);
        levelController.BackToMenu();
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        StartCoroutine(SlowDownAndShowUI());
    }

    IEnumerator SlowDownAndShowUI()
    {
        float elapsed = 0f;

        Time.timeScale = 0;
        while (elapsed < slowDownDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            playerRenderer.color = new Color(1, 1, 1, Mathf.Lerp(1, 0f, elapsed / slowDownDuration));
            yield return null;
        }

        gameOverPanel.SetActive(true); // 게임 오버 UI 띄우기

        yield return new WaitForSecondsRealtime(restartDelay); // Y초 뒤에 재시작

        reObject.SetActive(true); // 게임 오버 UI 띄우기

        reAble = true;
        // RestartGame();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // 다시 정상 속도로 변경
        isGameOver = false;
        LoadingController.LoadScene(SceneManager.GetActiveScene().name);
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

    public void BokGui()
    {
        Time.timeScale = 1f;
        menuUI.SetActive(false);
        lastEnterImage.color = new Color(0, 0, 0, 0.8f);
    }

    public void ReStart()
    {
        RestartGame();
    }

    public void ToMenu()
    {
        Time.timeScale = 1f;
        levelController.BackToMenu();
    }
}
