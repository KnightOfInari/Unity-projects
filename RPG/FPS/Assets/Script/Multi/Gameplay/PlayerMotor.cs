using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    //
    //THIS CLASS HAS METHODS CONTROLLING THE MOVEMENTS OF THE PLAYER
    // AND THE CAMERA ATTACHED TO IT
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;

    private void Start()
    {
        //initialize rb with a rigidbody component of the current game object;
        rb = GetComponent<Rigidbody>();
    }

    //get movement vector
    public void Move(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }
    // sets the players rotation and orientation
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    //Get a force vector for our thrusters
    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    // set the camera rotation angle on an horizontal plane
    public void RotateCamera(float _rotationX)
    {
        cameraRotationX = _rotationX;
    }

    //Update players movement and camera rotation on a fixed frame
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    //Perform Movement based on velocity variable
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    //Method for camera rotation. can turn horizontally without limit 
    //is clamped vertically so the player can't be upside down 
    void PerformRotation()
    {
        rb.MoveRotation(transform.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            // New rotationale calculation
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

        }
    }

}