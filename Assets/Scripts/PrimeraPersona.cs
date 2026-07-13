using UnityEngine;
using System.Collections;

public class PrimeraPersona : MonoBehaviour
{
    [Header("Movimiento (Estilo DOOM)")]
    public float velocidad = 10f;
    public float velocidadSprint = 14f;
    public float sensibilidad = 2f;
    public float gravedad = -20f;

    [Header("Cámara")]
    public Transform camara;

    [Header("Head Bob")]
    public float headBobFrecuencia = 12f;
    public float headBobAmplitud = 0.05f;

    [Header("Screen Shake")]
    private float shakeAmount = 0f;
    private float shakeDuration = 0f;

    // Estado de ralentización
    private bool estaRalentizado = false;
    private float multiplicadorVelocidad = 1f;

    private CharacterController cc;
    private float pitch = 0f;
    private Vector3 velY;
    private float headBobTimer = 0f;
    private float camaraYBase;
    private bool enMovimiento = false;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Auto-detectar cámara si no está asignada
        if (camara == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) camara = cam.transform;
            else if (Camera.main != null) camara = Camera.main.transform;
        }

        if (camara != null)
            camaraYBase = camara.localPosition.y;

        // Suscribirse a evento de daño para screen shake
        Vida.OnDanoRecibidoJugador += OnRecibirDano;
    }

    void OnDestroy()
    {
        Vida.OnDanoRecibidoJugador -= OnRecibirDano;
    }

    void Update()
    {
        if (GameManager.Instancia != null && GameManager.Instancia.EsJuegoTerminado()) return;

        // MIRAR
        float mx = Input.GetAxis("Mouse X") * sensibilidad;
        float my = Input.GetAxis("Mouse Y") * sensibilidad;
        transform.Rotate(0, mx, 0);
        pitch = Mathf.Clamp(pitch - my, -80f, 80f);

        // MOVIMIENTO (aplica multiplicador de ralentización)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float velActual = Input.GetKey(KeyCode.LeftShift) ? velocidadSprint : velocidad;
        velActual *= multiplicadorVelocidad;
        Vector3 mov = (transform.right * h + transform.forward * v).normalized * velActual;

        enMovimiento = mov.magnitude > 0.1f && cc.isGrounded;

        // GRAVEDAD
        if (cc.isGrounded && velY.y < 0) velY.y = -2f;
        velY.y += gravedad * Time.deltaTime;

        cc.Move((mov + velY) * Time.deltaTime);

        // HEAD BOB
        if (camara != null)
        {
            if (enMovimiento)
            {
                headBobTimer += Time.deltaTime * headBobFrecuencia;
                float bobOffset = Mathf.Sin(headBobTimer) * headBobAmplitud;
                camara.localPosition = new Vector3(camara.localPosition.x, camaraYBase + bobOffset, camara.localPosition.z);
            }
            else
            {
                headBobTimer = 0f;
                float newY = Mathf.Lerp(camara.localPosition.y, camaraYBase, Time.deltaTime * 8f);
                camara.localPosition = new Vector3(camara.localPosition.x, newY, camara.localPosition.z);
            }

            // Screen Shake
            Vector3 shakeOffset = Vector3.zero;
            if (shakeDuration > 0)
            {
                shakeDuration -= Time.deltaTime;
                shakeOffset = new Vector3(
                    Random.Range(-shakeAmount, shakeAmount),
                    Random.Range(-shakeAmount, shakeAmount),
                    0
                ) * Time.deltaTime;
            }

            camara.localEulerAngles = new Vector3(pitch, 0, 0) + shakeOffset * 100f;
        }
    }

    private void OnRecibirDano()
    {
        ActivarScreenShake(0.15f, 3f);
    }

    public void ActivarScreenShake(float duracion, float intensidad)
    {
        shakeDuration = duracion;
        shakeAmount = intensidad;
    }

    /// <summary>
    /// Aplica efecto de ralentización al jugador por una duración determinada.
    /// </summary>
    public void AplicarLentitud(float duracion, float multiplicador)
    {
        if (estaRalentizado) return; // No acumular efecto
        StartCoroutine(RutinaLentitud(duracion, multiplicador));
    }

    private IEnumerator RutinaLentitud(float duracion, float multiplicador)
    {
        estaRalentizado = true;
        multiplicadorVelocidad = multiplicador;

        // Mostrar alerta visual
        if (UIManager.Instancia != null)
            UIManager.Instancia.MostrarMensajeAlerta("¡RALENTIZADO!");

        yield return new WaitForSeconds(duracion);

        // Restaurar velocidad
        multiplicadorVelocidad = 1f;
        estaRalentizado = false;
    }
}
