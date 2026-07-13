using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Estado del Juego")]
    private int enemigosRestantes = 0;
    private bool juegoTerminado = false;

    [Header("Sistema de Oleadas")]
    public int totalWaves = 10;
    private int waveActual = 1;
    private int enemigosMatadosWave = 0;
    private int enemigosTotalesWave = 0;
    private bool transicionWave = false;

    [Header("Puntuación")]
    private int puntuacion = 0;
    private int rachaActual = 0;
    private float tiempoUltimaKill = 0f;
    private float ventanaRacha = 3f;
    private int mejorRacha = 0;

    // Eventos
    public static event System.Action<int, int> OnWaveCambiada;
    public static event System.Action<int> OnPuntuacionCambiada;
    public static event System.Action<int> OnRachaCambiada;
    public static event System.Action<string> OnMensajeWave;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoCrear()
    {
        if (Instancia == null)
        {
            GameObject go = new GameObject("-- GameManager --");
            go.AddComponent<GameManager>();
        }
    }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
        ResetEstado();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetEstado();
        RecrearSistemasTransitorios();
    }

    private void RecrearSistemasTransitorios()
    {
        // Recreate GeneradorEnemigos if missing
        if (FindAnyObjectByType<GeneradorEnemigos>() == null)
        {
            GameObject go = new GameObject("GeneradorEnemigos");
            GeneradorEnemigos gen = go.AddComponent<GeneradorEnemigos>();
            gen.spriteEnemigo = Resources.Load<Sprite>("enemigo");
        }

        // Recreate GeneradorBotiquines if missing
        GeneradorBotiquines.AutoCrearInstancia();

        // Recreate UIManager if missing (scene's UIManager may have null text references)
        if (UIManager.Instancia == null)
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas != null)
            {
                GameObject uiGO = new GameObject("UIManager");
                UIManager ui = uiGO.AddComponent<UIManager>();
                ui.ConstruirUI(canvas);
            }
        }

        // Setup player: add ArmaManager if missing
        SetupJugador.Setup();
    }

    private void ResetEstado()
    {
        enemigosRestantes = 0;
        juegoTerminado = false;
        puntuacion = 0;
        rachaActual = 0;
        mejorRacha = 0;
        waveActual = 1;
        enemigosMatadosWave = 0;
        enemigosTotalesWave = 0;
        transicionWave = false;
    }

    private void Update()
    {
        if (juegoTerminado && Input.GetKeyDown(KeyCode.R))
            ReiniciarNivel();

        if (rachaActual > 0 && Time.time - tiempoUltimaKill > ventanaRacha)
        {
            rachaActual = 0;
            OnRachaCambiada?.Invoke(rachaActual);
        }

        if (!transicionWave && !juegoTerminado && Time.time > 8f)
        {
            if (enemigosRestantes <= 0 && enemigosTotalesWave > 0)
            {
                if (waveActual >= totalWaves)
                    GanarJuego();
                else
                    StartCoroutine(TransicionWave());
            }
        }
    }

    private System.Collections.IEnumerator TransicionWave()
    {
        transicionWave = true;

        OnMensajeWave?.Invoke($"WAVE {waveActual} COMPLETADA!");
        yield return new WaitForSeconds(2f);

        waveActual++;
        enemigosMatadosWave = 0;
        enemigosTotalesWave = 0;

        OnWaveCambiada?.Invoke(waveActual, totalWaves);
        OnMensajeWave?.Invoke($"WAVE {waveActual}");
        yield return new WaitForSeconds(2f);
        OnMensajeWave?.Invoke("");

        GeneradorEnemigos gen = FindAnyObjectByType<GeneradorEnemigos>();
        if (gen != null)
        {
            gen.ReiniciarParaWave();
            TipoEnemigo tipo = gen.ObtenerTipoPorWave(waveActual);
            gen.SpawnEnemigo(tipo);
        }

        transicionWave = false;
    }

    public void RegistrarEnemigo()
    {
        enemigosRestantes++;
        enemigosTotalesWave++;
        if (UIManager.Instancia != null)
            UIManager.Instancia.ActualizarEnemigos(enemigosRestantes);
    }

    public void EnemigoMuerto()
    {
        if (juegoTerminado) return;

        enemigosRestantes--;
        enemigosMatadosWave++;

        rachaActual++;
        tiempoUltimaKill = Time.time;
        if (rachaActual > mejorRacha) mejorRacha = rachaActual;

        int multiplicador = Mathf.Min(rachaActual, 5);
        int puntos = 100 * multiplicador;
        puntuacion += puntos;

        OnPuntuacionCambiada?.Invoke(puntuacion);
        OnRachaCambiada?.Invoke(rachaActual);

        if (UIManager.Instancia != null)
        {
            UIManager.Instancia.ActualizarEnemigos(enemigosRestantes);
            UIManager.Instancia.ActualizarPuntuacion(puntuacion);

            if (rachaActual >= 3)
                UIManager.Instancia.MostrarMensajeAlerta($"¡RACHA x{rachaActual}! (+{puntos})");
        }

        // Victoria: todos los enemigos generados y eliminados
        if (enemigosRestantes <= 0)
        {
            GeneradorEnemigos gen = FindAnyObjectByType<GeneradorEnemigos>();
            if (gen != null && gen.TodosGenerados())
            {
                GanarJuego();
            }
        }
    }

    public void PlayerMurio()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (UIManager.Instancia != null)
            UIManager.Instancia.MostrarGameOver(puntuacion, mejorRacha);
    }

    private void GanarJuego()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SonidoManager.ReproducirVictoria();

        if (UIManager.Instancia != null)
            UIManager.Instancia.MostrarVictoria();
    }

    public void ReiniciarNivel()
    {
        // Asegurar que timeScale esté en 1 antes de cargar
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        juegoTerminado = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IntentarCompletarNivel()
    {
        if (juegoTerminado) return;
        if (enemigosRestantes <= 0)
            GanarJuego();
        else if (UIManager.Instancia != null)
            UIManager.Instancia.MostrarMensajeAlerta("¡Aún quedan enemigos!");
    }

    public int GetEnemigosRestantes() => enemigosRestantes;
    public bool EsJuegoTerminado() => juegoTerminado;
    public int GetPuntuacion() => puntuacion;
    public int GetWaveActual() => waveActual;
    public int GetTotalWaves() => totalWaves;
}
