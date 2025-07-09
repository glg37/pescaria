using UnityEngine;

public class GanchoController : MonoBehaviour
{
    public float descendSpeed = 5f;
    public float ascendSpeed = 3f;
    public float horizontalSpeed = 4f;
    public float minY = -10f;

    public Transform cameraToFollow;
    public Vector3 offsetGancho = new Vector3(0f, 2f, -10f);  // offset quando segue o gancho
    public Vector3 offsetPlayer = new Vector3(0f, 1f, -10f);  // offset quando volta pro player

    public Transform player;

    private Vector3 startPos;
    private bool isDescending = false;
    private bool isAscending = false;

    private float fixedCameraX;

    void Start()
    {
        startPos = transform.position;

        if (cameraToFollow == null)
        {
            Debug.LogWarning("CameraToFollow não atribuída.");
        }
        else
        {
            fixedCameraX = cameraToFollow.position.x;
        }
    }

    void Update()
    {
        if (!isDescending && !isAscending && Input.GetMouseButtonDown(0))
        {
            isDescending = true;
        }

        if (isDescending)
        {
            Descend();
        }
        else if (isAscending)
        {
            Ascend();
        }

        if (isDescending || isAscending)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            Vector3 pos = transform.position;
            pos.x += moveX * horizontalSpeed * Time.deltaTime;
            transform.position = pos;

            MoveCameraVerticalOnly();
        }
    }

    void Descend()
    {
        Vector3 pos = transform.position;
        pos.y -= descendSpeed * Time.deltaTime;

        if (pos.y <= minY)
        {
            pos.y = minY;
            isDescending = false;
            isAscending = true;
        }

        transform.position = pos;
    }

    void Ascend()
    {
        Vector3 pos = transform.position;
        pos.y += ascendSpeed * Time.deltaTime;

        if (pos.y >= startPos.y)
        {
            pos.y = startPos.y;
            isAscending = false;

            // Volta instantaneamente para o player
            if (cameraToFollow != null && player != null)
            {
                Vector3 targetPosition = new Vector3(
                    player.position.x + offsetPlayer.x,
                    player.position.y + offsetPlayer.y,
                    player.position.z + offsetPlayer.z
                );

                cameraToFollow.position = targetPosition;
            }
        }

        transform.position = pos;
    }

    void MoveCameraVerticalOnly()
    {
        if (cameraToFollow == null) return;

        Vector3 desiredPosition = new Vector3(
            fixedCameraX,
            transform.position.y + offsetGancho.y,
            transform.position.z + offsetGancho.z
        );

        cameraToFollow.position = desiredPosition;
    }
}
