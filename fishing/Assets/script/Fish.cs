using UnityEngine;

public class Fish : MonoBehaviour
{
    public enum Raridade
    {
        Comum,
        Incomum,
        Raro,
        Épico,
        Lendário
    }

    public Raridade raridade = Raridade.Comum;

    public float moveSpeed = 2f;
    public float limiteDistancia = 30f;

    private bool foiPego = false;
    private Vector3 pontoInicial;
    private Vector3 direcao;

    // Novas variáveis para restaurar o peixe ao estado original
    private Quaternion rotacaoOriginal;
    private Vector3 escalaOriginal;

    void Start()
    {
        pontoInicial = transform.position;
        rotacaoOriginal = transform.rotation;
        escalaOriginal = transform.localScale;

        if (transform.position.x >= 0)
            direcao = Vector3.left;
        else
            direcao = Vector3.right;

        int peixeLayer = LayerMask.NameToLayer("Peixe");
        Physics2D.IgnoreLayerCollision(peixeLayer, peixeLayer, true);
    }

    void Update()
    {
        if (foiPego) return;

        transform.position += direcao * moveSpeed * Time.deltaTime;

        float distancia = Vector3.Distance(transform.position, pontoInicial);
        if (distancia >= limiteDistancia)
        {
            transform.position = pontoInicial;
        }
    }

    public void Pegar()
    {
        foiPego = true;
    }

    public void Soltar()
    {
        foiPego = false;
    }

    public Vector3 GetPosicaoOriginal()
    {
        return pontoInicial;
    }

    public Quaternion GetRotacaoOriginal()
    {
        return rotacaoOriginal;
    }

    public Vector3 GetEscalaOriginal()
    {
        return escalaOriginal;
    }
}
