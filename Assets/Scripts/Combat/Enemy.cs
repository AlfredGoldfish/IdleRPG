using UnityEngine;

public class Enemy : MonoBehaviour, IHealth
{
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public int maxHealth = 10;
    private int currentHealth;
    public EnemySpawner spawner;

    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnMouseDown()
    {
        currentHealth -= 1;

        if (currentHealth <= 0)
        {
            spawner?.OnEnemyKilled(this);
            Destroy(gameObject);
        }
    }
}
