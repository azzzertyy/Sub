using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    float speedChangeAmount;

    [SerializeField]
    float maxBackwardSpeed;

    [SerializeField]
    float maxForwardSpeed;

    [SerializeField]
    float minimumSpeed;

    [SerializeField]
    float turnSpeedAmount;

    [SerializeField]
    float maxTurn;

    [SerializeField]
    float speedDegredation;

    [SerializeField]
    Camera cam;

    
    private Rigidbody rb;
    private float currentSpeed;
    private float turnSpeed;
    private PhotonView view;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
        if(view.IsMine)
        {
            Instantiate(cam, this.transform);
        }
    }

    public void Turn(Vector2 wheel)
    {
        if(view.IsMine)
        {
            turnSpeed = wheel.x;
        }
    }
 
    void FixedUpdate() 
    {
        if(view.IsMine)
        {            
            currentSpeed = Mathf.Clamp(currentSpeed, -maxBackwardSpeed, maxForwardSpeed);

            currentSpeed *= speedDegredation;
            turnSpeed *= speedDegredation;
            
            Debug.Log("turn speed: "+ turnSpeed);
            Debug.Log("current speed: "+ currentSpeed);

            rb.AddForce(transform.forward * currentSpeed);
            rb.AddTorque(transform.up * turnSpeed);
        }
    }
}
