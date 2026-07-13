using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DanoFeedback : MonoBehaviour
{
    [Header("Configuración visual (Estilo DOOM)")]
    public float duracionFlash = 0.4f;
    [Range(0f, 1f)]
    public float alphaMaximo = 0.6f; // Más intenso que antes

    private Image imagenFlash;
    private Coroutine corrutinaFlash;

    private void Awake()
    {
        imagenFlash = GetComponent<Image>();
        imagenFlash.color = new Color(0.8f, 0f, 0f, 0f);
        imagenFlash.raycastTarget = false;
    }

    private void OnEnable()
    {
        Vida.OnDanoRecibidoJugador += MostrarFlash;
    }

    private void OnDisable()
    {
        Vida.OnDanoRecibidoJugador -= MostrarFlash;
    }

    private void MostrarFlash()
    {
        if (corrutinaFlash != null)
        {
            StopCoroutine(corrutinaFlash);
        }
        corrutinaFlash = StartCoroutine(RutinaFlash());
    }

    private IEnumerator RutinaFlash()
    {
        float tiempo = 0f;

        // Flash rojo inmediato (DOOM tenía un flash de pantalla muy notable)
        imagenFlash.color = new Color(0.8f, 0f, 0f, alphaMaximo);

        while (tiempo < duracionFlash)
        {
            tiempo += Time.unscaledDeltaTime;
            float nuevoAlpha = Mathf.Lerp(alphaMaximo, 0f, tiempo / duracionFlash);
            imagenFlash.color = new Color(0.8f, 0f, 0f, nuevoAlpha);
            yield return null;
        }

        imagenFlash.color = new Color(0.8f, 0f, 0f, 0f);
        corrutinaFlash = null;
    }
}
