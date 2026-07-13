using UnityEngine;

public class SonidoManager : MonoBehaviour
{
    public static SonidoManager Instancia { get; private set; }

    [Header("Sonidos")]
    public AudioClip sonidoDisparoPistola;
    public AudioClip sonidoDisparoEscopeta;
    public AudioClip sonidoDisparoAmetralladora;
    public AudioClip sonidoDanoJugador;
    public AudioClip sonidoDanoEnemigo;
    public AudioClip sonidoVictoria;
    public AudioClip sonidoRecarga;
    public AudioClip sonidoSinBala;

    private AudioSource audioSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCrear()
    {
        if (Instancia != null) return;

        GameObject go = new GameObject("-- SonidoManager --");
        SonidoManager sm = go.AddComponent<SonidoManager>();
        sm.CargarSonidos();
    }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void CargarSonidos()
    {
        sonidoDisparoPistola = Resources.Load<AudioClip>("Sonidos/disparo");
        sonidoDisparoEscopeta = Resources.Load<AudioClip>("Sonidos/dsshotgn");
        sonidoDisparoAmetralladora = Resources.Load<AudioClip>("Sonidos/disparo");
        sonidoDanoJugador = Resources.Load<AudioClip>("Sonidos/jugador_dano");
        sonidoDanoEnemigo = Resources.Load<AudioClip>("Sonidos/enemigo_dano");
        sonidoVictoria = Resources.Load<AudioClip>("Sonidos/victoria");
        sonidoRecarga = Resources.Load<AudioClip>("Sonidos/dsdbload");
        sonidoSinBala = Resources.Load<AudioClip>("Sonidos/dsdbopn");
    }

    public static void ReproducirDisparo(int armaIndex)
    {
        if (Instancia == null || Instancia.audioSource == null) return;

        AudioClip clip = null;
        switch (armaIndex)
        {
            case 0: clip = Instancia.sonidoDisparoPistola; break;
            case 1: clip = Instancia.sonidoDisparoEscopeta; break;
            case 2: clip = Instancia.sonidoDisparoAmetralladora; break;
        }

        if (clip != null)
            Instancia.audioSource.PlayOneShot(clip);
    }

    public static void ReproducirDanoJugador()
    {
        if (Instancia == null || Instancia.sonidoDanoJugador == null) return;
        Instancia.audioSource.PlayOneShot(Instancia.sonidoDanoJugador);
    }

    public static void ReproducirDanoEnemigo()
    {
        if (Instancia == null || Instancia.sonidoDanoEnemigo == null) return;
        Instancia.audioSource.PlayOneShot(Instancia.sonidoDanoEnemigo);
    }

    public static void ReproducirVictoria()
    {
        if (Instancia == null || Instancia.sonidoVictoria == null) return;
        Instancia.audioSource.PlayOneShot(Instancia.sonidoVictoria);
    }

    public static void ReproducirRecarga()
    {
        if (Instancia == null || Instancia.sonidoRecarga == null) return;
        Instancia.audioSource.PlayOneShot(Instancia.sonidoRecarga);
    }

    public static void ReproducirSinBala()
    {
        if (Instancia == null || Instancia.sonidoSinBala == null) return;
        Instancia.audioSource.PlayOneShot(Instancia.sonidoSinBala);
    }
}
