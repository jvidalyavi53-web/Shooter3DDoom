using UnityEngine;
using System.Collections;

public class ArmaManager : MonoBehaviour
{
    [Header("Arma Actual")]
    public int armaActualIndex = 0;

    [Header("Pistola")]
    public int danoPistola = 2;
    public float cadenciaPistola = 0.25f;
    public int maxMunicionPistola = 15;
    public float retrocesoPistola = 1.5f;

    [Header("Escopeta")]
    public int pelletsEscopeta = 5;
    public int danoEscopeta = 3;
    public float cadenciaEscopeta = 0.8f;
    public int maxMunicionEscopeta = 8;
    public float retrocesoEscopeta = 4f;

    [Header("Ametralladora")]
    public int danoAmetralladora = 1;
    public float cadenciaAmetralladora = 0.1f;
    public int maxMunicionAmetralladora = 30;
    public float retrocesoAmetralladora = 0.8f;

    [Header("Recarga")]
    public float tiempoRecarga = 1.5f;

    [Header("Referencias")]
    public Camera camara;
    public GameObject muzzle;

    // Estado interno
    private int[] municionesActuales;
    private int[] maxMuniciones;
    private float[] cadencias;
    private float[] retrocesos;
    private string[] nombresArmas;
    private int[] danos;
    private int[] pellets;
    private bool estaRecargando = false;
    private float proximoDisparo = 0f;
    private float retrocesoActual = 0f;

    // Eventos
    public static event System.Action<int, int> OnMunicionCambiada;
    public static event System.Action<string> OnEstadoRecargaCambiado;
    public static event System.Action<string> OnArmaCambiada;

    void Start()
    {
        if (muzzle != null) muzzle.SetActive(false);

        // Auto-detectar cámara si no está asignada
        if (camara == null)
        {
            camara = GetComponentInChildren<Camera>();
            if (camara == null) camara = Camera.main;
        }

        // Inicializar arrays
        nombresArmas = new string[] { "Pistola", "Escopeta", "Ametralladora" };
        maxMuniciones = new int[] { maxMunicionPistola, maxMunicionEscopeta, maxMunicionAmetralladora };
        municionesActuales = new int[] { maxMunicionPistola, maxMunicionEscopeta, maxMunicionAmetralladora };
        cadencias = new float[] { cadenciaPistola, cadenciaEscopeta, cadenciaAmetralladora };
        retrocesos = new float[] { retrocesoPistola, retrocesoEscopeta, retrocesoAmetralladora };
        danos = new int[] { danoPistola, danoEscopeta, danoAmetralladora };
        pellets = new int[] { 1, pelletsEscopeta, 1 };

        OnMunicionCambiada?.Invoke(municionesActuales[0], maxMuniciones[0]);
        OnArmaCambiada?.Invoke(nombresArmas[0]);
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (GameManager.Instancia != null && GameManager.Instancia.EsJuegoTerminado()) return;

        // Suavizar retroceso
        retrocesoActual = Mathf.Lerp(retrocesoActual, 0f, Time.deltaTime * 15f);

        if (estaRecargando) return;

        // Cambiar arma con teclas 1-3
        if (Input.GetKeyDown(KeyCode.Alpha1)) CambiarArma(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CambiarArma(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CambiarArma(2);

        // Recargar con R
        if (Input.GetKeyDown(KeyCode.R) && municionesActuales[armaActualIndex] < maxMuniciones[armaActualIndex])
        {
            StartCoroutine(Recargar());
            return;
        }

        // Disparar
        if (Input.GetMouseButton(0) && Time.time >= proximoDisparo && municionesActuales[armaActualIndex] > 0)
        {
            proximoDisparo = Time.time + cadencias[armaActualIndex];
            HacerDisparo();
        }
        else if (Input.GetMouseButtonDown(0) && municionesActuales[armaActualIndex] <= 0)
        {
            SonidoManager.ReproducirSinBala();
        }
    }

    void LateUpdate()
    {
        // Aplicar retroceso a la cámara
        if (camara != null && retrocesoActual > 0.01f)
        {
            camara.transform.localRotation = Quaternion.Euler(-retrocesoActual, 0, 0);
        }
    }

    void HacerDisparo()
    {
        if (camara == null) return;

        municionesActuales[armaActualIndex]--;
        OnMunicionCambiada?.Invoke(municionesActuales[armaActualIndex], maxMuniciones[armaActualIndex]);

        // Muzzle flash
        if (muzzle != null)
        {
            muzzle.SetActive(true);
            Invoke("ApagarMuzzle", 0.05f);
        }

        // Sonido de disparo
        SonidoManager.ReproducirDisparo(armaActualIndex);

        retrocesoActual += retrocesos[armaActualIndex];

        // Disparar pellets
        int numPellets = pellets[armaActualIndex];
        for (int i = 0; i < numPellets; i++)
        {
            // Precisión: dispersión para escopeta
            Vector3 dir = camara.transform.forward;
            if (numPellets > 1)
            {
                float spread = 0.08f;
                dir += camara.transform.right * Random.Range(-spread, spread);
                dir += camara.transform.up * Random.Range(-spread, spread);
            }

            Ray ray = new Ray(camara.transform.position, dir);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vida v = hit.collider.GetComponentInParent<Vida>();
                if (v != null && !v.esJugador)
                {
                    v.RecibirDano(danos[armaActualIndex]);

                    // Flash de impacto
                    Renderer rend = hit.collider.GetComponentInParent<Renderer>();
                    if (rend != null)
                        StartCoroutine(FlashImpacto(rend));
                }

                // Efecto de impacto en pared
                CrearImpacto(hit.point, hit.normal);
            }
        }

        // Auto-recarga
        if (municionesActuales[armaActualIndex] <= 0)
            StartCoroutine(Recargar());
    }

    void CrearImpacto(Vector3 posicion, Vector3 normal)
    {
        GameObject impacto = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        impacto.transform.position = posicion + normal * 0.05f;
        impacto.transform.localScale = Vector3.one * 0.1f;
        impacto.transform.rotation = Quaternion.LookRotation(normal);

        Renderer rend = impacto.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.color = new Color(1f, 0.8f, 0.2f);
        rend.material = mat;

        Collider col = impacto.GetComponent<Collider>();
        if (col != null) Destroy(col);

        Destroy(impacto, 0.3f);
    }

    IEnumerator FlashImpacto(Renderer rend)
    {
        if (rend == null) yield break;
        Color original = rend.material.color;
        rend.material.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        if (rend != null) rend.material.color = original;
    }

    IEnumerator Recargar()
    {
        estaRecargando = true;
        OnEstadoRecargaCambiado?.Invoke("Recargando...");
        SonidoManager.ReproducirRecarga();
        yield return new WaitForSeconds(tiempoRecarga);

        municionesActuales[armaActualIndex] = maxMuniciones[armaActualIndex];
        estaRecargando = false;
        OnEstadoRecargaCambiado?.Invoke("");
        OnMunicionCambiada?.Invoke(municionesActuales[armaActualIndex], maxMuniciones[armaActualIndex]);
    }

    public void CambiarArma(int index)
    {
        if (index < 0 || index >= nombresArmas.Length) return;
        if (index == armaActualIndex) return;
        if (estaRecargando) return;

        armaActualIndex = index;
        OnMunicionCambiada?.Invoke(municionesActuales[armaActualIndex], maxMuniciones[armaActualIndex]);
        OnArmaCambiada?.Invoke(nombresArmas[armaActualIndex]);
    }

    public void RecargarArmaActual()
    {
        municionesActuales[armaActualIndex] = maxMuniciones[armaActualIndex];
        OnMunicionCambiada?.Invoke(municionesActuales[armaActualIndex], maxMuniciones[armaActualIndex]);
    }

    void ApagarMuzzle()
    {
        if (muzzle != null) muzzle.SetActive(false);
    }
}
