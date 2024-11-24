using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    public List<Player> players; // Assign each player in the Inspector
    public Dealer dealer;         // Assign the dealer in the Inspector
    public Deck deck;             // Assign the shared Deck in the Inspector
    private int minAnte = 1;
    private int maxAnte = 5;
    public int round = 1;
    [Header("UI Elements")]
    public TMP_Text anteText;
    public TMP_Text anteTextPressB;
    public TMP_Text playerRTurn;
    public TMP_Text playerRTurnDesc;
    public TMP_Text playerLTurn;
    public TMP_Text playerLTurnDesc;
    public TMP_Text pot;
    public int totalPot = 0;

    private int currentBet = 10;
    private int currentAnteAmount;
    private Player mainPlayer;
    private bool isWaitingForAnte;
    private Decision decision;
    [SerializeField] private TMP_InputField raiseInputField;
    [SerializeField] private TMP_Text mainPlayerActionText;
    private bool isMainPlayerTurn = false;
    private bool isWaitingForRaiseInput = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        dealer.animator.SetBool("Winner", false);
        dealer.animator.SetBool("PlaceCard", false);
        round = 1;
        //Debug.Log($"Initial round: {round}");
        if (players == null || players.Count < 3)
        {
            Debug.LogError("Not enough players assigned in the Inspector.");
            return;
        }

        anteText.text = "";
        anteTextPressB.text = "";
        playerLTurn.text = "";
        playerLTurnDesc.text = "";
        playerRTurn.text = "";
        playerRTurnDesc.text = "";
        mainPlayerActionText.text = "";
        pot.text = $"${totalPot}";

        raiseInputField.gameObject.SetActive(false);
        

        decision = gameObject.AddComponent<Decision>();

        UpdateGameState(GameState.Ante);
    }

    void Update()
    {
        if (isWaitingForAnte && Input.GetKeyDown(KeyCode.B))
        {
            PlaceBet();
        }
        if (isMainPlayerTurn)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                HandleMainPlayerAction("F");
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                ActivateRaiseInput();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                HandleMainPlayerAction("C");
            }
        }
        if (isWaitingForRaiseInput && Input.GetKeyDown(KeyCode.Return))
        {
            ProcessRaiseInput();
        }
        pot.text = $"${totalPot}";
    }

    private void ActivateRaiseInput()
    {
        raiseInputField.gameObject.SetActive(true);
        raiseInputField.text = "";
        raiseInputField.Select();
        raiseInputField.ActivateInputField();
        isWaitingForRaiseInput = true;
    }


    private void ProcessRaiseInput()
    {
        if (int.TryParse(raiseInputField.text, out int raiseAmount))
        {
            HandleMainPlayerAction("R", raiseAmount);
        }
        else
        {
            Debug.LogWarning("Invalid raise amount entered");
            mainPlayerActionText.text = "Invalid raise amount! Try again.";
            return;
        }
        
        raiseInputField.gameObject.SetActive(false);
        raiseInputField.text = "";
        isWaitingForRaiseInput = false;
    }

    public void OnRaiseInputSubmitted()
    {
        if (int.TryParse(raiseInputField.text, out int raiseAmount))
        {
            HandleMainPlayerAction("R", raiseAmount);
        }
        else
        {
            Debug.LogWarning("Invalid raise amount entered");
        }
        raiseInputField.gameObject.SetActive(false);
        raiseInputField.text = "";
    }

    private void HandleMainPlayerAction(string action, int raiseAmount = 0)
    {
        string result = decision.MakeMainPlayerDecision(mainPlayer, currentBet, action, raiseAmount);
        mainPlayerActionText.text = result;
        isMainPlayerTurn = false;
        StartCoroutine(DelayedNextTurn());
        mainPlayerActionText.text = "";
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        // Reset the current bet at the start of each round
        if (State == GameState.DealerTurn)
        {
            currentBet = UnityEngine.Random.Range(10, 101); // Randomize the bet between 10 and 100
            Debug.Log($"New round, current bet is: {currentBet}");
        }

        switch (newState)
        {
            case GameState.Ante:
                HandleAnte();
                break;
            case GameState.DealCards:
                DealCards();
                break;
            case GameState.PlayerRTurn:
                playerRTurn.text = "Player R's Turn";
                playerRTurnDesc.text = "Deciding...";
                HandlePlayerTurn(players[1]);
                break;
            case GameState.MainPlayerTurn:
                HandlePlayerTurn(mainPlayer);
                break;
            case GameState.PlayerLTurn:
                playerLTurn.text = "Player L's Turn";
                playerLTurnDesc.text = "Deciding...";
                HandlePlayerTurn(players[2]);

                break;
            case GameState.DealerTurn:
                HandleDealerTurn();
                break;
            case GameState.DeclareWinner:
                DeclareWinner();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleAnte()
    {
        currentAnteAmount = UnityEngine.Random.Range(minAnte, maxAnte + 1);

        anteText.text = $"Ante Bet: ${currentAnteAmount}";
        anteTextPressB.text = "Press 'B' to place ante bet";
        isWaitingForAnte = true;
        mainPlayer = players[0];
    }

    private void PlaceBet()
    {
        if (mainPlayer.funds >= currentAnteAmount)
        {
            mainPlayer.DeductFunds(currentAnteAmount);
            totalPot = totalPot + currentAnteAmount;
            Debug.Log($"{mainPlayer.name} paid ante: {currentAnteAmount}. New balance: {mainPlayer.funds}");
        }
        else
        {
            Debug.LogWarning($"{mainPlayer.name} does not have enough funds to place the ante.");
        }

        foreach (Player player in players)
        {
            if (player != mainPlayer && player.funds >= currentAnteAmount)
            {
                player.DeductFunds(currentAnteAmount);
                totalPot = totalPot + currentAnteAmount;
                Debug.Log($"{player.name} (AI) paid ante: {currentAnteAmount}. New balance: {player.funds}");
            }
        }

        anteText.text = "";
        anteTextPressB.text = "";
        isWaitingForAnte = false;

        UpdateGameState(GameState.DealCards);
    }

    private void DealCards()
    {
        // Shuffle the deck before dealing
        deck.ShuffleDeck();

        // Deal two cards to each player
        foreach (Player player in players)
        {
            player.AddCardToHand(deck.DrawCard());
            player.AddCardToHand(deck.DrawCard());
            Debug.Log($"{player.name} was dealt: {player.hand[0]}, {player.hand[1]}");

            // Check if the player is the main player and show their cards
            if (player == mainPlayer)
            {
                MainPlayer mainPlayerScript = player.GetComponent<MainPlayer>();
                if (mainPlayerScript != null)
                {
                    mainPlayerScript.ShowCard(player.hand[0], true, 0.08f); // Show the first card
                    mainPlayerScript.ShowCard(player.hand[1], false, -0.08f); // Show the second card
                }
            }
        }

        // Proceed to the next game state
        UpdateGameState(GameState.PlayerRTurn);
    }


    private void HandlePlayerTurn(Player player)
    {
        Debug.Log($"{player.name}'s turn.");
        
        if (player == mainPlayer)
        {
            isMainPlayerTurn = true;
            mainPlayerActionText.text = "Press F to Fold, R to Raise, or C to Call";
        }
        else
        {
            StartCoroutine(ProcessPlayerTurnWithDelay(player, (action) => {
                Debug.Log($"{player.name}'s action: {action}");
                string playerObjectName = player.gameObject.name;
                if (playerObjectName == "Player L")
                {
                    playerLTurnDesc.text = action;
                }
                else if (playerObjectName == "Player R")
                {
                    playerRTurnDesc.text = action;
                }
                StartCoroutine(DelayedNextTurn());
            }));
        }
    }

    private IEnumerator DelayedNextTurn()
    {
        yield return new WaitForSeconds(2f);
        playerLTurn.text = "";
        playerLTurnDesc.text = "";
        playerRTurn.text = "";
        playerRTurnDesc.text = "";
        NextTurn();
    }

    private IEnumerator ProcessPlayerTurnWithDelay(Player player, System.Action<string> onComplete)
    {
        // Wait for a random time between 3 and 6 seconds
        float delayTime = UnityEngine.Random.Range(3f, 6f);
        yield return new WaitForSeconds(delayTime);

        float handStrength = 0;
        // Evaluate hand strength
        if (round == 1)
        {
            handStrength = player.EvaluateStrength();
        }
        if (round > 1)
        {
            handStrength = player.EvaluateHandStrength(player.hand);
        }

        // Print out each card in the player's hand
        string handDescription = string.Join(", ", player.hand);
        Debug.Log($"{player.name}'s hand: {handDescription}");

        // Print the hand strength to the console
        Debug.Log($"{player.name}'s hand strength: {handStrength}");

        // Make a decision based on hand strength and round
        // Use the Decision instance to make a decision for the player
        string action = decision.MakeDecision(player, currentBet);

        // Return the action through the callback
        onComplete(action);
    }

    private void NextTurn()
    {
        switch (State)
        {
            case GameState.PlayerRTurn:
                UpdateGameState(GameState.MainPlayerTurn);
                break;
            case GameState.MainPlayerTurn:
                UpdateGameState(GameState.PlayerLTurn);
                break;
            case GameState.PlayerLTurn:
                UpdateGameState(GameState.DealerTurn);
                break;
            case GameState.DealerTurn:
                HandleDealerTurn();
                break;
            case GameState.DeclareWinner:
                DeclareWinner();
                break;
            default:
                Debug.LogError("Unexpected game state for turn progression.");
                break;
        }
    }

    private Player GetCurrentPlayer(GameState state)
    {
        switch (state)
        {
            case GameState.PlayerRTurn:
                return players[1]; // Assuming Player R is at index 1
            case GameState.MainPlayerTurn:
                return mainPlayer; // Main player is set in HandleAnte
            case GameState.PlayerLTurn:
                return players[2]; // Assuming Player L is at index 2
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleDealerTurn()
    {
        Debug.Log("Dealer's turn.");
        //Debug.Log($"{round} Round number");
        if (round == 1)
            dealer.DealFlop();
        if (round == 2)
            dealer.DealTurn();
        if (round == 3)
            dealer.DealRiver();
        //NextTurn();
        if (round < 4) // Check if there are more rounds
        {
            round++; // Move to the next round
            //Debug.Log($"{round} Round number");
            UpdateGameState(GameState.PlayerRTurn); // Reset for the next round
        }
        else // If it�s the last round, declare the winner
        {
            UpdateGameState(GameState.DeclareWinner);
        }
    }

    private void DeclareWinner()
    {
        float maxHandStrength = float.MinValue;
        Player winner = null;

        // Iterate through each player and find the player with the highest hand strength
        foreach (Player player in players)
        {
            if (player.handStrength > maxHandStrength)
            {
                maxHandStrength = player.handStrength;
                winner = player;
            }
        }
        Debug.Log($"Winner: {winner.name} with hand strength: {winner.handStrength}");

        // Add the total pot amount to the winner's funds
        winner.AddFunds(totalPot);
        anteText.text = $"{winner.name} wins";
        Debug.Log($"{winner.name} wins ${totalPot}. New balance: {winner.funds}");

        // Reset the game
        round = 1;
        foreach (Player player in players){
            if(player.gameObject.name == "Player L"){
                int i = 0;
                foreach (Card card in player.hand){
                    player.placeCard(card, -0.8389f+i*0.1f, 0.84f, -90f);
                    i++;
                    if(i==2){break;}
                }
            }

            if(player.gameObject.name == "Player R"){
                int i = 0;
                foreach (Card card in player.hand){
                    player.placeCard(card, 0.8389f-i*0.1f, 0.84f, -90f);
                    i++;
                    if(i==2){break;}
                }
            }
        }
        foreach (Player player in players)
        {
            player.ClearHand();
        }

        dealer.Flop.Clear();
        dealer.Turn = null;
        dealer.River = null;
        dealer.animator.SetBool("Winner", true);
    }

}

public enum GameState
{
    Ante,
    DealCards,
    PlayerRTurn,
    MainPlayerTurn,
    PlayerLTurn,
    DealerTurn,
    DeclareWinner
}
