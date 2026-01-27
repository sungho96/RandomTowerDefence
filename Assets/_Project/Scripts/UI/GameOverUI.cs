using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private GameObject panel; //게임오버 패널

    [Header("Confirm -> Options")]
    [SerializeField] private GameObject panelConfirm; //확인패널
    [SerializeField] private GameObject panelOptions; //옵션패널

    [Header("Buttons (Unity Button Component)")]
    [SerializeField] private Button confirmButton; //확인버튼
    [SerializeField] private Button retryButton;   //재시작버튼
    [SerializeField] private Button exitButton;    //종료버튼

    private bool Shown = false;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false); //게임오버패널 비활성화

        //버튼 클릭 콜백 1회 연결 (Inspector 누락 방지)
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnClickConfirm);

        if (retryButton != null)
            retryButton.onClick.AddListener(OnClickRetry);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnClickExit);
    }

    private void OnEnable()
    {
        //게임인스터스(싱글톤)있다면 Ongamevoer 구독
        if (GameState.Instance != null)
            GameState.Instance.OnGameOver += Show;

        //버튼 리스너 중복 등록 방지: Remove 후 Add
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(OnClickConfirm);
            confirmButton.onClick.AddListener(OnClickConfirm);
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(OnClickRetry);
            retryButton.onClick.AddListener(OnClickRetry);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnClickExit);
            exitButton.onClick.AddListener(OnClickExit);
        }
    }

    /// <summary>
    /// 이미 열려있으면 중복 실행 방지 (Shown 플래그)
    /// 호출되면 확인패널 ON 옵션패널 OFF
    /// </summary>
    private void Show()
    {
        if (Shown) return;
        Shown = true;

        if (panel != null)
            panel.SetActive(true);

        if (panelConfirm != null) panelConfirm.SetActive(true);
        if (panelOptions != null) panelOptions.SetActive(false);
    }
    /// <summary>
    /// 호출되면 확인패널 OFF 옵션패널 ON
    /// </summary>
    private void OnClickConfirm()
    {
        if (panelConfirm != null) panelConfirm.SetActive(false);
        if (panelOptions != null) panelOptions.SetActive(true);
    }
    /// <summary>
    /// 일시정지 해제 후 현재 씬을 다시 로드하여 재시작
    /// </summary>
    private void OnClickRetry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// (Editor) 로그만 출력 / (Build) 앱 종료
    /// </summary>
    private void OnClickExit()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        Debug.Log("[Gameover UI] Exit clicked (editor)");
#else
        Application.Quit();
#endif
    }
}
