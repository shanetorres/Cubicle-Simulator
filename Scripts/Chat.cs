using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat : MonoBehaviour
{

    GameObject managerCanvas;
    // Start is called before the first frame update
    void Start()
    {
        managerCanvas = GameObject.FindGameObjectWithTag("manager_canvas");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        managerCanvas.transform.LookAt(managerCanvas.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
