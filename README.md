# AgenteML (Imitation & Reinforcement Learning con Unity ML-Agents)

Proyecto de ejemplo en **Unity 6.2 (6000.2.1f1)** donde un agente:

1. se desplaza por un área,
2. **presiona un botón** para generar comida,
3. **come** la comida y **termina el episodio**.

Se entrena con **ML-Agents** desde Python (Anaconda) y se registra con **TensorBoard**.

---

## ✨ Características

* Escena lista para **entrenamiento en el Editor** y para **inferencia** con modelo `.onnx`.
* Spawneo **aleatorio por entorno** usando un `SpawnArea` local (evita que varios entornos se crucen).
* Evita **solapes** al crear comida (no aparece encima del agente ni dentro de paredes/botón).
* Heurística con **New Input System** (WASD + Space) para pruebas rápidas.
* **YAMLs** de ejemplo: `AgenteComida.yaml` y `MovimientoAMeta.yaml`.

---

## 🧰 Versiones probadas

| Componente                              | Versión                                |
| --------------------------------------- | -------------------------------------- |
| **Unity Editor**                        | **6.2 (6000.2.1f1)**                   |
| **Paquete Unity** `com.unity.ml-agents` | **3.0.0**                              |
| **Python**                              | **3.10.12** (Anaconda)                 |
| **PyTorch**                             | **2.2.1 + cu121**                      |
| **Trainer (CLI)** `mlagents-learn`      | **1.1.0** (así se ejecutó en los logs) |

---

## 📦 Estructura (resumen)

```
AgenteML/
├─ Assets/
│  ├─ Scenes/
│  ├─ Scripts/
│  │  └─ Imitation Learning/
│  │     ├─ AgenteComida.cs
│  │     ├─ BotonComida.cs
│  │     ├─ ComidaAparicion.cs
│  │     └─ SpawnArea.cs
│  └─ Prefabs/
├─ config/
│  ├─ AgenteComida.yaml
│  └─ MovimientoAMeta.yaml
└─ results/
   └─ (se crean al entrenar)
```

---

## 🚀 Instalación rápida (para cualquiera que clone este repo)

### 0) Requisitos

* Windows 10/11
* **Unity 6.2 (6000.2.1f1)** instalado (Unity Hub)
* **Anaconda/Miniconda**
* (Opcional) GPU NVIDIA con CUDA 12.1 para acelerar PyTorch

### 1) Clonar y abrir en Unity

```powershell
git clone <URL_DE_TU_REPO> AgenteML
```

* Abre **Unity Hub** → **Open** → selecciona la carpeta `AgenteML`.
* En **Package Manager**, asegúrate de tener **ML-Agents** `3.0.0`.

### 2) Crear entorno Python (Anaconda)

```powershell
conda create -n mlagents python=3.10.12 -y
conda activate mlagents
```

### 3) Instalar PyTorch (CUDA 12.1)

> Si no tienes GPU compatible, salta a la variante CPU más abajo.

```powershell
pip install torch==2.2.1 --index-url https://download.pytorch.org/whl/cu121
```

### 4) Instalar ML-Agents (versión como la que se ejecutó en tus logs)

```powershell
pip install ^
  mlagents==1.1.0 ^
  gym==0.26.2 ^
  pettingzoo==1.15.0 ^
  protobuf==3.20.3 ^
  grpcio==1.48.2 ^
  numpy==1.23.5 ^
  pillow==11.3.0 ^
  pyyaml==6.0.2 ^
  cloudpickle==3.1.1
```

> **Variante CPU (sin CUDA):**
>
> ```powershell
> pip install torch==2.2.1 --index-url https://download.pytorch.org/whl/cpu
> ```
>
> *(Luego instala ML-Agents igual que arriba.)*

---

## 🧪 Configuración mínima en la escena

En el **Agent** (componente **Behavior Parameters**):

* **Behavior Name**: `AgenteComida` (o el que uses en el YAML).
* **Observation** (Vector): 6 (según el script).
* **Actions**: **Discrete** con **3 ramas** → tamaños **\[3, 3, 2]**.

  * Rama 0: X (quieto/izq/der)
  * Rama 1: Z (quieto/atrás/adelante)
  * Rama 2: Botón (no/sí)
* **Behavior Type**: **Default** para entrenar; **Inference Only** para jugar con el `.onnx`.

**Botón**:

* `BoxCollider` con **Is Trigger = ON**.
* Script `BotonComida.cs` configurado con **Renderer** + **Materiales**.

**Comida**:

* Prefab con `Collider` (no trigger).

**SpawnArea**:

* Hijo del Environment con `BoxCollider` (Is Trigger ON) que **cubra la plataforma**.
* Script `SpawnArea.cs` con `margin` 0.3–0.5 (a gusto).

**FoodSpawnEnvironment**:

* Script que **resetea** y **reubica** botón/comida en cada episodio (autocablea a hijos del Environment).

---

## 🏃‍♂️ Entrenamiento (comandos reales que usaste)

Abre PowerShell:

```powershell
(base) PS C:\Users\Gigabyte> conda activate mlagents
(mlagents) PS C:\Users\Gigabyte> cd C:\Users\Gigabyte\Unity\AgenteML
```

### Entrenar **AgenteComida**

```powershell
(mlagents) PS C:\Users\Gigabyte\Unity\AgenteML> mlagents-learn .\config\AgenteComida.yaml --run-id=AgenteComida
```

* Espera: `Listening on port 5004. Start training by pressing the Play button in the Unity Editor.`
* Luego **Play** en Unity.

Export típico al detener:

* `results\AgenteComida\AgenteComida\AgenteComida-86911.onnx`
* Copia: `results\AgenteComida\AgenteComida.onnx`

### Entrenar **MovimientoAMeta**

```powershell
(mlagents) PS C:\Users\Gigabyte\Unity\AgenteML> mlagents-learn ".\config\MovimientoAMeta.yaml" --run-id=AgenteML
```

Export típico:

* `results\AgenteML\MovimientoAMeta\MovimientoAMeta-61618.onnx`
* Copia: `results\AgenteML\MovimientoAMeta.onnx`

> **Mensajes normales:**
> `pkg_resources is deprecated…` (info de setuptools)
> `Restarting worker[0] after 'Communicator has exited.'` (al parar Play / reiniciar; el trainer vuelve a escuchar solo).

---

## 📊 TensorBoard

```powershell
(mlagents) PS C:\Users\Gigabyte\Unity\AgenteML> tensorboard --logdir "C:\Users\Gigabyte\Unity\AgenteML\results"
```

Abre: `http://localhost:6006/`
El aviso `TensorFlow installation not found` es informativo; verás igualmente los **scalars** del entrenamiento.

---

## 🧠 Inferencia: usar el modelo en Unity

1. Toma el `.onnx` copiado (por ejemplo `results\AgenteComida\AgenteComida.onnx`).
2. Arrástralo a Unity (Assets).
3. En el **Agent → Behavior Parameters**:

   * **Model**: asigna el `.onnx`.
   * **Behavior Type**: **Inference Only**.
4. **Play** para ver al agente ejecutar la política.

---

## 🔄 Variantes de instalación

### A) Usar **mlagents==2.1.0** (a pedido)

Si quieres documentar o probar con esa versión:

```powershell
conda activate mlagents
pip install torch==2.2.1 --index-url https://download.pytorch.org/whl/cu121

# mlagents 2.1.0 + posibles deps (ajusta si pip te lo pide)
pip install mlagents==2.1.0 pyyaml pillow cloudpickle
```

> **Importante:** la compatibilidad exacta entre `mlagents (python)` y el paquete Unity `com.unity.ml-agents` depende del **Communicator API**.
> Si usas 2.1.0 y notas errores de conexión, vuelve a `mlagents==1.1.0` (que es lo que mostró tu log) o alinea la versión del paquete Unity con la recomendada por 2.1.0.

### B) PyTorch **CPU** (sin CUDA)

```powershell
pip install torch==2.2.1 --index-url https://download.pytorch.org/whl/cpu
```

---

## 🧩 Consejos / Problemas frecuentes

* **“Couldn’t connect to trainer… Will perform inference instead”**
  Lanza `mlagents-learn` **antes** de presionar **Play** en Unity.

* **Acciones fuera de rango / IndexOutOfRange**
  Asegura **3 ramas** discretas con tamaños **\[3, 3, 2]**.

* **Todo aparece en el mismo sitio**
  Amplía el `BoxCollider` de **SpawnArea** para cubrir la plataforma (ver gizmo cian).

* **La comida “no aparece”**
  En realidad se genera y se come en el mismo frame. Usa la versión de `ComidaAparicion` con **anti-overlap** (distancia mínima al agente y `LayerMask` de obstáculos/botón).

* **New Input System**
  En Heuristic, usa `UnityEngine.InputSystem` (`Keyboard.current`). No mezcles con `UnityEngine.Input`.

---

