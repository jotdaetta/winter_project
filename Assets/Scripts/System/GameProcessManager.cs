using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameProcessManager : MonoBehaviour
{
    [SerializeField] MenuController menuController;
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
    [SerializeField] InGameTimer gameTimer;
    [SerializeField] Text mission;


    Image lastEnterImage;
    public int enemyCount;

    private bool isGameOver = false;
    private bool reAble = false;

    void Awake()
    {
        menuController.isWorking = false;
    }
    void Start()
    {
        enemyCount = EnemiesCount.childCount;
        gameOverPanel.SetActive(false); // 게임 시작 시 UI 숨김
        gameTimer.TimerActive(true);

        SoundManager.Instance.Play("music.fight");
    }

    void Update()
    {
        if (reAble && Input.anyKeyDown)
        {
            reAble = false;
            RestartGame();
        }
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Menu")) && !isGameOver)
        {
            menuController.isWorking = true;
            gameTimer.TimerActive(false);
            Time.timeScale = 0;
            menuUI.SetActive(true);
        }
        if (enemyCount > 0)
        {
            mission.text = "임무 : 모든 적 제거";
        }
        else
        {
            mission.text = "임무 : 화살표 위치로 이동";
        }
    }

    public void GameClear()
    {
        if (enemyCount > 0) return;

        // levelController.Clear();
        // levelController.BackToMenu();
        gameTimer.TimerActive(false);
        gameTimer.EvaluateRecord();
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

        gameTimer.TimerActive(false);
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
        gameTimer.TimerActive(true);
        Time.timeScale = 1f;
        menuController.isWorking = false;
        menuUI.SetActive(false);
        if (lastEnterImage != null)
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
