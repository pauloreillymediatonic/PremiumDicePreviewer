using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f || transform.position.z < -15) 
       	 	{
        		transform.position = transform.position + new Vector3(0, 0, 1);
        	}
    
        if (Input.GetAxis("Mouse ScrollWheel") < 0f || transform.position.z > -1) 
 			{
   				transform.position = transform.position + new Vector3(0, 0, -1);
 			}	  
    }
}
