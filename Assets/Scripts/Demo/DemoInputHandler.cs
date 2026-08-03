using UnityEngine;

/// <summary>
/// 演示场景输入处理脚本
/// 处理键盘快捷键和基本交互
/// </summary>
public class DemoInputHandler : MonoBehaviour
{
    private GameManager gameManager;
    private BoidManager boidManager;
    private InteractionController interactionController;

    private void Start()
    {
        gameManager = GameManager.Instance;
        boidManager = FindObjectOfType<BoidManager>();
        interactionController = FindObjectOfType<InteractionController>();
    }

    private void Update()
    {
        // 暂停/恢复
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameManager?.TogglePause();
        }

        // 重启游戏
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager?.RestartGame();
        }

        // 返回菜单
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager?.ReturnToMenu();
        }

        // 添加鱼
        if (Input.GetKeyDown(KeyCode.Plus))
        {
            if (boidManager != null)
            {
                Vector3 randomPos = Random.insideUnitSphere * 20f;
                boidManager.SpawnBoid(randomPos);
            }
        }

        // 移除鱼
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (boidManager != null)
            {
                var boids = boidManager.GetActiveBoids();
                if (boids.Count > 0)
                {
                    boidManager.RemoveBoid(boids[boids.Count - 1]);
                }
            }
        }

        // 触发逃散
        if (Input.GetKeyDown(KeyCode.T))
        {
            interactionController?.TriggerEscape(Vector3.zero);
        }

        // 停止逃散
        if (Input.GetKeyDown(KeyCode.S))
        {
            interactionController?.StopEscape();
        }
    }
}
