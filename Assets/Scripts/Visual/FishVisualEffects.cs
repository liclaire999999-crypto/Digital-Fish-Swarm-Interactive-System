using UnityEngine;

/// <summary>
/// 鱼的视觉效果控制器
/// 管理发光、粒子、轨迹等效果
/// </summary>
public class FishVisualEffects : MonoBehaviour
{
    [Header("发光效果 | Glow Effects")]
    [SerializeField]
    private Material glowMaterial;  // 发光材质
    
    [SerializeField]
    private float glowIntensity = 1f;  // 发光强度
    
    [SerializeField]
    private Color glowColor = Color.cyan;  // 发光颜色
    
    [SerializeField]
    private float glowPulseSpeed = 2f;  // 发光脉冲速度
    
    [SerializeField]
    private float glowPulseAmount = 0.5f;  // 发光脉冲幅度

    [Header("拖尾效果 | Trail Effects")]
    [SerializeField]
    private TrailRenderer trailRenderer;  // 拖尾渲染器
    
    [SerializeField]
    private bool enableTrail = true;  // 启用拖尾
    
    [SerializeField]
    private float trailTime = 0.5f;  // 拖尾持续时间
    
    [SerializeField]
    private int trailResolution = 10;  // 拖尾分辨率
    
    [SerializeField]
    private float trailWidth = 0.1f;  // 拖尾宽度
    
    [SerializeField]
    private Color trailColor = new Color(0, 1, 1, 0.5f);  // 拖尾颜色

    [Header("粒子效果 | Particle Effects")]
    [SerializeField]
    private ParticleSystem swimParticles;  // 游动粒子
    
    [SerializeField]
    private ParticleSystem escapeParticles;  // 逃散粒子
    
    [SerializeField]
    private bool enableParticles = true;  // 启用粒子

    [Header("状态响应 | State Response")]
    [SerializeField]
    private float escapeGlowMultiplier = 2f;  // 逃散时发光倍数
    
    [SerializeField]
    private float escapeTrailMultiplier = 1.5f;  // 逃散时拖尾倍数

    private FishAgent fishAgent;
    private Renderer meshRenderer;
    private Vector3 lastPosition;  // 上一帧位置
    private float glowPhase = 0f;  // 发光相位
    private float baseGlowIntensity;  // 基础发光强度

    private void Start()
    {
        // 获取组件
        fishAgent = GetComponent<FishAgent>();
        meshRenderer = GetComponent<Renderer>();

        // 初始化发光材质
        if (glowMaterial == null && meshRenderer != null)
        {
            glowMaterial = meshRenderer.material;
        }
        baseGlowIntensity = glowIntensity;

        // 初始化拖尾
        if (trailRenderer == null)
        {
            trailRenderer = gameObject.AddComponent<TrailRenderer>();
        }
        
        if (enableTrail)
        {
            ConfigureTrailRenderer();
        }
        else
        {
            if (trailRenderer != null)
                trailRenderer.enabled = false;
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        if (glowMaterial == null)
            return;

        // 更新发光效果
        UpdateGlowEffect();

        // 更新拖尾
        UpdateTrailEffect();

        // 更新粒子
        UpdateParticleEffects();

        lastPosition = transform.position;
    }

    /// <summary>
    /// 更新发光效果
    /// </summary>
    private void UpdateGlowEffect()
    {
        glowPhase += Time.deltaTime * glowPulseSpeed;

        // 计算脉冲发光
        float pulseAmount = Mathf.Sin(glowPhase * Mathf.PI) * glowPulseAmount;
        float currentGlowIntensity = baseGlowIntensity + pulseAmount;

        // 如果处于逃散状态，增加发光强度
        if (fishAgent != null && fishAgent.IsEscaping())
        {
            currentGlowIntensity *= escapeGlowMultiplier;
        }

        // 应用发光参数
        glowMaterial.SetFloat("_GlowIntensity", currentGlowIntensity);
        glowMaterial.SetColor("_EmissionColor", glowColor * currentGlowIntensity);
    }

    /// <summary>
    /// 更新拖尾效果
    /// </summary>
    private void UpdateTrailEffect()
    {
        if (trailRenderer == null || !enableTrail)
            return;

        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        
        // 根据速度调整拖尾
        if (fishAgent != null && fishAgent.IsEscaping())
        {
            trailRenderer.time = trailTime * escapeTrailMultiplier;
        }
        else
        {
            trailRenderer.time = trailTime;
        }

        // 根据速度调整宽度
        float widthMultiplier = 1f + (speed * 0.5f);
        trailRenderer.widthCurve = AnimationCurve.EaseInOut(0, trailWidth * widthMultiplier, 1, trailWidth * 0.5f);
    }

    /// <summary>
    /// 更新粒子效果
    /// </summary>
    private void UpdateParticleEffects()
    {
        if (!enableParticles || fishAgent == null)
            return;

        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        bool isEscaping = fishAgent.IsEscaping();

        // 游动粒子
        if (swimParticles != null)
        {
            ParticleSystem.EmissionModule emission = swimParticles.emission;
            
            if (speed > 0.1f)
            {
                emission.enabled = true;
                // 速度越快，粒子发射率越高
                emission.rateOverTime = 5f + (speed * 10f);
            }
            else
            {
                emission.enabled = false;
            }
        }

        // 逃散粒子
        if (escapeParticles != null)
        {
            ParticleSystem.EmissionModule emission = escapeParticles.emission;
            
            if (isEscaping && speed > 0.1f)
            {
                emission.enabled = true;
                emission.rateOverTime = 15f + (speed * 20f);
            }
            else
            {
                emission.enabled = false;
            }
        }
    }

    /// <summary>
    /// 配置拖尾渲染器
    /// </summary>
    private void ConfigureTrailRenderer()
    {
        if (trailRenderer == null)
            return;

        trailRenderer.time = trailTime;
        trailRenderer.startWidth = trailWidth;
        trailRenderer.endWidth = trailWidth * 0.3f;
        trailRenderer.startColor = trailColor;
        trailRenderer.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0);
        trailRenderer.widthMultiplier = 1f;
        trailRenderer.numCornerVertices = 2;
        trailRenderer.numCapVertices = 2;
        trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trailRenderer.renderingMode = TrailRenderer.TrailRenderingMode.Billboard;
    }

    /// <summary>
    /// 设置发光颜色
    /// </summary>
    public void SetGlowColor(Color color)
    {
        glowColor = color;
        if (glowMaterial != null)
        {
            glowMaterial.SetColor("_EmissionColor", glowColor);
        }
    }

    /// <summary>
    /// 设置发光强度
    /// </summary>
    public void SetGlowIntensity(float intensity)
    {
        baseGlowIntensity = Mathf.Max(0, intensity);
    }

    /// <summary>
    /// 设置拖尾参数
    /// </summary>
    public void SetTrailParameters(float time, float width, Color color)
    {
        trailTime = time;
        trailWidth = width;
        trailColor = color;
        ConfigureTrailRenderer();
    }

    /// <summary>
    /// 启用/禁用拖尾
    /// </summary>
    public void SetTrailEnabled(bool enabled)
    {
        enableTrail = enabled;
        if (trailRenderer != null)
        {
            trailRenderer.enabled = enabled;
        }
    }

    /// <summary>
    /// 触发爆发粒子效果
    /// </summary>
    public void TriggerBurstParticles(int burstCount = 20)
    {
        if (escapeParticles != null)
        {
            escapeParticles.Emit(burstCount);
        }
    }

    /// <summary>
    /// 获取当前发光强度
    /// </summary>
    public float GetCurrentGlowIntensity()
    {
        return baseGlowIntensity;
    }
}
