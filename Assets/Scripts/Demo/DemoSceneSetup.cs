using UnityEngine;

/// <summary>
/// 演示场景初始化脚本
/// 自动配置整个演示场景
/// </summary>
public class DemoSceneSetup : MonoBehaviour
{
    [Header("场景配置 | Scene Configuration")]
    [SerializeField]
    private int fishCount = 50;  // 鱼数
    
    [SerializeField]
    private float gameTime = 180f;  // 游戏时长
    
    [SerializeField]
    private bool autoStartGame = true;  // 自动启动
    
    [SerializeField]
    private float startDelay = 2f;  // 启动延迟

    [Header("管理器预设 | Manager Presets")]
    [SerializeField]
    private bool createManagers = true;  // 自动创建管理器
    
    [SerializeField]
    private bool createBoidSystem = true;  // 自动创建 Boid 系统
    
    [SerializeField]
    private bool createInteractionSystem = true;  // 自动创建交互系统
    
    [SerializeField]
    private bool createVisualEffects = true;  // 自动创建视觉效果

    [Header("环境配置 | Environment Preset")]
    [SerializeField]
    private Color waterColor = new Color(0.2f, 0.6f, 0.8f);  // 水颜色
    
    [SerializeField]
    private float fogDensity = 0.15f;  // 雾密度
    
    [SerializeField]
    private float ambientLightIntensity = 1.2f;  // 环境光强度

    [Header("调试 | Debug")]
    [SerializeField]
    private bool showDebugLog = true;  // 显示调试日志
    
    [SerializeField]
    private bool visualizeSceneBounds = true;  // 可视化场景边界

    private void Start()
    {
        if (showDebugLog)
            Debug.Log("[DemoSceneSetup] Starting demo scene initialization...");

        // 步骤 1: 创建管理器
        if (createManagers)
        {
            SetupManagers();
        }

        // 步骤 2: 创建场景环境
        SetupEnvironment();

        // 步骤 3: 创建 Boid 系统
        if (createBoidSystem)
        {
            SetupBoidSystem();
        }

        // 步骤 4: 创建交互系统
        if (createInteractionSystem)
        {
            SetupInteractionSystem();
        }

        // 步骤 5: 创建视觉效果
        if (createVisualEffects)
        {
            SetupVisualEffects();
        }

        // 步骤 6: 配置游戏管理器
        ConfigureGameManager();

        if (showDebugLog)
            Debug.Log("[DemoSceneSetup] Demo scene setup complete!");
    }

    /// <summary>
    /// 设置管理器
    /// </summary>
    private void SetupManagers()
    {
        if (showDebugLog)
            Debug.Log("[DemoSceneSetup] Setting up managers...");

        // 检查 GameManager 是否存在
        if (GameManager.Instance == null)
        {
            GameObject gameManagerGO = new GameObject("GameManager");
            GameManager gameManager = gameManagerGO.AddComponent<GameManager>();
            if (showDebugLog)
                Debug.Log("[DemoSceneSetup] Created GameManager");
        }

        // 检查 UIManager 是否存在
        if (FindObjectOfType<UIManager>() == null)
        {
            GameObject uiManagerGO = new GameObject("UIManager");
            UIManager uiManager = uiManagerGO.AddComponent<UIManager>();
            if (showDebugLog)
                Debug.Log("[DemoSceneSetup] Created UIManager");
        }

        // 检查 SceneController 是否存在
        if (FindObjectOfType<SceneController>() == null)
        {
            GameObject sceneControllerGO = new GameObject("SceneController");
            SceneController sceneController = sceneControllerGO.AddComponent<SceneController>();
            if (showDebugLog)
                Debug.Log("[DemoSceneSetup] Created SceneController");
        }
    }

    /// <summary>
    /// 设置场景环境
    /// </summary>
    private void SetupEnvironment()
    {
        if (showDebugLog)
            Debug.Log("[DemoSceneSetup] Setting up environment...");

        // 设置水颜色
        RenderSettings.fogColor = waterColor;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fog = true;

        // 设置环境光
        RenderSettings.ambientLight = new Color(0.5f, 0.7f, 1f) * ambientLightIntensity;

        // 设置背景
        Camera.main.backgroundColor = waterColor;

        // 创建主光源（如果没有）
        if (FindObjectOfType<Light>() == null)
        {
            GameObject lightGO = new GameObject("Directional Light");
            Light light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightGO.transform.rotation = Quaternion.Euler(50, -30, 0);
            if (showDebugLog)
                Debug.Log("[DemoSceneSetup] Created directional light");
        }
    }

    /// <summary>
    /// 设置 Boid 系统
    /// </summary>
    private void SetupBoidSystem()
    {
        if (showDebugLog)
            Debug.Log("[DemoSceneSetup] Setting up Boid system...");

        BoidManager boidManager = FindObjectOfType<BoidManager>();
        if (boidManager == null)
        {
            GameObject boidManagerGO = new GameObject("BoidManager");
            boidManager = boidManagerGO.AddComponent<BoidManager>();
            if (showDebugLog)
                Debug.Log("[DemoSceneSetup] Created BoidManager");
        }
    }

    /// <summary>
    /// 设置交互系统
    /// </summary>
    private void SetupInteractionSystem()
    {
        if (showDebugLog)
            Debug.Log("[DemoSceneSetup] Setting up interaction system...");

        InteractionController interactionController = FindObjectOfType<InteractionController>();
        if (interactionController == null)
        {
            GameObject interactionGO = new GameObject("InteractionController");
            interactionGO.AddComponent<InteractionController>();
            if (showDebugLog)
                Debug.Log("[DemoSceneSetup] Created InteractionController");
        }
    }

    /// <summary>
    /// 设置视觉效果
    /// </summary>
    private void SetupVisualEffects()
    {
        if (showDebugLog)
            Debug.Log("[DemoSceneSetup] Setting up visual effects...");

        VisualEffectsManager visualEffectsManager = FindObjectOfType<VisualEffectsManager>();
        if (visualEffectsManager == null)
        {
            GameObject visualEffectsGO = new GameObject("VisualEffectsManager");
            visualEffectsGO.AddComponent<VisualEffectsManager>();
            if (showDebugLog)
                Debug.Log("[DemoSceneSetup] Created VisualEffectsManager");
        }

        // 创建水环境效果
        if (FindObjectOfType<WaterEnvironmentEffects>() == null)
        {
            GameObject waterGO = new GameObject("WaterEnvironmentEffects");
            waterGO.AddComponent<WaterEnvironmentEffects>();
            if (showDebugLog)
                Debug.Log("[DemoSceneSetup] Created WaterEnvironmentEffects");
        }
    }

    /// <summary>
    /// 配置游戏管理器
    /// </summary>
    private void ConfigureGameManager()
    {
        if (showDebugLog)
            Debug.Log("[DemoSceneSetup] Configuring GameManager...");

        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            if (showDebugLog)
                Debug.Log($"[DemoSceneSetup] GameManager configured: {fishCount} fish, {gameTime}s");
        }
    }
}
