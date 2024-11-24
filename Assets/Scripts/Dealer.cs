using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dealer : MonoBehaviour
{
    public Deck deck;                  // Reference to the shared Deck component
    public List<Card> Flop; // Flop cards, accessible publicly
    public Card Turn;     // Turn card, accessible publicly
    public Card River; // River card, accessible publicly
    public Animator animator;

    public GameObject cardDisplayParent; // Parent object to hold card displays

    void Start()
    {
        animator = GetComponent<Animator>();
        if (deck == null)
        {
            Debug.LogError("Deck not assigned to Dealer.");
        }
    }

    // Method to get unique random community cards that exclude the player's cards
    public List<Card> GetRandomCommunityCardsExcluding(List<Card> excludeCards, int count = 3)
    {
        // Get all remaining cards in the deck that are not in excludeCards
        List<Card> availableCards = deck.Cards.Except(excludeCards).ToList();

        // Shuffle and take the specified number of cards
        return availableCards.OrderBy(_ => Random.value).Take(count).ToList();
    }

    // Deals the Flop (3 cards) and stores it in the Flop list
    public void DealFlop()
    {
        animator.SetBool("PlaceCard", true);
        Flop = new List<Card> { deck.DrawCard(), deck.DrawCard(), deck.DrawCard() };
        placeFlop();
        Debug.Log($"Flop: {Flop[0]}, {Flop[1]}, {Flop[2]}");
    }

    public void placeFlop()
    {
        for (int i = 0; i < 3; i++)
        {
            Card card = Flop[i];
            string cardSuit;
            switch (card.suit)
            {
                case Card.Suit.Hearts:
                    cardSuit = "Heart";
                    break;
                case Card.Suit.Diamonds:
                    cardSuit = "Diamond";
                    break;
                case Card.Suit.Spades:
                    cardSuit = "Spade";
                    break;
                case Card.Suit.Clubs:
                    cardSuit = "Club";
                    break;
                default:
                    cardSuit = $"{card.suit}";
                    break;
            }

            string cardRank;
            switch (card.rank)
            {
                case Card.Rank.Two:
                    cardRank = "2";
                    break;
                case Card.Rank.Three:
                    cardRank = "3";
                    break;
                case Card.Rank.Four:
                    cardRank = "4";
                    break;
                case Card.Rank.Five:
                    cardRank = "5";
                    break;
                case Card.Rank.Six:
                    cardRank = "6";
                    break;
                case Card.Rank.Seven:
                    cardRank = "7";
                    break;
                case Card.Rank.Eight:
                    cardRank = "8";
                    break;
                case Card.Rank.Nine:
                    cardRank = "9";
                    break;
                case Card.Rank.Ten:
                    cardRank = "10";
                    break;
                default:
                    cardRank = $"{card.rank}";
                    break;
            }

            string cardName = $"Cards/{card.suit}/Card_{cardSuit}{cardRank}"; // Ensure your Card class has Value and Suit properties
            GameObject cardPrefab = Resources.Load<GameObject>(cardName);

            if (cardPrefab != null)
            {
                GameObject cardInstance = Instantiate(cardPrefab, cardDisplayParent.transform);
                cardInstance.transform.localScale = new Vector3(0.5f, 1.7f, 0.5f); // Make the card slightly bigger

                // Translate card position based on the positionOffset
                cardInstance.transform.localPosition = new Vector3(0.1f * i - 0.2f, 0.84f, 0); // Apply the offset to the X-axis

                // Disable gravity on the Riagidbody if it exists
                Rigidbody rb = cardInstance.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false; // Disable gravity
                    rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation; // Freeze position and rotation
                }

                cardInstance.transform.localRotation = Quaternion.Euler(-90, 0, 0); // Rotate around the Y-axis
            }
            else
            {
                Debug.LogWarning($"Card prefab {cardName} not found!");
            }
        }
    }





    public void placeCard(Card card, float offset)
    {
        string cardSuit;
        switch (card.suit)
        {
            case Card.Suit.Hearts:
                cardSuit = "Heart";
                break;
            case Card.Suit.Diamonds:
                cardSuit = "Diamond";
                break;
            case Card.Suit.Spades:
                cardSuit = "Spade";
                break;
            case Card.Suit.Clubs:
                cardSuit = "Club";
                break;
            default:
                cardSuit = $"{card.suit}";
                break;
        }

        string cardRank;
        switch (card.rank)
        {
            case Card.Rank.Two:
                cardRank = "2";
                break;
            case Card.Rank.Three:
                cardRank = "3";
                break;
            case Card.Rank.Four:
                cardRank = "4";
                break;
            case Card.Rank.Five:
                cardRank = "5";
                break;
            case Card.Rank.Six:
                cardRank = "6";
                break;
            case Card.Rank.Seven:
                cardRank = "7";
                break;
            case Card.Rank.Eight:
                cardRank = "8";
                break;
            case Card.Rank.Nine:
                cardRank = "9";
                break;
            case Card.Rank.Ten:
                cardRank = "10";
                break;
            default:
                cardRank = $"{card.rank}";
                break;
        }

        string cardName = $"Cards/{card.suit}/Card_{cardSuit}{cardRank}"; // Ensure your Card class has Value and Suit properties
        GameObject cardPrefab = Resources.Load<GameObject>(cardName);

        if (cardPrefab != null)
        {
            GameObject cardInstance = Instantiate(cardPrefab, cardDisplayParent.transform);
            cardInstance.transform.localScale = new Vector3(0.5f, 1.7f, 0.5f); // Make the card slightly bigger

            // Translate card position based on the positionOffset
            cardInstance.transform.localPosition = new Vector3(offset, 0.84f, 0); // Apply the offset to the X-axis

            // Disable gravity on the Riagidbody if it exists
            Rigidbody rb = cardInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false; // Disable gravity
                rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation; // Freeze position and rotation
            }

            cardInstance.transform.localRotation = Quaternion.Euler(-90, 0, 0); // Rotate around the Y-axis
        }
        else
        {
            Debug.LogWarning($"Card prefab {cardName} not found!");
        }

    }

    // Deals the Turn (1 card) and assigns it to the Turn variable
    public void DealTurn()
    {
        Turn = deck.DrawCard();
        placeCard(Turn, 0.106f);
        Debug.Log($"Turn: {Turn}");
    }

    // Deals the River (1 card) and assigns it to the River variable
    public void DealRiver()
    {
        River = deck.DrawCard();
        placeCard(Turn, 0.208f);
        Debug.Log($"River: {River}");
    }
}
