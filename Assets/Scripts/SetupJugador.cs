using UnityEngine;

public class SetupJugador : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Setup()
    {
        // Esperar un frame para que la escena esté lista
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) return;

        // Agregar ArmaManager si no existe
        if (jugador.GetComponent<ArmaManager>() == null)
        {
            ArmaManager arma = jugador.AddComponent<ArmaManager>();

            // Auto-detectar cámara
            Camera cam = jugador.GetComponentInChildren<Camera>();
            if (cam != null)
                arma.camara = cam;
            else
                arma.camara = Camera.main;
        }

        // Asegurar que PrimeraPersona tenga referencia a cámara
        PrimeraPersona pp = jugador.GetComponent<PrimeraPersona>();
        if (pp != null && pp.camara == null)
        {
            Camera cam = jugador.GetComponentInChildren<Camera>();
            if (cam != null)
                pp.camara = cam.transform;
        }
    }
}
