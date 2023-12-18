using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    Camera cam;

    [SerializeField] 
    GameObject fog;

    [SerializeField] 
    float accelerationRate;

    [SerializeField]
    public float maxSpeed;

    [SerializeField]
    float speedDamping;

    [SerializeField]
    float maxTurnAngle;

    [SerializeField]
    float ballastFillRate;

    [SerializeField]
    float maxBallast;
    
    [SerializeField]
    float buoyancyForce;

    [SerializeField]
    GameObject torpedoPrefab;

    [SerializeField]
    AudioSource explosionSound;
    private int health = 100;
    private float ballastLevel = 0f;
    private float turnVelocity;
    private Rigidbody rb;
    private PhotonView view;
    private float accelerateValue;
    private Vector2 turnValue;
    private float currentSpeed;
    private bool isMoving = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            Instantiate(cam, this.transform);
            Instantiate(fog, this.transform);
            AudioSource ambience = GetComponent<AudioSource>();
            ambience.Play();
        }
    }

    public void AccelerateInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isMoving = true;
            accelerateValue = context.ReadValue<float>();
            Debug.Log("Accelerate Value: " + accelerateValue);
        }
        else if (context.canceled)
        {
            isMoving = false;
            accelerateValue = 0f; 
        }
    }

    public void DecelerateInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isMoving = true;
            accelerateValue = -context.ReadValue<float>();
            Debug.Log("Accelerate Value: " + accelerateValue);
        }
        else if (context.canceled)
        {
            isMoving = false;
            accelerateValue = 0f;
        }
    }


    public void TurnInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            turnValue = context.ReadValue<Vector2>();
            Debug.Log("Turn Input: " + turnValue);
        }
        else if (context.canceled)
        {
            turnValue = Vector2.zero;
        }
    }

    public void BallastInputUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ballastLevel += ballastFillRate * Time.fixedDeltaTime;
            ballastLevel = Mathf.Clamp(ballastLevel, -maxBallast, maxBallast);
        }
    }

    public void BallastInputDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ballastLevel -= ballastFillRate * Time.fixedDeltaTime;
            ballastLevel = Mathf.Clamp(ballastLevel, -maxBallast, maxBallast);
        }
    }

    public void TorpedoInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FireTorpedo();
        }
    }

    void FixedUpdate()
    {
        if (view.IsMine)
        {
            Turning();
            Movement();
            Ballast();
        }
    }

    void Turning()
    {
        if (turnValue != Vector2.zero)
        {
            float turnSpeed = Mathf.Lerp(0, maxTurnAngle, Mathf.Abs(currentSpeed) / maxSpeed);

            // Adjust the sign of turnSpeed, so we can invert the turning when backwards.
            turnSpeed *= Mathf.Sign(currentSpeed);
            transform.Rotate(Vector3.up, turnValue.x * turnSpeed * Time.fixedDeltaTime);
        }
    }

    void Movement()
    {
        if (!isMoving)
        {

            //Set speed to 0 if between -1 and 1
            if (currentSpeed > -1 && currentSpeed < 1)
            {
                currentSpeed = 0;
            }

            //Degrade speed overtime
            currentSpeed *= (1 - speedDamping * Time.fixedDeltaTime);
        }
        
        currentSpeed += accelerateValue * accelerationRate * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
        
        Vector3 forwardMovement = transform.forward * currentSpeed;
        rb.MovePosition(rb.position + forwardMovement * Time.fixedDeltaTime);
    }

    void Ballast()
    {
        if(ballastLevel > -0.05 && ballastLevel < 0.05)
        {
            ballastLevel = 0;
        }
        float normalizedBallast = (ballastLevel + maxBallast) / (2f * maxBallast);
        float currentBuoyancyForce = Mathf.Lerp(-buoyancyForce, buoyancyForce, normalizedBallast);

        Vector3 buoyancyVector = Vector3.up * currentBuoyancyForce;
        rb.AddForceAtPosition(buoyancyVector, transform.position);
    }

    void FireTorpedo()
    {
        if(view.IsMine)
        {
            GameObject torpedo = PhotonNetwork.Instantiate(torpedoPrefab.name, transform.position + transform.forward * 30f, transform.rotation);
            Torpedo torpedoScript = torpedo.GetComponent<Torpedo>();
            torpedoScript.SetOwner(view.ViewID);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Local torpedo damage, called on the client (bad, but easy)
        if (view.IsMine)
        {
            if (other.CompareTag("Torpedo"))
            {
                Torpedo torpedoScript = other.GetComponent<Torpedo>();

                if (torpedoScript != null && torpedoScript.OwnerViewID != view.ViewID)
                {
                    TakeDamage(10);
                    torpedoScript.DestroyTorpedo();
                }
            }
        }
    }

    [PunRPC]
    void TakeDamage(int damage)
    {
        health -= damage;
        explosionSound.Play();
        if(health <= 0)
        {
            Debug.Log("Player Died: Todo");
        }

    }
}