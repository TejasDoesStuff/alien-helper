using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public DeckManager deckManager;
    public GameObject cardSlotPrefab;
    public Transform cardPanel;
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    private List<GameObject> cardSlots = new List<GameObject>();

    void Start()
    {
        UpdateHandUI();
    }

    public void UpdateHandUI()
    {
        foreach (var slot in cardSlots)
        {
            Destroy(slot);
        }
        cardSlots.Clear();

        for (int i = 0; i < deckManager.hand.Count; i++)
        {
            Card card = deckManager.hand[i];
            GameObject slot = Instantiate(cardSlotPrefab, cardPanel);
            cardSlots.Add(slot);

            CardSlot cardSlot = slot.GetComponent<CardSlot>();
            if (cardSlot != null)
            {
                cardSlot.Setup(card, i == deckManager.currentIndex, selectedColor, normalColor);
            }
        }
    }
}
