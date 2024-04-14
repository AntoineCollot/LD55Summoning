using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharController : MonoBehaviour
{
    [Header("Grounded")]
    [SerializeField, Range(1, 10)] float maxSpeed = 4;
    [SerializeField, Range(1, 80)] float maxAcceleration = 20;
    [SerializeField, Range(1, 80)] float maxDeceleration = 20;
    [SerializeField, Range(1, 80)] float maxTurnSpeed = 20;

    float acceleration;
    float deceleration;
    public Vector2 directionInput { get; private set; }
    Vector2 desiredVelocity;
    float turnSpeed;
    float maxSpeedChange;
    Vector2 velocity;

    public enum MovementState { Idle, Acceleration, MaxSpeed, Deceleration, Turning }
    public MovementState movementState { get; private set; }

    [Header("Components")]
    Rigidbody body;
    Transform camT;

    [Header("Inputs")]
    protected InputMap inputs;

    public float NormalizedMoveSpeed
    {
        get
        {
            Vector2 velocity = new Vector2(body.velocity.x, body.velocity.z);
            return velocity.magnitude / maxSpeed;
        }
    }
    public Vector3 MoveDirection => new Vector3(desiredVelocity.x, 0, desiredVelocity.y).normalized;

    void Awake()
    {
        inputs = new InputMap();
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        camT = Camera.main.transform;
    }

    void OnEnable()
    {
        inputs.Enable();
        inputs.Gameplay.Enable();

        inputs.Gameplay.Move.performed += OnMovement;
        inputs.Gameplay.Move.canceled += OnMovement;
    }

    void OnDisable()
    {
        inputs.Disable();

        inputs.Gameplay.Move.performed -= OnMovement;
        inputs.Gameplay.Move.canceled -= OnMovement;
    }

    void Update()
    {
        Vector2 currentDirectionInput = directionInput;
        //Check if we should freeze the inputs
        if (PlayerState.Instance.freezeInputsState.IsOn || !GameManager.Instance.GameIsPlaying)
        {
            currentDirectionInput = Vector2.zero;
        }

        currentDirectionInput = AlignWithCamera(currentDirectionInput);
        desiredVelocity = currentDirectionInput * maxSpeed;
    }

    private void FixedUpdate()
    {
        velocity = new Vector2(body.velocity.x, body.velocity.z);

        Vector2 currentDirectionInput = AlignWithCamera(directionInput);
        //Check if we should freeze the inputs
        if (PlayerState.Instance.freezeInputsState.IsOn)
        {
            currentDirectionInput = Vector2.zero;
        }

        acceleration = maxAcceleration;
        deceleration = maxDeceleration;
        turnSpeed = maxTurnSpeed;

        //Movement state
        movementState = MovementState.Idle;

        if (currentDirectionInput.sqrMagnitude > 0.01f)
        {
            if (velocity.sqrMagnitude > 0.01f && Vector2.Dot(velocity, currentDirectionInput) < 0)
            {
                movementState = MovementState.Turning;
            }
            else if (desiredVelocity.magnitude > velocity.magnitude)
            {
                movementState = MovementState.Acceleration;
            }
            else
            {
                movementState = MovementState.MaxSpeed;
            }
        }
        else if (desiredVelocity.magnitude < velocity.magnitude)
        {
            movementState = MovementState.Deceleration;
        }

        //Speed change update based on movement state
        switch (movementState)
        {
            case MovementState.Idle:
            case MovementState.MaxSpeed:
            case MovementState.Acceleration:
                maxSpeedChange = acceleration * Time.fixedDeltaTime;
                break;
            case MovementState.Deceleration:
                maxSpeedChange = deceleration * Time.fixedDeltaTime;
                break;
            case MovementState.Turning:
                maxSpeedChange = turnSpeed * Time.fixedDeltaTime;
                break;
        }

        //Update velocity
        velocity = Vector2.MoveTowards(velocity, desiredVelocity, maxSpeedChange);

        if (PlayerState.Instance.freezePositionState.IsOn)
            velocity = Vector2.zero;

        body.velocity = new Vector3(velocity.x, body.velocity.y, velocity.y);
    }

    Vector2 AlignWithCamera(Vector2 vector)
    {
        //Align with camera forward and right
        Vector2 cameraForward = new Vector2(camT.forward.x, camT.forward.z);
        Vector2 cameraRight = new Vector2(camT.right.x, camT.right.z);
        cameraForward.Normalize();
        cameraRight.Normalize();
        return vector.y * cameraForward + vector.x * cameraRight;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        directionInput = context.ReadValue<Vector2>();
    }

    public void LerpToPosition(Vector3 pos)
    {
        StopAllCoroutines();
        StartCoroutine(LerpToPositionAnim(pos, 0.3f));
    }

    IEnumerator LerpToPositionAnim(Vector3 pos, float time)
    {
        float t = 0;
        Vector3 startPos = transform.position;
        while(t<1)
        {
            t += Time.deltaTime / time;

            transform.position = Curves.QuadEaseInOut(startPos, pos, Mathf.Clamp01(t));

            yield return null;
        }
    }
}
