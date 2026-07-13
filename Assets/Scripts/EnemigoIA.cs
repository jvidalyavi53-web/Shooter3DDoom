using UnityEngine;
using UnityEngine.AI;

public class EnemigoIA : MonoBehaviour
{
    [Header("Configuración de Combate")]
    public float distanciaAtaque = 10f;
    public float cadenciaAtaque = 1.5f;
    public int danoAtaque = 1;
    public float probabilidadFallo = 0.15f;

    [Header("Movimiento")]
    public float velocidadMovimiento = 4.5f;
    public float velocidadPatrulla = 2f;
    public float distanciaDeteccion = 25f;

    [Header("Comportamiento")]
    public float tiempoEntreMovimientos = 2f;
    public float distanciaEsquive = 3f;
    public bool esSuicide = false;
    public bool esCongelador = false;

    private Transform jugadorTransform;
    private NavMeshAgent agente;
    private float proximoDisparo = 0f;
    private bool navMeshDisponible = false;
    private float tiempoActivacion = 0f;
    private float retardoActivacion = 0.5f;
    private float proximoMovimientoLateral = 0f;
    private int direccionEsquive = 1;
    private bool jugadorDetectado = false;
    private Renderer rendererRef;
    private Color colorOriginal;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        // Buscar SpriteRenderer primero, luego fallback a Renderer
        rendererRef = GetComponent<SpriteRenderer>();
        if (rendererRef == null)
            rendererRef = GetComponent<Renderer>();

        if (rendererRef != null)
            colorOriginal = rendererRef.material.color;

        GameObject jugadorGO = GameObject.FindWithTag("Player");
        if (jugadorGO != null)
            jugadorTransform = jugadorGO.transform;

        if (agente != null)
        {
            agente.speed = velocidadMovimiento;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agente.Warp(hit.position);
                navMeshDisponible = true;
            }
            else
            {
                agente.enabled = false;
            }
        }

        tiempoActivacion = Time.time + retardoActivacion;
    }

    void Update()
    {
        if (GameManager.Instancia != null && GameManager.Instancia.EsJuegoTerminado())
        {
            if (agente != null && agente.enabled) agente.isStopped = true;
            return;
        }

        if (jugadorTransform == null) return;

        float distancia = Vector3.Distance(transform.position, jugadorTransform.position);

        if (!jugadorDetectado && distancia <= distanciaDeteccion)
            jugadorDetectado = true;

        if (!jugadorDetectado) return;

        // MIRAR AL JUGADOR
        Vector3 dir = (jugadorTransform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);

        // MOVIMIENTO
        if (esSuicide)
        {
            // Suicide: siempre perseguir directamente
            MoverHaciaJugador();
        }
        else if (navMeshDisponible && agente != null && agente.enabled)
        {
            agente.isStopped = false;

            if (distancia > distanciaAtaque * 0.7f)
            {
                agente.speed = velocidadMovimiento;
                agente.SetDestination(jugadorTransform.position);
            }
            else if (distancia > 3f)
            {
                // Strafe lateral como DOOM
                agente.speed = velocidadPatrulla;

                if (Time.time >= proximoMovimientoLateral)
                {
                    proximoMovimientoLateral = Time.time + tiempoEntreMovimientos;
                    direccionEsquive = Random.value > 0.5f ? 1 : -1;
                }

                Vector3 lateral = transform.right * direccionEsquive * distanciaEsquive;
                Vector3 destino = transform.position + lateral;

                NavMeshHit navHit;
                if (NavMesh.SamplePosition(destino, out navHit, distanciaEsquive, NavMesh.AllAreas))
                    agente.SetDestination(navHit.position);
                else
                {
                    direccionEsquive *= -1;
                    agente.SetDestination(jugadorTransform.position);
                }
            }
            else
            {
                agente.speed = velocidadMovimiento;
                agente.SetDestination(jugadorTransform.position);
            }
        }
        else
        {
            MoverHaciaJugador();
        }

        // No atacar durante periodo de gracia
        if (Time.time < tiempoActivacion) return;

        // ATACAR
        if (esSuicide && distancia <= 2f)
        {
            // Explotar
            Explotar();
        }
        else if (!esSuicide && distancia <= distanciaAtaque && Time.time >= proximoDisparo)
        {
            proximoDisparo = Time.time + cadenciaAtaque;
            IntentarAtacar();
        }
    }

    private void MoverHaciaJugador()
    {
        if (jugadorTransform == null) return;
        Vector3 dir = (jugadorTransform.position - transform.position).normalized;
        dir.y = 0;
        transform.position += dir * velocidadMovimiento * Time.deltaTime;
    }

    private void IntentarAtacar()
    {
        if (jugadorTransform == null) return;

        float distancia = Vector3.Distance(transform.position, jugadorTransform.position);
        if (distancia > distanciaAtaque) return;

        // Probabilidad de fallo
        float falloAjustado = probabilidadFallo + (distancia / distanciaAtaque) * 0.1f;
        if (Random.value < falloAjustado)
        {
            EfectoDisparo();
            return;
        }

        // Línea de visión
        Vector3 origen = transform.position + Vector3.up * 0.5f;
        Vector3 destino = jugadorTransform.position + Vector3.up * 0.5f;
        Vector3 direccion = (destino - origen).normalized;
        float alcance = Vector3.Distance(origen, destino) + 0.5f;

        if (Physics.SphereCast(origen, 0.2f, direccion, out RaycastHit hit, alcance))
        {
            Vida v = hit.collider.GetComponentInParent<Vida>();
            if (v != null && v.esJugador)
            {
                EfectoDisparo();
                v.RecibirDano(danoAtaque);
                if (esCongelador) AplicarEfectoCongelador();
                return;
            }
        }

        // Melee
        if (distancia <= 3f)
        {
            EfectoDisparo();
            Vida vidaJugador = jugadorTransform.GetComponent<Vida>();
            if (vidaJugador != null)
            {
                vidaJugador.RecibirDano(danoAtaque);
                if (esCongelador) AplicarEfectoCongelador();
            }
        }
    }

    private void Explotar()
    {
        // Dañar al jugador
        Vida vidaJugador = jugadorTransform.GetComponent<Vida>();
        if (vidaJugador != null)
        {
            float dist = Vector3.Distance(transform.position, jugadorTransform.position);
            int danoEscalado = Mathf.RoundToInt(danoAtaque * Mathf.Clamp01(1f - dist / 5f));
            vidaJugador.RecibirDano(Mathf.Max(1, danoEscalado));
        }

        // Efecto visual
        EfectoDisparo();

        // Auto-destruir
        Vida miVida = GetComponent<Vida>();
        if (miVida != null)
            miVida.RecibirDano(999);
        else
            Destroy(gameObject);
    }

    private void EfectoDisparo()
    {
        if (rendererRef != null)
        {
            rendererRef.material.color = Color.white;
            Invoke("RestaurarColor", 0.1f);
        }
    }

    private void RestaurarColor()
    {
        if (rendererRef != null)
            rendererRef.material.color = colorOriginal;
    }

    private void AplicarEfectoCongelador()
    {
        if (jugadorTransform == null) return;
        PrimeraPersona pp = jugadorTransform.GetComponent<PrimeraPersona>();
        if (pp != null)
            pp.AplicarLentitud(3f, 0.4f); // 3 segundos al 40% de velocidad
    }
}
