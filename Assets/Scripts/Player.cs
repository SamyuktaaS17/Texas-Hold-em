using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class Player : MonoBehaviour {
    public float funds = 500f;
    public TextMeshProUGUI fundsText;
    public List<Card> hand = new List<Card>(); // Store the player's cards
    public float handStrength = 0f;
    private GameManager gm;
    private Dealer dealer;
    public GameObject cardDisplayParent; // Parent object to hold card displays
    public GameObject handCard1;
    public GameObject handCard2;


    void Start() {
        UpdateFundsUI();
        dealer = FindObjectOfType<Dealer>();
        gm = FindObjectOfType<GameManager>();
        if (dealer == null)
        {
            Debug.LogError("Dealer not found in the scene.");
        }

    }

    // Method to deduct funds for ante or bets
    public void DeductFunds(float amount) {
        if (funds >= amount) {
            funds -= amount;
            UpdateFundsUI();
        } else {
            Debug.Log($"{name} does not have enough funds!");
        }
    }

    // Method to evaluate hand strength based on simulation with random cards
    public float EvaluateStrength()
    {
        if (hand.Count != 2)
        {
            Debug.LogError("Player must have exactly 2 cards to evaluate strength.");
            return 0;
        }

        const int simulationCount = 20;
        float totalStrength = 0;

        // Simulate by drawing random cards and evaluating hand combinations
        for (int i = 0; i < simulationCount; i++)
        {
            List<Card> randomCommunityCards = dealer.GetRandomCommunityCardsExcluding(hand, 3); // Get 3 random cards excluding player's hand
            List<Card> simulatedHand = new List<Card>(hand); // Start with player's 2 cards
            simulatedHand.AddRange(randomCommunityCards); // Add the 3 community cards

            totalStrength += EvaluateHandStrength(simulatedHand); // Evaluate the strength of this 5-card hand
        }

        // Return the average strength across simulations
        handStrength = totalStrength / simulationCount;
        return handStrength;
    }



    // Method to evaluate and rank the player's hand strength
    public float EvaluateHandStrength(List<Card> cards)
    {
        List<Card> allCards = cards;
        int r = gm.round;
        //Debug.Log($"round inside handstrength : {r}");
        if (r==2)
        { // Check for and add community cards from the dealer
            if (dealer.Flop != null) allCards.AddRange(dealer.Flop);  
        }
        else if (r == 3)
        { // Check for and add community cards from the dealer
            //if (dealer.Turn != null) 
            allCards.Add(dealer.Turn);
        }

        else if (r == 4)
        { // Check for and add community cards from the dealer
            //Debug.Log("Into river condition");
            //Debug.Log($"{dealer.River} card in river condition");
            //if (dealer.River != null)
            allCards.Add(dealer.River);
            
        }

        // Sort cards by rank
        allCards = allCards.OrderBy(card => card.GetRankValue()).ToList();

        // Check for flush and straight
        bool isFlush = allCards.Select(card => card.suit).Distinct().Count() == 1;
        bool isStraight = IsStraight(allCards.Select(card => card.GetRankValue()).ToList());

        // Assign hand type based on detected hand characteristics
        int handType = GetHandType(isFlush, isStraight, allCards);

        // Calculate hand value using hexadecimal system for quick comparison
        return CalculateHandValue(allCards, handType);
    }

    private bool IsStraight(List<int> ranks)
    {
        // Check if the ranks are consecutive
        for (int i = 0; i < ranks.Count - 1; i++)
        {
            if (ranks[i + 1] != ranks[i] + 1) return false;
        }
        return true;
    }

    private int GetHandType(bool isFlush, bool isStraight, List<Card> cards)
    {
        if (isFlush && isStraight) return 10; // Straight Flush
        if (IsFourOfAKind(cards)) return 9;
        if (IsFullHouse(cards)) return 8;
        if (isFlush) return 7;
        if (isStraight) return 6;
        if (IsThreeOfAKind(cards)) return 5;
        if (IsTwoPair(cards)) return 4;
        if (IsPair(cards)) return 3;
        return 2; // High Card
    }

    private float CalculateHandValue(List<Card> cards, int handType)
    {
        // Sort cards by most significant rank value to least significant
        List<int> sortedRanks = cards.Select(card => card.GetRankValue()).OrderByDescending(r => r).ToList();

        float handValue = 0;
        // Calculate hand value based on hexadecimal system for quick comparison
        //float handValue = handType // Hand type multiplier
        for (int i = 0; i < sortedRanks.Count; i++)
        {
            handValue += sortedRanks[i] * handType;
        }
        handValue = handValue / (10*10);
        handStrength = handValue;
        return handValue;
    }

    // Check for Four of a Kind
    private bool IsFourOfAKind(List<Card> cards)
    {
        return cards.GroupBy(card => card.GetRankValue()).Any(group => group.Count() == 4);
    }

    // Check for Full House
    private bool IsFullHouse(List<Card> cards)
    {
        var groupedRanks = cards.GroupBy(card => card.GetRankValue()).ToList();
        return groupedRanks.Count == 2 && groupedRanks.Any(group => group.Count() == 3);
    }

    // Check for Three of a Kind
    private bool IsThreeOfAKind(List<Card> cards)
    {
        return cards.GroupBy(card => card.GetRankValue()).Any(group => group.Count() == 3);
    }

    // Check for Two Pair
    private bool IsTwoPair(List<Card> cards)
    {
        return cards.GroupBy(card => card.GetRankValue()).Count(group => group.Count() == 2) == 2;
    }

    // Check for One Pair
    private bool IsPair(List<Card> cards)
    {
        return cards.GroupBy(card => card.GetRankValue()).Any(group => group.Count() == 2);
    }


    // Method to add funds when the player wins a round
    public void AddFunds(float amount) {
        funds += amount;
        UpdateFundsUI();
    }

    // Method to update the funds UI
    private void UpdateFundsUI() {
        if (fundsText != null) {
            fundsText.text = $"${funds}";
        }
    }

    // Method to add cards to the player's hand
    public void AddCardToHand(Card card) {
        hand.Add(card);
    }

    // Method to clear the hand (useful for new rounds)
    public void ClearHand() {
        hand.Clear();
    }



    public void placeCard(Card card, float xoffset, float yoffset, float rotation)
    {
        handCard1.SetActive(false);
        handCard2.SetActive(false);
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
            cardInstance.transform.localPosition = new Vector3(xoffset, yoffset, 0); // Apply the offset to the X-axis

            // Disable gravity on the Riagidbody if it exists
            Rigidbody rb = cardInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false; // Disable gravity
                rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation; // Freeze position and rotation
            }

            cardInstance.transform.localRotation = Quaternion.Euler(rotation, 0, 0); // Rotate around the Y-axis
        }
        else
        {
            Debug.LogWarning($"Card prefab {cardName} not found!");
        }

    }
}
