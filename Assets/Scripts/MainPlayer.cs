using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour {
    public GameObject cardDisplayParent; // Parent object to hold card displays

    // Method to show the card prefab in the scene
    public void ShowCard(Card card, bool rotatePositive, float positionOffset) {
        string cardSuit;
        switch (card.suit) {
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
        switch (card.rank) {
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

        if (cardPrefab != null) {
            GameObject cardInstance = Instantiate(cardPrefab, cardDisplayParent.transform);
            cardInstance.transform.localScale = new Vector3(1.4f, 1.4f, 0.5f); // Make the card slightly bigger
            
            // Translate card position based on the positionOffset
            cardInstance.transform.localPosition = new Vector3(positionOffset+0.2f, -0.3f, -0.5f); // Apply the offset to the X-axis

            // Disable gravity on the Riagidbody if it exists
            Rigidbody rb = cardInstance.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.useGravity = false; // Disable gravity
                rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation; // Freeze position and rotation
            }

            // Rotate the card based on the rotatePositive flag
            float rotationAngle = rotatePositive ? 30f : -30f; // Set rotation angle
            cardInstance.transform.localRotation = Quaternion.Euler(0, 180, rotationAngle); // Rotate around the Y-axis
        } else {
            Debug.LogWarning($"Card prefab {cardName} not found!");
        }
    }

    // Method to clear displayed cards (if needed)
    public void ClearCards() {
        foreach (Transform child in cardDisplayParent.transform) {
            Destroy(child.gameObject); // Destroy displayed cards
        }
    }
}
