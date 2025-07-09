using UnityEngine;

public class SistemaDePescaSimplificado : MonoBehaviour
{
    public GanchoController ganchoController;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IniciarMinigame();
        }
    }

    void IniciarMinigame()
    {
        if (ganchoController != null)
        {
            // Move a câmera instantaneamente para o gancho
            if (ganchoController.cameraToFollow != null)
            {
                Vector3 target = ganchoController.transform.position + ganchoController.offsetGancho;
                ganchoController.cameraToFollow.position = target;
            }

            // Inicia o minigame
            ganchoController.IniciarMinigameComCallback(() =>
            {
                Debug.Log("Minigame finalizado!");
                // Aqui você pode colocar código para mostrar peixe depois
            });
        }
    }
}
