using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class FollowerController : MonoBehaviour
{
    public GameObject target;
    public Vector3 velocity;
    public bool isFollowing = true;

    public float speed;
    void Update()
    {
        Vector3 offset = (target.transform.position - this.transform.position);
        float distance = offset.magnitude;
        if (distance > 1)
        {
            if (!isFollowing)
            {
                turnRed();
                isFollowing = true;
            }
            this.velocity = offset.normalized * this.speed;
            this.transform.position += this.velocity * Time.deltaTime;
        }
        else
        {
            if (isFollowing)
            {
                turnGreen();
                isFollowing = false;
            }
            this.velocity = Vector3.zero;
        }
    }

    void turnRed()
    {
        this.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f);
    }

    void turnGreen()
    {
        this.gameObject.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f);
    }
}
