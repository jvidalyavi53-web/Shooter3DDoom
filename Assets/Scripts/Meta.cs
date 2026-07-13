using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Meta : MonoBehaviour
{
    private void Awake()
    {
        // Forzar a que el colisionador de la meta sea Trigger por defecto
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comprobar si el que colisiona es el jugador
        Vida jugadorVida = other.GetComponentInParent<Vida>();
        if (jugadorVida != null && jugadorVida.esJugador)
        {
            if (GameManager.Instancia != null)
            {
                GameManager.Instancia.IntentarCompletarNivel();
            }
        }
    }
}
