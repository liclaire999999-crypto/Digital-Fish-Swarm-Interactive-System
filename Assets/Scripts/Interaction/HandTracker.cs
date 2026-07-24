using UnityEngine;

/// <summary>
/// 手部追踪器
/// 负责跟踪手部运动和手势
/// </summary>
public class HandTracker : MonoBehaviour
{
    [Header("追踪配置 | Tracking Configuration")]
    [SerializeField]
    private float positionSmoothTime = 0.1f;  // 位置平滑时间
    
    [SerializeField]
    private float velocityThreshold = 0.5f;  // 速度阈值
    
    [SerializeField]
    private bool enableMovementTracking = true;  // 启用运动追踪

    [Header("组件引用 | Component References")]
    private GestureDetector gestureDetector;

    // 左手追踪
    private Vector3 leftHandPosition;  // 当前位置
    private Vector3 leftHandVelocity;  // 当前速度
    private Vector3 leftHandSmoothedPos;  // 平滑位置
    private Vector3 leftHandVelRef;  // 平滑参考

    // 右手追踪
    private Vector3 rightHandPosition;
    private Vector3 rightHandVelocity;
    private Vector3 rightHandSmoothedPos;
    private Vector3 rightHandVelRef;

    // 追踪事件
    public delegate void HandMovementEvent(bool isLeftHand, Vector3 position, Vector3 velocity);
    public event HandMovementEvent OnHandMoved;  // 手移动
    public event HandMovementEvent OnHandSpeedChanged;  // 手速度变化

    private void Start()
    {
        gestureDetector = GetComponent<GestureDetector>();
        if (gestureDetector == null)
        {
            gestureDetector = gameObject.AddComponent<GestureDetector>();
        }
    }

    private void Update()
    {
        if (!enableMovementTracking)
            return;

        // 追踪左手
        UpdateHandTracking(true);
        
        // 追踪右手
        UpdateHandTracking(false);
    }

    /// <summary>
    /// 更新手部追踪
    /// </summary>
    private void UpdateHandTracking(bool isLeftHand)
    {
        if (isLeftHand)
        {
            if (!gestureDetector.IsLeftHandDetected())
            {
                leftHandPosition = Vector3.zero;
                leftHandVelocity = Vector3.zero;
                return;
            }

            Vector3 newPos = gestureDetector.GetLeftHandPosition();
            Vector3 lastPos = leftHandPosition;
            
            // 计算速度
            leftHandVelocity = (newPos - lastPos) / Time.deltaTime;
            leftHandPosition = newPos;

            // 平滑位置
            leftHandSmoothedPos = Vector3.SmoothDamp(
                leftHandSmoothedPos,
                leftHandPosition,
                ref leftHandVelRef,
                positionSmoothTime
            );

            // 触发事件
            if (leftHandVelocity.magnitude > 0.01f)
            {
                OnHandMoved?.Invoke(true, leftHandSmoothedPos, leftHandVelocity);
            }
        }
        else
        {
            if (!gestureDetector.IsRightHandDetected())
            {
                rightHandPosition = Vector3.zero;
                rightHandVelocity = Vector3.zero;
                return;
            }

            Vector3 newPos = gestureDetector.GetRightHandPosition();
            Vector3 lastPos = rightHandPosition;
            
            // 计算速度
            rightHandVelocity = (newPos - lastPos) / Time.deltaTime;
            rightHandPosition = newPos;

            // 平滑位置
            rightHandSmoothedPos = Vector3.SmoothDamp(
                rightHandSmoothedPos,
                rightHandPosition,
                ref rightHandVelRef,
                positionSmoothTime
            );

            // 触发事件
            if (rightHandVelocity.magnitude > 0.01f)
            {
                OnHandMoved?.Invoke(false, rightHandSmoothedPos, rightHandVelocity);
            }
        }
    }

    // ===== 公共接口 =====

    /// <summary>
    /// 获取左手当前位置
    /// </summary>
    public Vector3 GetLeftHandPosition()
    {
        return leftHandSmoothedPos;
    }

    /// <summary>
    /// 获取右手当前位置
    /// </summary>
    public Vector3 GetRightHandPosition()
    {
        return rightHandSmoothedPos;
    }

    /// <summary>
    /// 获取左手速度
    /// </summary>
    public Vector3 GetLeftHandVelocity()
    {
        return leftHandVelocity;
    }

    /// <summary>
    /// 获取右手速度
    /// </summary>
    public Vector3 GetRightHandVelocity()
    {
        return rightHandVelocity;
    }

    /// <summary>
    /// 获取左手速度大小
    /// </summary>
    public float GetLeftHandSpeed()
    {
        return leftHandVelocity.magnitude;
    }

    /// <summary>
    /// 获取右手速度大小
    /// </summary>
    public float GetRightHandSpeed()
    {
        return rightHandVelocity.magnitude;
    }

    /// <summary>
    /// 检查手是否快速移动
    /// </summary>
    public bool IsHandMovingFast(bool isLeftHand)
    {
        if (isLeftHand)
        {
            return leftHandVelocity.magnitude > velocityThreshold;
        }
        else
        {
            return rightHandVelocity.magnitude > velocityThreshold;
        }
    }
}
