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
    float turnSpeed;

    private Rigidbody rb;
    private float currentSpeed;
    private PhotonView view;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
    }
 
    void FixedUpdate() 
    {
        if(!view.IsMine){return;}

        if(Input.GetKey(KeyCode.W))
        {
            currentSpeed += speedChangeAmount;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            currentSpeed -= speedChangeAmount;
        }
        else if (Mathf.Abs(currentSpeed) <= minimumSpeed)
        {
            currentSpeed = 0;
        }

        else if(Input.GetKey(KeyCode.A))
        {
            rb.AddTorque(transform.up * turnSpeed);
        }

        else if(Input.GetKey(KeyCode.D))
        {
            Debug.Log("pressed d");
            rb.AddTorque(transform.up * -turnSpeed);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxBackwardSpeed, maxForwardSpeed);
        rb.AddForce(transform.forward * currentSpeed);
    }
}
