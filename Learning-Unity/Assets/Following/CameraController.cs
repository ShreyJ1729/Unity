using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
    void Start()
    {
        this.offset = this.transform.position - this.target.transform.position;
    }

    void LateUpdate()
    {
        this.transform.position = this.target.transform.position + this.offset;
    }
}
