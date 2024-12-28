# Texas Hold'em Poker Game

Welcome to the **Texas Hold'em Poker Game**! This game is designed using Unity and simulates the classic poker experience. Whether you're a beginner or an experienced player, dive in and test your skills.

---

## Features

- **Four Rounds of Play**: Enjoy the full Texas Hold'em experience, including:
  - Pre-Flop
  - Flop
  - Turn
  - River
- **Game Manager**: Handles all game states, player actions, and dealer responsibilities.
- **Interactive Gameplay**: Make decisions such as call, raise, fold, or check.
- **Realistic Graphics**: Playing cards and dealer animations for an immersive experience.
- **Multiplayer Mode**: Play with up to four players locally.

---

## Folder Structure

The project assets are organized as follows:

- **Assets/**
  - **Playing Cards/**
    - **Image/**: Contains raw images of all 52 cards.
    - **PlayingCards/**: Prefabs and scripts to render playing cards in the game.
  - **Scripts/**
    - **GameManager.cs**: Controls game flow and manages player and dealer actions.
    - **PlayerController.cs**: Handles player-specific actions.
    - **DealerController.cs**: Automates dealer tasks.
  - **UI/**
    - **Canvas/**: Handles user interface elements like buttons and HUD.
  - **Scenes/**
    - **MainMenu.unity**: The main menu scene.
    - **Game.unity**: The gameplay scene.

---

## How to Play

1. **Setup**:
   - Open the game in Unity.
   - Ensure all assets and scripts are imported correctly.
   - Run the `MainMenu.unity` scene.

2. **Gameplay**:
   - Select the number of players (2-4).
   - Follow the instructions displayed on the screen for each round:
     - **Pre-Flop**: Players are dealt two cards each.
     - **Flop**: Three community cards are revealed.
     - **Turn**: One additional community card is revealed.
     - **River**: The final community card is revealed.
   - Use the buttons to make your decisions (call, raise, fold, check).

3. **Winning**:
   - The player with the best five-card hand wins the pot at the end of the River.

---

## Controls

- **Mouse**: Navigate through menus and interact with game elements.
- **Buttons**:
  - **Call**: Match the current highest bet.
  - **Raise**: Increase the current bet.
  - **Fold**: Forfeit the round.
  - **Check**: Pass your turn without betting.

---

## Technical Details

- **Engine**: Unity
- **Programming Language**: C#
- **Unity Version**: Recommended 2021.3 LTS or later

---


