using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeObjects : MonoBehaviour
{
    
    
    public GameObject throwItem;
    public GameObject buildItem;
    public GameObject pointer;
    public float throwVelocity = 10f;

    public int structureSize = 10;

    // Update is called once per frame
    void Update()
    {
        handleThrowing();
        handleBuilding();
    }
    
    void buildStructure(Vector3 location, int size)
    {
        for (int i = 1; i <= size; i++)
        {
            Instantiate(buildItem, new Vector3(location.x, location.y + i*1.25f, location.z), Quaternion.identity);
        }
    }

    void handleBuilding()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // if hit something, then make dot visible and check if plane
            if (hit.transform.gameObject != null)
            {
                pointer.transform.position = hit.point;
                if (Input.GetKeyDown(KeyCode.B) && hit.transform.gameObject.name == "Ground")
                {
                    buildStructure(hit.point, structureSize);
                }
            }
        }
    }

    void handleThrowing()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 throwDirection = transform.forward;
            GameObject thrownBall = Instantiate(throwItem, transform.position, transform.rotation);
            thrownBall.GetComponent<Rigidbody>().velocity = transform.forward.normalized * this.throwVelocity;
        }
    }
}
