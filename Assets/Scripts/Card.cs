using UnityEngine;

public class Card : MonoBehaviour {
    public enum Suit {
        Hearts = 0,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank {
        Two = 02,
        Three = 03,
        Four = 04,
        Five = 05,
        Six = 06,
        Seven = 07,
        Eight = 08,
        Nine = 09,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 01
    }

    public Suit suit; // Suit of the card
    public Rank rank; // Rank of the card

    // Constructor for initializing the car
    public Card(Suit suit, Rank rank) {
        this.suit = suit;
        this.rank = rank;
    }

    // Method to get the numerical value of the rank (useful for sorting and comparisons)
    public int GetRankValue()
    {
        return (int)rank; // Ranks 2 to Ace correspond to values 2 to 14
    }

    // Method to get the numerical value of the suit (useful for flush checks)
    public int GetSuitValue()
    {
        return (int)suit;
    }

    // Method to get a readable string for the card (e.g., "Ace of Spades")
    public override string ToString() {
        return $"{rank} of {suit}";
    }
}
