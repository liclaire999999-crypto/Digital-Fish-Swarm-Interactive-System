using UnityEngine;

/// <summary>
/// 演示场景控制面板
/// 主要游戏控制和参数调试
/// </summary>
public class DemoControlPanel : MonoBehaviour
{
    [Header("游戏控制 | Game Control")]
    [SerializeField]
    private bool showControlPanel = true;  // 显示控制板
    
    [SerializeField]
    private int panelWidth = 350;  // 控制板宽度
    
    [SerializeField]
    private int panelHeight = 600;  // 控制板高度

    [Header("参数调试 | Parameter Adjustment")]
    [SerializeField]
    private float fishSpawnRadius = 30f;  // 鱼群生成半径
    
    [SerializeField]
    private float escapeRadius = 20f;  // 逃散半径
    
    [SerializeField]
    private float escapeIntensity = 1f;  // 逃散强度

    [Header("统计信息 | Statistics")]
    [SerializeField]
    private bool showStatistics = true;  // 显示统计

    private GameManager gameManager;
    private BoidManager boidManager;
    private InteractionController interactionController;
    private VisualEffectsManager visualEffectsManager;
    private Vector2 scrollPosition = Vector2.zero;
    private bool isPanelExpanded = true;

    private void Start()
    {
        gameManager = GameManager.Instance;
        boidManager = FindObjectOfType<BoidManager>();
        interactionController = FindObjectOfType<InteractionController>();
        visualEffectsManager = FindObjectOfType<VisualEffectsManager>();
    }

    private void OnGUI()
    {
        if (!showControlPanel)
            return;

        // 控制板主窗口
        GUI.BeginGroup(new Rect(10, 10, panelWidth, panelHeight));
        GUI.Box(new Rect(0, 0, panelWidth, panelHeight), "Demo Control Panel");

        scrollPosition = GUI.BeginScrollView(
            new Rect(5, 25, panelWidth - 10, panelHeight - 30),
            scrollPosition,
            new Rect(0, 0, panelWidth - 25, 1200)
        );

        int y = 10;
        int itemHeight = 30;
        int buttonWidth = panelWidth - 20;
        int buttonHeight = 25;

        // ===== 游戏控制 =====
        GUI.Label(new Rect(10, y, 200, 20), "<b>=== Game Control ===");
        y += itemHeight;

        if (GUI.Button(new Rect(10, y, buttonWidth, buttonHeight), "▶ Start Game"))
        {
            gameManager?.StartGame();
        }
        y += itemHeight;

        if (GUI.Button(new Rect(10, y, buttonWidth, buttonHeight), "II Pause/Resume"))
        {
            gameManager?.TogglePause();
        }
        y += itemHeight;

        if (GUI.Button(new Rect(10, y, buttonWidth, buttonHeight), "↻ Restart Game"))
        {
            gameManager?.RestartGame();
        }
        y += itemHeight;

        if (GUI.Button(new Rect(10, y, buttonWidth, buttonHeight), "< Return to Menu"))
        {
            gameManager?.ReturnToMenu();
        }
        y += itemHeight + 10;

        // ===== 游戏信息 =====
        if (gameManager != null)
        {
            GUI.Label(new Rect(10, y, 200, 20), "<b>=== Game Info ===");
            y += itemHeight;

            GUI.Label(new Rect(10, y, buttonWidth, 20), 
                $"State: {gameManager.CurrentState}");
            y += itemHeight;

            GUI.Label(new Rect(10, y, buttonWidth, 20), 
                $"Time Remaining: {gameManager.GetRemainingTime():F1}s");
            y += itemHeight;

            GUI.Label(new Rect(10, y, buttonWidth, 20), 
                $"Fish Count: {gameManager.GetCurrentFishCount()}");
            y += itemHeight;

            GUI.Label(new Rect(10, y, buttonWidth, 20), 
                $"Progress: {gameManager.GetGameProgress() * 100:F1}%");
            y += itemHeight + 10;
        }

        // ===== Boid 控制 =====
        GUI.Label(new Rect(10, y, 200, 20), "<b>=== Boid Control ===");
        y += itemHeight;

        if (GUI.Button(new Rect(10, y, buttonWidth / 2 - 5, buttonHeight), "+ Add Fish"))
        {
            if (boidManager != null)
            {
                boidManager.SpawnBoid(Random.insideUnitSphere * fishSpawnRadius);
            }
        }

        if (GUI.Button(new Rect(10 + buttonWidth / 2 + 5, y, buttonWidth / 2 - 5, buttonHeight), "- Remove Fish"))
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
        y += itemHeight;

        if (GUI.Button(new Rect(10, y, buttonWidth, buttonHeight), "Clear All Fish"))
        {
            if (boidManager != null)
            {
                boidManager.ClearAllBoids();
            }
        }
        y += itemHeight + 10;

        // ===== 交互控制 =====
        GUI.Label(new Rect(10, y, 200, 20), "<b>=== Interaction Control ===");
        y += itemHeight;

        GUI.Label(new Rect(10, y, 100, 20), "Escape Radius:");
        escapeRadius = GUI.HorizontalSlider(
            new Rect(120, y + 5, buttonWidth - 120, 15),
            escapeRadius, 5f, 50f
        );
        y += itemHeight;

        GUI.Label(new Rect(10, y, 100, 20), "Escape Intensity:");
        escapeIntensity = GUI.HorizontalSlider(
            new Rect(120, y + 5, buttonWidth - 120, 15),
            escapeIntensity, 0.1f, 3f
        );
        y += itemHeight;

        if (interactionController != null)
        {
            interactionController.SetEscapeRadius(escapeRadius);
            interactionController.SetEscapeIntensity(escapeIntensity);
        }

        if (GUI.Button(new Rect(10, y, buttonWidth, buttonHeight), "Test Escape (Center)"))
        {
            interactionController?.TriggerEscape(Vector3.zero);
        }
        y += itemHeight;

        if (GUI.Button(new Rect(10, y, buttonWidth, buttonHeight), "Stop Escape"))
        {
            interactionController?.StopEscape();
        }
        y += itemHeight + 10;

        // ===== 视觉效果控制 =====
        GUI.Label(new Rect(10, y, 200, 20), "<b>=== Visual Effects ===");
        y += itemHeight;

        if (GUI.Button(new Rect(10, y, buttonWidth / 2 - 5, buttonHeight), "Enable All"))
        {
            visualEffectsManager?.SetAllEffectsEnabled(true);
        }

        if (GUI.Button(new Rect(10 + buttonWidth / 2 + 5, y, buttonWidth / 2 - 5, buttonHeight), "Disable All"))
        {
            visualEffectsManager?.SetAllEffectsEnabled(false);
        }
        y += itemHeight;

        if (GUI.Button(new Rect(10, y, buttonWidth, buttonHeight), "Trigger Burst"))
        {
            visualEffectsManager?.TriggerGlobalBurst(30);
        }
        y += itemHeight + 10;

        // ===== 帮助 =====
        GUI.Label(new Rect(10, y, 200, 20), "<b>=== Help ===");
        y += itemHeight;

        GUI.Label(new Rect(10, y, buttonWidth, 60), 
            "Keyboard Controls:\n" +
            "Space: Pause | R: Restart | ESC: Menu\n" +
            "+/-: Add/Remove Fish | T: Trigger Escape");
        y += 70;

        GUI.EndScrollView();
        GUI.EndGroup();
    }
}
