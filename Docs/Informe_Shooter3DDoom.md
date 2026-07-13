# INFORME TÉCNICO DEL PROYECTO

# **Shooter3D Doom**

---

## Videojuego de Disparos en Primera Persona (FPS) desarrollado en Unity

**Repositorio:** [https://github.com/jvidalyavi53-web/Shooter3DDoom](https://github.com/jvidalyavi53-web/Shooter3DDoom)

---

## INFORMACIÓN GENERAL

| Campo | Detalle |
|---|---|
| **Nombre del Proyecto** | Shooter3DDoom |
| **Motor de Desarrollo** | Unity 6 (6000.4.9f1) |
| **Lenguaje de Programación** | C# |
| **Render Pipeline** | Universal Render Pipeline (URP) |
| **Plataforma Objetivo** | PC (Windows) |
| **Género** | First Person Shooter (FPS) |
| **Inspiración** | DOOM (id Software, 1993) |

---

## 1. DESCRIPCIÓN DEL PROYECTO

**Shooter3DDoom** es un videojuego de disparos en primera persona (FPS) desarrollado en Unity 6, inspirado en el clásico DOOM de 1993. El jugador se enfrenta a oleadas progresivas de enemigos en un escenario tridimensional, utilizando un arsenal de tres armas diferentes. El objetivo es sobrevivir y eliminar a todos los enemigos a lo largo de 10 oleadas (waves) para lograr la victoria.

El juego combina mecánicas modernas de FPS con la estética retro de los shooters clásicos de los años 90, incluyendo enemigos tipo sprite con efecto billboard, sistema de puntuación con combos y rachas, y efectos visuales que emulan la experiencia original de DOOM.

---

## 2. ARQUITECTURA DEL PROYECTO

### 2.1 Estructura de Carpetas

```
Shooter3DDoom/
├── Assets/
│   ├── Scripts/           → 17 scripts C# (lógica del juego)
│   ├── Scenes/            → Escenas del juego (SampleScene)
│   ├── Materiales/        → Materiales del entorno 3D
│   ├── Texturas/          → Texturas del escenario
│   ├── Sprites/           → Sprites 2D (arma, enemigo, botiquín, muzzle)
│   ├── Sonidos/           → 7 efectos de audio (.wav)
│   ├── Resources/         → Recursos cargados dinámicamente
│   ├── Prefabs/           → Prefabs del juego
│   ├── Settings/          → Configuración de render URP
│   └── InputSystem_Actions → Configuración del sistema de input
├── Docs/                  → Documentación del proyecto
├── Packages/              → Paquetes de Unity
├── ProjectSettings/       → Configuración del proyecto
└── .gitignore             → Configuración de control de versiones
```

### 2.2 Patrón de Diseño

El proyecto utiliza los siguientes patrones de diseño:

- **Singleton Pattern**: Empleado en `GameManager`, `UIManager` y `SonidoManager` para garantizar una única instancia global accesible desde cualquier script.
- **Observer Pattern (Eventos)**: Se utiliza el sistema de eventos de C# (`System.Action`) para la comunicación desacoplada entre sistemas (vida, puntuación, oleadas, munición, etc.).
- **Auto-Creación en Runtime**: Los sistemas principales (`GameManager`, `UIManager`, `SonidoManager`, `GeneradorEnemigos`, `GeneradorBotiquines`) se crean automáticamente mediante el atributo `[RuntimeInitializeOnLoadMethod]`, eliminando la necesidad de configuración manual en el editor.

---

## 3. SCRIPTS DEL PROYECTO — DESCRIPCIÓN DETALLADA

El proyecto contiene **17 scripts en C#** organizados por responsabilidad:

---

### 3.1 GameManager.cs — Controlador Principal del Juego

| Característica | Detalle |
|---|---|
| **Líneas de código** | 261 |
| **Patrón** | Singleton + DontDestroyOnLoad |
| **Responsabilidad** | Control del estado global del juego |

**Funcionalidades:**

- Gestiona el estado global del juego (en curso, victoria, derrota).
- Implementa el **sistema de oleadas (waves)**: 10 oleadas progresivas con transiciones animadas.
- Controla el **sistema de puntuación** con multiplicadores por rachas de kills (hasta x5).
- Maneja el **sistema de rachas (combos)**: ventana de 3 segundos para mantener la racha activa.
- Detecta condiciones de **victoria** (completar todas las oleadas) y **derrota** (vida del jugador a 0).
- Se recrea automáticamente al cargar escenas.
- Pausa el juego (`Time.timeScale = 0`) al terminar y permite reiniciar con la tecla **R**.

**Eventos emitidos:**
- `OnWaveCambiada(int waveActual, int totalWaves)`
- `OnPuntuacionCambiada(int puntos)`
- `OnRachaCambiada(int racha)`
- `OnMensajeWave(string mensaje)`

---

### 3.2 ArmaManager.cs — Sistema de Armas

| Característica | Detalle |
|---|---|
| **Líneas de código** | 237 |
| **Responsabilidad** | Gestión del arsenal completo del jugador |

**Arsenal disponible:**

| Arma | Daño | Cadencia | Munición | Retroceso | Notas |
|---|---|---|---|---|---|
| **Pistola** | 2 | 0.25s | 15 | 1.5 | Arma inicial, equilibrada |
| **Escopeta** | 3 × 5 pellets | 0.8s | 8 | 4.0 | Dispersión por pellet, alta potencia a corta distancia |
| **Ametralladora** | 1 | 0.1s | 30 | 0.8 | Alta cadencia de fuego, bajo daño por disparo |

**Funcionalidades:**

- Cambio de armas con teclas **1**, **2**, **3**.
- Sistema de **disparo por Raycast** con detección de impactos.
- **Retroceso visual** en la cámara (`Quaternion.Euler`) con suavizado por interpolación.
- **Muzzle flash** (destello del cañón) al disparar.
- Sistema de **recarga manual** con tecla **R** y **auto-recarga** al vaciar el cargador.
- **Efecto de impacto** visual (esfera dorada efímera) al impactar superficies.
- **Flash de impacto** en enemigos (cambio de color a blanco brevemente).
- Sonido diferenciado de clic vacío al intentar disparar sin munición.

**Eventos emitidos:**
- `OnMunicionCambiada(int actuales, int maximas)`
- `OnEstadoRecargaCambiado(string estado)`
- `OnArmaCambiada(string nombreArma)`

---

### 3.3 EnemigoIA.cs — Inteligencia Artificial de los Enemigos

| Característica | Detalle |
|---|---|
| **Líneas de código** | 256 |
| **Responsabilidad** | Comportamiento autónomo de los enemigos |

**Sistema de IA:**

- **Detección del jugador** por distancia configurable.
- **Navegación con NavMesh** (Unity NavMeshAgent) para pathfinding inteligente con fallback a movimiento directo.
- **Strafe lateral** (esquive como en DOOM): los enemigos se mueven lateralmente cuando están en rango de ataque, cambiando dirección aleatoriamente.
- **Sistema de ataque a distancia** con Raycast incluyendo:
  - Probabilidad de fallo configurable (aumenta con la distancia).
  - Línea de visión mediante `SphereCast`.
  - Fallback a ataque cuerpo a cuerpo (melee) a distancia ≤ 3 unidades.
- **Comportamiento suicida**: enemigos tipo Suicide persiguen directamente al jugador y explotan al alcanzarlo con daño escalado por distancia.
- **Efecto Congelador**: ciertos enemigos aplican ralentización al jugador al impactar.
- **Periodo de gracia** al spawnear (0.5s sin atacar).
- **Feedback visual** de disparo (flash blanco al atacar).

---

### 3.4 TipoEnemigo.cs — Definición de Tipos de Enemigos

| Característica | Detalle |
|---|---|
| **Líneas de código** | 103 |
| **Responsabilidad** | Datos de configuración para cada variante de enemigo |

**Tipos de enemigos definidos:**

| Tipo | Vida | Velocidad | Daño | Fallo % | Dist. Ataque | Color | Especial |
|---|---|---|---|---|---|---|---|
| **Zombi** | 3 | 3.0 | 1 | 20% | 10 | Naranja oscuro | Enemigo básico |
| **Demonio** | 5 | 5.0 | 2 | 15% | 12 | Rojo intenso | Más agresivo y rápido |
| **Tanque** | 10 | 2.0 | 3 | 10% | 10 | Púrpura | Alta vida, lento |
| **Suicide** | 1 | 6.0 | 5 | 0% | 2 | Verde | Explota al contacto |
| **Corredor** | 2 | 8.0 | 2 | 25% | 3 | Cyan | Muy rápido, cuerpo a cuerpo |
| **Congelador** | 4 | 3.0 | 1 | 10% | 15 | Azul hielo | Ralentiza al jugador |

---

### 3.5 GeneradorEnemigos.cs — Spawner de Enemigos

| Característica | Detalle |
|---|---|
| **Líneas de código** | 247 |
| **Responsabilidad** | Generación procedural de enemigos |

**Funcionalidades:**

- Generación automática de enemigos dentro de los límites del mapa (-22 a +22 en X y Z).
- **Radio de seguridad** alrededor del jugador (12 unidades iniciales, 10 después) para evitar spawns injustos.
- Verificación post-spawn: destruye enemigos que aparezcan demasiado cerca (< 8 unidades).
- Validación de posición de spawn mediante Raycast (verificar suelo con normal adecuada).
- Hasta 30 intentos para encontrar posición válida.
- Creación dinámica completa del GameObject enemigo en código, incluyendo:
  - SpriteRenderer con sprite teñido según tipo de enemigo.
  - Billboard (mirar siempre a la cámara).
  - NavMeshAgent para pathfinding.
  - Componentes Vida y EnemigoIA configurados.
  - BoxCollider para detección de daño.
  - AudioSource para efectos de sonido.
- Máximo de 9 enemigos totales por wave.

---

### 3.6 PrimeraPersona.cs — Controlador del Jugador

| Característica | Detalle |
|---|---|
| **Líneas de código** | 153 |
| **Responsabilidad** | Movimiento y cámara en primera persona |

**Funcionalidades:**

- **Movimiento WASD** con velocidad normal (10) y sprint con **Shift** (14).
- **Rotación del ratón** con sensibilidad configurable y clamp vertical (-80° a +80°).
- **Gravedad** aplicada al CharacterController.
- **Head Bob**: balanceo sutil de la cámara al caminar (frecuencia 12, amplitud 0.05).
- **Screen Shake**: sacudida de cámara al recibir daño (duración 0.15s, intensidad 3).
- **Sistema de ralentización**: reducción temporal de velocidad al ser golpeado por Congeladores (40% de velocidad durante 3 segundos).
- Cursor bloqueado e invisible durante gameplay.

---

### 3.7 Vida.cs — Sistema de Vida y Muerte

| Característica | Detalle |
|---|---|
| **Líneas de código** | 158 |
| **Responsabilidad** | Gestión de vida para jugador y enemigos |

**Funcionalidades:**

- Sistema unificado para jugador (100 HP) y enemigos (HP variable según tipo).
- **Flash de daño** rojo al recibir impactos.
- **Drop de ítems** al morir un enemigo:
  - 25% probabilidad → Munición (cubo dorado).
  - 15% probabilidad → Vida (cubo verde).
- **Curación** con método `Curar(int)`.
- Sonidos diferenciados para daño al jugador y daño a enemigos.
- Flash blanco al morir (efecto de muerte).

**Eventos emitidos:**
- `OnVidaCambiada(int actuales, int maximas)`
- `OnDanoRecibidoJugador()`

---

### 3.8 UIManager.cs — Interfaz de Usuario

| Característica | Detalle |
|---|---|
| **Líneas de código** | 455 |
| **Responsabilidad** | Toda la interfaz gráfica (HUD) del juego |

**Elementos del HUD construidos programáticamente:**

| Elemento | Posición | Color | Información |
|---|---|---|---|
| **HP** | Inferior izquierda | Verde → Naranja → Rojo | Vida del jugador |
| **Munición** | Inferior derecha | Dorado → Naranja → Rojo | Munición actual / máxima |
| **Arma** | Inferior derecha (arriba de munición) | Gris | Nombre del arma + [1-2-3] |
| **Wave** | Superior centro | Naranja | WAVE X / 10 |
| **Enemigos** | Superior izquierda | Rojo claro | Contador de enemigos restantes |
| **Puntos** | Superior derecha | Blanco | Puntuación acumulada |
| **Racha** | Centro (arriba) | Naranja intenso | ¡xN COMBO! (cuando ≥ 3) |
| **Alerta** | Centro (abajo) | Rojo | Mensajes temporales |
| **Crosshair** | Centro | Blanco semi-transparente | Punto central + 4 líneas |

**Paneles especiales:**
- **Panel Game Over** (fondo rojo oscuro): Muestra "HAS MUERTO", estadísticas (puntuación y mejor racha), y opción de reiniciar.
- **Panel Victoria** (fondo verde oscuro): Muestra "¡VICTORIA!" y "TODAS LAS WAVE COMPLETADAS".
- **Flash de Daño**: Pantalla roja pulsante al recibir daño (componente `DanoFeedback`).

**Colores dinámicos:**
- La vida cambia de color según el porcentaje: verde (>50%), naranja (20-50%), rojo (<20%).
- La munición cambia de color según el porcentaje restante.
- Los combos intensifican su color con la racha.

---

### 3.9 SonidoManager.cs — Gestor de Audio

| Característica | Detalle |
|---|---|
| **Líneas de código** | 101 |
| **Patrón** | Singleton + DontDestroyOnLoad |
| **Responsabilidad** | Reproducción centralizada de efectos de sonido |

**Efectos de sonido:**

| Sonido | Archivo | Uso |
|---|---|---|
| Disparo Pistola | `disparo.wav` | Al disparar la pistola |
| Disparo Escopeta | `dsshotgn.wav` | Al disparar la escopeta |
| Disparo Ametralladora | `disparo.wav` | Al disparar la ametralladora |
| Daño Jugador | `jugador_dano.wav` | Cuando el jugador recibe daño |
| Daño Enemigo | `enemigo_dano.wav` | Cuando un enemigo recibe daño |
| Victoria | `victoria.wav` | Al completar todas las oleadas |
| Recarga | `dsdbload.wav` | Al recargar arma |
| Sin Bala | `dsdbopn.wav` | Al intentar disparar sin munición |

---

### 3.10 DanoFeedback.cs — Retroalimentación Visual de Daño

| Característica | Detalle |
|---|---|
| **Líneas de código** | 61 |
| **Responsabilidad** | Flash rojo en pantalla al recibir daño |

- Flash rojo inmediato con alpha máximo de 0.6 (estilo DOOM).
- Desvanecimiento suave durante 0.4 segundos usando `Time.unscaledDeltaTime`.
- Se suscribe al evento `Vida.OnDanoRecibidoJugador`.

---

### 3.11 GeneradorBotiquines.cs — Spawner de Botiquines

| Característica | Detalle |
|---|---|
| **Líneas de código** | 141 |
| **Responsabilidad** | Generación periódica de ítems de curación |

- Genera botiquines cada 12 segundos.
- Máximo 4 botiquines simultáneos en el escenario.
- Creación procedural de cubos verdes con collider trigger.
- Verificación de posición válida (sin colisión con paredes, sobre suelo).
- Auto-limpieza del contador al destruirse un botiquín.

---

### 3.12 Botiquin.cs — Lógica de Recolección de Botiquines

| Característica | Detalle |
|---|---|
| **Líneas de código** | 77 |
| **Responsabilidad** | Detección y recolección de ítems de curación |

- Doble sistema de detección: por distancia en Update y por `OnTriggerEnter`.
- No se puede recoger si el jugador tiene vida completa.
- Restaura 1 punto de vida al ser recogido.
- Soporte para sonido de recolección.

---

### 3.13 DropItem.cs — Ítems Dropeados por Enemigos

| Característica | Detalle |
|---|---|
| **Líneas de código** | 75 |
| **Responsabilidad** | Comportamiento de ítems soltados por enemigos |

- **Animación de flotación** y rotación continua (90°/s).
- **Desvanecimiento** gradual en los últimos 2 segundos de vida.
- Auto-destrucción a los 15 segundos.
- Recolección por proximidad (< 1.5 unidades).
- Dos tipos: **Munición** (recarga arma actual) y **Vida** (cura 2 HP).

---

### 3.14 Billboard.cs — Efecto Billboard para Sprites

| Característica | Detalle |
|---|---|
| **Líneas de código** | 24 |
| **Responsabilidad** | Hacer que sprites 2D miren siempre a la cámara |

- Rotación automática en `LateUpdate()` hacia la cámara principal.
- Rotación de 180° para orientar correctamente el sprite.
- Esencial para emular el estilo visual retro de DOOM.

---

### 3.15 SetupJugador.cs — Configuración Automática del Jugador

| Característica | Detalle |
|---|---|
| **Líneas de código** | 35 |
| **Responsabilidad** | Añadir componentes faltantes al jugador |

- Agrega `ArmaManager` al jugador si no lo tiene.
- Auto-detecta y asigna la cámara.
- Vincula la cámara al controlador de primera persona.
- Se ejecuta automáticamente al cargar la escena.

---

### 3.16 Meta.cs — Zona de Completar Nivel

| Característica | Detalle |
|---|---|
| **Líneas de código** | 29 |
| **Responsabilidad** | Trigger de victoria por zona |

- Colisionador configurado como Trigger automáticamente.
- Al entrar el jugador, intenta completar el nivel a través del `GameManager`.
- Solo el jugador puede activar la meta (verificación por componente `Vida` con `esJugador = true`).

---

### 3.17 Disparar.cs — Script Legacy (Deprecado)

| Característica | Detalle |
|---|---|
| **Líneas de código** | 9 |
| **Estado** | Deprecado — Solo mantiene compatibilidad |

- Script original de disparo, ahora reemplazado completamente por `ArmaManager.cs`.
- Se mantiene vacío para evitar errores de referencias en la escena.

---

## 4. MECÁNICAS DE JUEGO

### 4.1 Sistema de Oleadas (Waves)

```
Wave 1 → Wave 2 → Wave 3 → ... → Wave 10 → VICTORIA
```

- El juego consta de **10 oleadas** progresivas.
- Cada oleada genera hasta **9 enemigos** de tipos aleatorios.
- Al eliminar todos los enemigos de una oleada, se muestra un mensaje de transición ("WAVE X COMPLETADA!") con 2 segundos de pausa.
- Los 6 tipos de enemigos pueden aparecer desde la primera oleada.
- La oleada final desencadena la condición de victoria.

### 4.2 Sistema de Puntuación y Rachas

| Racha | Multiplicador | Puntos por Kill |
|---|---|---|
| 1 kill | x1 | 100 |
| 2 kills | x2 | 200 |
| 3 kills | x3 | 300 |
| 4 kills | x4 | 400 |
| 5+ kills | x5 (máximo) | 500 |

- La ventana para mantener la racha activa es de **3 segundos** entre kills.
- Al alcanzar racha ≥ 3, se muestra un mensaje "¡RACHA xN! (+puntos)" en pantalla.
- El multiplicador máximo es x5.

### 4.3 Controles

| Acción | Control |
|---|---|
| Mover | W / A / S / D |
| Mirar | Ratón |
| Sprint | Shift izquierdo |
| Disparar | Click izquierdo (mantener para ametralladora) |
| Recargar | R |
| Pistola | 1 |
| Escopeta | 2 |
| Ametralladora | 3 |
| Reiniciar (Game Over) | R o Click izquierdo |

---

## 5. RECURSOS GRÁFICOS Y DE AUDIO

### 5.1 Sprites (Assets/Sprites/)

| Archivo | Tamaño | Uso |
|---|---|---|
| `enemigo.png` | 1.6 KB | Sprite del enemigo (billboard) |
| `arma.png` | 2.0 KB | Sprite del arma del jugador |
| `botiquin.png` | 432 B | Sprite del botiquín |
| `muzzle.png` | 1.3 KB | Destello del disparo |

### 5.2 Audio (Assets/Sonidos/)

| Archivo | Tamaño | Efecto |
|---|---|---|
| `disparo.wav` | 19 KB | Sonido de disparo (pistola/ametralladora) |
| `dsshotgn.wav` | 9.5 KB | Sonido de escopeta (estilo DOOM) |
| `dsdbload.wav` | 4.7 KB | Sonido de recarga |
| `dsdbopn.wav` | 3.3 KB | Sonido de arma vacía |
| `enemigo_dano.wav` | 16 KB | Sonido de impacto en enemigo |
| `jugador_dano.wav` | 22 KB | Sonido de daño al jugador |
| `victoria.wav` | 56 KB | Fanfarria de victoria |

> **Nota:** Los archivos con prefijo "ds" (`dsshotgn`, `dsdbload`, `dsdbopn`) son nombres originales de los efectos de sonido de DOOM, lo que confirma la inspiración directa del juego.

---

## 6. DIAGRAMA DE ARQUITECTURA

```
                    ┌──────────────────┐
                    │   GameManager    │
                    │   (Singleton)    │
                    └────────┬─────────┘
                             │
          ┌──────────────────┼──────────────────┐
          │                  │                  │
          ▼                  ▼                  ▼
┌─────────────────┐ ┌──────────────┐ ┌────────────────┐
│GeneradorEnemigos│ │  UIManager   │ │ SonidoManager  │
│                 │ │  (Singleton) │ │  (Singleton)   │
└────────┬────────┘ └──────────────┘ └────────────────┘
         │
    ┌────┼────┐
    │    │    │
    ▼    ▼    ▼
┌──────┐┌──┐┌─────────┐
│Vida  ││IA││Billboard│
└──┬───┘└──┘└─────────┘
   │
   ▼
┌──────────┐       ┌─────────────────┐
│ DropItem │       │ PrimeraPersona  │
└──────────┘       │   (Jugador)     │
                   └────────┬────────┘
                            │
                   ┌────────┼────────┐
                   │                 │
                   ▼                 ▼
              ┌──────────┐   ┌──────────────┐
              │ArmaManager│   │  Vida (HP)   │
              └──────────┘   └──────────────┘
```

---

## 7. FLUJO DEL JUEGO

```
INICIO → Auto-creación de sistemas
       → SetupJugador (agrega ArmaManager)
       → Wave 1 comienza
       → Generación de enemigos (hasta 9)
           │
           ├── ¿Jugador vivo? → NO → GAME OVER (Panel rojo)
           │                              │
           │                    ¿Presiona R? → SÍ → REINICIAR
           │
           └── ¿Todos eliminados?
                   │
                   ├── NO → Seguir jugando
                   │
                   └── SÍ → ¿Wave = 10?
                               │
                               ├── NO → Transición (+1 Wave)
                               │
                               └── SÍ → ¡VICTORIA! (Panel verde)
```

---

## 8. CARACTERÍSTICAS TÉCNICAS DESTACADAS

### 8.1 Auto-Creación en Runtime
Todos los sistemas principales se crean automáticamente al iniciar el juego utilizando `[RuntimeInitializeOnLoadMethod]`. Esto significa que **no se requiere configuración manual en el editor de Unity** — basta con tener la escena con un jugador y el terreno para que funcione.

### 8.2 Generación Procedural
Los enemigos se crean completamente en código (GameObjects, componentes, materiales, colliders, NavMeshAgent), sin necesidad de prefabs predefinidos en la escena.

### 8.3 Comunicación por Eventos
Los sistemas se comunican mediante eventos C# (`System.Action`), lo que permite un **acoplamiento bajo** entre componentes. Por ejemplo:
- `Vida` emite `OnDanoRecibidoJugador` → `DanoFeedback` responde con flash rojo y `PrimeraPersona` con screen shake.
- `ArmaManager` emite `OnMunicionCambiada` → `UIManager` actualiza la interfaz.

### 8.4 Persistencia entre Escenas
`GameManager`, `SonidoManager` y `UIManager` usan `DontDestroyOnLoad()` para persistir entre cargas de escena, recreando los sistemas transitorios automáticamente.

---

## 9. RESUMEN DE MÉTRICAS DEL CÓDIGO

| Métrica | Valor |
|---|---|
| **Total de scripts** | 17 |
| **Total de líneas de código** | ~2,335 |
| **Scripts activos** | 16 (Disparar.cs deprecado) |
| **Singletons** | 3 (GameManager, UIManager, SonidoManager) |
| **Eventos del sistema** | 9 |
| **Tipos de enemigos** | 6 |
| **Armas del jugador** | 3 |
| **Efectos de sonido** | 7 |
| **Sprites** | 4 |

---

## 10. REQUISITOS TÉCNICOS

| Requisito | Especificación |
|---|---|
| **Motor** | Unity 6 (6000.4.9f1) |
| **Render Pipeline** | Universal Render Pipeline (URP) |
| **Sistema Operativo** | Windows 10/11 |
| **Dependencias** | NavMesh, Unity UI, Input System |
| **Hardware mínimo** | Cualquier PC que soporte Unity 6 |

---

## 11. CONCLUSIONES

Shooter3DDoom es un proyecto completo de videojuego FPS que demuestra:

1. **Arquitectura robusta**: Uso correcto de patrones Singleton, Observer y auto-creación para un código mantenible.
2. **Gameplay completo**: Sistema de oleadas, puntuación, rachas, múltiples armas y tipos de enemigos variados.
3. **Generación procedural**: Enemigos y botiquines creados dinámicamente sin depender de prefabs manuales.
4. **Feedback sensorial**: Flash de daño, screen shake, head bob, efectos de impacto, y HUD dinámico con colores adaptativos.
5. **Inspiración fiel a DOOM**: Enemigos billboard, sonidos retro, mecánicas de combate frenéticas y estilo visual que homenajea al clásico de 1993.

---

*Informe generado el 12 de julio de 2026.*
