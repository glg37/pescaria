using UnityEngine;

public class SistemaDePescaCompleto : MonoBehaviour
{
    [SerializeField] private GanchoController ganchoController;
    [SerializeField] private Animator animatorPersonagem;
    [SerializeField] private float tempoDaAnimacao = 0.5f;

    private bool minigameEmProgresso = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !minigameEmProgresso)
        {
            minigameEmProgresso = true;
            JogarLinha();
        }
    }

    private void JogarLinha()
    {
        animatorPersonagem?.SetTrigger("Pescar");
        Invoke(nameof(IniciarMinigame), tempoDaAnimacao);
    }

    private void IniciarMinigame()
    {
        if (ganchoController == null) return;

        if (ganchoController.Camera != null)
        {
            Vector3 target = ganchoController.transform.position + ganchoController.OffsetGancho;
            ganchoController.Camera.position = target;
        }

        ganchoController.IniciarMinigameComCallback(() =>
        {
            Debug.Log("Minigame finalizado!");
            minigameEmProgresso = false;
        });
    }

    
    public void AnimationEvent_IniciarMinigame()
    {
        IniciarMinigame();
    }
}
