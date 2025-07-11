using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class GanchoController : MonoBehaviour
{
    [Header("Movimento")]
    public float descendSpeed = 5f;
    public float ascendSpeed = 3f;
    public float horizontalSpeed = 4f;
    public float minY = -10f;
    public float minX = -5f;
    public float maxX = 5f;

    [Header("Refer�ncias")]
    public Transform cameraToFollow;
    public Transform player;
    public TextMeshProUGUI raridadeText;

    [Header("Offsets de C�mera")]
    public Vector3 offsetGancho = new Vector3(0f, 2f, -10f);
    public Vector3 offsetPlayer = new Vector3(0f, 1f, -10f);

    [Header("Tempo de exibi��o")]
    public float tempoExibicao = 3f;

    [Header("Ponto onde o peixe aparece preso no gancho")]
    public Transform fishAttachPoint;

    [Header("Pontua��o")]
    public int pontuacaoTotal = 0;
    public TextMeshProUGUI textoPontuacao;

    [Header("Cores das Raridades")]
    public Color corComum = Color.gray;
    public Color corIncomum = Color.green;
    public Color corRaro = Color.blue;
    public Color corEpico = new Color(0.6f, 0f, 1f);
    public Color corLendario = new Color(1f, 0.5f, 0f);

    [Header("Bot�o de Soltar Peixe")]
    public Button botaoSoltarPeixe;

    private Vector3 startPos;
    private bool isMoving = false;
    private bool goingDown = false;
    private bool minigameAtivo = false;
    private float fixedCameraX;
    private Action onFinishedCallback;
    private GameObject peixePego;

    void Start()
    {
        startPos = transform.position;
        if (cameraToFollow != null)
            fixedCameraX = cameraToFollow.position.x;

        if (botaoSoltarPeixe != null)
        {
            botaoSoltarPeixe.gameObject.SetActive(false);
            botaoSoltarPeixe.onClick.AddListener(SoltarPeixe);
        }
    }

    public void IniciarMinigameComCallback(Action callback)
    {
        if (minigameAtivo) return;

        
        if (peixePego != null)
        {
            Destroy(peixePego);
            peixePego = null;

            if (botaoSoltarPeixe != null)
                botaoSoltarPeixe.gameObject.SetActive(false);
        }

        transform.position = startPos;
        onFinishedCallback = callback;
        isMoving = true;
        goingDown = true;
        minigameAtivo = true;
    }


    void Update()
    {

        if (!isMoving) return;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x + Input.GetAxisRaw("Horizontal") * horizontalSpeed * Time.deltaTime, minX, maxX);

        float vSpeed = goingDown ? -descendSpeed : ascendSpeed;
        pos.y += vSpeed * Time.deltaTime;

        if (goingDown && pos.y <= minY)
        {
            pos.y = minY;
            goingDown = false;
        }
        else if (!goingDown && pos.y >= startPos.y)
        {
            FinalizarMinigame(ref pos);
            return;
        }

        transform.position = pos;
        AtualizarCameraY();
    }

    void AtualizarCameraY()
    {
        if (!cameraToFollow) return;

        cameraToFollow.position = new Vector3(
            fixedCameraX,
            transform.position.y + offsetGancho.y,
            transform.position.z + offsetGancho.z
        );
    }

    void FinalizarMinigame(ref Vector3 pos)
    {
        pos.y = startPos.y;
        isMoving = false;

        if (cameraToFollow && player)
            cameraToFollow.position = player.position + offsetPlayer;

        if (peixePego != null)
        {
            Fish scriptPeixe = peixePego.GetComponent<Fish>();
            if (scriptPeixe != null && raridadeText != null)
            {
                var raridade = scriptPeixe.raridade;

                int pontos = PontosPorRaridade(raridade);
                pontuacaoTotal += pontos;
                if (textoPontuacao != null)
                    textoPontuacao.text = $"Pontos: {pontuacaoTotal}";

                raridadeText.text = $"Voc� pegou um peixe {raridade}!";
                raridadeText.color = CorDaRaridade(raridade);
                raridadeText.gameObject.SetActive(true);
                Invoke(nameof(EsconderTextoRaridade), tempoExibicao);
            }

            if (botaoSoltarPeixe != null)
                botaoSoltarPeixe.gameObject.SetActive(true);
        }

        minigameAtivo = false;
        onFinishedCallback?.Invoke();
    }

    void EsconderTextoRaridade()
    {
        if (raridadeText != null)
            raridadeText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isMoving && other.CompareTag("Peixe") && peixePego == null)
        {
            peixePego = other.gameObject;

            Fish scriptPeixe = peixePego.GetComponent<Fish>();
            if (scriptPeixe != null)
            {
                scriptPeixe.Pegar();
            }

            if (fishAttachPoint != null)
            {
                peixePego.transform.SetParent(fishAttachPoint);
                peixePego.transform.localPosition = Vector3.zero;
                peixePego.transform.localRotation = Quaternion.identity;
            }
            else
            {
                peixePego.transform.SetParent(this.transform);
                peixePego.transform.localPosition = Vector3.zero;
            }

            var rb = peixePego.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = false;
            }

            var col = peixePego.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

           
        }
    }


    void SoltarPeixe()
    {
        if (peixePego != null)
        {
            Fish scriptPeixe = peixePego.GetComponent<Fish>();
            if (scriptPeixe != null)
            {
                // **Subtrair pontos ao soltar**
                int pontosASubtrair = PontosPorRaridade(scriptPeixe.raridade);
                pontuacaoTotal -= pontosASubtrair;
                if (pontuacaoTotal < 0) pontuacaoTotal = 0; // N�o deixar negativo

                if (textoPontuacao != null)
                    textoPontuacao.text = $"Pontos: {pontuacaoTotal}";

                // Restaura posi��o, rota��o e escala originais
                peixePego.transform.SetParent(null);
                peixePego.transform.position = scriptPeixe.GetPosicaoOriginal();
                peixePego.transform.rotation = scriptPeixe.GetRotacaoOriginal();
                peixePego.transform.localScale = scriptPeixe.GetEscalaOriginal();

                // Reativa o Rigidbody2D
                var rb = peixePego.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.simulated = true;
                }

                // Reativa o collider
                var col = peixePego.GetComponent<Collider2D>();
                if (col != null)
                    col.enabled = true;

                // Permite o peixe voltar a se mover normalmente
                scriptPeixe.Soltar();
            }

            // Remove refer�ncia ao peixe preso
            peixePego = null;

            // Esconde o bot�o de soltar
            if (botaoSoltarPeixe != null)
                botaoSoltarPeixe.gameObject.SetActive(false);
        }
    }




    Color CorDaRaridade(Fish.Raridade raridade)
    {
        switch (raridade)
        {
            case Fish.Raridade.Comum: return corComum;
            case Fish.Raridade.Incomum: return corIncomum;
            case Fish.Raridade.Raro: return corRaro;
            case Fish.Raridade.�pico: return corEpico;
            case Fish.Raridade.Lend�rio: return corLendario;
            default: return Color.white;
        }
    }

    int PontosPorRaridade(Fish.Raridade raridade)
    {
        switch (raridade)
        {
            case Fish.Raridade.Comum: return 10;
            case Fish.Raridade.Incomum: return 25;
            case Fish.Raridade.Raro: return 50;
            case Fish.Raridade.�pico: return 100;
            case Fish.Raridade.Lend�rio: return 250;
            default: return 0;
        }
    }
}
