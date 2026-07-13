using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camaraTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            camaraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (camaraTransform == null) return;

        // Hace que el objeto mire siempre a la cámara (billboard effect)
        transform.LookAt(camaraTransform);
        transform.Rotate(0, 180, 0); // Rotar 180 para que el frente mire a la cámara
    }
}
