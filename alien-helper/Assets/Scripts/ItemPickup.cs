using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int itemCount;
    public InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.FindWithTag("InventoryManager").GetComponent<InventoryManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inventoryManager.collectItem();
            Debug.Log("MONKEY BALLS");
            Destroy(gameObject);
        }
    }
}
