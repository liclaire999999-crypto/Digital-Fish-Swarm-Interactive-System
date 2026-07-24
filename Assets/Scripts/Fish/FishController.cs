using UnityEngine;

/// <summary>
/// 鱼的控制器
/// 负责鱼的移动、转向和物理行为
/// </summary>
public class FishController : MonoBehaviour
{
    [Header("运动配置 | Movement Configuration")]
    [SerializeField]
    private float rotationSmoothTime = 0.3f;  // 转向平滑时间
    
    [SerializeField]
    private float positionSmoothTime = 0.1f;  // 位置平滑时间
    
    [SerializeField]
    private bool useQuaternionRotation = true;  // 是否使用四元数平滑旋转

    [Header("视觉设置 | Visual Settings")]
    [SerializeField]
    private bool showDebugInfo = false;  // 显示调试信息

    private FishAgent fishAgent;
    private BoidSettings boidSettings;
    
    private Vector3 lastPosition;  // 上一帧位置
    private Vector3 targetDirection;  // 目标方向
    private Quaternion targetRotation;  // 目标旋转
    private Quaternion currentRotation;  // 当前旋转
    private float rotationVelocity = 0f;  // 旋转速度参数

    /// <summary>
    /// 初始化鱼的控制器
    /// </summary>
    public void Initialize(FishAgent agent, BoidSettings settings)
    {
        fishAgent = agent;
        boidSettings = settings;
        lastPosition = transform.position;
        currentRotation = transform.rotation;
        targetRotation = currentRotation;
    }

    /// <summary>
    /// 更新鱼的移动和转向
    /// </summary>
    public void UpdateMovement(Vector3 velocity, Vector3 position)
    {
        // 更新位置
        UpdatePosition(position);

        // 更新转向
        UpdateRotation(velocity);

        lastPosition = position;
    }

    /// <summary>
    /// 更新位置（已在 BoidAgent 中处理，这里作为备用）
    /// </summary>
    private void UpdatePosition(Vector3 newPosition)
    {
        // 如果需要平滑位置更新，可以在这里实现
        // 目前由 BoidAgent 直接管理
        transform.position = newPosition;
    }

    /// <summary>
    /// 更新转向（面向移动方向）
    /// </summary>
    private void UpdateRotation(Vector3 velocity)
    {
        if (velocity.magnitude < 0.01f)
            return;  // 速度过小时不转向

        // 计算目标方向
        targetDirection = velocity.normalized;
        targetRotation = Quaternion.LookRotation(targetDirection);

        // 平滑旋转
        if (useQuaternionRotation)
        {
            currentRotation = Quaternion.Lerp(
                currentRotation,
                targetRotation,
                Time.deltaTime / rotationSmoothTime
            );
        }
        else
        {
            // 替代方案：使用欧拉角平滑
            Vector3 currentEuler = currentRotation.eulerAngles;
            Vector3 targetEuler = targetRotation.eulerAngles;
            
            currentEuler = SmoothEulerAngles(currentEuler, targetEuler, rotationSmoothTime);
            currentRotation = Quaternion.Euler(currentEuler);
        }

        transform.rotation = currentRotation;
    }

    /// <summary>
    /// 平滑欧拉角（处理 0-360 循环）
    /// </summary>
    private Vector3 SmoothEulerAngles(Vector3 current, Vector3 target, float smoothTime)
    {
        Vector3 result = current;

        result.x = Mathf.LerpAngle(current.x, target.x, Time.deltaTime / smoothTime);
        result.y = Mathf.LerpAngle(current.y, target.y, Time.deltaTime / smoothTime);
        result.z = Mathf.LerpAngle(current.z, target.z, Time.deltaTime / smoothTime);

        return result;
    }

    /// <summary>
    /// 立即面向指定方向
    /// </summary>
    public void LookAtDirection(Vector3 direction)
    {
        if (direction.magnitude > 0.01f)
        {
            targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = targetRotation;
            currentRotation = targetRotation;
        }
    }

    /// <summary>
    /// 立即面向指定位置
    /// </summary>
    public void LookAtPosition(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        LookAtDirection(direction);
    }

    /// <summary>
    /// 获取当前面向的方向
    /// </summary>
    public Vector3 GetForwardDirection()
    {
        return transform.forward;
    }

    /// <summary>
    /// 获取当前速度方向
    /// </summary>
    public Vector3 GetVelocityDirection()
    {
        return targetDirection;
    }

    /// <summary>
    /// 设置转向平滑时间
    /// </summary>
    public void SetRotationSmoothTime(float time)
    {
        rotationSmoothTime = Mathf.Max(0.01f, time);
    }

    /// <summary>
    /// 获取当前旋转
    /// </summary>
    public Quaternion GetCurrentRotation()
    {
        return currentRotation;
    }

    /// <summary>
    /// 强制重置到指定旋转
    /// </summary>
    public void ForceRotation(Quaternion rotation)
    {
        currentRotation = rotation;
        targetRotation = rotation;
        transform.rotation = rotation;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugInfo || Application.isPlaying)
            return;

        // 画转向向量
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
}
