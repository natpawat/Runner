using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger hit: " + other.name);

        ChickenController player = other.GetComponent<ChickenController>();

        if (player != null)
        {
            player.AddCoins(coinValue);
            Destroy(gameObject);
        }
    }
}