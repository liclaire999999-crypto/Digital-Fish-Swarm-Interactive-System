using UnityEngine;

/// <summary>
/// Boids 算法参数配置
/// 包含所有群体行为的参数设置
/// </summary>
[CreateAssetMenu(fileName = "BoidSettings", menuName = "Fish Swarm/Boid Settings")]
public class BoidSettings : ScriptableObject
{
    [Header("群体规模 | Swarm Scale")]
    [SerializeField]
    public int maxBoidCount = 100;
    
    [SerializeField]
    public float spawnRadius = 50f;

    [Header("基础参数 | Basic Parameters")]
    [SerializeField]
    public float maxSpeed = 5f;
    
    [SerializeField]
    public float maxForce = 0.2f;
    
    [SerializeField]
    public float mass = 1f;

    [Header("聚集行为 | Cohesion")]
    [SerializeField]
    public float cohesionRadius = 15f;
    
    [SerializeField]
    public float cohesionWeight = 1f;
    
    [SerializeField]
    public float cohesionSeparationDistance = 1f;

    [Header("分离行为 | Separation")]
    [SerializeField]
    public float separationRadius = 5f;
    
    [SerializeField]
    public float separationWeight = 1.5f;
    
    [SerializeField]
    public float separationDistance = 2f;

    [Header("对齐行为 | Alignment")]
    [SerializeField]
    public float alignmentRadius = 10f;
    
    [SerializeField]
    public float alignmentWeight = 1f;

    [Header("惊散行为 | Escape/Flee")]
    [SerializeField]
    public float escapeRadius = 20f;
    
    [SerializeField]
    public float escapeWeight = 2f;
    
    [SerializeField]
    public float escapeForceMultiplier = 2f;

    [Header("边界行为 | Boundary Behavior")]
    [SerializeField]
    public float boundsX = 100f;
    
    [SerializeField]
    public float boundsY = 50f;
    
    [SerializeField]
    public float boundsZ = 100f;
    
    [SerializeField]
    public float boundaryPadding = 10f;
    
    [SerializeField]
    public float boundaryForce = 0.15f;

    [Header("视野参数 | Vision Parameters")]
    [SerializeField]
    public float visionAngle = 300f;
    
    [SerializeField]
    public float visionDistance = 20f;

    [Header("行为开关 | Behavior Toggles")]
    [SerializeField]
    public bool enableCohesion = true;
    
    [SerializeField]
    public bool enableSeparation = true;
    
    [SerializeField]
    public bool enableAlignment = true;
    
    [SerializeField]
    public bool enableEscape = true;
    
    [SerializeField]
    public bool enableBoundary = true;

    [Header("性能优化 | Performance")]
    [SerializeField]
    public bool useSpacialPartitioning = true;
    
    [SerializeField]
    public float gridCellSize = 25f;

    /// <summary>
    /// 验证参数合法性
    /// </summary>
    public bool ValidateSettings()
    {
        if (maxBoidCount < 1) maxBoidCount = 1;
        if (maxSpeed < 0.1f) maxSpeed = 0.1f;
        if (maxForce < 0.01f) maxForce = 0.01f;
        if (mass < 0.1f) mass = 0.1f;
        if (cohesionRadius < 1f) cohesionRadius = 1f;
        if (separationRadius < 1f) separationRadius = 1f;
        if (alignmentRadius < 1f) alignmentRadius = 1f;
        if (escapeRadius < 1f) escapeRadius = 1f;
        return true;
    }

    /// <summary>
    /// 获取克隆的设置副本
    /// </summary>
    public BoidSettings Clone()
    {
        BoidSettings clone = ScriptableObject.CreateInstance<BoidSettings>();
        
        clone.maxBoidCount = this.maxBoidCount;
        clone.spawnRadius = this.spawnRadius;
        clone.maxSpeed = this.maxSpeed;
        clone.maxForce = this.maxForce;
        clone.mass = this.mass;
        
        clone.cohesionRadius = this.cohesionRadius;
        clone.cohesionWeight = this.cohesionWeight;
        clone.cohesionSeparationDistance = this.cohesionSeparationDistance;
        
        clone.separationRadius = this.separationRadius;
        clone.separationWeight = this.separationWeight;
        clone.separationDistance = this.separationDistance;
        
        clone.alignmentRadius = this.alignmentRadius;
        clone.alignmentWeight = this.alignmentWeight;
        
        clone.escapeRadius = this.escapeRadius;
        clone.escapeWeight = this.escapeWeight;
        clone.escapeForceMultiplier = this.escapeForceMultiplier;
        
        clone.boundsX = this.boundsX;
        clone.boundsY = this.boundsY;
        clone.boundsZ = this.boundsZ;
        clone.boundaryPadding = this.boundaryPadding;
        clone.boundaryForce = this.boundaryForce;
        
        clone.visionAngle = this.visionAngle;
        clone.visionDistance = this.visionDistance;
        
        clone.enableCohesion = this.enableCohesion;
        clone.enableSeparation = this.enableSeparation;
        clone.enableAlignment = this.enableAlignment;
        clone.enableEscape = this.enableEscape;
        clone.enableBoundary = this.enableBoundary;
        
        clone.useSpacialPartitioning = this.useSpacialPartitioning;
        clone.gridCellSize = this.gridCellSize;
        
        return clone;
    }
}
