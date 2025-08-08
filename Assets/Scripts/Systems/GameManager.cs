using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public GameObject revivePanel;   // canvas panel with “Revive?” button

    Player player;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        player = Object.FindFirstObjectByType<Player>();
        revivePanel.SetActive(false);
    }

    public void PlayerDied()
    {
        Time.timeScale = 0f;
        revivePanel.SetActive(true);
    }

    // Hook to button OnClick
    public void OnReviveClicked()
    {
        revivePanel.SetActive(false);
        player.Revive();
        Time.timeScale = 1f;
    }
}
