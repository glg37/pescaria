using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GanchoController : MonoBehaviour
{
    [Header("Movimento")]
    public float descendSpeed = 5f;
    public float ascendSpeed = 3f;
    public float horizontalSpeed = 4f;
    public float minY = -10f;
    public float minX = -5f;
    public float maxX = 5f;

    [Header("Referências")]
    public Transform cameraToFollow;
    public Transform player;
    public Transform peixeSlotNoPlayer;        // Onde o peixe aparece no jogador
    public TextMeshProUGUI raridadeText;       // Texto UI para mostrar raridade

    [Header("Offsets de Câmera")]
    public Vector3 offsetGancho = new Vector3(0f, 2f, -10f);
    public Vector3 offsetPlayer = new Vector3(0f, 1f, -10f);

    [Header("Tempo de exibição")]
    public float tempoExibicao = 3f;

    [Header("Ponto onde o peixe fica preso no gancho")]
    public Transform fishAttachPoint;

    private Vector3 startPos;
    private bool isMoving = false;
    private bool goingDown = false;
    private float fixedCameraX;
    private Action onFinishedCallback;

    private GameObject peixePego;

    void Start()
    {
        startPos = transform.position;
        if (cameraToFollow != null)
            fixedCameraX = cameraToFollow.position.x;
    }

    public void IniciarMinigameComCallback(Action callback)
    {
        transform.position = startPos;
        onFinishedCallback = callback;
        isMoving = true;
        goingDown = true;
    }

    void Update()
    {
        if (!isMoving) return;

        Vector3 pos = transform.position;

        // Movimento horizontal
        pos.x = Mathf.Clamp(pos.x + Input.GetAxisRaw("Horizontal") * horizontalSpeed * Time.deltaTime, minX, maxX);

        // Movimento vertical
        float vSpeed = goingDown ? -descendSpeed : ascendSpeed;
        pos.y += vSpeed * Time.deltaTime;

        // Verifica limites
        if (goingDown && pos.y <= minY)
        {
            pos.y = minY;
            goingDown = false;
        }
        else if (!goingDown && pos.y >= startPos.y)
        {
            FinalizarMinigame(ref pos);
            return; // Evita atualização da câmera após fim do minigame
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

        // Volta a câmera para o jogador
        if (cameraToFollow && player)
            cameraToFollow.position = player.position + offsetPlayer;

        // Mostra peixe no player
        if (peixePego != null && peixeSlotNoPlayer != null)
        {
            peixePego.transform.SetParent(peixeSlotNoPlayer);
            peixePego.transform.localPosition = Vector3.zero;
            peixePego.transform.localRotation = Quaternion.identity;
            peixePego.transform.localScale = Vector3.one;

            Fish scriptPeixe = peixePego.GetComponent<Fish>();
            if (scriptPeixe != null && raridadeText != null)
            {
                var raridade = scriptPeixe.raridade;
                raridadeText.text = $"Você pegou um peixe {raridade}!";
                raridadeText.color = CorDaRaridade(raridade);
                raridadeText.gameObject.SetActive(true);
                Invoke(nameof(EsconderRaridadeETirarPeixe), tempoExibicao);
            }
        }

        onFinishedCallback?.Invoke();
    }

    private void EsconderRaridadeETirarPeixe()
    {
        if (raridadeText != null)
            raridadeText.gameObject.SetActive(false);

        if (peixePego != null)
        {
            Destroy(peixePego);
            peixePego = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Peixe") && peixePego == null)
        {
            peixePego = other.gameObject;

            // Anexa o peixe ao ponto de fisgada do gancho
            if (fishAttachPoint != null)
            {
                peixePego.transform.SetParent(fishAttachPoint);
                peixePego.transform.localPosition = Vector3.zero;
                peixePego.transform.localRotation = Quaternion.identity;
            }
            else
            {
                // fallback caso não tenha setado o ponto
                peixePego.transform.SetParent(this.transform);
                peixePego.transform.localPosition = Vector3.zero;
            }

            peixePego.GetComponent<Collider2D>().enabled = false;
        }
    }

    Color CorDaRaridade(Fish.Raridade raridade)
    {
        switch (raridade)
        {
            case Fish.Raridade.Comum: return Color.gray;
            case Fish.Raridade.Incomum: return Color.green;
            case Fish.Raridade.Raro: return Color.blue;
            case Fish.Raridade.Épico: return new Color(0.6f, 0f, 1f);
            case Fish.Raridade.Lendário: return new Color(1f, 0.5f, 0f);
            default: return Color.white;
        }
    }
}
