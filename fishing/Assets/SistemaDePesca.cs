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
    private float tempoUltimoClique = 0f;
    private float intervaloClique = 0.3f;
    private GameObject peixeAtual;

    void Start()
    {
        textoRaridade.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && podePescar)
        {
            if (Time.time - tempoUltimoClique < intervaloClique)
            {
                // Clique duplo detectado
                Pescar();
            }

            tempoUltimoClique = Time.time;
        }
    }

    void Pescar()
    {
        podePescar = false;

        // Toca a animação de pesca
        animator.SetTrigger("Pescar");

        // Sorteia peixe imediatamente
        float chance = Random.value;
        GameObject prefabEscolhido = null;
        string raridade = "";

        if (chance < 0.6f)
        {
            prefabEscolhido = peixeRaroPrefab;
            raridade = "Raro";
        }
        else if (chance < 0.9f)
        {
            prefabEscolhido = peixeEpicoPrefab;
            raridade = "Épico";
        }
        else
        {
            prefabEscolhido = peixeLendarioPrefab;
            raridade = "Lendário";
        }

        // Instancia o peixe e mostra o texto imediatamente
        peixeAtual = Instantiate(prefabEscolhido, localAparicaoPeixe.position, Quaternion.identity);
        textoRaridade.text = $"Você pescou um peixe {raridade}!";
        textoRaridade.gameObject.SetActive(true);

        // Começa o timer para esconder depois
        Invoke(nameof(EsconderPeixeERaridade), 2f);
    }

    void EsconderPeixeERaridade()
    {
        if (peixeAtual != null) Destroy(peixeAtual);
        textoRaridade.gameObject.SetActive(false);
        podePescar = true;
    }
}
