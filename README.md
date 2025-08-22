# Juego ML-Agents (Unity 6)

Proyecto de ejemplo con **dos entornos** en Unity 6:

* **Imitation Learning (AgenteComida)**: el agente navega hasta un botón, lo pulsa y recoge comida generada aleatoriamente.
* **Reinforcement Learning (MovimientoAMeta)**: el agente aprende a moverse hasta una meta.

Incluye las escenas, prefabs, scripts, configuraciones `.yaml` y guía paso a paso para instalar dependencias, entrenar con **ml-agents 1.1.0** y visualizar métricas en **TensorBoard**.

---

## Requisitos

* **Unity**: 6.2 (6000.2.1f1) o compatible.
* **Paquetes Unity** (el proyecto ya los referencia; Unity los resolverá al abrir):

  * `com.unity.ml-agents` (2.x/3.x; probado con 2.0.1).
  * `com.unity.ai.inference` (AI Inference / Sentis) para usar modelos ONNX en el Editor/Player.
  * **Input System** activo: *Edit → Project Settings → Player → Active Input Handling → Input System Package*.
* **Python** (vía Anaconda/Miniconda): 3.10.12
* **GPU opcional**: si usas CUDA 12.1, instalaremos PyTorch 2.2.1+cu121; si no tienes GPU, ver sección “CPU”.

> **Nota**: El proyecto ya trae la estructura de Assets/Packages/ProjectSettings. No edites `Packages/manifest.json` manualmente; usa el **Package Manager**.

---

## Estructura del proyecto

```
Assets/
  Materials/
  Prefabs/
    Comida.prefab
  Scenes/
    AgenteML (Imitation Learning).unity
    AgenteML (Reinforcement Learning).unity
  Scripts/
    Imitation Learning/
      AgenteComida.cs
      BotonComida.cs
      Comida.cs
      ComidaAparicion.cs
      AparicionComidaEscenario.cs
    Reinforcement Learning/
      MovimientoAMeta.cs
      Meta.cs
      Pared.cs
config/
  AgenteComida.yaml
  MovimientoAMeta.yaml
```

---

## Instalación (Conda) — **Windows (PowerShell o Git Bash)**

> Si no tienes conda, instala **Miniconda**.

1. **Crear y activar el entorno**

```powershell
conda create -n mlagents python=3.10.12 -y
conda activate mlagents
```

2. **Instalar PyTorch 2.2.1 (CUDA 12.1)**

> Si no tienes GPU NVIDIA/CUDA 12.1, salta a la sección **CPU**.

```powershell
pip install torch==2.2.1 --index-url https://download.pytorch.org/whl/cu121
```

3. **Instalar ML-Agents 1.1.0 y utilidades**

```powershell
pip install mlagents==1.1.0 tensorboard
```

> ML-Agents 1.1.0 trae `mlagents-learn`. Internamente arrastrará dependencias compatibles (gym 0.26.2, PettingZoo 1.15.0, grpcio 1.48.2, protobuf 3.20.3, numpy 1.23.5, etc).
> Si tu red bloquea PyPI, asegúrate de tener acceso a `pypi.org` y `download.pytorch.org`.

### Alternativa **CPU** (sin GPU)

```powershell
pip install torch==2.2.1 --index-url https://download.pytorch.org/whl/cpu
pip install mlagents==1.1.0 tensorboard
```

---

## Clonar y abrir en Unity

```bash
git clone https://github.com/JGlJosue/Juego_ML_Agents.git
cd Juego_ML_Agents
```

* Abre la carpeta del repo con **Unity Hub** (Unity 6.2).
* Unity resolverá los paquetes. Verifica en *Window → Package Manager* que:

  * **ML-Agents** esté instalado.
  * **AI Inference** (com.unity.ai.inference) esté instalado (si no aparece, usa “Install package by name” e ingresa `com.unity.ai.inference`).
* Abre alguna escena:

  * `Assets/Scenes/AgenteML (Imitation Learning).unity`
  * `Assets/Scenes/AgenteML (Reinforcement Learning).unity`

> **Input System**: El proyecto usa `UnityEngine.InputSystem` (WASD + Space en heurística). Asegúrate de tener **Input System** activo.

---

## Entrenamiento

Abre una terminal con el entorno conda activo y sitúate en la raíz del proyecto:

```powershell
conda activate mlagents
cd C:\Users\Gigabyte\Unity\AgenteML   # ajusta a tu ruta local
```

### 1) Imitation Learning – AgenteComida

```powershell
mlagents-learn .\config\AgenteComida.yaml --run-id=AgenteComida
```

* Espera el mensaje:

  ```
  [INFO] Listening on port 5004. Start training by pressing the Play button in the Unity Editor.
  ```
* Vuelve a Unity y pulsa **Play**.
* Durante el entrenamiento verás logs tipo:

  ```
  [INFO] AgenteComida. Step: 20000 ... Mean Reward: 0.234 ...
  ```
* Al parar el entrenamiento, se exportará el modelo:

  ```
  results\AgenteComida\AgenteComida\AgenteComida-XXXX.onnx
  Copied to results\AgenteComida\AgenteComida.onnx
  ```

### 2) Reinforcement Learning – MovimientoAMeta

```powershell
mlagents-learn ".\config\MovimientoAMeta.yaml" --run-id=AgenteML
```

* Igual que antes: pulsa **Play** en Unity para conectar con el trainer.

---

## Ver métricas en TensorBoard

Con el entorno activado:

```powershell
tensorboard --logdir ".\results"
```

Abre la URL que imprime (por defecto [http://localhost:6006/](http://localhost:6006/)) y selecciona tu `run-id` (por ejemplo, **AgenteComida** o **AgenteML**).

---

## Usar el modelo ONNX en el juego

1. Copia el `.onnx` desde `results\<run-id>\` a alguna carpeta dentro de `Assets/` (por ejemplo `Assets/Models/`).
2. Selecciona el objeto **Agent** en la escena → componente **Behavior Parameters**:

   * **Behavior Type**: *Inference Only*
   * **Model**: arrastra tu `.onnx`
3. Pulsa **Play** para ver la inferencia local.

> En Unity 6, la ejecución ONNX la realiza **AI Inference** (Sentis). Si el modelo no se asigna, verifica que el paquete esté instalado.

---

## Controles (modo Heurístico)

* **W/A/S/D**: mover al agente
* **Space**: accionar botón

---

## Notas importantes / Troubleshooting

* **“Couldn’t connect to trainer… Will perform inference instead.”**
  Lanza `mlagents-learn` antes de pulsar Play en Unity. Si ya está en uso el puerto 5004, usa por ejemplo `--base-port=5006` y en Unity vuelve a darle Play.
* **“API version mismatch”**
  Usa **ml-agents 1.1.0** (Python) y el paquete ML-Agents que trae el proyecto (Unity).
* **Input System**
  Si ves `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but ...`, activa *Input System* en Project Settings.
* **Colisiones**

  * El **Botón** debe tener **BoxCollider** (no “IsTrigger”), y el script `BotonComida` con el *Pivot* (sub-objeto) asignado si usas movimiento vertical visual.
  * La **Comida** (prefab) debe tener **Collider** + script `Comida`; la recogida llama a `EndEpisode()` en `AgenteComida`.
* **CUDA**
  Si no tienes CUDA 12.1, instala la variante **CPU** de PyTorch (ver arriba).

---

## Comandos usados en este proyecto (historial verificado)

```powershell
# Crear/activar entorno
conda create -n mlagents python=3.10.12 -y
conda activate mlagents

# PyTorch 2.2.1 con CUDA 12.1
pip install torch==2.2.1 --index-url https://download.pytorch.org/whl/cu121
# (opción CPU)
# pip install torch==2.2.1 --index-url https://download.pytorch.org/whl/cpu

# ML-Agents + TensorBoard
pip install mlagents==1.1.0 tensorboard

# Entrenar (desde la raíz del repo)
mlagents-learn .\config\AgenteComida.yaml --run-id=AgenteComida
mlagents-learn ".\config\MovimientoAMeta.yaml" --run-id=AgenteML

# Visualizar métricas
tensorboard --logdir ".\results"
```

---

## Licencia

MIT (o la que prefieras para tu repo).

---

## Agradecimientos

* Unity ML-Agents
* Comunidad de aprendizaje por refuerzo

---

## Arreglo a posibles Fallos

En caso de que la instalacion con anaconda genere problemas con las versiones entre paquetes usa los siguientes comandos y continua con la configuracion del entorno:
conda activate mlagents
python -m pip install --upgrade pip wheel setuptools

# Limpia por si quedó algo de la instalación local
pip uninstall -y mlagents mlagents-envs gym pettingzoo protobuf grpcio numpy

# INSTALACIÓN (UNA SOLA LÍNEA)
pip install mlagents==1.1.0 mlagents-envs==1.1.0 gym==0.26.2 pettingzoo==1.15.0 protobuf==3.20.3 grpcio==1.48.2 numpy==1.23.5

---
