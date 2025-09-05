using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform targetObject;
    public float followTightness;
    public float sensitivity = 1;
    public float speed = 30;

    Vector3 wantedPosition;
    //private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        //camera = GetComponent<Camera>();    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        wantedPosition = targetObject.position;
        wantedPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * followTightness);
    }
}
