using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 单个鱼的 Boids 代理
/// 实现四大群体行为：聚集、分离、对齐、惊散
/// </summary>
public class BoidAgent : MonoBehaviour
{
    [Header("Boids 配置 | Boids Configuration")]
    private BoidSettings settings;
    
    [Header("物理属性 | Physics Properties")]
    private Vector3 velocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    private Vector3 position;
    
    [Header("状态 | State")]
    private Vector3 escapeTarget = Vector3.zero;  // 逃散目标
    private bool isEscaping = false;  // 是否处于逃散状态
    private float escapeTimeRemaining = 0f;  // 逃散剩余时间
    
    [SerializeField]
    private float escapeDecayTime = 2f;  // 逃散衰减时间（秒）
    
    [Header("邻近搜索 | Neighbor Search")]
    private List<BoidAgent> neighbors = new List<BoidAgent>();
    private BoidManager manager;  // 引用管理器

    private void OnEnable()
    {
        if (manager != null)
            manager.RegisterBoid(this);
    }

    private void OnDisable()
    {
        if (manager != null)
            manager.UnregisterBoid(this);
    }

    /// <summary>
    /// 初始化鱼代理
    /// </summary>
    public void Initialize(BoidSettings settings, BoidManager manager, Vector3 initialPosition)
    {
        this.settings = settings;
        this.manager = manager;
        this.position = initialPosition;
        this.transform.position = position;
        
        // 随机初始速度
        velocity = Random.onUnitSphere * settings.maxSpeed * 0.5f;
        acceleration = Vector3.zero;
        
        isEscaping = false;
        escapeTimeRemaining = 0f;
    }

    /// <summary>
    /// 更新鱼的位置和行为
    /// </summary>
    public void UpdateBoid(List<BoidAgent> allBoids)
    {
        if (settings == null || manager == null)
            return;

        // 清空加速度
        acceleration = Vector3.zero;

        // 查询邻居
        neighbors.Clear();
        FindNeighbors(allBoids);

        // 应用各种行为力
        Vector3 separationForce = Vector3.zero;
        Vector3 cohesionForce = Vector3.zero;
        Vector3 alignmentForce = Vector3.zero;
        Vector3 escapeForce = Vector3.zero;

        // 分离行为 | Separation
        if (settings.enableSeparation && neighbors.Count > 0)
        {
            separationForce = CalculateSeparation();
            acceleration += separationForce * settings.separationWeight;
        }

        // 聚集行为 | Cohesion
        if (settings.enableCohesion && neighbors.Count > 0)
        {
            cohesionForce = CalculateCohesion();
            acceleration += cohesionForce * settings.cohesionWeight;
        }

        // 对齐行为 | Alignment
        if (settings.enableAlignment && neighbors.Count > 0)
        {
            alignmentForce = CalculateAlignment();
            acceleration += alignmentForce * settings.alignmentWeight;
        }

        // 惊散行为 | Escape/Flee
        if (settings.enableEscape)
        {
            if (isEscaping)
            {
                escapeForce = CalculateEscape();
                acceleration += escapeForce * settings.escapeWeight;
                
                // 更新逃散时间
                escapeTimeRemaining -= Time.deltaTime;
                if (escapeTimeRemaining <= 0)
                {
                    isEscaping = false;
                }
            }
        }

        // 边界行为 | Boundary
        if (settings.enableBoundary)
        {
            Vector3 boundaryForce = CalculateBoundaryForce();
            acceleration += boundaryForce;
        }

        // 限制加速度大小
        if (acceleration.magnitude > 0)
        {
            acceleration = Vector3.ClampMagnitude(acceleration, settings.maxForce);
        }

        // 更新速度和位置
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, settings.maxSpeed);
        
        position += velocity * Time.deltaTime;
        transform.position = position;

        // 使鱼面向运动方向
        if (velocity.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(velocity);
        }
    }

    /// <summary>
    /// 查找邻居鱼（视野范围内的其他鱼）
    /// </summary>
    private void FindNeighbors(List<BoidAgent> allBoids)
    {
        neighbors.Clear();
        
        float maxRadius = Mathf.Max(
            settings.separationRadius,
            settings.cohesionRadius,
            settings.alignmentRadius
        );

        foreach (BoidAgent other in allBoids)
        {
            if (other == this)
                continue;

            float distance = Vector3.Distance(position, other.position);
            
            if (distance > maxRadius)
                continue;

            // 检查视角
            Vector3 directionToOther = (other.position - position).normalized;
            float angle = Vector3.Angle(velocity.normalized, directionToOther);
            
            if (angle > settings.visionAngle / 2f)
                continue;

            neighbors.Add(other);
        }
    }

    /// <summary>
    /// 计算分离力（避免拥挤）
    /// </summary>
    private Vector3 CalculateSeparation()
    {
        Vector3 steer = Vector3.zero;
        int count = 0;

        foreach (BoidAgent other in neighbors)
        {
            float distance = Vector3.Distance(position, other.position);
            
            if (distance < settings.separationRadius && distance > 0)
            {
                // 计算反向向量
                Vector3 diff = (position - other.position).normalized;
                diff /= distance;  // 距离越近，影响越大
                steer += diff;
                count++;
            }
        }

        if (count > 0)
        {
            steer /= count;
            steer = steer.normalized * settings.maxSpeed;
            steer -= velocity;
            steer = Vector3.ClampMagnitude(steer, settings.maxForce);
        }

        return steer;
    }

    /// <summary>
    /// 计算聚集力（靠近群体中心）
    /// </summary>
    private Vector3 CalculateCohesion()
    {
        Vector3 steering = Vector3.zero;
        int count = 0;

        foreach (BoidAgent other in neighbors)
        {
            float distance = Vector3.Distance(position, other.position);
            
            if (distance < settings.cohesionRadius && distance > settings.cohesionSeparationDistance)
            {
                steering += other.position;
                count++;
            }
        }

        if (count > 0)
        {
            steering /= count;  // 计算平均位置
            steering -= position;  // 方向向量
            steering = steering.normalized * settings.maxSpeed;
            steering -= velocity;
            steering = Vector3.ClampMagnitude(steering, settings.maxForce);
        }

        return steering;
    }

    /// <summary>
    /// 计算对齐力（匹配群体方向）
    /// </summary>
    private Vector3 CalculateAlignment()
    {
        Vector3 steering = Vector3.zero;
        int count = 0;

        foreach (BoidAgent other in neighbors)
        {
            float distance = Vector3.Distance(position, other.position);
            
            if (distance < settings.alignmentRadius)
            {
                steering += other.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            steering /= count;  // 计算平均速度
            steering = steering.normalized * settings.maxSpeed;
            steering -= velocity;
            steering = Vector3.ClampMagnitude(steering, settings.maxForce);
        }

        return steering;
    }

    /// <summary>
    /// 计算边界反弹力
    /// </summary>
    private Vector3 CalculateBoundaryForce()
    {
        Vector3 force = Vector3.zero;
        float padding = settings.boundaryPadding;

        // X 轴边界
        if (position.x < -settings.boundsX + padding)
        {
            force.x = settings.boundaryForce;
        }
        else if (position.x > settings.boundsX - padding)
        {
            force.x = -settings.boundaryForce;
        }

        // Y 轴边界
        if (position.y < -settings.boundsY + padding)
        {
            force.y = settings.boundaryForce;
        }
        else if (position.y > settings.boundsY - padding)
        {
            force.y = -settings.boundaryForce;
        }

        // Z 轴边界
        if (position.z < -settings.boundsZ + padding)
        {
            force.z = settings.boundaryForce;
        }
        else if (position.z > settings.boundsZ - padding)
        {
            force.z = -settings.boundaryForce;
        }

        return force;
    }

    /// <summary>
    /// 计算逃散力（远离目标）
    /// </summary>
    private Vector3 CalculateEscape()
    {
        Vector3 directionAwayFromTarget = (position - escapeTarget).normalized;
        Vector3 desiredVelocity = directionAwayFromTarget * settings.maxSpeed * settings.escapeForceMultiplier;
        Vector3 steer = desiredVelocity - velocity;
        return Vector3.ClampMagnitude(steer, settings.maxForce * settings.escapeForceMultiplier);
    }

    /// <summary>
    /// 触发逃散行为（由手势交互调用）
    /// </summary>
    public void TriggerEscape(Vector3 threatPosition)
    {
        float distanceToThreat = Vector3.Distance(position, threatPosition);
        
        if (distanceToThreat < settings.escapeRadius)
        {
            isEscaping = true;
            escapeTarget = threatPosition;
            escapeTimeRemaining = escapeDecayTime;
        }
    }

    /// <summary>
    /// 停止逃散行为
    /// </summary>
    public void StopEscape()
    {
        isEscaping = false;
        escapeTimeRemaining = 0f;
    }

    /// <summary>
    /// 获取当前位置
    /// </summary>
    public Vector3 GetPosition() => position;

    /// <summary>
    /// 获取当前速度
    /// </summary>
    public Vector3 GetVelocity() => velocity;

    /// <summary>
    /// 获取邻居数量
    /// </summary>
    public int GetNeighborCount() => neighbors.Count;

    /// <summary>
    /// 获取是否处于逃散状态
    /// </summary>
    public bool IsEscaping() => isEscaping;

    /// <summary>
    /// 设置位置（用于生成或重置）
    /// </summary>
    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
        transform.position = position;
    }
}
