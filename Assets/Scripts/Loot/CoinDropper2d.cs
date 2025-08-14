using UnityEngine;

public class CoinDropper2D : MonoBehaviour
{
    public GameObject coinPrefab;
    public Metal dropMetal;
    public int dropAmount = 1;

    public void DropCoin()
    {
        if (coinPrefab != null)
        {
            var coinObj = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            var pickup = coinObj.GetComponent<CoinPickup2D>();
            if (pickup != null)
            {
                pickup.Initialize(dropMetal, (ulong)dropAmount); // cast int to ulong
            }
        }
    }
}
