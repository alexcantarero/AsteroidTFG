# AsteroidTFG - Imitation Learning AI 🚀🤖

Este repositorio contiene el código fuente y el entorno de simulación del Trabajo de Fin de Grado (TFG) titulado **"Desarrollo y entrenamiento de una inteligencia artificial con personalidad para jugar un videojuego de naves"**, desarrollado en la Universitat Politècnica de Catalunya (UPC - FIB).
El objetivo principal de este proyecto es explorar el uso de la Inteligencia Artificial mediante **Aprendizaje por Imitación (Imitation Learning)** y **Behavioral Cloning**. A través de este entorno, se entrena a un agente autónomo (NPC) para que sea capaz de adquirir y replicar fielmente el estilo de juego, la personalidad y las tácticas de un experto humano, superando la predictibilidad de las clásicas máquinas de estados finitos.

---

## 🌟 Características Principales

* **Entorno de Simulación 2D:** Un videojuego completamente funcional inspirado en el clásico *Asteroids*, desarrollado en Unity.
* **Mecánicas de Combate:** Implementación de físicas de movimiento, sistema de colisiones y dos tipos de disparo estratégicos:
  * *Disparo Primario:* Alta cadencia y largo alcance.
  * *Disparo Secundario:* Corto alcance, mayor cobertura y baja cadencia.
* **Inteligencia Artificial Orgánica:** Agentes entrenados mediante *Behavioral Cloning* que mimetizan la toma de decisiones humana frente a la aparición de obstáculos.
* **Perfiles de IA Entrenados:** El repositorio incluye los modelos `.ONNX` preentrenados con tres perfiles conductuales distintos:
  1. *Conservador:* Mantiene la posición central y utiliza el arma principal.
  2. *Agresivo (Primario):* Persigue a los asteroides de forma temeraria con el arma principal.
  3. *Agresivo (Secundario):* Asume riesgos extremos acercándose a los objetivos para usar el arma secundaria.

---

## 🛠️ Tecnologías y Requisitos

Para ejecutar o modificar este proyecto, necesitarás las siguientes herramientas:

* **Unity:** Motor gráfico utilizado para el desarrollo del entorno (se recomienda usar la misma versión con la que se desarrolló el proyecto).
* **C#:** Lenguaje de programación para la lógica del videojuego (scripts modulares: `GameManager`, `AsteroidBehavior`, `testingAgent`, etc.).
* **Unity ML-Agents Toolkit:** Plugin oficial de Unity para el entrenamiento de agentes.
* **Python 3.x:** Entorno necesario para la ejecución del motor de entrenamiento.
* **PyTorch:** Framework de Machine Learning utilizado por ML-Agents para ajustar los pesos de la red neuronal.

---

## ⚙️ Instalación y Configuración

1. **Clonar el repositorio:**
```bash
   git clone [https://github.com/alexcantarero/AsteroidTFG.git](https://github.com/alexcantarero/AsteroidTFG.git)
