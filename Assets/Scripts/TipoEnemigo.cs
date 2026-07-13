using UnityEngine;

[System.Serializable]
public class TipoEnemigo
{
    public string nombre;
    public int vida;
    public float velocidad;
    public int dano;
    public float probabilidadFallo;
    public float distanciaAtaque;
    public float distanciaDeteccion;
    public bool esSuicide;
    public bool esCongelador;
    public Color color;

    // Stats predefinidos para cada tipo de enemigo
    public static TipoEnemigo Zombi => new TipoEnemigo
    {
        nombre = "Zombi",
        vida = 3,
        velocidad = 3f,
        dano = 1,
        probabilidadFallo = 0.20f,
        distanciaAtaque = 10f,
        distanciaDeteccion = 18f,
        esSuicide = false,
        esCongelador = false,
        color = new Color(0.8f, 0.3f, 0.1f)
    };

    public static TipoEnemigo Demonio => new TipoEnemigo
    {
        nombre = "Demonio",
        vida = 5,
        velocidad = 5f,
        dano = 2,
        probabilidadFallo = 0.15f,
        distanciaAtaque = 12f,
        distanciaDeteccion = 22f,
        esSuicide = false,
        esCongelador = false,
        color = new Color(0.9f, 0.2f, 0.1f)
    };

    public static TipoEnemigo Tanque => new TipoEnemigo
    {
        nombre = "Tanque",
        vida = 10,
        velocidad = 2f,
        dano = 3,
        probabilidadFallo = 0.10f,
        distanciaAtaque = 10f,
        distanciaDeteccion = 20f,
        esSuicide = false,
        esCongelador = false,
        color = new Color(0.4f, 0.1f, 0.6f)
    };

    public static TipoEnemigo Suicide => new TipoEnemigo
    {
        nombre = "Suicide",
        vida = 1,
        velocidad = 6f,
        dano = 5,
        probabilidadFallo = 0f,
        distanciaAtaque = 2f,
        distanciaDeteccion = 35f,
        esSuicide = true,
        esCongelador = false,
        color = new Color(0.2f, 0.8f, 0.1f)
    };

    public static TipoEnemigo Corredor => new TipoEnemigo
    {
        nombre = "Corredor",
        vida = 2,
        velocidad = 8f,
        dano = 2,
        probabilidadFallo = 0.25f,
        distanciaAtaque = 3f,
        distanciaDeteccion = 30f,
        esSuicide = false,
        esCongelador = false,
        color = new Color(0f, 0.9f, 0.9f) // Cyan
    };

    // NUEVO: Congelador - Ralentiza al jugador al impactar
    public static TipoEnemigo Congelador => new TipoEnemigo
    {
        nombre = "Congelador",
        vida = 4,
        velocidad = 3f,
        dano = 1,
        probabilidadFallo = 0.10f,
        distanciaAtaque = 15f,
        distanciaDeteccion = 25f,
        esSuicide = false,
        esCongelador = true,
        color = new Color(0.3f, 0.5f, 1f) // Azul hielo
    };
}
