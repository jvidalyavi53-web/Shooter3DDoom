# 🎮 Shooter3D Doom

<div align="center">

**Videojuego de Disparos en Primera Persona (FPS) inspirado en DOOM**

Desarrollado con **Unity 6** (6000.4.9f1) · **C#** · **Universal Render Pipeline**

[![Unity](https://img.shields.io/badge/Unity-6000.4.9f1-000000?logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-10.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/Licencia-Educativo-blue)](#)

</div>

---

## 📋 Descripción

**Shooter3DDoom** es un videojuego FPS en primera persona inspirado en el clásico **DOOM (1993)** de id Software. El jugador se enfrenta a **10 oleadas progresivas** de enemigos en un escenario tridimensional, utilizando un arsenal de **3 armas** diferentes. El objetivo es sobrevivir y eliminar a todos los enemigos para lograr la victoria.

El juego combina mecánicas modernas de FPS con la estética retro de los shooters clásicos de los años 90, incluyendo enemigos tipo sprite con efecto billboard, sistema de puntuación con combos y rachas, y efectos visuales que emulan la experiencia original de DOOM.

---

## 🎯 Características Principales

- 🔫 **3 armas** con comportamientos únicos: Pistola, Escopeta y Ametralladora
- 👹 **6 tipos de enemigos**: Zombi, Demonio, Tanque, Suicide, Corredor y Congelador
- 🌊 **10 oleadas** progresivas de dificultad
- 💯 **Sistema de puntuación** con rachas y multiplicadores (hasta x5)
- 🎯 **Crosshair** dinámico con HUD completo
- 💥 **Feedback visual**: flash de daño, screen shake, head bob, efectos de impacto
- 🔊 **Efectos de sonido** retro estilo DOOM
- 💊 **Sistema de drops**: munición y botiquines al eliminar enemigos
- 🏗️ **Generación procedural**: enemigos y botiquines creados dinámicamente en código
- 🤖 **IA con NavMesh**: pathfinding inteligente con strafe lateral

---

## 🕹️ Controles

| Acción | Tecla |
|---|---|
| Mover | `W` `A` `S` `D` |
| Mirar | Ratón |
| Sprint | `Shift izquierdo` |
| Disparar | `Click izquierdo` (mantener para ametralladora) |
| Recargar | `R` |
| Pistola | `1` |
| Escopeta | `2` |
| Ametralladora | `3` |
| Reiniciar (Game Over) | `R` o `Click izquierdo` |

---

## 🔫 Arsenal

| Arma | Daño | Cadencia | Munición | Características |
|---|---|---|---|---|
| **Pistola** | 2 | 0.25s | 15 | Equilibrada, arma inicial |
| **Escopeta** | 3 × 5 pellets | 0.8s | 8 | Dispersión, alta potencia a corta distancia |
| **Ametralladora** | 1 | 0.1s | 30 | Alta cadencia de fuego |

---

## 👹 Tipos de Enemigos

| Tipo | Vida | Velocidad | Daño | Habilidad Especial |
|---|---|---|---|---|
| 🟠 **Zombi** | 3 | 3.0 | 1 | Enemigo básico |
| 🔴 **Demonio** | 5 | 5.0 | 2 | Más agresivo y rápido |
| 🟣 **Tanque** | 10 | 2.0 | 3 | Alta vida, resistente |
| 🟢 **Suicide** | 1 | 6.0 | 5 | Explota al contacto |
| 🔵 **Corredor** | 2 | 8.0 | 2 | Extremadamente rápido |
| 🧊 **Congelador** | 4 | 3.0 | 1 | Ralentiza al jugador al impactar |

---

## 🏗️ Arquitectura del Proyecto

```
Shooter3DDoom/
├── Assets/
│   ├── Scripts/           → 17 scripts C# (lógica del juego)
│   │   ├── GameManager.cs          → Control global del juego y oleadas
│   │   ├── ArmaManager.cs          → Sistema de armas (pistola, escopeta, ametralladora)
│   │   ├── EnemigoIA.cs            → Inteligencia artificial de enemigos
│   │   ├── TipoEnemigo.cs          → Definición de 6 tipos de enemigos
│   │   ├── GeneradorEnemigos.cs    → Spawner procedural de enemigos
│   │   ├── PrimeraPersona.cs       → Controlador FPS del jugador
│   │   ├── Vida.cs                 → Sistema de vida y muerte
│   │   ├── UIManager.cs            → Interfaz de usuario completa (HUD)
│   │   ├── SonidoManager.cs        → Gestor centralizado de audio
│   │   ├── DanoFeedback.cs         → Flash rojo de daño en pantalla
│   │   ├── GeneradorBotiquines.cs  → Spawner de botiquines de salud
│   │   ├── Botiquin.cs             → Lógica de recolección de botiquines
│   │   ├── DropItem.cs             → Ítems dropeados por enemigos
│   │   ├── Billboard.cs            → Sprites que miran siempre a la cámara
│   │   ├── SetupJugador.cs         → Configuración automática del jugador
│   │   ├── Meta.cs                 → Zona de completar nivel
│   │   └── Disparar.cs             → Script legacy (deprecado)
│   ├── Scenes/            → Escenas del juego
│   ├── Sprites/           → Sprites 2D (enemigo, arma, botiquín, muzzle)
│   ├── Sonidos/           → 7 efectos de audio WAV
│   ├── Materiales/        → Materiales del entorno
│   ├── Texturas/          → Texturas del escenario
│   └── Resources/         → Recursos cargados en runtime
├── Docs/
│   └── Informe_Shooter3DDoom.md  → 📄 Informe técnico completo
├── Packages/              → Paquetes de Unity
└── ProjectSettings/       → Configuración del proyecto
```

---

## 📄 Documentación

El informe técnico completo del proyecto se encuentra en:

> 📂 **[`Docs/Informe_Shooter3DDoom.md`](Docs/Informe_Shooter3DDoom.md)**

Este informe incluye:
- Descripción detallada de los **17 scripts** con sus funcionalidades
- **Diagramas de arquitectura** del sistema
- **Flujo del juego** desde inicio hasta victoria/derrota
- Tabla completa de **mecánicas de juego** (armas, enemigos, oleadas, puntuación)
- **Métricas del código** (~2,335 líneas de C#)
- **Recursos gráficos y de audio** utilizados
- **Patrones de diseño** implementados (Singleton, Observer, Auto-creación)

---

## ⚙️ Requisitos

| Requisito | Especificación |
|---|---|
| **Motor** | Unity 6 (6000.4.9f1) o superior |
| **Render Pipeline** | Universal Render Pipeline (URP) |
| **Sistema Operativo** | Windows 10/11 |
| **Dependencias** | NavMesh, Unity UI, Input System |

---

## 🚀 Instalación y Ejecución

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/jvidalyavi53-web/Shooter3DDoom.git
   ```

2. **Abrir con Unity Hub:**
   - Abrir Unity Hub
   - Click en "Add" → Seleccionar la carpeta `Shooter3DDoom`
   - Abrir con Unity 6 (6000.4.9f1)

3. **Ejecutar el juego:**
   - Abrir la escena `Assets/Scenes/SampleScene`
   - Presionar el botón **Play** ▶️ en el editor

> **Nota:** No se requiere configuración manual. Todos los sistemas del juego se crean automáticamente al iniciar gracias al sistema de auto-creación en runtime.

---

## 🎨 Patrones de Diseño

| Patrón | Uso |
|---|---|
| **Singleton** | `GameManager`, `UIManager`, `SonidoManager` — instancia única global |
| **Observer (Eventos)** | 9 eventos C# para comunicación desacoplada entre sistemas |
| **Auto-Creación** | `[RuntimeInitializeOnLoadMethod]` para setup automático sin configuración manual |
| **Generación Procedural** | Enemigos y botiquines creados completamente en código |

---

## 📊 Métricas del Proyecto

| Métrica | Valor |
|---|---|
| Scripts C# | 17 |
| Líneas de código | ~2,335 |
| Tipos de enemigos | 6 |
| Armas del jugador | 3 |
| Oleadas (waves) | 10 |
| Efectos de sonido | 7 |
| Sprites | 4 |
| Eventos del sistema | 9 |

---

## 🛠️ Tecnologías Utilizadas

- **Unity 6** (6000.4.9f1) — Motor de videojuegos
- **C# 10** — Lenguaje de programación
- **Universal Render Pipeline (URP)** — Pipeline de renderizado
- **NavMesh** — Sistema de navegación para IA de enemigos
- **Unity UI** — Sistema de interfaz de usuario
- **Input System** — Sistema de entrada

---

<div align="center">

Desarrollado como proyecto educativo inspirado en **DOOM (1993)** de id Software.

</div>
