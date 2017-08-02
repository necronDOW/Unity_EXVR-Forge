using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class tmpPlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float lookSensitivity = 3.0f;
    [SerializeField] private Camera camera;

    private Rigidbody rb;
    private Vector3 velocity, rotation, cameraRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;
        velocity = (movHorizontal + movVertical).normalized * speed;

        float yRot = Input.GetAxisRaw("Mouse X");
        rotation = new Vector3(0.0f, yRot, 0.0f) * lookSensitivity;

        float xRot = Input.GetAxisRaw("Mouse Y");
        cameraRotation = new Vector3(xRot, 0.0f, 0.0f) * lookSensitivity;
    }

    private void FixedUpdate()
    {
        if (velocity != Vector3.zero)
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        if (camera)
            camera.transform.Rotate(-cameraRotation);
    }
}
