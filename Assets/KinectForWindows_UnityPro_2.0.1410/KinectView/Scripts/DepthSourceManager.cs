using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;
using System;
using System.Linq;

public class DepthSourceManager : MonoBehaviour
{   
    private KinectSensor _Sensor;
    private DepthFrameReader _Reader_Depth;
    private ushort[] _Data_Depth;

    private int[,] depth_matrix;
    private int[,] infrared_matrix;
    private int width;
    private int height;

    private int check_distance = 5;

    public ushort maxDepth;

    GameObject marker_object;
    InfraredSourceManager marker_script;

    private List<InfraredSourceManager.Marker> markers_to_draw;
    public List<int> markers_depth;

    public ushort[] GetData()
    {
        return _Data_Depth;
    }

    void Start () 
    {
        _Sensor = KinectSensor.GetDefault();
        
        if (_Sensor != null) 
        {
            _Reader_Depth = _Sensor.DepthFrameSource.OpenReader();
            _Data_Depth = new ushort[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];
            var frameDesc = _Sensor.InfraredFrameSource.FrameDescription;
            Debug.Log("Width: " + frameDesc.Width + ", Height: " + frameDesc.Height);
            width = frameDesc.Width;
            height = frameDesc.Height;

            depth_matrix = new int[height, width];
            marker_object = GameObject.Find("InfraredManager");
            marker_script = marker_object.GetComponent<InfraredSourceManager>();
        }
    }

    List<InfraredSourceManager.Coord> get_border(List<InfraredSourceManager.Coord> marker_pixels)
    {
        List<InfraredSourceManager.Coord> ress = new List<InfraredSourceManager.Coord>();

        foreach (InfraredSourceManager.Coord pixel in marker_pixels)
        {
            //checkear los bordes 
            if (pixel.x - check_distance > 0 && pixel.y - check_distance > 0)
            {
                int value = infrared_matrix[pixel.x- check_distance, pixel.y- check_distance];
                if (value != 255)
                {
                    bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x - check_distance && list_pixel.y == pixel.y - check_distance);
                    if (!contains)
                    {
                        InfraredSourceManager.Coord new_border = new InfraredSourceManager.Coord(pixel.x - check_distance, pixel.y - check_distance);
                        ress.Add(new_border);
                    }
                }
            }
            if (pixel.x + check_distance < width && pixel.y+ check_distance < height)
            {
                try
                {
                    int value = infrared_matrix[pixel.x + check_distance, pixel.y + check_distance];
                    if (value != 255)
                    {
                        bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x + check_distance && list_pixel.y == pixel.y + check_distance);
                        if (!contains)
                        {
                            InfraredSourceManager.Coord new_border = new InfraredSourceManager.Coord(pixel.x + check_distance, pixel.y + check_distance);
                            ress.Add(new_border);
                        }
                    }
                }
                catch (Exception e)
                {
                }
                
            }
            if (pixel.x - check_distance > 0 )
            {
                int value = infrared_matrix[pixel.x - check_distance, pixel.y];
                if (value != 255)
                {
                    bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x - check_distance && list_pixel.y == pixel.y);
                    if (!contains)
                    {
                        InfraredSourceManager.Coord new_border = new InfraredSourceManager.Coord(pixel.x - check_distance, pixel.y);
                        ress.Add(new_border);
                    }
                }
            }
            if (pixel.y - check_distance > 0)
            {
                int value = infrared_matrix[pixel.x , pixel.y - check_distance];
                if (value != 255)
                {
                    bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x && list_pixel.y == pixel.y - check_distance);
                    if (!contains)
                    {
                        InfraredSourceManager.Coord new_border = new InfraredSourceManager.Coord(pixel.x, pixel.y - check_distance);
                        ress.Add(new_border);
                    }
                }
            }
            if (pixel.x + check_distance < width)
            {
                try
                {
                    int value = infrared_matrix[pixel.x + check_distance, pixel.y];
                    if (value != 255)
                    {
                        bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x + check_distance && list_pixel.y == pixel.y);
                        if (!contains)
                        {
                            InfraredSourceManager.Coord new_border = new InfraredSourceManager.Coord(pixel.x + check_distance, pixel.y);
                            ress.Add(new_border);
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
            if (pixel.y + check_distance < height)
            {
                try
                { 
                    int value = infrared_matrix[pixel.x, pixel.y + check_distance];
                    if (value != 255)
                    {
                        bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x && list_pixel.y == pixel.y + check_distance);
                        if (!contains)
                        {
                            InfraredSourceManager.Coord new_border = new InfraredSourceManager.Coord(pixel.x, pixel.y + check_distance);
                            ress.Add(new_border);
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        return ress;
    }

    int get_minimum(List<InfraredSourceManager.Coord> border)
    {
        int minimum = -1;

        foreach (InfraredSourceManager.Coord pixel in border)
        {
            int depth_value = depth_matrix[pixel.x, pixel.y];
            //Debug.Log(depth_matrix[pixel.x + 50, pixel.y + 50]);
            if (minimum != -1)
            {
                if (depth_value != 0 && depth_value < minimum)
                    minimum = depth_value;
            }
            else
            {
                if (depth_value != 0)
                    minimum = depth_value;
            }
        }

        //Debug.Log("MINIMO" + minimum);

        return minimum;
    }

    void Update () 
    {
        if (marker_script == null || marker_script.markers == null)
        {
            return;
        }
        if (_Reader_Depth != null)
        {
            var depth_frame = _Reader_Depth.AcquireLatestFrame();
            if (depth_frame != null)
            {
                depth_frame.CopyFrameDataToArray(_Data_Depth);
                ushort minDepth = depth_frame.DepthMinReliableDistance;
                maxDepth = depth_frame.DepthMaxReliableDistance;

                int column = 0;
                int row = 0;

                foreach (var depth in _Data_Depth)
                {
                    byte distance = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);
                    /*if (distance!=0 && counter < 10)
                    {
                        Debug.Log("distancia: " + distance);
                        Debug.Log("X:" + column + " Y:" + row);
                        counter++;
                    }*/
                    if (column == width)
                    {
                        column = 0;
                        row++;
                    }

                    try
                    {
                        depth_matrix[row, column] = depth;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("ERROR: row" + row + " column" + column);
                    }

                    depth_matrix[row, column] = depth;
                    column++;
                }
                depth_frame.Dispose();
                depth_frame = null;

                markers_to_draw = marker_script.markers;
                infrared_matrix = marker_script.depth_matrix;

                if (markers_to_draw == null)
                {
                    markers_depth = null;
                }
                else
                {
                    List<int> tmp_marker_minimum = new List<int>();

                    //algoritmo para llenar una lista con los bordes para cada marcador marcadores
                    for (int i = 0; i < markers_to_draw.Count; i++)
                    {
                        List<InfraredSourceManager.Coord> marker_pixels = markers_to_draw[i].pixels;
                        List<InfraredSourceManager.Coord> borders = get_border(marker_pixels);
                        tmp_marker_minimum.Add(get_minimum(borders));
                    }
                    markers_depth = tmp_marker_minimum;
                }
            }
        }
    }
    
    void OnApplicationQuit()
    {
        if (_Reader_Depth != null)
        {
            _Reader_Depth.Dispose();
            _Reader_Depth = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
