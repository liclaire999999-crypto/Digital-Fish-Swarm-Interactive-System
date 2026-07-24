using UnityEngine;

/// <summary>
/// 鱼的动画控制器
/// 管理鱼的游动、空闲、逃散等动画状态
/// </summary>
public class FishAnimator : MonoBehaviour
{
    [Header("动画参数 | Animation Parameters")]
    [SerializeField]
    private float swimSpeedMultiplier = 1f;  // 游动速度倍数
    
    [SerializeField]
    private float tailWagSpeed = 5f;  // 尾巴摇摆速度
    
    [SerializeField]
    private float tailWagAmount = 15f;  // 尾巴摇摆幅度（度数）
    
    [SerializeField]
    private float finFlutterSpeed = 8f;  // 鱼鳍抖动速度

    [Header("动画状态 | Animation States")]
    private float currentSwimSpeed = 0f;  // 当前游动速度
    private float swimSpeedVelocity = 0f;  // 平滑参数
    private float smoothTime = 0.2f;  // 平滑时间
    
    private bool isSwimming = false;
    private bool isEscaping = false;

    [Header("骨骼引用 | Bone References")]
    [SerializeField]
    private Transform tailBone;  // 尾巴骨骼
    
    [SerializeField]
    private Transform leftFinBone;  // 左鱼鳍骨骼
    
    [SerializeField]
    private Transform rightFinBone;  // 右鱼鳍骨骼

    private Animator animator;  // Animator 组件（可选）
    private float animationTime = 0f;  // 用于正弦波计算

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 初始化动画控制器
    /// </summary>
    public void Initialize()
    {
        animator = GetComponent<Animator>();
        currentSwimSpeed = 0f;
        isSwimming = false;
        isEscaping = false;
        animationTime = 0f;
    }

    private void Update()
    {
        // 更新动画时间
        animationTime += Time.deltaTime;

        // 更新骨骼动画
        if (isSwimming)
        {
            UpdateTailAnimation();
            UpdateFinAnimation();
        }
    }

    /// <summary>
    /// 播放游动动画
    /// </summary>
    public void PlaySwimAnimation(float speed)
    {
        isSwimming = true;
        isEscaping = false;

        // 平滑改变游动速度
        currentSwimSpeed = Mathf.SmoothDamp(
            currentSwimSpeed,
            speed * swimSpeedMultiplier,
            ref swimSpeedVelocity,
            smoothTime
        );

        // 如果有 Animator，设置参数
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsSwimming", true);
        }
    }

    /// <summary>
    /// 播放空闲动画
    /// </summary>
    public void PlayIdleAnimation()
    {
        isSwimming = false;
        isEscaping = false;
        currentSwimSpeed = Mathf.SmoothDamp(
            currentSwimSpeed,
            0f,
            ref swimSpeedVelocity,
            smoothTime
        );

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsSwimming", false);
        }
    }

    /// <summary>
    /// 播放逃散动画
    /// </summary>
    public void PlayFleeAnimation(float speed)
    {
        isSwimming = true;
        isEscaping = true;

        // 逃散时加快动画速度
        currentSwimSpeed = Mathf.SmoothDamp(
            currentSwimSpeed,
            speed * swimSpeedMultiplier * 1.5f,  // 加快 50%
            ref swimSpeedVelocity,
            smoothTime * 0.5f  // 更快响应
        );

        if (animator != null)
        {
            animator.SetFloat("Speed", speed * 1.5f);
            animator.SetBool("IsEscaping", true);
        }
    }

    /// <summary>
    /// 更新尾巴动画（摇摆）
    /// </summary>
    private void UpdateTailAnimation()
    {
        if (tailBone == null)
            return;

        // 使用正弦波产生摇摆效果
        float wag = Mathf.Sin(animationTime * tailWagSpeed * currentSwimSpeed) * tailWagAmount;
        
        Vector3 currentRotation = tailBone.localEulerAngles;
        currentRotation.z = wag;
        tailBone.localEulerAngles = currentRotation;
    }

    /// <summary>
    /// 更新鱼鳍动画（抖动）
    /// </summary>
    private void UpdateFinAnimation()
    {
        if (leftFinBone == null && rightFinBone == null)
            return;

        float flutter = Mathf.Sin(animationTime * finFlutterSpeed * currentSwimSpeed) * 10f;

        if (leftFinBone != null)
        {
            Vector3 leftRotation = leftFinBone.localEulerAngles;
            leftRotation.z = flutter;
            leftFinBone.localEulerAngles = leftRotation;
        }

        if (rightFinBone != null)
        {
            Vector3 rightRotation = rightFinBone.localEulerAngles;
            rightRotation.z = -flutter;  // 相反方向
            rightFinBone.localEulerAngles = rightRotation;
        }
    }

    /// <summary>
    /// 设置尾巴摇摆速度
    /// </summary>
    public void SetTailWagSpeed(float speed)
    {
        tailWagSpeed = speed;
    }

    /// <summary>
    /// 设置尾巴摇摆幅度
    /// </summary>
    public void SetTailWagAmount(float amount)
    {
        tailWagAmount = amount;
    }

    /// <summary>
    /// 获取当前游动速度
    /// </summary>
    public float GetCurrentSwimSpeed()
    {
        return currentSwimSpeed;
    }

    /// <summary>
    /// 获取是否处于游动状态
    /// </summary>
    public bool IsSwimming()
    {
        return isSwimming;
    }

    /// <summary>
    /// 获取是否处于逃散状态
    /// </summary>
    public bool IsEscaping()
    {
        return isEscaping;
    }
}
