using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deck : MonoBehaviour {
    private List<Card> deck;

    // Property to expose the deck list as IEnumerable<Card>
    public IEnumerable<Card> Cards => deck;

    void Start() {
        InitializeDeck();
        ShuffleDeck();
    }

    // Initializes a standard 52-card deck
    public void InitializeDeck() {
        deck = new List<Card>();
        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit))) {
            foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank))) {
                deck.Add(new Card(suit, rank));
            }
        }
    }

    // Shuffles the deck using the Fisher-Yates algorithm
    public void ShuffleDeck() {
        for (int i = deck.Count - 1; i > 0; i--) {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // Draws a card from the top of the deck and removes it
    public Card DrawCard() {
        if (deck.Count == 0) {
            Debug.LogError("Deck is empty!");
            return null;
        }
        Card drawnCard = deck[0];
        deck.RemoveAt(0);
        return drawnCard;
    }

    // Method to get multiple random cards excluding specified ones
    public List<Card> GetRandomCardsExcluding(List<Card> excludeCards, int count)
    {
        // Filter out excluded cards from the deck
        List<Card> availableCards = deck.Except(excludeCards).ToList();

        if (availableCards.Count < count)
        {
            Debug.LogError("Not enough cards available in the deck after excluding specified cards.");
            return null;
        }

        // Shuffle and take the specified number of cards
        return availableCards.OrderBy(_ => Random.value).Take(count).ToList();
    }
}
