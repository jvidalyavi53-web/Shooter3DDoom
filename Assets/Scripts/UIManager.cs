using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instancia { get; private set; }

    [Header("Textos del HUD")]
    public Text textMunicion;
    public Text textEnemigos;
    public Text textVida;
    public Text textAlerta;
    public Text textPuntuacion;
    public Text textRacha;
    public Text textArma;
    public Text textWave;

    private Coroutine corrutinaAlerta;

    [Header("Paneles")]
    public GameObject panelGameOver;
    public GameObject panelVictoria;
    private Text textGameOverStats;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCrear()
    {
        if (Instancia != null) return;
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject go = new GameObject("UIManager");
        UIManager ui = go.AddComponent<UIManager>();
        ui.ConstruirUI(canvas);
    }

    public void ConstruirUI(Canvas canvas)
    {
        // Panel Game Over
        panelGameOver = CrearPanel(canvas.gameObject, "PanelGameOver", new Color(0.1f, 0, 0, 0.85f));
        CrearTexto(panelGameOver.transform, "TxtGameOver", "HAS MUERTO", TextAnchor.MiddleCenter,
            new Vector2(0, 80), new Vector2(500, 80), 56, Color.red);

        GameObject statsGO = CrearTextoGO(panelGameOver.transform, "TxtStats", "", TextAnchor.MiddleCenter,
            new Vector2(0, 0), new Vector2(500, 60), 22, Color.white);
        textGameOverStats = statsGO.GetComponent<Text>();

        // Texto reiniciar Game Over
        CrearTexto(panelGameOver.transform, "TxtReintentar", "Presiona R o click para reiniciar",
            TextAnchor.MiddleCenter, new Vector2(0, -70), new Vector2(500, 50), 24, new Color(1f, 0.8f, 0.2f));
        panelGameOver.SetActive(false);

        // Panel Victoria
        panelVictoria = CrearPanel(canvas.gameObject, "PanelVictoria", new Color(0, 0.05f, 0, 0.85f));
        CrearTexto(panelVictoria.transform, "TxtVictoria", "¡VICTORIA!", TextAnchor.MiddleCenter,
            new Vector2(0, 60), new Vector2(600, 80), 56, new Color(0.2f, 1f, 0.2f));
        CrearTexto(panelVictoria.transform, "TxtVictoriaSub", "TODAS LAS WAVE COMPLETADAS", TextAnchor.MiddleCenter,
            new Vector2(0, 10), new Vector2(500, 50), 24, Color.white);

        // Texto reiniciar Victoria
        CrearTexto(panelVictoria.transform, "TxtReintentarV", "Presiona R o click para jugar de nuevo",
            TextAnchor.MiddleCenter, new Vector2(0, -60), new Vector2(500, 50), 24, new Color(1f, 0.8f, 0.2f));
        panelVictoria.SetActive(false);

        // HUD
        textVida = CrearHUDText(canvas.transform, "TxtVida", "HP: 100",
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 20), new Vector2(250, 60),
            TextAnchor.LowerLeft, 32, Color.green);

        textMunicion = CrearHUDText(canvas.transform, "TxtMunicion", "15 / 15",
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(-20, 20), new Vector2(250, 60),
            TextAnchor.LowerRight, 32, new Color(1f, 0.8f, 0.2f));

        textArma = CrearHUDText(canvas.transform, "TxtArma", "PISTOLA [1-2-3]",
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(-20, 65), new Vector2(300, 45),
            TextAnchor.LowerRight, 24, new Color(0.8f, 0.8f, 0.8f));

        textWave = CrearHUDText(canvas.transform, "TxtWave", "WAVE 1 / 10",
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -10), new Vector2(400, 50),
            TextAnchor.UpperCenter, 28, new Color(1f, 0.5f, 0.1f));

        textEnemigos = CrearHUDText(canvas.transform, "TxtEnemigos", "ENEMIGOS: 0",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -10), new Vector2(250, 50),
            TextAnchor.UpperLeft, 26, new Color(1f, 0.3f, 0.3f));

        textPuntuacion = CrearHUDText(canvas.transform, "TxtPuntuacion", "PUNTOS: 0",
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(-10, -40), new Vector2(300, 50),
            TextAnchor.UpperRight, 26, Color.white);

        textRacha = CrearHUDText(canvas.transform, "TxtRacha", "",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(500, 70),
            TextAnchor.MiddleCenter, 40, new Color(1f, 0.5f, 0.1f));

        textAlerta = CrearHUDText(canvas.transform, "TxtAlerta", "",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -100), new Vector2(700, 60),
            TextAnchor.MiddleCenter, 28, new Color(1f, 0.2f, 0.2f));

        CrearCrosshair(canvas.transform);

        // Flash de daño
        GameObject flashGO = new GameObject("FlashDano");
        flashGO.transform.SetParent(canvas.transform, false);
        RectTransform flashRT = flashGO.AddComponent<RectTransform>();
        flashRT.anchorMin = Vector2.zero;
        flashRT.anchorMax = Vector2.one;
        flashRT.offsetMin = Vector2.zero;
        flashRT.offsetMax = Vector2.zero;
        flashGO.AddComponent<CanvasRenderer>();
        Image flashImg = flashGO.AddComponent<Image>();
        flashImg.color = new Color(1, 0, 0, 0);
        flashImg.raycastTarget = false;
        flashGO.AddComponent<DanoFeedback>();
    }

    private static Sprite _spriteBlancoCache;

    private static Sprite ObtenerSpriteBlanco()
    {
        if (_spriteBlancoCache == null)
        {
            Texture2D tex = new Texture2D(2, 2);
            Color[] pixels = new Color[4];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            _spriteBlancoCache = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 100f);
        }
        return _spriteBlancoCache;
    }

    private void CrearCrosshair(Transform parent)
    {
        // Contenedor padre del crosshair, centrado en pantalla
        GameObject crosshairGO = new GameObject("Crosshair");
        crosshairGO.transform.SetParent(parent, false);
        RectTransform rtParent = crosshairGO.AddComponent<RectTransform>();
        rtParent.anchorMin = new Vector2(0.5f, 0.5f);
        rtParent.anchorMax = new Vector2(0.5f, 0.5f);
        rtParent.anchoredPosition = Vector2.zero;
        rtParent.sizeDelta = new Vector2(30, 30);

        Color crosshairColor = new Color(1f, 1f, 1f, 0.9f);

        // Punto central
        CrearParteCrosshair(crosshairGO.transform, "CenterDot", Vector2.zero, new Vector2(4, 4), crosshairColor);

        // Línea superior (gap de 4px desde el centro)
        CrearParteCrosshair(crosshairGO.transform, "Top", new Vector2(0, 8), new Vector2(2, 10), crosshairColor);

        // Línea inferior
        CrearParteCrosshair(crosshairGO.transform, "Bottom", new Vector2(0, -8), new Vector2(2, 10), crosshairColor);

        // Línea izquierda
        CrearParteCrosshair(crosshairGO.transform, "Left", new Vector2(-8, 0), new Vector2(10, 2), crosshairColor);

        // Línea derecha
        CrearParteCrosshair(crosshairGO.transform, "Right", new Vector2(8, 0), new Vector2(10, 2), crosshairColor);
    }

    private void CrearParteCrosshair(Transform parent, string nombre, Vector2 posicion, Vector2 tamano, Color color)
    {
        GameObject parte = new GameObject("Crosshair_" + nombre);
        parte.transform.SetParent(parent, false);
        RectTransform rt = parte.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = posicion;
        rt.sizeDelta = tamano;
        parte.AddComponent<CanvasRenderer>();
        Image img = parte.AddComponent<Image>();
        img.sprite = ObtenerSpriteBlanco();
        img.type = Image.Type.Simple;
        img.color = color;
        img.raycastTarget = false;
    }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    private void Start()
    {
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);

        // Forzar valores iniciales correctos después de reiniciar
        ForzarValoresIniciales();
    }

    private void ForzarValoresIniciales()
    {
        if (textVida != null)
        {
            textVida.text = "HP: 100";
            textVida.color = Color.green;
        }

        if (textMunicion != null)
        {
            textMunicion.text = "15 / 15";
            textMunicion.color = new Color(1f, 0.8f, 0.2f);
        }

        if (textArma != null)
        {
            textArma.text = "PISTOLA [1-2-3]";
            textArma.color = new Color(0.8f, 0.8f, 0.8f);
        }

        if (textWave != null)
        {
            textWave.text = "WAVE 1 / 10";
            textWave.color = new Color(1f, 0.5f, 0.1f);
        }

        if (textEnemigos != null)
        {
            textEnemigos.text = "ENEMIGOS: 0";
            textEnemigos.color = new Color(1f, 0.3f, 0.3f);
        }

        if (textPuntuacion != null)
        {
            textPuntuacion.text = "PUNTOS: 0";
            textPuntuacion.color = Color.white;
        }

        if (textRacha != null) textRacha.text = "";
        if (textAlerta != null) textAlerta.text = "";
    }

    private void Update()
    {
        // Detectar restart cuando GameOver o Victoria están activos
        bool gameOverActivo = panelGameOver != null && panelGameOver.activeSelf;
        bool victoriaActivo = panelVictoria != null && panelVictoria.activeSelf;

        if ((gameOverActivo || victoriaActivo) && Input.GetKeyDown(KeyCode.R))
        {
            if (GameManager.Instancia != null)
                GameManager.Instancia.ReiniciarNivel();
        }

        // También detectar click izquierdo en los paneles
        if ((gameOverActivo || victoriaActivo) && Input.GetMouseButtonDown(0))
        {
            if (GameManager.Instancia != null)
                GameManager.Instancia.ReiniciarNivel();
        }
    }

    private void OnEnable()
    {
        Vida.OnVidaCambiada += ActualizarVida;
        GameManager.OnPuntuacionCambiada += ActualizarPuntuacion;
        GameManager.OnRachaCambiada += ActualizarRacha;
        GameManager.OnWaveCambiada += ActualizarWave;
        GameManager.OnMensajeWave += MostrarMensajeAlerta;
        ArmaManager.OnMunicionCambiada += ActualizarMunicionArma;
        ArmaManager.OnArmaCambiada += ActualizarArma;
        ArmaManager.OnEstadoRecargaCambiado += ActualizarEstadoRecarga;
    }

    private void OnDisable()
    {
        Vida.OnVidaCambiada -= ActualizarVida;
        GameManager.OnPuntuacionCambiada -= ActualizarPuntuacion;
        GameManager.OnRachaCambiada -= ActualizarRacha;
        GameManager.OnWaveCambiada -= ActualizarWave;
        GameManager.OnMensajeWave -= MostrarMensajeAlerta;
        ArmaManager.OnMunicionCambiada -= ActualizarMunicionArma;
        ArmaManager.OnArmaCambiada -= ActualizarArma;
        ArmaManager.OnEstadoRecargaCambiado -= ActualizarEstadoRecarga;
    }

    public void ActualizarMunicionArma(int actuales, int maximas)
    {
        if (textMunicion != null)
        {
            textMunicion.text = $"{actuales} / {maximas}";
            float ratio = (float)actuales / maximas;
            if (ratio <= 0.2f) textMunicion.color = Color.red;
            else if (ratio <= 0.5f) textMunicion.color = new Color(1f, 0.5f, 0f);
            else textMunicion.color = new Color(1f, 0.8f, 0.2f);
        }
    }

    public void ActualizarEstadoRecarga(string estado)
    {
        if (textMunicion != null && !string.IsNullOrEmpty(estado))
        {
            textMunicion.text = estado;
            textMunicion.color = new Color(0.5f, 0.5f, 1f);
        }
    }

    public void ActualizarArma(string nombreArma)
    {
        if (textArma != null)
            textArma.text = $"{nombreArma} [1-2-3]";
    }

    public void ActualizarWave(int waveActual, int totalWaves)
    {
        if (textWave != null)
            textWave.text = $"WAVE {waveActual} / {totalWaves}";
    }

    public void ActualizarEnemigos(int enemigos)
    {
        if (textEnemigos != null)
            textEnemigos.text = $"ENEMIGOS: {enemigos}";
    }

    public void ActualizarVida(int actuales, int maximas)
    {
        if (textVida != null)
        {
            textVida.text = $"HP: {actuales}";
            float ratio = (float)actuales / maximas;
            if (ratio <= 0.2f) textVida.color = Color.red;
            else if (ratio <= 0.5f) textVida.color = new Color(1f, 0.5f, 0f);
            else textVida.color = Color.green;
        }
    }

    public void ActualizarPuntuacion(int puntos)
    {
        if (textPuntuacion != null)
            textPuntuacion.text = $"PUNTOS: {puntos}";
    }

    public void ActualizarRacha(int racha)
    {
        if (textRacha != null)
        {
            if (racha >= 3)
            {
                textRacha.text = $"¡x{racha} COMBO!";
                textRacha.color = new Color(1f, 0.3f + racha * 0.05f, 0.1f);
            }
            else
            {
                textRacha.text = "";
            }
        }
    }

    public void MostrarGameOver(int puntuacionFinal, int mejorRacha)
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
            if (textGameOverStats != null)
                textGameOverStats.text = $"Puntuación: {puntuacionFinal}   |   Mejor Racha: x{mejorRacha}";
        }
    }

    public void MostrarGameOver() => MostrarGameOver(0, 0);

    public void MostrarVictoria()
    {
        if (panelVictoria != null)
            panelVictoria.SetActive(true);
    }

    public void MostrarMensajeAlerta(string mensaje)
    {
        if (corrutinaAlerta != null) StopCoroutine(corrutinaAlerta);
        corrutinaAlerta = StartCoroutine(RutinaAlerta(mensaje));
    }

    private IEnumerator RutinaAlerta(string mensaje)
    {
        if (textAlerta != null) textAlerta.text = mensaje;
        yield return new WaitForSeconds(2f);
        if (textAlerta != null) textAlerta.text = "";
        corrutinaAlerta = null;
    }

    // Helpers de UI
    private GameObject CrearPanel(GameObject parent, string nombre, Color color)
    {
        GameObject panel = new GameObject(nombre);
        panel.transform.SetParent(parent.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        panel.AddComponent<CanvasRenderer>();
        Image img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    private void CrearTexto(Transform parent, string nombre, string contenido,
        TextAnchor alineacion, Vector2 posicion, Vector2 tamano, int fontSize, Color color)
    {
        CrearTextoGO(parent, nombre, contenido, alineacion, posicion, tamano, fontSize, color);
    }

    private GameObject CrearTextoGO(Transform parent, string nombre, string contenido,
        TextAnchor alineacion, Vector2 posicion, Vector2 tamano, int fontSize, Color color)
    {
        GameObject txtGO = new GameObject(nombre);
        txtGO.transform.SetParent(parent, false);
        RectTransform rt = txtGO.AddComponent<RectTransform>();
        rt.anchoredPosition = posicion;
        rt.sizeDelta = tamano;
        txtGO.AddComponent<CanvasRenderer>();
        Text txt = txtGO.AddComponent<Text>();
        txt.text = contenido;
        txt.alignment = alineacion;
        txt.fontSize = fontSize;
        txt.color = color;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontStyle = FontStyle.Bold;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        return txtGO;
    }

    private Text CrearHUDText(Transform parent, string nombre, string contenido,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offset, Vector2 tamano,
        TextAnchor alineacion, int fontSize, Color color)
    {
        GameObject txtGO = new GameObject(nombre);
        txtGO.transform.SetParent(parent, false);
        RectTransform rt = txtGO.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = offset;
        rt.sizeDelta = tamano;
        txtGO.AddComponent<CanvasRenderer>();
        Text txt = txtGO.AddComponent<Text>();
        txt.text = contenido;
        txt.alignment = alineacion;
        txt.fontSize = fontSize;
        txt.color = color;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontStyle = FontStyle.Bold;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        return txt;
    }
}
