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

    bool endedPresentation = false;

    public void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("CamWaypoints");
        controller = GameObject.Find("cameraController");
        Debug.Log("Waypoints: " + waypoints.Length.ToString());
        Vector3 vec = waypoints[0].GetComponent<Transform>().globalPosition;
        pointA = new Vector3(vec.x, vec.y, vec.z);
        vec = waypoints[1].GetComponent<Transform>().globalPosition;
        pointB = new Vector3(vec.x, vec.y, vec.z);
        vec = waypoints[2].GetComponent<Transform>().globalPosition;
        pointC = new Vector3(vec.x, vec.y, vec.z);
        vec = waypoints[3].GetComponent<Transform>().globalPosition;
        pointD = new Vector3(vec.x, vec.y, vec.z);
    }

    public Vector3 CustomLerp(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return ((p0 + (p1 - p0) * t) * (1 - t) + (p1 + (p2 - p1) * t) * t) * (1 - t) + ((p1 + (p2 - p1) * t) * (1 - t) + (p2 + (p3 - p2 * t)) * t) * t;
    }

    public void Update()
    {
        if (!endedPresentation)
        {
            if (index < waypoints.Length)
            {
                t += Time.deltaTime * speed;
                Debug.Log("I'm here");

                // Movement
                Vector3 newpos = CustomLerp(pointA, pointB, pointC, pointD, t);
                gameObject.GetComponent<Camera>().ScriptMovement(newpos.x, newpos.y, newpos.z);

                //Rotation
                Vector3 nextPoint = CustomLerp(pointA, pointB, pointC, pointD, t + 0.01f);
                Vector3 dir = nextPoint - controller.transform.globalPosition;
                gameObject.GetComponent<Camera>().ScriptRotation(dir.x, dir.y, dir.z);

                if (t >= 1f)
                {
                    index += 4;
                    Debug.Log("INDEX: " + index.ToString());
                    float oldMagnitude = Vector3.Magnitude(pointB - pointA);

                    pointA = pointD;
                    Vector3 vec = waypoints[index].transform.globalPosition;
                    pointB = new Vector3(vec.x, vec.y, vec.z);


                    vec = waypoints[index + 1].transform.globalPosition;
                    pointC = new Vector3(vec.x, vec.y, vec.z);
                    vec = waypoints[index + 2].transform.globalPosition;
                    pointD = new Vector3(vec.x, vec.y, vec.z);
                    Debug.Log("PointA: " + pointA.ToString());
                    Debug.Log("PointB: " + pointB.ToString());
                    Debug.Log("PointC: " + pointC.ToString());
                    Debug.Log("PointD: " + pointD.ToString());

                    // Adjust speed so its the same no matter the length to the next waypoint
                    //speed = speed * oldMagnitude / Vector3.Magnitude(pointB - pointA);

                    t = 0;
                }
            }

            else
            {
                endedPresentation = true;
                gameObject.GetComponent<Camera>().LockControlls(false);
            }
        }
    }
}