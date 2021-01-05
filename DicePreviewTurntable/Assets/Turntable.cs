using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turntable : MonoBehaviour
{
	public float speed = 5f;

    private float currentAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
        	transform.Rotate(((Input.GetAxis("Mouse Y")) * (speed * 50) * Time.deltaTime), (-(Input.GetAxis("Mouse X")) * (speed * 50) * Time.deltaTime), 0, Space.World);
        }
        else
        {
            if ((transform.rotation.y >= 0.20) | (transform.rotation.y <= -0.20))
                {
                    speed *= -1;
                }

        	transform.Rotate(0, Time.deltaTime * speed, 0, Space.World);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.rotation = new Quaternion(0,0,0,0);
        }
    }

    public void ResetRot()
    {
        transform.rotation = new Quaternion(0,0,0,0);
    }
}
