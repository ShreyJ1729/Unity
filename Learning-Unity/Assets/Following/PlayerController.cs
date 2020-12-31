using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    public Vector3 velocity;
    void Update()
    {
        this.velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            this.velocity += Vector3.forward * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.velocity += Vector3.back * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.velocity += Vector3.left * speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.velocity += Vector3.right * speed;
        }
        
        transform.position += this.velocity * Time.deltaTime;
    }
}
