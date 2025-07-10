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
            
            if (ganchoController.cameraToFollow != null)
            {
                Vector3 target = ganchoController.transform.position + ganchoController.offsetGancho;
                ganchoController.cameraToFollow.position = target;
            }

           
            ganchoController.IniciarMinigameComCallback(() =>
            {
                Debug.Log("Minigame finalizado!");
               
            });
        }
    }
}
