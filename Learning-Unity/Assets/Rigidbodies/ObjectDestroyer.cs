using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < -100)
        {
            Destroy(this.gameObject);
        }
    }
}
