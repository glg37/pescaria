using UnityEngine;

public class Fish : MonoBehaviour, IFishing 
{
    public enum Raridade
    {
        Comum,
        Incomum,
        Raro,
        Épico,
        Lendário
    }

    [SerializeField] private Raridade raridade = Raridade.Comum;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float limiteDistancia = 30f;

    private bool foiPego = false;
    private Vector3 pontoInicial;
    private Vector3 direcao;

    private Quaternion rotacaoOriginal;
    private Vector3 escalaOriginal;

    public Raridade TipoRaridade => raridade;

    private void Start()
    {
        pontoInicial = transform.position;
        rotacaoOriginal = transform.rotation;
        escalaOriginal = transform.localScale;

        direcao = (transform.position.x >= 0) ? Vector3.left : Vector3.right;

        int peixeLayer = LayerMask.NameToLayer("Peixe");
        Physics2D.IgnoreLayerCollision(peixeLayer, peixeLayer, true);
    }

    private void Update()
    {
        if (foiPego) return;

        transform.position += direcao * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, pontoInicial) >= limiteDistancia)
        {
            transform.position = pontoInicial;
        }
    }

    public void Pegar() => foiPego = true;
    public void Soltar() => foiPego = false;

    public Vector3 GetPosicaoOriginal() => pontoInicial;
    public Quaternion GetRotacaoOriginal() => rotacaoOriginal;
    public Vector3 GetEscalaOriginal() => escalaOriginal;
}
