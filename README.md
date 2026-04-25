# RNG Slot Game (Unity WebGL)

## Game Overview

This is a 2D slot machine game built using Unity.
The player starts with a fixed amount of money and must reach a target amount to win.

The core gameplay revolves around spinning three reels:

* Match all 3 symbols → Win money
* Match 2 symbols → Receive an item (with partial loss)
* No match → Lose the bet amount

The game introduces strategy through items that influence outcomes.

---

## Gameplay Features

* Smooth reel spinning system
* Randomized symbol generation using RNG
* Betting system with adjustable bet values
* Strategy-based item system
* Win condition (reach target money)
* Lose condition (money reaches zero)

---

## Items System

Players can use a limited number of items per round:

* Peek – Reveals upcoming symbols
* Lock – Locks one reel to a chosen symbol
* Re-roll – Re-spin a selected reel
* Double – Doubles win or loss
* Insurance – Prevents loss for one round

---

## Controls

* Spin Button → Start spinning reels
* + / - Buttons → Adjust bet amount
* Item Buttons → Activate special abilities

---

## How to Run (WebGL Build)

WebGL builds cannot be opened directly via double-click.

1. Open the `Build/WebGL` folder in VS Code
2. Install the Live Server extension
3. Right-click `index.html` → Open with Live Server

---

## Bonus Features

* Item-based gameplay mechanics (adds strategy)
* Hover-based UI feedback for buttons
* Individual reel control system with looping logic
* Re-roll system affecting only selected reel
* Dynamic UI updates (money, bet, predictions)

---

## Thought Process / Approach

The goal was to build a clean and functional slot machine system while introducing strategic depth.

### Key Design Decisions:

* Reel System
  Implemented using RectTransform movement with manual looping logic for continuous spinning

* RNG System
  Used Unity’s Random.Range() for fair and unpredictable outcomes

* Item Mechanics
  Designed to balance risk vs reward, with limited usage per round

* Code Structure
  SlotSpinScript → Game logic and flow
  ReelScript → Reel animation and positioning
  ButtonScript → UI interaction and feedback

* Game Feel
  Staggered reel stopping using coroutines and visual UI feedback

---

## Project Structure

Assets/
├── Scripts/
├── Sprites/
├── UI/
├── Prefabs/
└── Scenes/

---

## Future Improvements

* Add sound effects
* Improve reel animation with easing
* Add more item variations
* Add difficulty scaling

---

## Author

Anik Hussain Chatterjee
Unity Developer Intern Applicant
