using UnityEngine;

public class GeneradorBotiquines : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabBotiquin;
    public float tiempoEntreSpawns = 12f;
    public int maxBotiquinesEnEscena = 4;

    [Header("Límites del Mapa")]
    public float limiteMinX = -20f;
    public float limiteMaxX = 20f;
    public float limiteMinZ = -20f;
    public float limiteMaxZ = 20f;

    private float proximoSpawn;
    private int botiquinesActivos = 0;
    private bool listoParaSpawnear = false;

    public static void AutoCrearInstancia()
    {
        if (FindAnyObjectByType<GeneradorBotiquines>() != null) return;

        GameObject go = new GameObject("GeneradorBotiquines");
        GeneradorBotiquines gen = go.AddComponent<GeneradorBotiquines>();
        gen.prefabBotiquin = CrearPrefabBotiquin();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCrear()
    {
        AutoCrearInstancia();
    }

    static GameObject CrearPrefabBotiquin()
    {
        GameObject botiquin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        botiquin.name = "Botiquin";
        botiquin.SetActive(false);

        botiquin.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        Rigidbody rb = botiquin.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        BoxCollider col = botiquin.GetComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = Vector3.one;

        Renderer rend = botiquin.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.1f, 0.9f, 0.2f);
        rend.material = mat;

        Botiquin b = botiquin.AddComponent<Botiquin>();
        b.vidaRestaurada = 1;

        return botiquin;
    }

    void Awake()
    {
        proximoSpawn = Time.timeSinceLevelLoad + 3f;
    }

    void Start()
    {
        if (!listoParaSpawnear)
        {
            proximoSpawn = Time.timeSinceLevelLoad + 3f;
            listoParaSpawnear = true;
        }
    }

    void Update()
    {
        if (!listoParaSpawnear) return;
        if (Time.timeScale == 0f) return;
        if (GameManager.Instancia != null && GameManager.Instancia.EsJuegoTerminado()) return;

        if (botiquinesActivos < maxBotiquinesEnEscena && Time.timeSinceLevelLoad >= proximoSpawn)
        {
            proximoSpawn = Time.timeSinceLevelLoad + tiempoEntreSpawns;
            Spawn();
        }
    }

    private void Spawn()
    {
        if (prefabBotiquin == null) return;

        Vector3 posicionSpawn;
        if (BuscarPosicionAleatoria(out posicionSpawn))
        {
            GameObject nuevoBotiquin = Instantiate(prefabBotiquin, posicionSpawn, Quaternion.identity);
            nuevoBotiquin.SetActive(true);
            botiquinesActivos++;
            StartCoroutine(EsperarDestruccion(nuevoBotiquin));
        }
    }

    private bool BuscarPosicionAleatoria(out Vector3 posicion)
    {
        posicion = Vector3.zero;

        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(limiteMinX, limiteMaxX);
            float z = Random.Range(limiteMinZ, limiteMaxZ);
            Vector3 candidato = new Vector3(x, 1f, z);

            // Verificar que no esté dentro de una pared
            if (!Physics.CheckSphere(candidato, 0.4f, ~0, QueryTriggerInteraction.Ignore))
            {
                // Verificar que hay suelo debajo
                Ray ray = new Ray(candidato + Vector3.up * 2f, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, 4f))
                {
                    if (hit.normal.y > 0.8f)
                    {
                        posicion = hit.point + Vector3.up * 0.5f;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private System.Collections.IEnumerator EsperarDestruccion(GameObject obj)
    {
        yield return new WaitWhile(() => obj != null);
        if (this != null)
        {
            botiquinesActivos--;
        }
    }
}
