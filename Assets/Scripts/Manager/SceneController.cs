using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 场景控制器
/// 负责场景管理和环境设置
/// </summary>
public class SceneController : MonoBehaviour
{
    [Header("场景设置 | Scene Configuration")]
    [SerializeField]
    private string currentSceneName = "MainScene";  // 当前场景名
    
    [SerializeField]
    private bool autoInitializeScene = true;  // 自动初始化场景

    [Header("环境设置 | Environment Settings")]
    [SerializeField]
    private Color ambientLightColor = new Color(0.5f, 0.7f, 1f);  // 环境光颜色
    
    [SerializeField]
    private float ambientLightIntensity = 1f;  // 环境光强度
    
    [SerializeField]
    private Color backgroundColor = new Color(0.1f, 0.3f, 0.5f);  // 背景颜色
    
    [SerializeField]
    private bool enableFog = true;  // 启用雾
    
    [SerializeField]
    private float fogDensity = 0.1f;  // 雾密度

    [Header("摄像机设置 | Camera Configuration")]
    [SerializeField]
    private Camera mainCamera;  // 主摄像机
    
    [SerializeField]
    private Vector3 cameraDefaultPos = new Vector3(0, 30, -50);  // 摄像机默认位置
    
    [SerializeField]
    private float cameraFieldOfView = 60f;  // 视野角度
    
    [SerializeField]
    private float cameraNearClip = 0.3f;  // 近裁剪面
    
    [SerializeField]
    private float cameraFarClip = 1000f;  // 远裁剪面

    [Header("物理设置 | Physics Configuration")]
    [SerializeField]
    private Vector3 gravityDirection = Vector3.down;  // 重力方向
    
    [SerializeField]
    private float gravityScale = 1f;  // 重力缩放

    [Header("边界设置 | Boundary Configuration")]
    [SerializeField]
    private Vector3 sceneCenter = Vector3.zero;  // 场景中心
    
    [SerializeField]
    private Vector3 sceneBounds = new Vector3(100, 50, 100);  // 场景边界
    
    [SerializeField]
    private bool visualizeSceneBounds = true;  // 可视化边界

    [Header("场景对象 | Scene Objects")]
    [SerializeField]
    private List<GameObject> sceneObjects = new List<GameObject>();  // 场景对象
    
    private Dictionary<string, GameObject> sceneObjectMap = new Dictionary<string, GameObject>();

    private void Start()
    {
        if (autoInitializeScene)
        {
            InitializeScene();
        }
    }

    private void OnDrawGizmos()
    {
        if (!visualizeSceneBounds)
            return;

        // 绘制场景边界
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(sceneCenter, sceneBounds);
    }

    /// <summary>
    /// 初始化场景
    /// </summary>
    public void InitializeScene()
    {
        Debug.Log($"[SceneController] Initializing scene: {currentSceneName}");

        // 设置环境光
        SetupAmbientLight();

        // 设置背景
        SetupBackground();

        // 设置雾
        SetupFog();

        // 设置摄像机
        SetupCamera();

        // 设置物理
        SetupPhysics();

        // 初始化场景对象
        InitializeSceneObjects();

        Debug.Log("[SceneController] Scene initialization complete.");
    }

    /// <summary>
    /// 设置环境光
    /// </summary>
    private void SetupAmbientLight()
    {
        RenderSettings.ambientLight = ambientLightColor * ambientLightIntensity;
        RenderSettings.ambientMode = AmbientMode.Flat;
    }

    /// <summary>
    /// 设置背景
    /// </summary>
    private void SetupBackground()
    {
        Camera.main.backgroundColor = backgroundColor;
    }

    /// <summary>
    /// 设置雾
    /// </summary>
    private void SetupFog()
    {
        RenderSettings.fog = enableFog;
        if (enableFog)
        {
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogColor = backgroundColor;
        }
    }

    /// <summary>
    /// 设置摄像机
    /// </summary>
    private void SetupCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            mainCamera.transform.position = cameraDefaultPos;
            mainCamera.fieldOfView = cameraFieldOfView;
            mainCamera.nearClipPlane = cameraNearClip;
            mainCamera.farClipPlane = cameraFarClip;
        }
    }

    /// <summary>
    /// 设置物理
    /// </summary>
    private void SetupPhysics()
    {
        Physics.gravity = gravityDirection.normalized * Physics.gravity.magnitude * gravityScale;
    }

    /// <summary>
    /// 初始化场景对象
    /// </summary>
    private void InitializeSceneObjects()
    {
        sceneObjectMap.Clear();

        foreach (GameObject obj in sceneObjects)
        {
            if (obj != null)
            {
                sceneObjectMap[obj.name] = obj;
                obj.SetActive(true);
            }
        }
    }

    // ===== 公共接口 =====

    /// <summary>
    /// 获取场景中心
    /// </summary>
    public Vector3 GetSceneCenter()
    {
        return sceneCenter;
    }

    /// <summary>
    /// 获取场景边界
    /// </summary>
    public Vector3 GetSceneBounds()
    {
        return sceneBounds;
    }

    /// <summary>
    /// 检查点是否在场景内
    /// </summary>
    public bool IsPointInScene(Vector3 point)
    {
        Vector3 relativePos = point - sceneCenter;
        return Mathf.Abs(relativePos.x) <= sceneBounds.x / 2 &&
               Mathf.Abs(relativePos.y) <= sceneBounds.y / 2 &&
               Mathf.Abs(relativePos.z) <= sceneBounds.z / 2;
    }

    /// <summary>
    /// 获取场景对象
    /// </summary>
    public GameObject GetSceneObject(string name)
    {
        if (sceneObjectMap.TryGetValue(name, out GameObject obj))
        {
            return obj;
        }
        return null;
    }

    /// <summary>
    /// 注册场景对象
    /// </summary>
    public void RegisterSceneObject(string name, GameObject obj)
    {
        if (!sceneObjectMap.ContainsKey(name))
        {
            sceneObjectMap[name] = obj;
        }
    }

    /// <summary>
    /// 设置摄像机位置
    /// </summary>
    public void SetCameraPosition(Vector3 position)
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = position;
        }
    }

    /// <summary>
    /// 设置摄像机观看目标
    /// </summary>
    public void SetCameraLookAt(Vector3 target)
    {
        if (mainCamera != null)
        {
            mainCamera.transform.LookAt(target);
        }
    }

    /// <summary>
    /// 重置场景
    /// </summary>
    public void ResetScene()
    {
        Debug.Log("[SceneController] Resetting scene...");
        InitializeScene();
    }

    /// <summary>
    /// 获取当前场景名
    /// </summary>
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }

    /// <summary>
    /// 设置雾效参数
    /// </summary>
    public void SetFogParameters(bool enabled, float density)
    {
        enableFog = enabled;
        fogDensity = density;
        SetupFog();
    }

    /// <summary>
    /// 设置环境光参数
    /// </summary>
    public void SetAmbientLight(Color color, float intensity)
    {
        ambientLightColor = color;
        ambientLightIntensity = intensity;
        SetupAmbientLight();
    }

    /// <summary>
    /// 获取主摄像机
    /// </summary>
    public Camera GetMainCamera()
    {
        return mainCamera;
    }
}
