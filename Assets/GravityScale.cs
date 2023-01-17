using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityScale : MonoBehaviour
{

    [SerializeField] private float gravityScale = 1f;

    public static float customGravity = -9.81f;

    Rigidbody rb;


    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 gravity = customGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }
}
