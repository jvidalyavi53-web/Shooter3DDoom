using UnityEngine;

public class DropItem : MonoBehaviour
{
    public string tipo = "Munición";
    private float tiempoVida = 15f;
    private float tiempoCreacion;
    private Transform jugadorTransform;

    void Start()
    {
        tiempoCreacion = Time.time;
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null) jugadorTransform = jugador.transform;
    }

    void Update()
    {
        // Rotar y flotar
        transform.Rotate(0, Time.deltaTime * 90f, 0);
        float bob = Mathf.Sin(Time.time * 3f) * 0.1f;
        transform.position += Vector3.up * bob * Time.deltaTime;

        // Desaparecer gradualmente
        if (Time.time > tiempoCreacion + tiempoVida - 2f)
        {
            Renderer rend = GetComponent<Renderer>();
            if (rend != null)
            {
                Color c = rend.material.color;
                c.a = Mathf.Lerp(1f, 0f, (Time.time - (tiempoCreacion + tiempoVida - 2f)) / 2f);
                rend.material.color = c;
            }
        }

        // Auto-destruir
        if (Time.time > tiempoCreacion + tiempoVida)
        {
            Destroy(gameObject);
            return;
        }

        // Verificar distancia al jugador (funciona sin Rigidbody)
        if (jugadorTransform == null) return;

        float dist = Vector3.Distance(transform.position, jugadorTransform.position);
        if (dist < 1.5f)
        {
            Recoger();
        }
    }

    private void Recoger()
    {
        if (tipo == "Vida")
        {
            Vida vida = jugadorTransform.GetComponent<Vida>();
            if (vida != null)
            {
                vida.Curar(2);
                Destroy(gameObject);
            }
        }
        else if (tipo == "Munición")
        {
            ArmaManager arma = jugadorTransform.GetComponent<ArmaManager>();
            if (arma != null)
            {
                arma.RecargarArmaActual();
                Destroy(gameObject);
            }
        }
    }
}
