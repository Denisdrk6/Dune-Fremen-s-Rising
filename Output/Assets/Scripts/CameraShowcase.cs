using System;
using RagnarEngine;

public class CameraShowcase : RagnarComponent
{
    public GameObject controller;
    public GameObject[] waypoints;
    private int index = 0;
    private Camera camComponent;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 pointC;
    private Vector3 pointD;
    public float speed = 0.35f;
    private float t = 0f;
    private float tEnd = 0f;

    private float ogRotation;
    private float endRotation;

    public float targetZoom = 1.0f;

    public bool linear = false;

    private bool endedPresentation = false;

    public void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("CamWaypoints");
        controller = GameObject.Find("cameraController");
        Debug.Log("Waypoints: " + waypoints.Length.ToString());
        if (!linear)
        {
            Vector3 vec = waypoints[0].transform.globalPosition;
            pointA = new Vector3(vec.x, vec.y, vec.z);
            vec = waypoints[1].transform.globalPosition;
            pointB = new Vector3(vec.x, vec.y, vec.z);
            vec = waypoints[2].transform.globalPosition;
            pointC = new Vector3(vec.x, vec.y, vec.z);
            vec = waypoints[3].transform.globalPosition;
            pointD = new Vector3(vec.x, vec.y, vec.z);
        }
        else
        {
            Vector3 vec = controller.transform.globalPosition;
            pointA = new Vector3(vec.x, vec.y, vec.z);
            vec = waypoints[0].GetComponent<Transform>().globalPosition;
            pointB = new Vector3(vec.x, vec.y, vec.z);
        }

        camComponent = gameObject.GetComponent<Camera>();

        ogRotation = camComponent.GetAngle();

        camComponent.LockControlls(true);
    }

    // Lerps
    public Vector3 BeziersCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return ((p0 + (p1 - p0) * t) * (1 - t) + (p1 + (p2 - p1) * t) * t) * (1 - t) + ((p1 + (p2 - p1) * t) * (1 - t) + (p2 + (p3 - p2 * t)) * t) * t;
    }
    private float Lerp(float a, float b, float t)
    {
        return a * (1 - t) + b * t;
    }
    private float EaseOut(float t)
    {
        return 1 - ((1 - t) * (1 - t));
    }
    private float EaseInOut(float t)
    {
        return t * t / (2.0f * (t * t - t) + 1.0f);
    }
    private Vector3 CustomLerp(Vector3 a, Vector3 b, float t)
    {
        return a * (1 - t) + b * t;
    }

    // Update
    public void Update()
    {
        if (!endedPresentation)
        {
            // Cubic
            if (!linear)
            {
                if (index < waypoints.Length)
                {
                    Debug.Log("Index: " + index.ToString() + "Length: " + waypoints.Length.ToString());
                    t += Time.deltaTime * speed;

                    // Movement
                    Vector3 newpos = BeziersCurve(pointA, pointB, pointC, pointD, t);
                    camComponent.ScriptMovement(newpos.x, newpos.y, newpos.z);


                    //Rotation
                    float newT = t + 0.04f;
                    // This predicts the future
                    if ((newT > 1f))
                    {
                        if ((t >= 1.0f) && (index == 8)) // BUG FIX, DON'T TOUCH!
                        {
                            index = waypoints.Length;
                            endRotation = camComponent.GetAngle();
                        }
                        else
                        {
                            Vector3 tempPointA = pointD;
                            Vector3 vec = waypoints[index + 4].transform.globalPosition;
                            Vector3 tempPointB = new Vector3(vec.x, vec.y, vec.z);


                            vec = waypoints[index + 1 + 4].transform.globalPosition;
                            Vector3 tempPointC = new Vector3(vec.x, vec.y, vec.z);
                            vec = waypoints[index + 2 + 4].transform.globalPosition;
                            Vector3 tempPointD = new Vector3(vec.x, vec.y, vec.z);

                            Vector3 nextPoint = BeziersCurve(tempPointA, tempPointB, tempPointC, tempPointD, newT - 1.0f);
                            Vector3 dir = nextPoint - controller.transform.globalPosition;
                            camComponent.ScriptRotation(dir.x, dir.y, dir.z);
                        }

                    }
                    // This calculates the present
                    else
                    {
                        Vector3 nextPoint = BeziersCurve(pointA, pointB, pointC, pointD, newT);
                        Vector3 dir = nextPoint - controller.transform.globalPosition;
                        gameObject.GetComponent<Camera>().ScriptRotation(dir.x, dir.y, dir.z);
                    }

                    if (t >= 1.0f)
                    {
                        index += 4;
                        Debug.Log("INDEX: " + index.ToString());

                        // Get next curve
                        pointA = pointD;
                        Vector3 vec = waypoints[index].transform.globalPosition;
                        pointB = new Vector3(vec.x, vec.y, vec.z);
                        vec = waypoints[index + 1].transform.globalPosition;
                        pointC = new Vector3(vec.x, vec.y, vec.z);
                        vec = waypoints[index + 2].transform.globalPosition;
                        pointD = new Vector3(vec.x, vec.y, vec.z);

                        // Adjust speed so its the same no matter the length to the next waypoint
                        //speed = speed * oldMagnitude / Vector3.Magnitude(pointB - pointA);

                        t = 0.0f;
                    }
                }

                else if ((camComponent.GetAngle() != ogRotation || camComponent.GetZoom() < targetZoom) && tEnd < 1.0f)
                {
                    tEnd += Time.deltaTime * speed;
                    if (camComponent.GetAngle() != ogRotation) camComponent.ScriptRotationAngle(Lerp(endRotation, ogRotation, EaseOut(tEnd)));
                    if (camComponent.GetZoom() < targetZoom) camComponent.ScriptZoom(EaseInOut(tEnd));
                }

                else
                {
                    endedPresentation = true;
                    camComponent.LockControlls(false);
                }
            }

            // Linear
            else
            {
                if (index < waypoints.Length)
                {
                    t += Time.deltaTime * speed;

                    Vector3 newpos = CustomLerp(pointA, pointB, EaseInOut(t));
                    //gameObject.transform.globalPosition.Set(newpos.x, newpos.y, newpos.z);
                    gameObject.GetComponent<Camera>().ScriptMovement(newpos.x, newpos.y, newpos.z);

                    if (t >= 1)
                    {
                        index++;
                        float oldMagnitude = Vector3.Magnitude(pointB - pointA);

                        pointA = pointB;
                        Vector3 vec = waypoints[index].transform.globalPosition;
                        pointB = new Vector3(vec.x, vec.y, vec.z);

                        // Adjust speed so its the same no matter the length to the next waypoint
                        speed = speed * oldMagnitude / Vector3.Magnitude(pointB - pointA);

                        t = 0;
                    }
                }
                else
                {
                    endedPresentation = true;
                    camComponent.LockControlls(false);
                }
            }
        }
    }
}