using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{
    [Header("Referencias")]
    public Sprite spriteEnemigo;

    [Header("Configuración del Spawner")]
    public float tiempoInicial = 5f;
    public float tiempoEntreSpawns = 3f;
    public float radioSeguridadInicial = 12f;
    public int maxEnemigosTotal = 9;

    [Header("Límites del Mapa")]
    public float limiteMinX = -22f;
    public float limiteMaxX = 22f;
    public float limiteMinZ = -22f;
    public float limiteMaxZ = 22f;

    private Transform jugadorTransform;
    private float proximoSpawn;
    private int totalEnemigosGenerados = 0;

    public bool TodosGenerados()
    {
        return totalEnemigosGenerados >= maxEnemigosTotal;
    }

    public void ReiniciarParaWave()
    {
        totalEnemigosGenerados = 0;
        proximoSpawn = Time.timeSinceLevelLoad + tiempoEntreSpawns;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCrear()
    {
        if (FindAnyObjectByType<GeneradorEnemigos>() != null) return;

        GameObject go = new GameObject("GeneradorEnemigos");
        GeneradorEnemigos gen = go.AddComponent<GeneradorEnemigos>();

        // Buscar el sprite del enemigo en Resources
        gen.spriteEnemigo = Resources.Load<Sprite>("enemigo");
    }

    void Start()
    {
        GameObject jugadorGO = GameObject.FindWithTag("Player");
        if (jugadorGO != null)
        {
            jugadorTransform = jugadorGO.transform;
        }

        proximoSpawn = Time.timeSinceLevelLoad + tiempoInicial;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (GameManager.Instancia != null && GameManager.Instancia.EsJuegoTerminado()) return;

        // No generar más si ya se alcanzó el máximo total
        if (totalEnemigosGenerados >= maxEnemigosTotal) return;

        // Buscar jugador si no lo tenemos
        if (jugadorTransform == null)
        {
            GameObject jugadorGO = GameObject.FindWithTag("Player");
            if (jugadorGO != null) jugadorTransform = jugadorGO.transform;
            else return;
        }

        if (Time.timeSinceLevelLoad >= proximoSpawn)
        {
            Spawn();
        }
    }

    public void SpawnEnemigo(TipoEnemigo tipo)
    {
        if (jugadorTransform == null) return;

        // No generar más si ya se alcanzó el máximo total
        if (totalEnemigosGenerados >= maxEnemigosTotal) return;

        Vector3 posicionSpawn;
        if (BuscarPosicionValida(out posicionSpawn))
        {
            GameObject enemigo = CrearEnemigo(posicionSpawn, tipo);

            // Verificación POST-SPAWN: destruir si quedó demasiado cerca
            float dist = Vector3.Distance(
                new Vector3(posicionSpawn.x, 0, posicionSpawn.z),
                new Vector3(jugadorTransform.position.x, 0, jugadorTransform.position.z)
            );
            if (dist < 8f)
            {
                Destroy(enemigo);
                return;
            }

            totalEnemigosGenerados++;
            enemigo.SetActive(true);
        }
    }

    private void Spawn()
    {
        if (jugadorTransform == null) return;

        // Determinar oleada actual
        int wave = (GameManager.Instancia != null) ? GameManager.Instancia.GetWaveActual() : 1;
        TipoEnemigo tipo = ObtenerTipoPorWave(wave);

        SpawnEnemigo(tipo);

        // Programar próximo spawn
        proximoSpawn = Time.timeSinceLevelLoad + tiempoEntreSpawns;
    }

    public TipoEnemigo ObtenerTipoPorWave(int wave)
    {
        float rand = Random.value;

        // Todos los tipos aparecen desde Wave 1 para testing rápido
        if (rand < 0.15f) return TipoEnemigo.Congelador;
        if (rand < 0.35f) return TipoEnemigo.Corredor;
        if (rand < 0.50f) return TipoEnemigo.Suicide;
        if (rand < 0.65f) return TipoEnemigo.Tanque;
        if (rand < 0.80f) return TipoEnemigo.Demonio;
        return TipoEnemigo.Zombi;
    }

    private GameObject CrearEnemigo(Vector3 posicion, TipoEnemigo tipo)
    {
        // Crear GameObject vacío (sin MeshFilter para evitar conflicto con SpriteRenderer)
        GameObject enemigo = new GameObject("Enemigo_" + tipo.nombre);
        enemigo.transform.position = posicion;
        enemigo.transform.localScale = new Vector3(1f, 1.5f, 1f);

        // Collider para recibir daño
        BoxCollider col = enemigo.AddComponent<BoxCollider>();
        col.size = new Vector3(0.8f, 1.5f, 0.1f);
        col.center = Vector3.zero;

        // Renderer
        if (spriteEnemigo != null)
        {
            // Usar SpriteRenderer para el sprite billboard
            SpriteRenderer sr = enemigo.AddComponent<SpriteRenderer>();
            sr.sprite = spriteEnemigo;
            sr.color = tipo.color; // Teñir el sprite del color de su tipo para distinguirlos
            sr.sortingOrder = 0;
        }
        else
        {
            // Fallback: crear Quad con material de color
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.SetParent(enemigo.transform);
            quad.transform.localPosition = Vector3.zero;
            quad.transform.localScale = Vector3.one;

            Renderer rend = quad.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.SetFloat("_Surface", 1);
            mat.color = tipo.color;
            mat.renderQueue = 3000;
            rend.material = mat;
        }

        // Billboard: mirar siempre a la cámara
        Billboard billboard = enemigo.AddComponent<Billboard>();

        // NavMeshAgent para movimiento
        UnityEngine.AI.NavMeshAgent agente = enemigo.AddComponent<UnityEngine.AI.NavMeshAgent>();
        agente.speed = tipo.velocidad;
        agente.stoppingDistance = tipo.distanciaAtaque * 0.5f;

        // Sistema de vida
        Vida vida = enemigo.AddComponent<Vida>();
        vida.vidaMax = tipo.vida;
        vida.esJugador = false;

        // IA del enemigo
        EnemigoIA ia = enemigo.AddComponent<EnemigoIA>();
        ia.distanciaAtaque = tipo.distanciaAtaque;
        ia.distanciaDeteccion = tipo.distanciaDeteccion;
        ia.danoAtaque = tipo.dano;
        ia.probabilidadFallo = tipo.probabilidadFallo;
        ia.velocidadMovimiento = tipo.velocidad;
        ia.velocidadPatrulla = tipo.velocidad * 0.5f;
        ia.esSuicide = tipo.esSuicide;
        ia.esCongelador = tipo.esCongelador; // Asignar el nuevo flag

        // Audio
        enemigo.AddComponent<AudioSource>();

        return enemigo;
    }

    private Color ObtenerColorPorTipo(TipoEnemigo tipo)
    {
        return tipo.color;
    }

    private bool BuscarPosicionValida(out Vector3 posicion)
    {
        posicion = Vector3.zero;

        if (jugadorTransform == null) return false;

        for (int i = 0; i < 30; i++)
        {
            float x = Random.Range(limiteMinX, limiteMaxX);
            float z = Random.Range(limiteMinZ, limiteMaxZ);
            Vector3 candidato = new Vector3(x, 1f, z);

            float dist = Vector3.Distance(
                new Vector3(candidato.x, 0, candidato.z),
                new Vector3(jugadorTransform.position.x, 0, jugadorTransform.position.z)
            );

            // Durante los primeros segundos, radio más grande
            float radioMinimo = radioSeguridadInicial;
            if (Time.time > tiempoInicial + 8f)
            {
                radioMinimo = 10f; // Radio normal después de la fase inicial
            }

            if (dist < radioMinimo) continue;

            // Verificar suelo
            if (Physics.Raycast(candidato + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
            {
                if (hit.normal.y > 0.7f)
                {
                    posicion = hit.point + Vector3.up * 0.1f;
                    return true;
                }
            }
        }

        return false;
    }
}
