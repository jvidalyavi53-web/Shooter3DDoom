using UnityEngine;

public class Vida : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int vidaMax = 100;
    public bool esJugador = false;
    private int vidaActual;

    [Header("Feedback")]
    private Renderer rendererRef;
    private Color colorOriginal;

    // Eventos
    public static event System.Action<int, int> OnVidaCambiada;
    public static event System.Action OnDanoRecibidoJugador;

    void Start()
    {
        vidaActual = vidaMax;

        // Buscar SpriteRenderer primero, luego fallback
        rendererRef = GetComponent<SpriteRenderer>();
        if (rendererRef == null)
            rendererRef = GetComponent<Renderer>();

        if (rendererRef != null)
            colorOriginal = rendererRef.material.color;

        if (esJugador)
        {
            OnVidaCambiada?.Invoke(vidaActual, vidaMax);
        }
        else
        {
            if (GameManager.Instancia != null)
                GameManager.Instancia.RegistrarEnemigo();
        }
    }

    public void RecibirDano(int cantidad)
    {
        if (vidaActual <= 0) return;

        vidaActual -= cantidad;

        // Flash de daño
        if (!esSuicide() && rendererRef != null)
        {
            rendererRef.material.color = Color.red;
            Invoke("RestaurarColor", 0.15f);
            if (!esJugador) SonidoManager.ReproducirDanoEnemigo();
        }

        if (esJugador)
        {
            OnVidaCambiada?.Invoke(vidaActual, vidaMax);
            OnDanoRecibidoJugador?.Invoke();
            SonidoManager.ReproducirDanoJugador();
        }

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private bool esSuicide()
    {
        EnemigoIA ia = GetComponent<EnemigoIA>();
        return ia != null && ia.esSuicide;
    }

    private void RestaurarColor()
    {
        if (rendererRef != null)
            rendererRef.material.color = colorOriginal;
    }

    public void Curar(int cantidad)
    {
        if (vidaActual <= 0) return;
        vidaActual = Mathf.Clamp(vidaActual + cantidad, 0, vidaMax);
        if (esJugador) OnVidaCambiada?.Invoke(vidaActual, vidaMax);
    }

    private void Morir()
    {
        if (esJugador)
        {
            if (GameManager.Instancia != null)
                GameManager.Instancia.PlayerMurio();
        }
        else
        {
            // Efecto de muerte
            EfectoMuerte();

            if (GameManager.Instancia != null)
                GameManager.Instancia.EnemigoMuerto();

            // Drop aleatorio
            DropItem();

            Destroy(gameObject, 0.1f);
        }
    }

    private void EfectoMuerte()
    {
        if (rendererRef != null)
        {
            // Flash blanco antes de morir
            rendererRef.material.color = Color.white;
        }
    }

    private void DropItem()
    {
        float rand = Random.value;

        if (rand < 0.25f)
        {
            // Drop munición
            CrearDrop(transform.position, new Color(1f, 0.8f, 0.2f), "Munición");
        }
        else if (rand < 0.40f)
        {
            // Drop vida
            CrearDrop(transform.position + Vector3.up * 0.2f, Color.green, "Vida");
        }
    }

    private void CrearDrop(Vector3 posicion, Color color, string tipo)
    {
        GameObject drop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        drop.transform.position = posicion;
        drop.transform.localScale = Vector3.one * 0.3f;
        drop.transform.Rotate(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f);

        Renderer rend = drop.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.color = color;
        rend.material = mat;

        BoxCollider col = drop.GetComponent<BoxCollider>();
        col.isTrigger = true;

        DropItem dropScript = drop.AddComponent<DropItem>();
        dropScript.tipo = tipo;

        // Auto-destruir después de 15 segundos si no se recoge
        Destroy(drop, 15f);
    }

    public int VidaActual() => vidaActual;
}
