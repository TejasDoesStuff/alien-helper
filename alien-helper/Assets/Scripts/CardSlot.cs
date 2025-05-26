using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    public TMP_Text cardName;
    public TMP_Text usesText;

    public void Setup(Card card, bool isSelected, Color selectedColor, Color normalColor)
    {
        cardName.text = card.cardType.ToString();
        usesText.text = "Uses: " + card.uses;
        GetComponent<Image>().color = isSelected ? selectedColor : normalColor;
    }
}
