using System;
using RagnarEngine;

public class CameraShowcase : RagnarComponent
{
    public GameObject controller;
    public GameObject[] waypoints;
    private int index = 0;

    private Vector3 pointA;
    private Vector3 pointB;
    private float speed = 1.0f;
    private float t = 0;

    public void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("CamWaypoints");
        controller = GameObject.Find("cameraController");
        Vector3 vec = controller.transform.globalPosition;
        pointA = new Vector3(vec.x, vec.y, vec.z);
        vec = waypoints[0].GetComponent<Transform>().globalPosition;
        pointB = new Vector3(vec.x, vec.y, vec.z);
    }

    private Vector3 CustomLerp(Vector3 a, Vector3 b, float t)
    {
        return a * (1 - t) + b * t;
    }

    public void Update()
    {
        if (index < waypoints.Length)
        {
            t += Time.deltaTime * speed;

            Vector3 newpos = CustomLerp(pointA, pointB, t);
            //gameObject.transform.globalPosition.Set(newpos.x, newpos.y, newpos.z);
            gameObject.GetComponent<Camera>().ScriptMovement(newpos.x, newpos.y, newpos.z);

            if (t >= 1)
            {
                index++;

                pointA = pointB;
                Vector3 vec = waypoints[index].transform.globalPosition;
                pointB = new Vector3(vec.x, vec.y, vec.z);

                t = 0;
            }
        }
    }
}