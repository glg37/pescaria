using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GanchoController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float descendSpeed = 5f;
    [SerializeField] private float ascendSpeed = 3f;
    [SerializeField] private float horizontalSpeed = 4f;
    [SerializeField] private float minY = -10f, minX = -5f, maxX = 5f;

    [Header("Referências")]
    [SerializeField] private Transform cameraToFollow;
    [SerializeField] private Transform player;
    [SerializeField] private TextMeshProUGUI raridadeText;
    [SerializeField] private TextMeshProUGUI textoPontuacao;

    [Header("Offsets")]
    [SerializeField] private Vector3 offsetGancho = new Vector3(0f, 2f, -10f);
    [SerializeField] private Vector3 offsetPlayer = new Vector3(0f, 1f, -10f);

    [Header("Extras")]
    [SerializeField] private Transform fishAttachPoint;
    [SerializeField] private Button botaoSoltarPeixe;
    [SerializeField] private float tempoExibicao = 3f;
    [SerializeField] private Color corComum, corIncomum, corRaro, corEpico, corLendario;

    private int pontuacaoTotal;
    private Vector3 startPos;
    private float fixedCameraX;
    private bool isMoving, goingDown, minigameAtivo;
    private Action onFinishedCallback;
    private GameObject peixePego;

    public Transform Camera => cameraToFollow;
    public Vector3 OffsetGancho => offsetGancho;

    private void Start()
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
            botaoSoltarPeixe?.gameObject.SetActive(false);
        }

        transform.position = startPos;
        onFinishedCallback = callback;
        isMoving = true;
        goingDown = true;
        minigameAtivo = true;
    }

    private void Update()
    {
        if (!isMoving) return;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + Input.GetAxisRaw("Horizontal") * horizontalSpeed * Time.deltaTime, minX, maxX);
        pos.y += (goingDown ? -descendSpeed : ascendSpeed) * Time.deltaTime;

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

    private void AtualizarCameraY()
    {
        if (cameraToFollow == null) return;
        cameraToFollow.position = new Vector3(fixedCameraX, transform.position.y + offsetGancho.y, transform.position.z + offsetGancho.z);
    }

    private void FinalizarMinigame(ref Vector3 pos)
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
                int pontos = PontosPorRaridade(scriptPeixe.TipoRaridade);
                pontuacaoTotal += pontos;
                textoPontuacao.text = $"Pontos: {pontuacaoTotal}";

                raridadeText.text = $"Você pegou um peixe {scriptPeixe.TipoRaridade}!";
                raridadeText.color = CorDaRaridade(scriptPeixe.TipoRaridade);
                raridadeText.gameObject.SetActive(true);
                Invoke(nameof(EsconderTextoRaridade), tempoExibicao);
            }
            botaoSoltarPeixe?.gameObject.SetActive(true);
        }

        minigameAtivo = false;
        onFinishedCallback?.Invoke();
    }

    private void EsconderTextoRaridade()
    {
        raridadeText?.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isMoving || peixePego != null || !other.CompareTag("Peixe")) return;

        peixePego = other.gameObject;
        Fish scriptPeixe = peixePego.GetComponent<Fish>();
        scriptPeixe?.Pegar();

        peixePego.transform.SetParent(fishAttachPoint != null ? fishAttachPoint : transform);
        peixePego.transform.localPosition = Vector3.zero;
        peixePego.transform.localRotation = Quaternion.identity;

        var rb = peixePego.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        var col = peixePego.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    private void SoltarPeixe()
    {
        if (peixePego == null) return;

        Fish scriptPeixe = peixePego.GetComponent<Fish>();
        if (scriptPeixe != null)
        {
            pontuacaoTotal -= PontosPorRaridade(scriptPeixe.TipoRaridade);
            if (pontuacaoTotal < 0) pontuacaoTotal = 0;
            textoPontuacao.text = $"Pontos: {pontuacaoTotal}";

            peixePego.transform.SetParent(null);
            peixePego.transform.position = scriptPeixe.GetPosicaoOriginal();
            peixePego.transform.rotation = scriptPeixe.GetRotacaoOriginal();
            peixePego.transform.localScale = scriptPeixe.GetEscalaOriginal();

            var rb = peixePego.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.simulated = true;
            }

            var col = peixePego.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            scriptPeixe.Soltar();
        }

        peixePego = null;
        botaoSoltarPeixe?.gameObject.SetActive(false);
    }

    private Color CorDaRaridade(Fish.Raridade raridade)
    {
        return raridade switch
        {
            Fish.Raridade.Comum => corComum,
            Fish.Raridade.Incomum => corIncomum,
            Fish.Raridade.Raro => corRaro,
            Fish.Raridade.Épico => corEpico,
            Fish.Raridade.Lendário => corLendario,
            _ => Color.white
        };
    }

    private int PontosPorRaridade(Fish.Raridade raridade)
    {
        return raridade switch
        {
            Fish.Raridade.Comum => 10,
            Fish.Raridade.Incomum => 25,
            Fish.Raridade.Raro => 50,
            Fish.Raridade.Épico => 100,
            Fish.Raridade.Lendário => 250,
            _ => 0
        };
    }
}