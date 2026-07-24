using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Boids 管理器
/// 负责创建、更新和管理所有鱼的代理
/// </summary>
public class BoidManager : MonoBehaviour
{
    [Header("Boids 配置 | Boids Settings")]
    [SerializeField]
    private BoidSettings boidSettings;  // 如果为 null，将创建默认配置
    
    [SerializeField]
    private GameObject boidPrefab;  // 鱼的预制体
    
    [Header("管理参数 | Management Parameters")]
    [SerializeField]
    private bool autoInitialize = true;  // 启动时自动初始化
    
    [SerializeField]
    private int initialBoidCount = 50;  // 初始鱼群数量

    [Header("性能监控 | Performance Monitoring")]
    [SerializeField]
    private bool showDebugInfo = true;
    
    private List<BoidAgent> activeBoids = new List<BoidAgent>();
    private List<BoidAgent> inactiveBoids = new List<BoidAgent>();  // 对象池
    
    private BoidSettings runtimeSettings;  // 运行时配置副本
    private bool isInitialized = false;
    
    private float fps = 0f;  // FPS 计算
    private float fpsUpdateTimer = 0f;

    private void Start()
    {
        if (autoInitialize)
        {
            Initialize();
        }
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        // 更新所有 Boids
        UpdateAllBoids();

        // 计算 FPS
        UpdateFPS();
    }

    private void OnGUI()
    {
        if (showDebugInfo && isInitialized)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.Label($"Boids Count: {activeBoids.Count}");
            GUILayout.Label($"FPS: {fps:F1}");
            GUILayout.Label($"Active Boids: {activeBoids.Count}");
            GUILayout.Label($"Pooled Boids: {inactiveBoids.Count}");
            GUILayout.EndArea();
        }
    }

    /// <summary>
    /// 初始化 Boids 管理器
    /// </summary>
    public void Initialize()
    {
        if (isInitialized)
            return;

        // 验证或创建配置
        if (boidSettings == null)
        {
            Debug.LogWarning("BoidSettings not assigned! Creating default settings.");
            runtimeSettings = ScriptableObject.CreateInstance<BoidSettings>();
        }
        else
        {
            runtimeSettings = boidSettings.Clone();
        }

        runtimeSettings.ValidateSettings();

        // 验证预制体
        if (boidPrefab == null)
        {
            Debug.LogError("Boid Prefab not assigned! Cannot initialize BoidManager.");
            return;
        }

        // 创建初始 Boids
        int boidCount = Mathf.Min(initialBoidCount, runtimeSettings.maxBoidCount);
        for (int i = 0; i < boidCount; i++)
        {
            SpawnBoid();
        }

        isInitialized = true;
        Debug.Log($"BoidManager initialized with {activeBoids.Count} boids.");
    }

    /// <summary>
    /// 生成一条新鱼
    /// </summary>
    public BoidAgent SpawnBoid()
    {
        if (activeBoids.Count >= runtimeSettings.maxBoidCount)
        {
            Debug.LogWarning("Max boid count reached!");
            return null;
        }

        BoidAgent boid;

        // 从对象池获取或创建新的
        if (inactiveBoids.Count > 0)
        {
            boid = inactiveBoids[inactiveBoids.Count - 1];
            inactiveBoids.RemoveAt(inactiveBoids.Count - 1);
            boid.gameObject.SetActive(true);
        }
        else
        {
            GameObject boidGO = Instantiate(boidPrefab, transform);
            boid = boidGO.GetComponent<BoidAgent>();
            if (boid == null)
            {
                boid = boidGO.AddComponent<BoidAgent>();
            }
        }

        // 初始化鱼
        Vector3 spawnPosition = GetRandomSpawnPosition();
        boid.Initialize(runtimeSettings, this, spawnPosition);
        
        activeBoids.Add(boid);
        return boid;
    }

    /// <summary>
    /// 移除鱼（放入对象池）
    /// </summary>
    public void RemoveBoid(BoidAgent boid)
    {
        if (activeBoids.Contains(boid))
        {
            activeBoids.Remove(boid);
            boid.gameObject.SetActive(false);
            inactiveBoids.Add(boid);
        }
    }

    /// <summary>
    /// 注册 Boid（由 BoidAgent 调用）
    /// </summary>
    public void RegisterBoid(BoidAgent boid)
    {
        if (!activeBoids.Contains(boid))
        {
            activeBoids.Add(boid);
        }
    }

    /// <summary>
    /// 注销 Boid（由 BoidAgent 调用）
    /// </summary>
    public void UnregisterBoid(BoidAgent boid)
    {
        activeBoids.Remove(boid);
        inactiveBoids.Remove(boid);
    }

    /// <summary>
    /// 更新所有 Boids
    /// </summary>
    private void UpdateAllBoids()
    {
        // 更新每个 Boid 的行为
        for (int i = 0; i < activeBoids.Count; i++)
        {
            activeBoids[i].UpdateBoid(activeBoids);
        }
    }

    /// <summary>
    /// 获取随机生成位置
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-runtimeSettings.spawnRadius, runtimeSettings.spawnRadius);
        float y = Random.Range(-runtimeSettings.spawnRadius * 0.5f, runtimeSettings.spawnRadius * 0.5f);
        float z = Random.Range(-runtimeSettings.spawnRadius, runtimeSettings.spawnRadius);
        
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 触发所有 Boids 的逃散行为
    /// （由手势识别模块调用）
    /// </summary>
    public void TriggerFleeAll(Vector3 threatPosition)
    {
        foreach (BoidAgent boid in activeBoids)
        {
            boid.TriggerEscape(threatPosition);
        }
    }

    /// <summary>
    /// 停止所有 Boids 的逃散行为
    /// </summary>
    public void StopFleeAll()
    {
        foreach (BoidAgent boid in activeBoids)
        {
            boid.StopEscape();
        }
    }

    /// <summary>
    /// 获取所有活跃的 Boids
    /// </summary>
    public List<BoidAgent> GetActiveBoids()
    {
        return new List<BoidAgent>(activeBoids);
    }

    /// <summary>
    /// 获取活跃 Boids 数量
    /// </summary>
    public int GetActiveBoidCount()
    {
        return activeBoids.Count;
    }

    /// <summary>
    /// 清空所有 Boids
    /// </summary>
    public void ClearAllBoids()
    {
        foreach (BoidAgent boid in activeBoids)
        {
            Destroy(boid.gameObject);
        }
        activeBoids.Clear();
        inactiveBoids.Clear();
    }

    /// <summary>
    /// 设置 Boids 配置
    /// </summary>
    public void SetSettings(BoidSettings newSettings)
    {
        if (newSettings != null)
        {
            runtimeSettings = newSettings.Clone();
            runtimeSettings.ValidateSettings();
        }
    }

    /// <summary>
    /// 获取当前配置
    /// </summary>
    public BoidSettings GetSettings()
    {
        return runtimeSettings;
    }

    /// <summary>
    /// 计算 FPS
    /// </summary>
    private void UpdateFPS()
    {
        fpsUpdateTimer += Time.deltaTime;
        if (fpsUpdateTimer >= 0.5f)  // 每 0.5 秒更新一次
        {
            fps = 1f / Time.deltaTime;
            fpsUpdateTimer = 0f;
        }
    }

    /// <summary>
    /// 获取当前 FPS
    /// </summary>
    public float GetCurrentFPS()
    {
        return fps;
    }
}
