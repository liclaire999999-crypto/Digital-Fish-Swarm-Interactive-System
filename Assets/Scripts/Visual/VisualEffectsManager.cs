using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 视觉效果管理器
/// 协调所有视觉效果和外观设置
/// </summary>
public class VisualEffectsManager : MonoBehaviour
{
    [Header("效果配置 | Effects Configuration")]
    [SerializeField]
    private bool enableAllEffects = true;  // 启用所有效果
    
    [SerializeField]
    private float effectIntensity = 1f;  // 整体效果强度（0-1）
    
    [SerializeField]
    private bool enableGlow = true;  // 启用发光
    
    [SerializeField]
    private bool enableTrails = true;  // 启用拖尾
    
    [SerializeField]
    private bool enableParticles = true;  // 启用粒子

    [Header("颜色方案 | Color Scheme")]
    [SerializeField]
    private Color normalGlowColor = Color.cyan;  // 正常发光颜色
    
    [SerializeField]
    private Color escapeGlowColor = Color.red;  // 逃散发光颜色
    
    [SerializeField]
    private Color trailColor = new Color(0, 1, 1, 0.5f);  // 拖尾颜色
    
    [SerializeField]
    private Color escapeTrailColor = new Color(1, 0.2f, 0.2f, 0.7f);  // 逃散拖尾颜色

    [Header("环境效果 | Environment Effects")]
    [SerializeField]
    private WaterEnvironmentEffects waterEffects;  // 水下环境效果
    
    [SerializeField]
    private bool enableEnvironmentEffects = true;  // 启用环境效果

    [Header("性能优化 | Performance")]
    [SerializeField]
    private int maxActiveEffects = 100;  // 最大活跃效果数
    
    [SerializeField]
    private float lodDistance = 50f;  // LOD 距离
    
    [SerializeField]
    private bool useLOD = true;  // 使用 LOD 系统

    private List<FishVisualEffects> activeFishEffects = new List<FishVisualEffects>();
    private Camera mainCamera;
    private float globalIntensity = 1f;  // 全局强度缓存

    private void Start()
    {
        mainCamera = Camera.main;
        
        if (waterEffects == null)
        {
            waterEffects = FindObjectOfType<WaterEnvironmentEffects>();
        }
    }

    private void Update()
    {
        if (!enableAllEffects)
            return;

        // 更新 LOD 系统
        if (useLOD)
        {
            UpdateLOD();
        }

        // 更新全局效果参数
        UpdateGlobalEffects();
    }

    /// <summary>
    /// 注册鱼的视觉效果
    /// </summary>
    public void RegisterFishEffects(FishVisualEffects fishEffects)
    {
        if (!activeFishEffects.Contains(fishEffects))
        {
            activeFishEffects.Add(fishEffects);
        }
    }

    /// <summary>
    /// 注销鱼的视觉效果
    /// </summary>
    public void UnregisterFishEffects(FishVisualEffects fishEffects)
    {
        activeFishEffects.Remove(fishEffects);
    }

    /// <summary>
    /// 更新 LOD 系统
    /// </summary>
    private void UpdateLOD()
    {
        if (mainCamera == null)
            return;

        Vector3 cameraPos = mainCamera.transform.position;

        foreach (FishVisualEffects fishEffect in activeFishEffects)
        {
            if (fishEffect == null)
                continue;

            float distance = Vector3.Distance(fishEffect.transform.position, cameraPos);

            // 根据距离调整效果
            if (distance > lodDistance)
            {
                // 禁用远处的拖尾
                fishEffect.SetTrailEnabled(false);
            }
            else
            {
                fishEffect.SetTrailEnabled(enableTrails);
            }
        }
    }

    /// <summary>
    /// 更新全局效果参数
    /// </summary>
    private void UpdateGlobalEffects()
    {
        // 同步全局强度
        globalIntensity = Mathf.Clamp01(effectIntensity);
    }

    /// <summary>
    /// 设置所有鱼的发光颜色
    /// </summary>
    public void SetAllFishGlowColor(Color color)
    {
        normalGlowColor = color;
        
        foreach (FishVisualEffects fishEffect in activeFishEffects)
        {
            if (fishEffect != null)
            {
                fishEffect.SetGlowColor(color);
            }
        }
    }

    /// <summary>
    /// 设置所有鱼的发光强度
    /// </summary>
    public void SetAllFishGlowIntensity(float intensity)
    {
        foreach (FishVisualEffects fishEffect in activeFishEffects)
        {
            if (fishEffect != null)
            {
                fishEffect.SetGlowIntensity(intensity * globalIntensity);
            }
        }
    }

    /// <summary>
    /// 设置拖尾参数（所有鱼）
    /// </summary>
    public void SetAllFishTrailParameters(float time, float width, Color color)
    {
        foreach (FishVisualEffects fishEffect in activeFishEffects)
        {
            if (fishEffect != null)
            {
                fishEffect.SetTrailParameters(time, width, color);
            }
        }
    }

    /// <summary>
    /// 触发全局爆发效果
    /// </summary>
    public void TriggerGlobalBurst(int particlesPerFish = 20)
    {
        foreach (FishVisualEffects fishEffect in activeFishEffects)
        {
            if (fishEffect != null)
            {
                fishEffect.TriggerBurstParticles(particlesPerFish);
            }
        }
    }

    /// <summary>
    /// 设置效果强度
    /// </summary>
    public void SetEffectIntensity(float intensity)
    {
        effectIntensity = Mathf.Clamp01(intensity);
        globalIntensity = effectIntensity;
    }

    /// <summary>
    /// 启用/禁用所有效果
    /// </summary>
    public void SetAllEffectsEnabled(bool enabled)
    {
        enableAllEffects = enabled;
    }

    /// <summary>
    /// 启用/禁用发光
    /// </summary>
    public void SetGlowEnabled(bool enabled)
    {
        enableGlow = enabled;
    }

    /// <summary>
    /// 启用/禁用拖尾
    /// </summary>
    public void SetTrailsEnabled(bool enabled)
    {
        enableTrails = enabled;
        
        foreach (FishVisualEffects fishEffect in activeFishEffects)
        {
            if (fishEffect != null)
            {
                fishEffect.SetTrailEnabled(enabled);
            }
        }
    }

    /// <summary>
    /// 启用/禁用粒子
    /// </summary>
    public void SetParticlesEnabled(bool enabled)
    {
        enableParticles = enabled;
    }

    /// <summary>
    /// 获取活跃效果数量
    /// </summary>
    public int GetActiveEffectsCount()
    {
        return activeFishEffects.Count;
    }

    /// <summary>
    /// 获取全局强度
    /// </summary>
    public float GetGlobalIntensity()
    {
        return globalIntensity;
    }
}
