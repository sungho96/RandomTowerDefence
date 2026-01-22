using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [Header("Confirm -> Options")]
    [SerializeField] private GameObject panelConfirm;
    [SerializeField] private GameObject panelOptions;

    [Header("Buttons (Unity Button Component)")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;

    private bool Shown = false;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnClickConfirm);

        if (retryButton != null)
            retryButton.onClick.AddListener(OnClickRetry);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnClickExit);
    }

    private void OnEnable()
    {
        if (GameState.Instance != null)
            GameState.Instance.OnGameOver += Show;

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

    private void Show()
    {
        if (Shown) return;
        Shown = true;

        if (panel != null)
            panel.SetActive(true);

        if (panelConfirm != null) panelConfirm.SetActive(true);
        if (panelOptions != null) panelOptions.SetActive(false);
    }

    private void OnClickConfirm()
    {
        if (panelConfirm != null) panelConfirm.SetActive(false);
        if (panelOptions != null) panelOptions.SetActive(true);
    }

    private void OnClickRetry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnClickExit()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        Debug.Log("[Gameover UI] Exit clicked (editor)");
#else
        Applcation.Quit();
#endif
    }
}
