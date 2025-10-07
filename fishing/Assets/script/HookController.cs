using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class HookController : MonoBehaviour,IHook
{
    [Header("Movimento")]
    [SerializeField] private float descendSpeed = 5f;
    [SerializeField] private float ascendSpeed = 3f;
    [SerializeField] private float horizontalSpeed = 4f;
    [SerializeField] private float minY = -10f, minX = -5f, maxX = 5f;

    [Header("Referências")]
    [SerializeField] private Transform cameraToFollow;
    [SerializeField] private Transform player;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI textPoints;

    [Header("Offsets")]
    [SerializeField] private Vector3 offsetHook = new Vector3(0f, 2f, -10f);
    [SerializeField] private Vector3 offsetPlayer = new Vector3(0f, 1f, -10f);

    [Header("Extras")]
    [SerializeField] private Transform fishAttachPoint;
    [SerializeField] private Button botaoSoltarPeixe;
    [SerializeField] private float timeDisplay = 3f;
    [SerializeField] private Color corComum, corIncomum, corRaro, corEpico, corLendario;

    private int Totalpoints;
    private Vector3 startPos;
    private float fixedCameraX;
    private bool isMoving, goingDown, minigameActive;
    private Action onFinishedCallback;
    private GameObject fishCaught;

  
    public Vector3 StartPosition => startPos;

    public Transform Camera => cameraToFollow;
    public Vector3 OffsetHook => offsetHook;

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
        if (minigameActive) return;

        if (fishCaught != null)
        {
            Destroy(fishCaught);
            fishCaught = null;
            botaoSoltarPeixe?.gameObject.SetActive(false);
        }

        transform.position = startPos;
        onFinishedCallback = callback;
        isMoving = true;
        goingDown = true;
        minigameActive = true;
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
        cameraToFollow.position = new Vector3(fixedCameraX, transform.position.y + offsetHook.y, transform.position.z + offsetHook.z);
    }

    private void FinalizarMinigame(ref Vector3 pos)
    {
        pos.y = startPos.y;
        isMoving = false;

        if (cameraToFollow && player)
            cameraToFollow.position = player.position + offsetPlayer;

        if (fishCaught != null)
        {
            Fish scriptPeixe = fishCaught.GetComponent<Fish>();
            if (scriptPeixe != null && rarityText != null)
            {
                int pontos = PontosPorRaridade(scriptPeixe.TipoRaridade);
                Totalpoints += pontos;
                textPoints.text = $"Pontos: {Totalpoints}";

                rarityText.text = $"Você pegou um peixe {scriptPeixe.TipoRaridade}!";
                rarityText.color = CorDaRaridade(scriptPeixe.TipoRaridade);
                rarityText.gameObject.SetActive(true);
                Invoke(nameof(EsconderTextoRaridade), timeDisplay);
            }
            botaoSoltarPeixe?.gameObject.SetActive(true);
        }

        minigameActive = false;
        onFinishedCallback?.Invoke();
    }

    private void EsconderTextoRaridade()
    {
        rarityText?.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isMoving || fishCaught != null || !other.CompareTag("Peixe")) return;

        fishCaught = other.gameObject;
        Fish scriptPeixe = fishCaught.GetComponent<Fish>();
        scriptPeixe?.Pegar();

        fishCaught.transform.SetParent(fishAttachPoint != null ? fishAttachPoint : transform);
        fishCaught.transform.localPosition = Vector3.zero;
        fishCaught.transform.localRotation = Quaternion.identity;

        var rb = fishCaught.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        var col = fishCaught.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    private void SoltarPeixe()
    {
        if (fishCaught == null) return;

        Fish scriptPeixe = fishCaught.GetComponent<Fish>();
        if (scriptPeixe != null)
        {
            Totalpoints -= PontosPorRaridade(scriptPeixe.TipoRaridade);
            if (Totalpoints < 0) Totalpoints = 0;
            textPoints.text = $"Pontos: {Totalpoints}";

            fishCaught.transform.SetParent(null);
            fishCaught.transform.position = scriptPeixe.GetPosicaoOriginal();
            fishCaught.transform.rotation = scriptPeixe.GetRotacaoOriginal();
            fishCaught.transform.localScale = scriptPeixe.GetEscalaOriginal();

            var rb = fishCaught.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.simulated = true;
            }

            var col = fishCaught.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            scriptPeixe.Soltar();
        }

        fishCaught = null;
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
