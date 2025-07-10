using UnityEngine;

public class SistemaDePescaCompleto : MonoBehaviour
{
    public GanchoController ganchoController;
    public Animator animatorPersonagem;
    public float tempoDaAnimacao = 0.5f; 

    private bool minigameEmProgresso = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !minigameEmProgresso)
        {
            minigameEmProgresso = true;
            JogarLinha();
        }
    }

    void JogarLinha()
    {
        if (animatorPersonagem != null)
        {
            animatorPersonagem.SetTrigger("Pescar");
        }

        Invoke(nameof(IniciarMinigame), tempoDaAnimacao);
    }

    void IniciarMinigame()
    {
        if (ganchoController != null)
        {
            if (ganchoController.cameraToFollow != null)
            {
                Vector3 target = ganchoController.transform.position + ganchoController.offsetGancho;
                ganchoController.cameraToFollow.position = target;
            }

            ganchoController.IniciarMinigameComCallback(() =>
            {
                Debug.Log("Minigame finalizado!");
                minigameEmProgresso = false;
            });
        }
    }
}
