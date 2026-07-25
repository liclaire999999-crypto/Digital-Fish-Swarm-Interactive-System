using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏管理器
/// 负责游戏全局状态、流程管理和模块协调
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("单例设置 | Singleton Settings")]
    public static GameManager Instance { get; private set; }

    [Header("游戏状态 | Game States")]
    public enum GameState
    {
        Initializing,  // 初始化中
        Menu,          // 菜单状态
        Playing,       // 游戏中
        Paused,        // 暂停
        GameOver       // 游戏结束
    }
    
    private GameState currentState = GameState.Initializing;
    public GameState CurrentState => currentState;

    [Header("管理器引用 | Manager References")]
    [SerializeField]
    private BoidManager boidManager;  // Boids 管理器
    
    [SerializeField]
    private InteractionController interactionController;  // 交互控制器
    
    [SerializeField]
    private VisualEffectsManager visualEffectsManager;  // 视觉效果管理器
    
    private UIManager uiManager;  // UI 管理器
    private SceneController sceneController;  // 场景控制器

    [Header("游戏配置 | Game Configuration")]
    [SerializeField]
    private int initialFishCount = 50;  // 初始鱼群数量
    
    [SerializeField]
    private float gameTime = 180f;  // 游戏时长（秒）
    
    [SerializeField]
    private bool enableAutoStart = true;  // 自动启动游戏
    
    [SerializeField]
    private float autoStartDelay = 2f;  // 自动启动延迟

    [Header("游戏统计 | Game Statistics")]
    [SerializeField]
    private bool trackStatistics = true;  // 追踪统计
    
    private float elapsedTime = 0f;  // 已用时间
    private int totalFishEscaped = 0;  // 总逃散鱼数
    private float maxSwarmSpeed = 0f;  // 最大群速
    private int currentFishCount = 0;  // 当前鱼数

    [Header("事件委托 | Event Delegates")]
    public delegate void GameStateChangeEvent(GameState newState);
    public delegate void GameTimerEvent(float remainingTime);
    public delegate void StatisticsEvent(int fishCount, float speed);
    
    public event GameStateChangeEvent OnGameStateChanged;  // 游戏状态变化
    public event GameTimerEvent OnGameTimeUpdated;  // 游戏时间更新
    public event StatisticsEvent OnStatisticsUpdated;  // 统计信息更新

    private bool isPaused = false;  // 是否暂停
    private float pausedTime = 0f;  // 暂停时间

    private void Awake()
    {
        // 实现单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Initialize();

        if (enableAutoStart)
        {
            Invoke(nameof(StartGame), autoStartDelay);
        }
    }

    private void Update()
    {
        if (currentState != GameState.Playing)
            return;

        // 更新游戏时间
        if (!isPaused)
        {
            elapsedTime += Time.deltaTime;
            OnGameTimeUpdated?.Invoke(Mathf.Max(0, gameTime - elapsedTime));

            // 检查游戏是否结束
            if (elapsedTime >= gameTime)
            {
                EndGame();
            }
        }

        // 更新统计信息
        if (trackStatistics)
        {
            UpdateStatistics();
        }
    }

    /// <summary>
    /// 初始化游戏管理器
    /// </summary>
    private void Initialize()
    {
        Debug.Log("[GameManager] Initializing...");

        // 获取或创建必要的管理器
        if (boidManager == null)
        {
            boidManager = FindObjectOfType<BoidManager>();
        }

        if (interactionController == null)
        {
            interactionController = FindObjectOfType<InteractionController>();
        }

        if (visualEffectsManager == null)
        {
            visualEffectsManager = FindObjectOfType<VisualEffectsManager>();
        }

        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("[GameManager] UIManager not found!");
        }

        sceneController = FindObjectOfType<SceneController>();
        if (sceneController == null)
        {
            Debug.LogWarning("[GameManager] SceneController not found!");
        }

        SetGameState(GameState.Menu);
        Debug.Log("[GameManager] Initialization complete.");
    }

    /// <summary>
    /// 设置游戏状态
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (currentState == newState)
            return;

        GameState previousState = currentState;
        currentState = newState;

        Debug.Log($"[GameManager] Game state changed: {previousState} -> {newState}");

        OnGameStateChanged?.Invoke(currentState);
        uiManager?.UpdateGameState(currentState);

        // 根据状态执行相应操作
        switch (newState)
        {
            case GameState.Menu:
                HandleMenuState();
                break;
            case GameState.Playing:
                HandlePlayingState();
                break;
            case GameState.Paused:
                HandlePausedState();
                break;
            case GameState.GameOver:
                HandleGameOverState();
                break;
        }
    }

    /// <summary>
    /// 处理菜单状态
    /// </summary>
    private void HandleMenuState()
    {
        Time.timeScale = 0f;  // 暂停物理引擎
        
        if (interactionController != null)
            interactionController.SetInteractionEnabled(false);

        uiManager?.ShowMenuUI();
    }

    /// <summary>
    /// 处理游戏中状态
    /// </summary>
    private void HandlePlayingState()
    {
        Time.timeScale = 1f;  // 恢复物理引擎
        isPaused = false;
        
        if (boidManager != null && currentFishCount == 0)
        {
            boidManager.Initialize();
            currentFishCount = initialFishCount;
        }

        if (interactionController != null)
            interactionController.SetInteractionEnabled(true);

        uiManager?.ShowGameplayUI();
    }

    /// <summary>
    /// 处理暂停状态
    /// </summary>
    private void HandlePausedState()
    {
        Time.timeScale = 0f;  // 暂停物理引擎
        isPaused = true;
        pausedTime = elapsedTime;
        
        if (interactionController != null)
            interactionController.SetInteractionEnabled(false);

        uiManager?.ShowPauseUI();
    }

    /// <summary>
    /// 处理游戏结束状态
    /// </summary>
    private void HandleGameOverState()
    {
        Time.timeScale = 0f;  // 暂停物理引擎
        
        if (interactionController != null)
            interactionController.SetInteractionEnabled(false);

        uiManager?.ShowGameOverUI(GetGameStatistics());
    }

    /// <summary>
    /// 启动游戏
    /// </summary>
    public void StartGame()
    {
        elapsedTime = 0f;
        totalFishEscaped = 0;
        currentFishCount = initialFishCount;
        SetGameState(GameState.Playing);
    }

    /// <summary>
    /// 暂停/恢复游戏
    /// </summary>
    public void TogglePause()
    {
        if (currentState != GameState.Playing)
            return;

        if (isPaused)
        {
            SetGameState(GameState.Playing);
        }
        else
        {
            SetGameState(GameState.Paused);
        }
    }

    /// <summary>
    /// 结束游戏
    /// </summary>
    public void EndGame()
    {
        SetGameState(GameState.GameOver);
    }

    /// <summary>
    /// 重启游戏
    /// </summary>
    public void RestartGame()
    {
        if (boidManager != null)
        {
            boidManager.ClearAllBoids();
        }

        elapsedTime = 0f;
        totalFishEscaped = 0;
        currentFishCount = 0;
        
        StartGame();
    }

    /// <summary>
    /// 返回菜单
    /// </summary>
    public void ReturnToMenu()
    {
        if (boidManager != null)
        {
            boidManager.ClearAllBoids();
        }

        SetGameState(GameState.Menu);
    }

    /// <summary>
    /// 更新统计信息
    /// </summary>
    private void UpdateStatistics()
    {
        if (boidManager == null)
            return;

        currentFishCount = boidManager.GetActiveBoidCount();

        // 计算群速
        var activeBoids = boidManager.GetActiveBoids();
        if (activeBoids.Count > 0)
        {
            float totalSpeed = 0f;
            foreach (var boid in activeBoids)
            {
                totalSpeed += boid.GetVelocity().magnitude;
            }
            float avgSpeed = totalSpeed / activeBoids.Count;
            maxSwarmSpeed = Mathf.Max(maxSwarmSpeed, avgSpeed);
        }

        OnStatisticsUpdated?.Invoke(currentFishCount, maxSwarmSpeed);
    }

    /// <summary>
    /// 获取游戏统计信息
    /// </summary>
    public Dictionary<string, object> GetGameStatistics()
    {
        return new Dictionary<string, object>
        {
            { "ElapsedTime", elapsedTime },
            { "GameTime", gameTime },
            { "FishCount", currentFishCount },
            { "MaxSwarmSpeed", maxSwarmSpeed },
            { "TotalFishEscaped", totalFishEscaped },
            { "Score", CalculateScore() }
        };
    }

    /// <summary>
    /// 计算游戏分数
    /// </summary>
    private int CalculateScore()
    {
        // 分数 = 存活鱼数 × 时间倍数 + 群速度倍数
        int score = currentFishCount * 100;
        score += (int)(elapsedTime * 10);
        score += (int)(maxSwarmSpeed * 50);
        return score;
    }

    /// <summary>
    /// 记录鱼逃散事件
    /// </summary>
    public void RecordFishEscaped()
    {
        totalFishEscaped++;
        currentFishCount--;
    }

    /// <summary>
    /// 获取剩余游戏时间
    /// </summary>
    public float GetRemainingTime()
    {
        return Mathf.Max(0, gameTime - elapsedTime);
    }

    /// <summary>
    /// 获取当前鱼群数量
    /// </summary>
    public int GetCurrentFishCount()
    {
        return currentFishCount;
    }

    /// <summary>
    /// 获取游戏进度（0-1）
    /// </summary>
    public float GetGameProgress()
    {
        return Mathf.Clamp01(elapsedTime / gameTime);
    }

    /// <summary>
    /// 是否暂停中
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }
}
