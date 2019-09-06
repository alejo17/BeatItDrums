using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

public class DetectJoints : MonoBehaviour {

    public GameObject BodySrcManager;
    public JointType TrackedJoint;
    public float multiplier = 10f;

    private BodySourceManager bodyManager;
    private Body[] bodies;

    private List<InfraredSourceManager.Marker> markers_to_draw;
    private List<int> markers_depth;
    public GameObject b;
    public Transform panel;

    private List<GameObject> my_balls = new List<GameObject>();

    GameObject marker_object;
    InfraredSourceManager marker_script;

    // my 80 largo y 60 alto 
    // kinect 424 y 512
    private int my_horizontal = 100;
    private int my_vertical = 80;
    private int my_depth = 60;
    private int kinect_horizontal = 512;
    private int kinect_vertical = 424;
    private int kinect_depth = 4500;

    private int init_balls = 10;
    private int nowhere = 200;

    GameObject CreateA(int x, int y, int z)
    {
        GameObject a = (GameObject)Instantiate(b);
        a.transform.position = new Vector3(x,y,z);
        a.transform.SetParent(panel.transform, false);
        return a;
    }

    // Use this for initialization
    void Start () {
        if (BodySrcManager == null)
        {
            Debug.Log("Assgin Game Object with Body Source Manager");
        }
        else
        {
            bodyManager = BodySrcManager.GetComponent<BodySourceManager>();
        }

        marker_object = GameObject.Find("InfraredManager");
        marker_script = marker_object.GetComponent<InfraredSourceManager>();

        for (int i = 0; i < init_balls; i++)
        {
            my_balls.Add(CreateA(nowhere, -nowhere, 0));
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (marker_script == null || marker_script.markers == null || marker_script.markers_depth == null)
        {
            return;
        }
        //reset positions
        markers_to_draw = marker_script.markers;
        markers_depth = marker_script.markers_depth;
        kinect_depth = marker_script.maxDepth / 3;

        int i = 0;
        for (; i< markers_to_draw.Count && i < my_balls.Count && markers_to_draw.Count == markers_depth.Count; i++)
        {
            float new_x = markers_to_draw[i].pixels[0].x;
            float new_y = markers_to_draw[i].pixels[0].y;
            float new_z = markers_depth[i];

            new_x = my_horizontal * new_x / kinect_horizontal - my_horizontal / 2;
            new_y = my_vertical * new_y / kinect_vertical - my_vertical / 2 ;
            new_z = my_depth * new_z / kinect_depth;

            my_balls[i].transform.position = new Vector3(new_y, -new_x, -new_z + 50);
            //Debug.Log(new_z);
        }
        for (; i < my_balls.Count; i++)
        {
            my_balls[i].transform.position = new Vector3(nowhere, nowhere, 0);
        }
        //float new_x = 116 * markerScript.marker_x / 424 - 58;
        //float new_y = 150 * markerScript.marker_y / 512 - 75;
        /*float new_x = markerScript.marker_x + 512/2;
        float new_y = markerScript.marker_y + 424/2;*/
        //Debug.Log("x: " + new_x + ", y: " + new_y);
        //this.gameObject.transform.position = new Vector3(new_y, -new_x, 0);
        /*if (bodyManager == null)
        {
            return;
        }
        bodies = bodyManager.GetData();

        if (bodies == null)
        {
            return;
        }

        // get the first tracked body…
        foreach (var body in bodies)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                var pos = body.Joints[TrackedJoint].Position;   

                this.gameObject.transform.position = new Vector3(new_x, new_y, 0);

                //this.gameObject.transform.position = new Vector3(pos.X * multiplier, pos.Y * multiplier, pos.Z * multiplier);
                break;
            }
        }*/


    }
}
