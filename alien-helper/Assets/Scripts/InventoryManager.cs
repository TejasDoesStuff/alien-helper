using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    private int itemCount;
    public TMP_Text itemText;

    void Start()
    {
        itemCount = 0;
        itemText.text = "Items: " + itemCount.ToString();
    }

    public void collectItem()
    {
        itemCount++;
        itemText.text = "Items: " + itemCount.ToString();
    }
}
