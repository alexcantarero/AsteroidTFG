# AsteroidTFG - Imitation Learning AI 🚀🤖

This repository contains the source code and simulation environment for the Bachelor's Thesis (TFG) titled **"Development and training of a personality-driven artificial intelligence to play a spaceship video game"**, developed at the Universitat Politècnica de Catalunya (UPC - FIB).
The main objective of this project is to explore the use of Artificial Intelligence through **Imitation Learning** and **Behavioral Cloning**. Through this environment, an autonomous agent (NPC) is trained to accurately acquire and replicate the playstyle, personality, and tactics of a human expert, overcoming the predictability of classic finite state machines.

---

## 🌟 Main Features

* **2D Simulation Environment:** A fully functional video game inspired by the classic *Asteroids*, developed in Unity.
* **Combat Mechanics:** Implementation of movement physics, a collision system, and two types of strategic firing:
  * *Primary Fire:* High fire rate and long range.
  * *Secondary Fire:* Short range, wider spread, and low fire rate.
* **Organic Artificial Intelligence:** Agents trained via *Behavioral Cloning* that mimic human decision-making when facing obstacles.
* **Trained AI Profiles:** The repository includes pre-trained `.ONNX` models with three distinct behavioral profiles:
  1. *Conservative:* Holds the center position and uses the main weapon.
  2. *Aggressive (Primary):* Recklessly chases asteroids using the main weapon.
  3. *Aggressive (Secondary):* Takes extreme risks by getting close to targets to use the secondary weapon.

---

## 🛠️ Technologies and Requirements

To run or modify this project, you will need the following tools:

* **Unity:** Game engine used to develop the environment (it is recommended to use the same version the project was developed with).
* **C#:** Programming language for the game logic (modular scripts: `GameManager`, `AsteroidBehavior`, `testingAgent`, etc.).
* **Unity ML-Agents Toolkit:** Official Unity plugin for agent training.
* **Python 3.x:** Required environment to run the training engine.
* **PyTorch:** Machine Learning framework used by ML-Agents to adjust the neural network weights.

---

## ⚙️ Installation 

1. **Clone the repository:**
```bash
   git clone [https://github.com/alexcantarero/AsteroidTFG.git](https://github.com/alexcantarero/AsteroidTFG.git)
```
