using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fillImage;
    private Enemy enemy;

    public void Setup(Enemy enemyRef)
    {
        enemy = enemyRef;
    }

    private void Update()
    {
        if (enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        float percent = (float)enemy.CurrentHealth / enemy.MaxHealth;
        fillImage.fillAmount = percent;

        // Follow the enemy
        transform.position = enemy.transform.position + Vector3.up * 1.5f;
    }
}
