using UnityEngine;

/// Classe base de tudo que o aspirador pode sugar (fantasmas, etc.).
/// Depois: class Fantasma : Sugavel, class FantasmaEspecial : Sugavel... (herança/polimorfismo).
public class Sugavel : MonoBehaviour
{
    [SerializeField] float tempoParaCapturar = 1.5f;
    [SerializeField] float velocidadePuxada = 3f;
    [SerializeField] float recuperacao = 1f;

    float progresso;
    int ultimoFrameSugado = -10;
    Vector3 escalaInicial;

    public float Progresso01 => Mathf.Clamp01(progresso / tempoParaCapturar);

    protected virtual void Awake() => escalaInicial = transform.localScale;

    protected virtual void Update()
    {
        bool sendoSugado = Time.frameCount - ultimoFrameSugado <= 1;
        if (!sendoSugado && progresso > 0f)
            progresso = Mathf.Max(0f, progresso - recuperacao * Time.deltaTime);

        // encolhe enquanto é sugado (feedback visual provisório)
        transform.localScale = Vector3.Lerp(escalaInicial, escalaInicial * 0.2f, Progresso01);
    }

    public virtual void ReceberSucao(Vector3 posicaoBocal, float dt)
    {
        ultimoFrameSugado = Time.frameCount;
        progresso += dt;
        transform.position = Vector3.MoveTowards(transform.position, posicaoBocal, velocidadePuxada * dt);
        if (progresso >= tempoParaCapturar) Capturar();
    }

    protected virtual void Capturar() => Destroy(gameObject);
}
