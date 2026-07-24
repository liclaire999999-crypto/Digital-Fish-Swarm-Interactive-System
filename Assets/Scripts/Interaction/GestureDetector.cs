using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 手势识别器
/// 负责检测手部位置和基本手势
/// 支持未来集成 MediaPipe
/// </summary>
public class GestureDetector : MonoBehaviour
{
    [Header("检测配置 | Detection Settings")]
    [SerializeField]
    private float detectionUpdateRate = 0.1f;  // 检测更新频率（秒）
    
    [SerializeField]
    private float handSensitivity = 1f;  // 手部检测灵敏度
    
    [SerializeField]
    private bool enableDebugDraw = true;  // 绘制调试信息

    [Header("手势阈值 | Gesture Thresholds")]
    [SerializeField]
    private float pinchThreshold = 0.05f;  // 捏合阈值
    
    [SerializeField]
    private float palmOpenThreshold = 0.8f;  // 手掌张开阈值
    
    [SerializeField]
    private float gestureConfidenceThreshold = 0.7f;  // 手势置信度阈值

    [Header("模拟输入 | Simulated Input")]
    [SerializeField]
    private bool useMouseSimulation = true;  // 使用鼠标模拟（调试用）
    
    [SerializeField]
    private bool useKeyboardSimulation = true;  // 使用键盘模拟

    // 手部状态
    private class HandState
    {
        public Vector3 position;  // 手部位置（世界坐标）
        public Vector3 screenPosition;  // 屏幕坐标
        public float confidence;  // 检测置信度（0-1）
        public bool isDetected;  // 是否被检测到
        public bool isOpen;  // 手是否张开
        public bool isPinching;  // 是否在捏合
        public Vector3 palmNormal;  // 手掌法线方向
    }

    private HandState leftHand = new HandState();
    private HandState rightHand = new HandState();
    private float detectionTimer = 0f;

    // 手势事件委托
    public delegate void HandGestureEvent(bool isLeftHand, Vector3 handPosition);
    public event HandGestureEvent OnHandDetected;  // 手被检测到
    public event HandGestureEvent OnHandLost;  // 手丢失
    public event HandGestureEvent OnHandGrabbed;  // 手抓住（捏合）
    public event HandGestureEvent OnHandReleased;  // 手释放
    public event HandGestureEvent OnHandPinched;  // 手捏合

    private void Start()
    {
        InitializeHandStates();
    }

    private void Update()
    {
        detectionTimer += Time.deltaTime;

        if (detectionTimer >= detectionUpdateRate)
        {
            // 更新手部检测
            UpdateHandDetection();
            detectionTimer = 0f;
        }
    }

    private void OnGUI()
    {
        if (enableDebugDraw)
        {
            DrawDebugInfo();
        }
    }

    /// <summary>
    /// 初始化手部状态
    /// </summary>
    private void InitializeHandStates()
    {
        leftHand = new HandState
        {
            position = Vector3.zero,
            confidence = 0f,
            isDetected = false,
            isOpen = true,
            isPinching = false
        };

        rightHand = new HandState
        {
            position = Vector3.zero,
            confidence = 0f,
            isDetected = false,
            isOpen = true,
            isPinching = false
        };
    }

    /// <summary>
    /// 更新手部检测
    /// </summary>
    private void UpdateHandDetection()
    {
        // 优先使用 MediaPipe（当集成时）
        if (!DetectHandsFromMediaPipe())
        {
            // 降级到模拟输入
            if (useMouseSimulation)
            {
                DetectHandsFromMouse();
            }
            else if (useKeyboardSimulation)
            {
                DetectHandsFromKeyboard();
            }
            else
            {
                // 无有效输入源
                leftHand.isDetected = false;
                rightHand.isDetected = false;
            }
        }

        // 更新手势状态
        UpdateHandGestures();
    }

    /// <summary>
    /// 从 MediaPipe 检测手部（预留接口，待集成）
    /// </summary>
    private bool DetectHandsFromMediaPipe()
    {
        // TODO: 集成 MediaPipe 手部检测
        // 返回 true 表示成功检测到手
        return false;
    }

    /// <summary>
    /// 从鼠标模拟手部检测
    /// </summary>
    private void DetectHandsFromMouse()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        
        // 判断按键状态模拟左右手
        bool leftMouseDown = Input.GetMouseButton(0);
        bool rightMouseDown = Input.GetMouseButton(1);

        if (leftMouseDown || rightMouseDown)
        {
            // 将屏幕坐标转换为世界坐标
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
            Vector3 worldPos = ray.origin + ray.direction * 10f;  // 假设距离为 10

            if (leftMouseDown)
            {
                UpdateHandState(leftHand, worldPos, mouseScreenPos, 1f, Input.GetMouseButtonDown(0));
            }

            if (rightMouseDown)
            {
                UpdateHandState(rightHand, worldPos, mouseScreenPos, 1f, Input.GetMouseButtonDown(1));
            }
        }
        else
        {
            leftHand.isDetected = false;
            rightHand.isDetected = false;
        }
    }

    /// <summary>
    /// 从键盘模拟手部检测
    /// </summary>
    private void DetectHandsFromKeyboard()
    {
        // Q/W - 左手
        // E/R - 右手
        // A/S/D/F 控制位置

        Vector3 offset = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) offset.z += 1;  // 前
        if (Input.GetKey(KeyCode.S)) offset.z -= 1;  // 后
        if (Input.GetKey(KeyCode.A)) offset.x -= 1;  // 左
        if (Input.GetKey(KeyCode.D)) offset.x += 1;  // 右
        if (Input.GetKey(KeyCode.Space)) offset.y += 1;  // 上
        if (Input.GetKey(KeyCode.C)) offset.y -= 1;  // 下

        // 左手
        if (Input.GetKey(KeyCode.Q))
        {
            leftHand.position += offset * Time.deltaTime * 5f;
            UpdateHandState(leftHand, leftHand.position, Vector3.zero, 0.9f, false);
        }

        // 右手
        if (Input.GetKey(KeyCode.E))
        {
            rightHand.position += offset * Time.deltaTime * 5f;
            UpdateHandState(rightHand, rightHand.position, Vector3.zero, 0.9f, false);
        }
    }

    /// <summary>
    /// 更新单个手的状态
    /// </summary>
    private void UpdateHandState(HandState hand, Vector3 worldPos, Vector3 screenPos, float confidence, bool isNewDetection)
    {
        bool wasDetected = hand.isDetected;
        
        hand.position = worldPos;
        hand.screenPosition = screenPos;
        hand.confidence = confidence;
        hand.isDetected = confidence >= gestureConfidenceThreshold;

        // 触发手被检测事件
        if (!wasDetected && hand.isDetected)
        {
            OnHandDetected?.Invoke(hand == leftHand, hand.position);
        }
    }

    /// <summary>
    /// 更新手势状态
    /// </summary>
    private void UpdateHandGestures()
    {
        // 检测左手捏合
        if (leftHand.isDetected)
        {
            bool wasPinching = leftHand.isPinching;
            leftHand.isPinching = DetectPinchGesture(leftHand);

            if (!wasPinching && leftHand.isPinching)
            {
                OnHandGrabbed?.Invoke(true, leftHand.position);
            }
            else if (wasPinching && !leftHand.isPinching)
            {
                OnHandReleased?.Invoke(true, leftHand.position);
            }
        }

        // 检测右手捏合
        if (rightHand.isDetected)
        {
            bool wasPinching = rightHand.isPinching;
            rightHand.isPinching = DetectPinchGesture(rightHand);

            if (!wasPinching && rightHand.isPinching)
            {
                OnHandGrabbed?.Invoke(false, rightHand.position);
            }
            else if (wasPinching && !rightHand.isPinching)
            {
                OnHandReleased?.Invoke(false, rightHand.position);
            }
        }
    }

    /// <summary>
    /// 检测捏合手势
    /// </summary>
    private bool DetectPinchGesture(HandState hand)
    {
        // TODO: 从 MediaPipe 获取手指距离
        // 这里使用模拟值
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Q))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 绘制调试信息
    /// </summary>
    private void DrawDebugInfo()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 300, 10, 290, 200));
        GUILayout.Label("=== Hand Detection ===");
        
        GUILayout.Label($"Left Hand: {(leftHand.isDetected ? "✓" : "✗")}");
        GUILayout.Label($"  Position: {leftHand.position}");
        GUILayout.Label($"  Confidence: {leftHand.confidence:F2}");
        GUILayout.Label($"  Pinching: {leftHand.isPinching}");

        GUILayout.Label($"Right Hand: {(rightHand.isDetected ? "✓" : "✗")}");
        GUILayout.Label($"  Position: {rightHand.position}");
        GUILayout.Label($"  Confidence: {rightHand.confidence:F2}");
        GUILayout.Label($"  Pinching: {rightHand.isPinching}");

        GUILayout.Label("\n[Controls]");
        GUILayout.Label("LMB / Q - Detect");
        GUILayout.Label("WASD/Space - Move");
        
        GUILayout.EndArea();
    }

    // ===== 公共接口 =====

    /// <summary>
    /// 获取左手位置
    /// </summary>
    public Vector3 GetLeftHandPosition()
    {
        return leftHand.position;
    }

    /// <summary>
    /// 获取右手位置
    /// </summary>
    public Vector3 GetRightHandPosition()
    {
        return rightHand.position;
    }

    /// <summary>
    /// 获取左手是否被检测到
    /// </summary>
    public bool IsLeftHandDetected()
    {
        return leftHand.isDetected;
    }

    /// <summary>
    /// 获取右手是否被检测到
    /// </summary>
    public bool IsRightHandDetected()
    {
        return rightHand.isDetected;
    }

    /// <summary>
    /// 获取左手是否在捏合
    /// </summary>
    public bool IsLeftHandPinching()
    {
        return leftHand.isPinching;
    }

    /// <summary>
    /// 获取右手是否在捏合
    /// </summary>
    public bool IsRightHandPinching()
    {
        return rightHand.isPinching;
    }

    /// <summary>
    /// 获取左手置信度
    /// </summary>
    public float GetLeftHandConfidence()
    {
        return leftHand.confidence;
    }

    /// <summary>
    /// 获取右手置信度
    /// </summary>
    public float GetRightHandConfidence()
    {
        return rightHand.confidence;
    }
}
