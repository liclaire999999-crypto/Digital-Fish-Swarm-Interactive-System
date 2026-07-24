using UnityEngine;

/// <summary>
/// 鱼的代理脚本
/// 负责鱼的整体管理，连接 BoidAgent 和 FishController
/// </summary>
public class FishAgent : MonoBehaviour
{
    [Header("组件引用 | Component References")]
    private BoidAgent boidAgent;  // Boids 算法代理
    private FishController fishController;  // 鱼的控制器
    private FishAnimator fishAnimator;  // 鱼的动画控制

    [Header("鱼的属性 | Fish Properties")]
    [SerializeField]
    private string fishName = "Fish";
    
    [SerializeField]
    private int fishID = 0;  // 鱼的唯一 ID

    [Header("视觉配置 | Visual Configuration")]
    [SerializeField]
    private Color fishColor = Color.cyan;
    
    [SerializeField]
    private float meshScale = 1f;  // 鱼网格缩放

    [Header("状态 | State")]
    private bool isInitialized = false;
    private bool isActive = true;

    private void Awake()
    {
        // 获取组件
        boidAgent = GetComponent<BoidAgent>();
        fishController = GetComponent<FishController>();
        fishAnimator = GetComponent<FishAnimator>();

        // 确保所有必需组件存在
        if (boidAgent == null)
            boidAgent = gameObject.AddComponent<BoidAgent>();
        
        if (fishController == null)
            fishController = gameObject.AddComponent<FishController>();
        
        if (fishAnimator == null)
            fishAnimator = gameObject.AddComponent<FishAnimator>();
    }

    /// <summary>
    /// 初始化鱼代理
    /// </summary>
    public void Initialize(BoidSettings settings, BoidManager manager, int id, Vector3 spawnPosition)
    {
        if (isInitialized)
            return;

        fishID = id;
        gameObject.name = $"{fishName}_{fishID}";

        // 初始化 Boid 代理
        boidAgent.Initialize(settings, manager, spawnPosition);

        // 初始化控制器
        fishController.Initialize(this, settings);

        // 初始化动画
        fishAnimator.Initialize();

        // 应用视觉配置
        ApplyVisualConfiguration();

        isInitialized = true;
    }

    /// <summary>
    /// 应用视觉配置
    /// </summary>
    private void ApplyVisualConfiguration()
    {
        // 设置渲染器颜色（如果有）
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(renderer.material);
            material.color = fishColor;
            renderer.material = material;
        }

        // 应用缩放
        transform.localScale = Vector3.one * meshScale;
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    private void Update()
    {
        if (!isInitialized || !isActive)
            return;

        // 从 BoidAgent 获取速度和位置
        Vector3 velocity = boidAgent.GetVelocity();
        Vector3 position = boidAgent.GetPosition();

        // 更新控制器
        fishController.UpdateMovement(velocity, position);

        // 更新动画
        UpdateAnimation(velocity);
    }

    /// <summary>
    /// 更新动画状态
    /// </summary>
    private void UpdateAnimation(Vector3 velocity)
    {
        float speed = velocity.magnitude;
        bool isMoving = speed > 0.1f;

        // 播放游动动画
        if (isMoving)
        {
            fishAnimator.PlaySwimAnimation(speed);
        }
        else
        {
            fishAnimator.PlayIdleAnimation();
        }

        // 根据逃散状态改变动画
        if (boidAgent.IsEscaping())
        {
            fishAnimator.PlayFleeAnimation(speed);
        }
    }

    /// <summary>
    /// 触发逃散行为（由手势交互调用）
    /// </summary>
    public void TriggerFlee(Vector3 threatPosition)
    {
        boidAgent.TriggerEscape(threatPosition);
    }

    /// <summary>
    /// 停止逃散
    /// </summary>
    public void StopFlee()
    {
        boidAgent.StopEscape();
    }

    /// <summary>
    /// 获取鱼的位置
    /// </summary>
    public Vector3 GetPosition()
    {
        return boidAgent.GetPosition();
    }

    /// <summary>
    /// 获取鱼的速度
    /// </summary>
    public Vector3 GetVelocity()
    {
        return boidAgent.GetVelocity();
    }

    /// <summary>
    /// 获取鱼的 ID
    /// </summary>
    public int GetFishID()
    {
        return fishID;
    }

    /// <summary>
    /// 获取是否处于逃散状态
    /// </summary>
    public bool IsEscaping()
    {
        return boidAgent.IsEscaping();
    }

    /// <summary>
    /// 设置鱼的颜色
    /// </summary>
    public void SetFishColor(Color newColor)
    {
        fishColor = newColor;
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(renderer.material);
            material.color = newColor;
            renderer.material = material;
        }
    }

    /// <summary>
    /// 设置活跃状态
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);
    }

    /// <summary>
    /// 获取邻近鱼的数量
    /// </summary>
    public int GetNeighborCount()
    {
        return boidAgent.GetNeighborCount();
    }

    /// <summary>
    /// 获取 BoidAgent 引用
    /// </summary>
    public BoidAgent GetBoidAgent()
    {
        return boidAgent;
    }

    /// <summary>
    /// 获取 FishController 引用
    /// </summary>
    public FishController GetFishController()
    {
        return fishController;
    }

    /// <summary>
    /// 获取 FishAnimator 引用
    /// </summary>
    public FishAnimator GetFishAnimator()
    {
        return fishAnimator;
    }
}
