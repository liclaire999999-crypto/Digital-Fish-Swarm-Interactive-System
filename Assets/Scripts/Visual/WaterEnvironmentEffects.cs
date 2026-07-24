using UnityEngine;

/// <summary>
/// 水下环境效果管理器
/// 管理全局视觉效果和环境设置
/// </summary>
public class WaterEnvironmentEffects : MonoBehaviour
{
    [Header("水下光效 | Underwater Lighting")]
    [SerializeField]
    private Light mainLight;  // 主光源
    
    [SerializeField]
    private float baseIntensity = 1f;  // 基础光照强度
    
    [SerializeField]
    private Color waterTint = new Color(0.2f, 0.6f, 0.8f, 1f);  // 水色调
    
    [SerializeField]
    private float waterDensity = 0.3f;  // 水的密度（雾）

    [Header("后处理效果 | Post Processing")]
    [SerializeField]
    private bool enableFog = true;  // 启用雾
    
    [SerializeField]
    private bool enableBloom = true;  // 启用辉光
    
    [SerializeField]
    private float bloomIntensity = 1f;  // 辉光强度

    [Header("粒子系统 | Particle Systems")]
    [SerializeField]
    private ParticleSystem bubbleParticles;  // 气泡粒子
    
    [SerializeField]
    private ParticleSystem lightRaysParticles;  // 光线粒子
    
    [SerializeField]
    private float bubbleEmissionRate = 10f;  // 气泡发射率

    [Header("动画参数 | Animation Parameters")]
    [SerializeField]
    private float lightWaveSpeed = 1f;  // 光波动速度
    
    [SerializeField]
    private float lightWaveAmount = 0.2f;  // 光波动幅度
    
    [SerializeField]
    private AnimationCurve lightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);  // 光照曲线

    [Header("摄像机效果 | Camera Effects")]
    [SerializeField]
    private Camera mainCamera;  // 主摄像机
    
    [SerializeField]
    private float cameraDepthFade = 0.5f;  // 摄像机深度衰减
    
    [SerializeField]
    private bool enableCameraShake = false;  // 启用摄像机震动
    
    [SerializeField]
    private float shakeIntensity = 0.1f;  // 摄像机震动强度

    private float wavePhase = 0f;  // 波动相位
    private float baseExposure;  // 基础曝光
    private Vector3 originalCameraPos;  // 原始摄像机位置

    private void Start()
    {
        // 初始化环境设置
        InitializeEnvironment();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera != null)
        {
            originalCameraPos = mainCamera.transform.position;
        }
    }

    private void Update()
    {
        // 更新光照波动
        UpdateLightWave();
        
        // 更新粒子效果
        UpdateParticleEffects();
        
        // 更新摄像机效果
        if (enableCameraShake)
        {
            UpdateCameraShake();
        }
    }

    /// <summary>
    /// 初始化环境设置
    /// </summary>
    private void InitializeEnvironment()
    {
        // 设置全局雾
        if (enableFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = waterDensity;
            RenderSettings.fogColor = waterTint;
        }

        // 设置主光源
        if (mainLight == null)
        {
            mainLight = FindObjectOfType<Light>();
        }
        
        if (mainLight != null)
        {
            baseIntensity = mainLight.intensity;
        }
    }

    /// <summary>
    /// 更新光照波动效果
    /// </summary>
    private void UpdateLightWave()
    {
        if (mainLight == null)
            return;

        wavePhase += Time.deltaTime * lightWaveSpeed;

        // 计算波动
        float waveValue = Mathf.Sin(wavePhase) * lightWaveAmount;
        float newIntensity = baseIntensity + waveValue;

        // 应用光照曲线
        float curveValue = lightCurve.Evaluate((wavePhase % (2 * Mathf.PI)) / (2 * Mathf.PI));
        newIntensity = Mathf.Lerp(baseIntensity * 0.8f, baseIntensity * 1.2f, curveValue);

        mainLight.intensity = newIntensity;
    }

    /// <summary>
    /// 更新粒子效果
    /// </summary>
    private void UpdateParticleEffects()
    {
        // 更新气泡粒子
        if (bubbleParticles != null)
        {
            ParticleSystem.EmissionModule emission = bubbleParticles.emission;
            emission.rateOverTime = bubbleEmissionRate;
        }

        // 更新光线粒子
        if (lightRaysParticles != null)
        {
            // 根据时间调整光线粒子的强度
            ParticleSystem.MainModule mainModule = lightRaysParticles.main;
            mainModule.simulationSpeed = 0.5f + (Mathf.Sin(wavePhase) * 0.5f);
        }
    }

    /// <summary>
    /// 更新摄像机震动
    /// </summary>
    private void UpdateCameraShake()
    {
        if (mainCamera == null)
            return;

        Vector3 shake = new Vector3(
            Mathf.PerlinNoise(Time.time * 10f, 0) - 0.5f,
            Mathf.PerlinNoise(Time.time * 10f, 1) - 0.5f,
            Mathf.PerlinNoise(Time.time * 10f, 2) - 0.5f
        ) * shakeIntensity;

        mainCamera.transform.position = originalCameraPos + shake;
    }

    /// <summary>
    /// 设置水色调
    /// </summary>
    public void SetWaterTint(Color tint)
    {
        waterTint = tint;
        RenderSettings.fogColor = waterTint;
    }

    /// <summary>
    /// 设置水密度（雾)
    /// </summary>
    public void SetWaterDensity(float density)
    {
        waterDensity = Mathf.Clamp01(density);
        RenderSettings.fogDensity = waterDensity;
    }

    /// <summary>
    /// 设置光照强度
    /// </summary>
    public void SetLightIntensity(float intensity)
    {
        baseIntensity = Mathf.Max(0, intensity);
        if (mainLight != null)
        {
            mainLight.intensity = baseIntensity;
        }
    }

    /// <summary>
    /// 触发全屏闪光效果
    /// </summary>
    public void TriggerFlash(float duration = 0.5f)
    {
        StartCoroutine(FlashCoroutine(duration));
    }

    private System.Collections.IEnumerator FlashCoroutine(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            // 逐渐恢复光照
            float t = elapsed / duration;
            if (mainLight != null)
            {
                mainLight.intensity = Mathf.Lerp(baseIntensity * 1.5f, baseIntensity, t);
            }
            
            yield return null;
        }
        
        if (mainLight != null)
        {
            mainLight.intensity = baseIntensity;
        }
    }

    /// <summary>
    /// 启用/禁用摄像机震动
    /// </summary>
    public void SetCameraShakeEnabled(bool enabled)
    {
        enableCameraShake = enabled;
    }

    /// <summary>
    /// 设置摄像机震动强度
    /// </summary>
    public void SetShakeIntensity(float intensity)
    {
        shakeIntensity = Mathf.Max(0, intensity);
    }
}
