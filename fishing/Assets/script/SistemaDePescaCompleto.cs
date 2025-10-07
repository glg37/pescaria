using UnityEngine;
using TMPro;

public class SistemaDePescaCompleto : MonoBehaviour, IFishingMinigame
{
    [SerializeField] private HookController HookController;
    [SerializeField] private Animator animatorCharacter;
    [SerializeField] private float tempoDaAnimacao = 0.5f;

    [SerializeField] private TextMeshProUGUI textTutorial;
    [SerializeField] private float timeTutorial = 5f;

    private bool minigameInProgress = false;
    private bool tutorialDisplayed = false; 

    private void Start()
    {
        if (textTutorial != null)
        {
            textTutorial.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
       
        if (!tutorialDisplayed && !minigameInProgress && HookStopped())
        {
            MostrarTutorial();
        }

        if (Input.GetMouseButtonDown(0) && !minigameInProgress)
        {
            minigameInProgress = true;
            Playline();
        }
    }

    private bool HookStopped()
    {
        if (HookController == null) return false;

        Vector3 posAtual = HookController.transform.position;
        Vector3 posInicial = HookController.StartPosition; 

    
        float margem = 0.1f;
        return Vector3.Distance(posAtual, posInicial) < margem;
    }

    private void Playline()
    {
        if (textTutorial != null)
        {
            textTutorial.gameObject.SetActive(false); 
        }

        animatorCharacter?.SetTrigger("Pescar");
        Invoke(nameof(IniciarMinigame), tempoDaAnimacao);
    }

    private void IniciarMinigame()
    {
        if (HookController == null) return;

        if (HookController.Camera != null)
        {
            Vector3 target = HookController.transform.position + HookController.OffsetHook;
            HookController.Camera.position = target;
        }

        HookController.IniciarMinigameComCallback(() =>
        {
            Debug.Log("Minigame finalizado!");
            minigameInProgress = false;
        });
    }

    private void MostrarTutorial()
    {
        if (textTutorial != null)
        {
            textTutorial.gameObject.SetActive(true);
            tutorialDisplayed = true;
            Invoke(nameof(EsconderTutorial), timeTutorial);
        }
    }

    private void EsconderTutorial()
    {
        if (textTutorial != null)
            textTutorial.gameObject.SetActive(false);
    }

    public void AnimationEvent_IniciarMinigame()
    {
        IniciarMinigame();
    }
}
