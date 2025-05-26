using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    public int handSize = 5;
    public int currentIndex = 0;

    public CardManager cardManager;

    void Start()
    {
        createDeck();
    }

    public void drawHand()
    {
        hand.Clear();
        for (int i = 0; i < handSize && deck.Count > 0; i++)
        {
            Card drawn = deck[0];
            hand.Add(drawn);
            deck.RemoveAt(0);
        }

        cardManager.UpdateHandUI();
    }

    public Card getCard()
    {
        return hand[currentIndex];
    }

    public void removeUsedCards()
    {
        hand.RemoveAll(c => c.isUsed());
        cardManager.UpdateHandUI();
    }

    public void printHand()
    {
        Debug.Log("=== Current Hand ===");
        for (int i = 0; i < hand.Count; i++)
        {
            Debug.Log($"Slot {i + 1}: {hand[i].cardType} - Uses Left: {hand[i].uses}");
        }
    }

    public void createDeck()
    {
        for (int i = 0; i < 10; i++)
        {
            int rand = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(CardType)).Length);
            CardType type = (CardType)rand;
            deck.Add(new Card(type));
        }

        drawHand();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            printHand();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            drawHand();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { currentIndex = 0; cardManager.UpdateHandUI(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { currentIndex = 1; cardManager.UpdateHandUI(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { currentIndex = 2; cardManager.UpdateHandUI(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { currentIndex = 3; cardManager.UpdateHandUI(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { currentIndex = 4; cardManager.UpdateHandUI(); }

        if (hand.Count == 0)
        {
            drawHand();
        }

        if(deck.Count == 0 && hand.Count == 0)
        {
            createDeck();
        }
    }
}