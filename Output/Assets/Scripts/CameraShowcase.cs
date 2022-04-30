using System;
using RagnarEngine;

public class CameraShowcase : RagnarComponent
{
    public GameObject controller;
    public GameObject[] waypoints;
    private int index = 0;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 pointC;
    private Vector3 pointD;
    private float speed = 0.5f;
    private float t = 0;

    public void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("CamWaypoints");
        controller = GameObject.Find("cameraController");
        Vector3 vec = controller.transform.globalPosition;
        pointA = new Vector3(vec.x, vec.y, vec.z);
        vec = waypoints[0].GetComponent<Transform>().globalPosition;
        pointB = new Vector3(vec.x, vec.y, vec.z);
        vec = waypoints[1].GetComponent<Transform>().globalPosition;
        pointC = new Vector3(vec.x, vec.y, vec.z);
        vec = waypoints[2].GetComponent<Transform>().globalPosition;
        pointD = new Vector3(vec.x, vec.y, vec.z);

    }

    public Vector3 CustomLerp(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return ((p0 + (p1 - p0) * t) * (1 - t) + (p1 + (p2 - p1) * t) * t) * (1 - t) + ((p1 + (p2 - p1) * t) * (1 - t) + (p2 + (p3 - p2 * t)) * t) * t;
    }

    public void Update()
    {
        if (index < waypoints.Length)
        {
            t += Time.deltaTime * speed;

            Vector3 newpos = CustomLerp(pointA, pointB, pointC, pointD, t);
            //gameObject.transform.globalPosition.Set(newpos.x, newpos.y, newpos.z);
            gameObject.GetComponent<Camera>().ScriptMovement(newpos.x, newpos.y, newpos.z);

            if (t >= 1f)
            {
                index += 4;
                float oldMagnitude = Vector3.Magnitude(pointB - pointA);

                pointA = pointD;
                Vector3 vec = waypoints[index].transform.globalPosition;
                pointB = new Vector3(vec.x, vec.y, vec.z);
                vec = waypoints[index + 1].transform.globalPosition;
                pointC = new Vector3(vec.x, vec.y, vec.z);
                vec = waypoints[index + 2].transform.globalPosition;
                pointD = new Vector3(vec.x, vec.y, vec.z);


                // Adjust speed so its the same no matter the length to the next waypoint
                speed = speed * oldMagnitude / Vector3.Magnitude(pointB - pointA);

                t = 0;
            }
        }
    }
}