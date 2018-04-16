using UnityEngine;
//THIS CLASS REQUIRES CONFIGURABLEJOINT AND PLAYERMOTOR TO WORK
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
public class PlayerControler : MonoBehaviour
{

    //SerializeField permits to access and set a non public variable in the inspector
    // Speed is the player movement speed aka how fast he goes.
    [SerializeField]
    private float speed = 5f;
    //lookSensitivity is how fast you can turn the camera
    [SerializeField]
    private float lookSensitivity = 3f;

    //The player thruster makes him go upward, it's basically a "how quick i can go up" force.
    [SerializeField]
    private float thrusterForce = 1000;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    //Settings for the SpringJoint of the player
    [Header("SpringJoint settings")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;


    void Start()
    {
        //sets the motor, animator and joint to the appropriate components.
        //No check is needed, the components are required for the script to work so they can never be null
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    void Update()
    {
        //this update method calculates the movement of the player

        if (PauseMenu.IsOn)
        {
            return;
        }

        //Setting Target Position for spring
        // This makes the physics act rigth to apply gravity when flying
        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        // Calculate mov. velocity as a 3D vector
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        //Final movement vector
        Vector3 velocity = (moveHorizontal + moveVertical) * speed;

        //Animate movemnt
        animator.SetFloat("ForwardVelocity", zMov);

        //Apply the movemnt
        motor.Move(velocity);

        //Calculate rotation as a 3D vecotor (turning around)
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

        // Apply rotation
        motor.Rotate(rotation);

        //Calculate camera rotation as a 3D vecotor (turning around)
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * lookSensitivity;

        // Apply rotation
        motor.RotateCamera(cameraRotationX);

        //Calculate the thrusterforce based on player input
        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                //setting the jointSettings to 0 to avoid a springlike behaviour while going upwards
                SetJointSettings(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            //Set jointSettings back to normal
            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0, 1);

        // Apply the thruster force
        motor.ApplyThruster(_thrusterForce);
    }

    //This method sets the joint settings to given value
    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
