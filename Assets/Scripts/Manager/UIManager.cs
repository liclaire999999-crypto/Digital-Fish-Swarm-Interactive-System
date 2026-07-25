using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// UI 管理器
/// 负责所有 UI 界面的显示和隐藏
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI 面板引用 | UI Panel References")]
    [SerializeField]
    private Canvas mainCanvas;  // 主画布
    
    [SerializeField]
    private GameObject menuPanel;  // 菜单面板
    
    [SerializeField]
    private GameObject gameplayPanel;  // 游戏界面面板
    
    [SerializeField]
    private GameObject pausePanel;  // 暂停面板
    
    [SerializeField]
    private GameObject gameOverPanel;  // 游戏结束面板
    
    [SerializeField]
    private GameObject settingsPanel;  // 设置面板

    [Header("UI 组件引用 | UI Component References")]
    [SerializeField]
    private Text timerText;  // 计时器文本
    
    [SerializeField]
    private Text fishCountText;  // 鱼群数量文本
    
    [SerializeField]
    private Text speedText;  // 群速度文本
    
    [SerializeField]
    private Text scoreText;  // 分数文本
    
    [SerializeField]
    private Slider gameProgressSlider;  // 游戏进度条
    
    [SerializeField]
    private Text stateText;  // 游戏状态文本

    [Header("按钮引用 | Button References")]
    [SerializeField]
    private Button startButton;  // 开始按钮
    
    [SerializeField]
    private Button pauseButton;  // 暂停按钮
    
    [SerializeField]
    private Button resumeButton;  // 恢复按钮
    
    [SerializeField]
    private Button restartButton;  // 重启按钮
    
    [SerializeField]
    private Button menuButton;  // 菜单按钮
    
    [SerializeField]
    private Button settingsButton;  // 设置按钮
    
    [SerializeField]
    private Button quitButton;  // 退出按钮

    [Header("UI 配置 | UI Configuration")]
    [SerializeField]
    private bool autoCreateCanvas = true;  // 自动创建画布
    
    [SerializeField]
    private Color activeTextColor = Color.white;  // 活跃文本颜色
    
    [SerializeField]
    private Color inactiveTextColor = Color.gray;  // 非活跃文本颜色

    private GameManager gameManager;
    private float updateTimer = 0f;  // UI 更新计时器
    private float updateInterval = 0.1f;  // UI 更新间隔

    private void Start()
    {
        gameManager = GameManager.Instance;
        
        if (mainCanvas == null && autoCreateCanvas)
        {
            CreateDefaultCanvas();
        }

        InitializeUIElements();
        RegisterButtonListeners();
        RegisterGameManagerListeners();
    }

    /// <summary>
    /// 创建默认画布
    /// </summary>
    private void CreateDefaultCanvas()
    {
        GameObject canvasGO = new GameObject("MainCanvas");
        mainCanvas = canvasGO.AddComponent<Canvas>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        RectTransform rectTransform = canvasGO.GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;

        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    /// <summary>
    /// 初始化 UI 元素
    /// </summary>
    private void InitializeUIElements()
    {
        // 隐藏所有面板
        HideAllPanels();

        // 初始化文本
        UpdateTimerDisplay(0f);
        UpdateFishCountDisplay(0);
        UpdateSpeedDisplay(0f);
    }

    /// <summary>
    /// 注册按钮监听
    /// </summary>
    private void RegisterButtonListeners()
    {
        if (startButton != null)
            startButton.onClick.AddListener(() => gameManager?.StartGame());

        if (pauseButton != null)
            pauseButton.onClick.AddListener(() => gameManager?.TogglePause());

        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => gameManager?.TogglePause());

        if (restartButton != null)
            restartButton.onClick.AddListener(() => gameManager?.RestartGame());

        if (menuButton != null)
            menuButton.onClick.AddListener(() => gameManager?.ReturnToMenu());

        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => ToggleSettingsPanel());

        if (quitButton != null)
            quitButton.onClick.AddListener(() => Application.Quit());
    }

    /// <summary>
    /// 注册游戏管理器事件监听
    /// </summary>
    private void RegisterGameManagerListeners()
    {
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged += UpdateGameState;
            gameManager.OnGameTimeUpdated += UpdateTimerDisplay;
            gameManager.OnStatisticsUpdated += UpdateStatistics;
        }
    }

    private void Update()
    {
        // 定期更新 UI（性能优化）
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval && gameManager != null)
        {
            if (gameManager.CurrentState == GameManager.GameState.Playing)
            {
                // 更新进度条
                if (gameProgressSlider != null)
                {
                    gameProgressSlider.value = gameManager.GetGameProgress();
                }
            }
            updateTimer = 0f;
        }
    }

    // ===== 状态管理 =====

    /// <summary>
    /// 隐藏所有面板
    /// </summary>
    private void HideAllPanels()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    /// <summary>
    /// 显示菜单 UI
    /// </summary>
    public void ShowMenuUI()
    {
        HideAllPanels();
        if (menuPanel != null) menuPanel.SetActive(true);
        UpdateStateText("Menu");
    }

    /// <summary>
    /// 显示游戏 UI
    /// </summary>
    public void ShowGameplayUI()
    {
        HideAllPanels();
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        UpdateStateText("Playing");
    }

    /// <summary>
    /// 显示暂停 UI
    /// </summary>
    public void ShowPauseUI()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        UpdateStateText("Paused");
    }

    /// <summary>
    /// 显示游戏结束 UI
    /// </summary>
    public void ShowGameOverUI(Dictionary<string, object> statistics)
    {
        HideAllPanels();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        
        DisplayGameStatistics(statistics);
        UpdateStateText("Game Over");
    }

    /// <summary>
    /// 切换设置面板
    /// </summary>
    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }

    /// <summary>
    /// 更新游戏状态
    /// </summary>
    public void UpdateGameState(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Menu:
                ShowMenuUI();
                break;
            case GameManager.GameState.Playing:
                ShowGameplayUI();
                break;
            case GameManager.GameState.Paused:
                ShowPauseUI();
                break;
            case GameManager.GameState.GameOver:
                // ShowGameOverUI 由调用者处理
                break;
        }
    }

    // ===== UI 更新方法 =====

    /// <summary>
    /// 更新计时器显示
    /// </summary>
    public void UpdateTimerDisplay(float remainingTime)
    {
        if (timerText != null)
        {
            int minutes = (int)(remainingTime / 60);
            int seconds = (int)(remainingTime % 60);
            timerText.text = $"Time: {minutes:D2}:{seconds:D2}";
        }
    }

    /// <summary>
    /// 更新鱼群数量显示
    /// </summary>
    public void UpdateFishCountDisplay(int count)
    {
        if (fishCountText != null)
        {
            fishCountText.text = $"Fish: {count}";
        }
    }

    /// <summary>
    /// 更新群速度显示
    /// </summary>
    public void UpdateSpeedDisplay(float speed)
    {
        if (speedText != null)
        {
            speedText.text = $"Speed: {speed:F2}";
        }
    }

    /// <summary>
    /// 更新分数显示
    /// </summary>
    public void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    /// <summary>
    /// 更新游戏状态文本
    /// </summary>
    private void UpdateStateText(string state)
    {
        if (stateText != null)
        {
            stateText.text = state;
        }
    }

    /// <summary>
    /// 更新统计信息
    /// </summary>
    private void UpdateStatistics(int fishCount, float speed)
    {
        UpdateFishCountDisplay(fishCount);
        UpdateSpeedDisplay(speed);
    }

    /// <summary>
    /// 显示游戏统计信息
    /// </summary>
    private void DisplayGameStatistics(Dictionary<string, object> statistics)
    {
        if (statistics == null)
            return;

        if (statistics.TryGetValue("Score", out object scoreObj) && scoreText != null)
        {
            UpdateScoreDisplay((int)scoreObj);
        }

        // 可以在此添加其他统计信息显示
        Debug.Log("[UIManager] Game Statistics:");
        foreach (var kvp in statistics)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value}");
        }
    }

    /// <summary>
    /// 显示通知消息
    /// </summary>
    public void ShowNotification(string message, float duration = 2f)
    {
        Debug.Log($"[UIManager] Notification: {message}");
        // TODO: 实现通知 UI
    }
}
