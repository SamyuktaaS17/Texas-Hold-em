using System;
using UnityEngine;
using TMPro;

public class Decision : MonoBehaviour
{
    private GameManager gm;
    private const float callThreshold = 0.30f; // Arbitrary threshold for calling
    private const float raiseThreshold = 0.95f; // Arbitrary threshold for raising
    private const float maxRaiseAmount = 50f; // Max raise amount per round
    private const float minRaiseAmount = 10f; // Min raise amount per round


    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        if (gm == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    // Method to make a decision based on hand strength and round
    public string MakeDecision(Player player, int currentBet)
    {
        int round = gm.round;
        float handStrength = 0;
        if (round==1)
            handStrength = player.handStrength;
        if(round>1)
            handStrength = player.handStrength;
        //Debug.Log($"{handStrength} is handstrength in decision class!");
        //float callAmount = player.CurrentBet;

        if (round == 1)
        {
            //float callAmount = CalculateCallAmount(round);
            player.DeductFunds(currentBet);
            gm.totalPot += currentBet;
            return $"Call with ${currentBet:F2}";
        }
        // Decision-making logic based on hand strength and round
        if (handStrength >= raiseThreshold)
        {
            float raiseAmount1 = UnityEngine.Random.Range(minRaiseAmount, maxRaiseAmount);
            // Convert raiseAmount to int by ceiling the float value
            int raiseAmount = Mathf.CeilToInt(raiseAmount1);
            int finalAmount = raiseAmount + currentBet;
            gm.totalPot += finalAmount;
            player.DeductFunds(finalAmount);
            return $"Raise by ${raiseAmount:F2}";
        }
        else if (handStrength >= callThreshold)
        {
            //float callAmount = CalculateCallAmount(round);
            player.DeductFunds(currentBet);
            gm.totalPot += currentBet;
            return $"Call with ${currentBet:F2}";
        }
        else
        {
            return "Fold";
        }
    }

    // Calculate call amount based on the current round
    private float CalculateCallAmount(int round)
    {
        // Increase call amount slightly in later rounds
        switch (round)
        {
            case 1: return 10f;
            case 2: return 20f;
            case 3: return 30f;
            case 4: return 40f;
            default: return 10f; // Default amount
        }
    }




    public string MakeMainPlayerDecision(Player player, int currentBet, string action, int raiseAmount = 0)
    {
        int round = gm.round;
        float handStrength = 0;
        if (round == 1)
            handStrength = player.handStrength;
        if (round > 1)
            handStrength = player.handStrength;

        switch (action.ToUpper())
        {
            case "F":
                return "Fold";

            case "R":
                if (raiseAmount <= 0)
                {
                    Debug.LogWarning("Invalid raise amount");
                    return "Invalid raise amount";
                }
                int finalAmount = raiseAmount + currentBet;
                if (player.funds >= finalAmount)
                {
                    gm.totalPot += finalAmount;
                    player.DeductFunds(finalAmount);
                    return $"Raise by ${raiseAmount:F2}";
                }
                else
                {
                    Debug.LogWarning("Not enough funds to raise");
                    return "Not enough funds";
                }

            case "C":
                if (player.funds >= currentBet)
                {
                    player.DeductFunds(currentBet);
                    gm.totalPot += currentBet;
                    return $"Call with ${currentBet:F2}";
                }
                else
                {
                    Debug.LogWarning("Not enough funds to call");
                    return "Not enough funds";
                }

            default:
                return "Invalid action";
        }
    }
}
