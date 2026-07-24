using UnityEngine;

/// <summary>
/// 交互控制器
/// 将手势映射到鱼群行为
/// </summary>
public class InteractionController : MonoBehaviour
{
    [Header("管理器引用 | Manager References")]
    [SerializeField]
    private BoidManager boidManager;  // Boids 管理器
    
    private GestureDetector gestureDetector;
    private HandTracker handTracker;

    [Header("交互设置 | Interaction Settings")]
    [SerializeField]
    private bool enableInteraction = true;  // 启用交互
    
    [SerializeField]
    private float escapeRadius = 20f;  // 逃散半径
    
    [SerializeField]
    private float escapeIntensity = 1f;  // 逃散强度倍数
    
    [SerializeField]
    private float handTrackingSmooth = 0.2f;  // 手追踪平滑度

    [Header("事件触发 | Event Triggers")]
    [SerializeField]
    private bool triggerOnHandDetected = true;  // 手被检测到时触发
    
    [SerializeField]
    private bool triggerOnGrab = true;  // 捏合时触发
    
    [SerializeField]
    private bool triggerOnHover = false;  // 悬停时触发

    [Header("调试信息 | Debug")]
    [SerializeField]
    private bool showDebugInfo = true;
    
    [SerializeField]
    private bool drawInteractionVisualization = true;

    private Vector3 lastInteractionPos = Vector3.zero;  // 上次交互位置
    private bool isInteracting = false;  // 是否正在交互
    private float interactionTimer = 0f;  // 交互计时器

    private void Start()
    {
        // 获取或创建必要的组件
        gestureDetector = GetComponent<GestureDetector>();
        if (gestureDetector == null)
        {
            gestureDetector = gameObject.AddComponent<GestureDetector>();
        }

        handTracker = GetComponent<HandTracker>();
        if (handTracker == null)
        {
            handTracker = gameObject.AddComponent<HandTracker>();
        }

        // 获取 BoidManager
        if (boidManager == null)
        {
            boidManager = FindObjectOfType<BoidManager>();
        }

        // 注册手势事件
        if (gestureDetector != null)
        {
            gestureDetector.OnHandDetected += HandleHandDetected;
            gestureDetector.OnHandLost += HandleHandLost;
            gestureDetector.OnHandGrabbed += HandleHandGrabbed;
            gestureDetector.OnHandReleased += HandleHandReleased;
        }

        // 注册追踪事件
        if (handTracker != null)
        {
            handTracker.OnHandMoved += HandleHandMoved;
        }
    }

    private void OnDestroy()
    {
        // 注销事件
        if (gestureDetector != null)
        {
            gestureDetector.OnHandDetected -= HandleHandDetected;
            gestureDetector.OnHandLost -= HandleHandLost;
            gestureDetector.OnHandGrabbed -= HandleHandGrabbed;
            gestureDetector.OnHandReleased -= HandleHandReleased;
        }

        if (handTracker != null)
        {
            handTracker.OnHandMoved -= HandleHandMoved;
        }
    }

    private void Update()
    {
        if (!enableInteraction || boidManager == null)
            return;

        // 更新交互
        UpdateInteraction();
    }

    private void OnGUI()
    {
        if (showDebugInfo)
        {
            DrawDebugInfo();
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawInteractionVisualization || !Application.isPlaying)
            return;

        // 绘制交互区域
        if (gestureDetector != null)
        {
            if (gestureDetector.IsLeftHandDetected())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(gestureDetector.GetLeftHandPosition(), escapeRadius);
            }

            if (gestureDetector.IsRightHandDetected())
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(gestureDetector.GetRightHandPosition(), escapeRadius);
            }
        }
    }

    /// <summary>
    /// 更新交互逻辑
    /// </summary>
    private void UpdateInteraction()
    {
        if (isInteracting)
        {
            interactionTimer += Time.deltaTime;
        }

        // 持续逃散交互
        if (triggerOnHover && gestureDetector != null)
        {
            Vector3 interactionPos = Vector3.zero;
            int handCount = 0;

            if (gestureDetector.IsLeftHandDetected())
            {
                interactionPos += gestureDetector.GetLeftHandPosition();
                handCount++;
            }

            if (gestureDetector.IsRightHandDetected())
            {
                interactionPos += gestureDetector.GetRightHandPosition();
                handCount++;
            }

            if (handCount > 0)
            {
                interactionPos /= handCount;
                TriggerEscape(interactionPos);
            }
        }
    }

    // ===== 事件处理 =====

    /// <summary>
    /// 手被检测到事件
    /// </summary>
    private void HandleHandDetected(bool isLeftHand, Vector3 handPosition)
    {
        if (!triggerOnHandDetected || boidManager == null)
            return;

        Debug.Log($"[Interaction] Hand detected: {(isLeftHand ? "Left" : "Right")} at {handPosition}");

        if (triggerOnHandDetected)
        {
            TriggerEscape(handPosition);
        }
    }

    /// <summary>
    /// 手丢失事件
    /// </summary>
    private void HandleHandLost(bool isLeftHand, Vector3 handPosition)
    {
        Debug.Log($"[Interaction] Hand lost: {(isLeftHand ? "Left" : "Right")}");
        
        if (!gestureDetector.IsLeftHandDetected() && !gestureDetector.IsRightHandDetected())
        {
            StopEscape();
        }
    }

    /// <summary>
    /// 手抓住事件（捏合）
    /// </summary>
    private void HandleHandGrabbed(bool isLeftHand, Vector3 handPosition)
    {
        if (!triggerOnGrab || boidManager == null)
            return;

        Debug.Log($"[Interaction] Hand grabbed: {(isLeftHand ? "Left" : "Right")} at {handPosition}");

        isInteracting = true;
        interactionTimer = 0f;
        
        TriggerEscape(handPosition, escapeIntensity * 1.5f);  // 捏合时加强效果
    }

    /// <summary>
    /// 手释放事件
    /// </summary>
    private void HandleHandReleased(bool isLeftHand, Vector3 handPosition)
    {
        Debug.Log($"[Interaction] Hand released: {(isLeftHand ? "Left" : "Right")}");
        
        isInteracting = false;
        
        if (!gestureDetector.IsLeftHandDetected() && !gestureDetector.IsRightHandDetected())
        {
            StopEscape();
        }
    }

    /// <summary>
    /// 手移动事件
    /// </summary>
    private void HandleHandMoved(bool isLeftHand, Vector3 position, Vector3 velocity)
    {
        lastInteractionPos = position;
        
        // 根据速度调整逃散强度
        if (triggerOnHover)
        {
            float speed = velocity.magnitude;
            float intensity = 1f + (speed * 0.5f);  // 速度越快，效果越强
            intensity = Mathf.Clamp(intensity, 1f, 2f);
            
            TriggerEscape(position, intensity);
        }
    }

    // ===== 公共接口 =====

    /// <summary>
    /// 触发鱼群逃散
    /// </summary>
    public void TriggerEscape(Vector3 threatPosition, float intensity = 1f)
    {
        if (boidManager == null)
            return;

        // 获取所有 Boids
        var boids = boidManager.GetActiveBoids();

        foreach (BoidAgent boid in boids)
        {
            float distance = Vector3.Distance(boid.GetPosition(), threatPosition);
            
            if (distance < escapeRadius)
            {
                // 根据距离计算逃散强度
                float distanceFactor = 1f - (distance / escapeRadius);
                float finalIntensity = intensity * distanceFactor;
                
                // 调整 Boids 参数以增强逃散
                boid.TriggerEscape(threatPosition);
            }
        }
    }

    /// <summary>
    /// 停止逃散
    /// </summary>
    public void StopEscape()
    {
        if (boidManager == null)
            return;

        boidManager.StopFleeAll();
    }

    /// <summary>
    /// 设置逃散半径
    /// </summary>
    public void SetEscapeRadius(float radius)
    {
        escapeRadius = Mathf.Max(1f, radius);
    }

    /// <summary>
    /// 设置逃散强度
    /// </summary>
    public void SetEscapeIntensity(float intensity)
    {
        escapeIntensity = Mathf.Max(0.1f, intensity);
    }

    /// <summary>
    /// 启用/禁用交互
    /// </summary>
    public void SetInteractionEnabled(bool enabled)
    {
        enableInteraction = enabled;
    }

    /// <summary>
    /// 获取是否正在交互
    /// </summary>
    public bool IsInteracting()
    {
        return isInteracting;
    }

    /// <summary>
    /// 获取交互时长
    /// </summary>
    public float GetInteractionDuration()
    {
        return interactionTimer;
    }

    /// <summary>
    /// 绘制调试信息
    /// </summary>
    private void DrawDebugInfo()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Label("=== Interaction Controller ===");
        GUILayout.Label($"Enabled: {enableInteraction}");
        GUILayout.Label($"Interacting: {isInteracting}");
        GUILayout.Label($"Escape Radius: {escapeRadius:F1}");
        GUILayout.Label($"Escape Intensity: {escapeIntensity:F2}");
        GUILayout.Label($"Last Interaction: {lastInteractionPos}");
        GUILayout.EndArea();
    }
}
