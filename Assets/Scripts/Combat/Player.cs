using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 20;
    public int CurrentHealth { get; private set; }

    void Awake()
    {
        CurrentHealth = maxHealth;      // just set the value
    }

    void Start()
    {
        // by Start() the HUD script’s Awake has run, so Instance exists
        if (PlayerHealthBar.Instance != null)
            PlayerHealthBar.Instance.Refresh(this);
    }

    public void TakeDamage(int dmg)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - dmg);

        if (PlayerHealthBar.Instance != null)
            PlayerHealthBar.Instance.Refresh(this);

        if (CurrentHealth <= 0)
            GameManager.Instance.PlayerDied();
    }

    public void Revive()
    {
        CurrentHealth = maxHealth;

        if (PlayerHealthBar.Instance != null)
            PlayerHealthBar.Instance.Refresh(this);
    }
}
