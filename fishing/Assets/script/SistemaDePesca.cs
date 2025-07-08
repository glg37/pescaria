using UnityEngine;
using TMPro;

public class SistemaDePesca : MonoBehaviour
{
    public Animator animator;
    public GameObject peixeRaroPrefab;
    public GameObject peixeEpicoPrefab;
    public GameObject peixeLendarioPrefab;

    public Transform localAparicaoPeixe;
    public TextMeshProUGUI textoRaridade;

    private bool podePescar = true;
    private GameObject peixeAtual;

    // Variáveis para armazenar o resultado da pesca
    private GameObject peixeSorteado;
    private string raridadeSorteada;

    void Start()
    {
        textoRaridade.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && podePescar)
        {
            Pescar();
        }
    }

    void Pescar()
    {
        podePescar = false;

        // Ativa a animação de pescar
        animator.SetTrigger("Pescar");

        // Sorteia o peixe (mas ainda não instancia)
        float chance = Random.value;

        if (chance < 0.6f)
        {
            peixeSorteado = peixeRaroPrefab;
            raridadeSorteada = "Raro";
        }
        else if (chance < 0.9f)
        {
            peixeSorteado = peixeEpicoPrefab;
            raridadeSorteada = "Épico";
        }
        else
        {
            peixeSorteado = peixeLendarioPrefab;
            raridadeSorteada = "Lendário";
        }

        // Aguarda 1 segundo antes de mostrar peixe e texto
        Invoke(nameof(MostrarPeixeERaridade), 2f);
    }

    void MostrarPeixeERaridade()
    {
        peixeAtual = Instantiate(peixeSorteado, localAparicaoPeixe.position, Quaternion.identity);
        textoRaridade.text = $"Você pescou um peixe {raridadeSorteada}!";
        textoRaridade.gameObject.SetActive(true);

        // Esconde peixe/texto depois de um tempo
        Invoke(nameof(EsconderPeixeERaridade), 1f);
    }

    void EsconderPeixeERaridade()
    {
        if (peixeAtual != null) Destroy(peixeAtual);
        textoRaridade.gameObject.SetActive(false);
        podePescar = true;
    }
}
