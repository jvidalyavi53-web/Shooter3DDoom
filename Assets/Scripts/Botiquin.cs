using UnityEngine;

public class Botiquin : MonoBehaviour
{
    [Header("Configuración del Botiquín")]
    public int vidaRestaurada = 1;
    public AudioClip sonidoRecoleccion;
    public float rangoRecoleccion = 1.5f;

    private Transform jugadorTransform;
    private Vida vidaJugador;
    private bool recolectado = false;

    void Start()
    {
        GameObject jugadorGO = GameObject.FindWithTag("Player");
        if (jugadorGO != null)
        {
            jugadorTransform = jugadorGO.transform;
            vidaJugador = jugadorGO.GetComponent<Vida>();
        }
    }

    void Update()
    {
        if (recolectado) return;
        if (jugadorTransform == null || vidaJugador == null) return;

        // No recolectar si el jugador tiene vida completa
        if (vidaJugador.VidaActual() >= vidaJugador.vidaMax) return;

        // Verificar distancia (ignorando diferencia de altura)
        Vector3 posBotiquin = transform.position;
        Vector3 posJugador = jugadorTransform.position;
        posBotiquin.y = 0;
        posJugador.y = 0;

        float distanciaHorizontal = Vector3.Distance(posBotiquin, posJugador);
        if (distanciaHorizontal <= rangoRecoleccion)
        {
            Recolectar();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (recolectado) return;
        if (vidaJugador == null) return;
        if (vidaJugador.VidaActual() >= vidaJugador.vidaMax) return;

        // Verificar si es el jugador usando tag o componente
        if (other.CompareTag("Player") || other.GetComponentInParent<Vida>() != null)
        {
            Vida v = other.GetComponentInParent<Vida>();
            if (v != null && v.esJugador)
            {
                Recolectar();
            }
        }
    }

    private void Recolectar()
    {
        if (recolectado) return;
        recolectado = true;

        vidaJugador.Curar(vidaRestaurada);

        if (sonidoRecoleccion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecoleccion, transform.position);
        }

        Destroy(gameObject);
    }
}
