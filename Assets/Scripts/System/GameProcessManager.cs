using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProcessManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; // 게임 오버 UI
    [SerializeField] private GameObject reObject;
    [SerializeField] private float slowDownDuration = 2f; // X초 동안 게임을 느려지게 함
    [SerializeField] private float restartDelay = 3f; // Y초 뒤에 재시작
    [SerializeField] SpriteRenderer playerRenderer;

    private bool isGameOver = false;
    private bool reAble = false;

    void Start()
    {
        gameOverPanel.SetActive(false); // 게임 시작 시 UI 숨김
    }

    void Update()
    {
        if (reAble && Input.anyKeyDown)
        {
            reAble = false;
            RestartGame();
        }
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
