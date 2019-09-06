using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;
using System;
using System.Linq;

public class InfraredSourceManager : MonoBehaviour
{
    private KinectSensor _Sensor;

    private InfraredFrameReader _Reader_Infrared;
    private DepthFrameReader _Reader_Depth;

    private ushort[] _Data_Depth;

    private ushort[] _Data_Infrared;
    private byte[] _RawData;

    public int[,] infrared_matrix;
    public int[,] depth_matrix;

    private int width;
    private int height;

    private int check_distance = 5;
    public ushort maxDepth;

    public List<int> markers_depth;
    public List<Marker> markers;

    public struct Coord
    {
        public int x, y;

        public Coord(int p1, int p2)
        {
            x = p1;
            y = p2;
        }
    }

    public struct Marker
    {
        public List<Coord> pixels;

        public Marker(List<Coord> p)
        {
            pixels = p;
        }

        public void Add(List<Coord> p)
        {
            pixels.AddRange(p);
        }
    }

    // I'm not sure this makes sense for the Kinect APIs
    // Instead, this logic should be in the VIEW
    private Texture2D _Texture;

    public Texture2D GetInfraredTexture()
    {
        return _Texture;
    }

    void Start()
    {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Reader_Infrared = _Sensor.InfraredFrameSource.OpenReader();
            _Reader_Depth = _Sensor.DepthFrameSource.OpenReader();

            _Data_Depth = new ushort[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];

            var frameDesc = _Sensor.InfraredFrameSource.FrameDescription;
            _Data_Infrared = new ushort[frameDesc.LengthInPixels];
            _RawData = new byte[frameDesc.LengthInPixels * 4];
            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.BGRA32, false);

            Debug.Log("Width: " + frameDesc.Width + ", Height: " + frameDesc.Height);
            width = frameDesc.Width;
            height = frameDesc.Height;

            infrared_matrix = new int[height, width];
            depth_matrix = new int[height, width];

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }

    ////////////////////////////////////////DEPTH////////////////////////////////////////////////////////
    List<Coord> get_border(List<Coord> marker_pixels)
    {
        List<Coord> ress = new List<Coord>();

        foreach (Coord pixel in marker_pixels)
        {
            //checkear los bordes 
            if (pixel.x - check_distance > 0 && pixel.y - check_distance > 0)
            {
                int value = infrared_matrix[pixel.x - check_distance, pixel.y - check_distance];
                if (value != 255)
                {
                    bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x - check_distance && list_pixel.y == pixel.y - check_distance);
                    if (!contains)
                    {
                        Coord new_border = new Coord(pixel.x - check_distance, pixel.y - check_distance);
                        ress.Add(new_border);
                    }
                }
            }
            if (pixel.x + check_distance < width && pixel.y + check_distance < height)
            {
                try
                {
                    int value = infrared_matrix[pixel.x + check_distance, pixel.y + check_distance];
                    if (value != 255)
                    {
                        bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x + check_distance && list_pixel.y == pixel.y + check_distance);
                        if (!contains)
                        {
                            Coord new_border = new Coord(pixel.x + check_distance, pixel.y + check_distance);
                            ress.Add(new_border);
                        }
                    }
                }
                catch (Exception e)
                {
                }

            }
            if (pixel.x - check_distance > 0)
            {
                int value = infrared_matrix[pixel.x - check_distance, pixel.y];
                if (value != 255)
                {
                    bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x - check_distance && list_pixel.y == pixel.y);
                    if (!contains)
                    {
                        Coord new_border = new Coord(pixel.x - check_distance, pixel.y);
                        ress.Add(new_border);
                    }
                }
            }
            if (pixel.y - check_distance > 0)
            {
                int value = infrared_matrix[pixel.x, pixel.y - check_distance];
                if (value != 255)
                {
                    bool contains = ress.Any(list_pixel => list_pixel.x == pixel.x && list_pixel.y == pixel.y - check_distance);
                    if (!contains)
                    {
                        Coord new_border = new Coord(pixel.x, pixel.y - check_distance);
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
                            Coord new_border = new Coord(pixel.x + check_distance, pixel.y);
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
                            Coord new_border = new Coord(pixel.x, pixel.y + check_distance);
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

    int get_minimum(List<Coord> border)
    {
        int minimum = -1;

        foreach (Coord pixel in border)
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

    ////////////////////////////////////////DEPTH////////////////////////////////////////////////////////


    ////////////////////////////////////////INFRA////////////////////////////////////////////////////////
    //esta a distancia 1 en mi matriz? tambien cuenta diagonales como distancia 1
    bool is_neighbor(Coord a, Coord b)
    {
        if (Math.Abs(a.x - b.x) <= 1 && Math.Abs(a.y - b.y) <= 1)
            return true;
        return false;
    }

    List<Marker> find_markers(List<Coord> blank_spaces)
    {
        //el tamaño minimo
        int min_size = 1;

        //Recorrer todos los espacios en blanco, se comprara el que sigue con el anterior
        //si la distancia es 1 entonces forman parte del mismo marker, sino se compara con el anterior 
        //del anterior, hasta regresar al primer elemento....si no tiene distancia 1 con ninguno, entonces 
        //forma parte de otro marcador y se repite el proceso

        List<Marker> result = new List<Marker>();

        bool new_marker = true;
        int checkpoint = 0;
        List<Coord> marker_spaces = null;//solo para evitar "Use of unassigned local variable"

        for (int i = 0; i< blank_spaces.Count; i++)
        {
            //si agregaremos un nuevo marker entonces se crea una nueva lista y se agrega el primer elemento
            //solo se compara los puntos con los del nuevo marker, ya no con los anteriores, para tener mejor rendimiento (checkpoint)
            if (new_marker)
            {
                marker_spaces = null;
                marker_spaces = new List<Coord>();
                marker_spaces.Add(blank_spaces[i]);
                new_marker = false;
                checkpoint = i;
            }
            else
            {
                int pivot = i - 1;

                while(pivot != checkpoint - 1)
                {
                    if (is_neighbor(blank_spaces[i], blank_spaces[pivot])){//comparo i contra todos hasta el checkpoint, si son vecinos los junto
                        marker_spaces.Add(blank_spaces[i]);
                        break;//si ya es vecino, ya no necesito comparar con los demas
                    }
                    else//busco el anterior
                    {
                        pivot--;
                    }
                }

                if (pivot == checkpoint - 1)//es decir que el while no termino por encontrar un vecino, sino que no encontro ningun vecino
                {
                    //Debug.Log("Contador del marker: " + marker_spaces.Count);
                    if (marker_spaces.Count > min_size)//si es lo suficientemente grande lo guardo
                    {
                        Marker to_save = new Marker(marker_spaces);
                        result.Add(to_save);//agrego mi marker a la lista
                    }

                    //Preparar el nuevo marker
                    new_marker = true;
                    i--;//regreso el i en 1 para que no se pase ese valor inicial de la nueva lista
                }
            }
        }
        //No guarda el ultimo marker...asi que
        if (marker_spaces != null && marker_spaces.Count > min_size)//si es lo suficientemente grande lo guardo
        {
            Marker to_save = new Marker(marker_spaces);
            result.Add(to_save);//agrego mi marker a la lista
        }

        //ALGORITMO PARA AMORTIGUAR MARCADORES PEQUEÑOS NO CIRCULARES
        ////buscaremos por una fusion, entre grupos que son vecinos pero que el algoritmo inicial no reconoce
        //es decir, puede ser vecino con un elemento de otro marker...entonces fusion
        int fusion_times = 5;
        for(int x = 0; x < fusion_times; x++)
        {
            int total_markers = result.Count;
            bool not_fus;
            for (int a = 0; a < total_markers; a++)
            {
                for (int b = a + 1; b < total_markers; b++)
                {
                    not_fus = true;
                    //comparo el a con el b, pero no cuando son el mismo, ni repito lo que ya verifique
                    //comparo todos los del A con los del B
                    for (int j = 0; j < result[a].pixels.Count && not_fus; j++)
                    {
                        for (int k = 0; k < result[b].pixels.Count && not_fus; k++)
                        {
                            if (is_neighbor(result[a].pixels[j], result[b].pixels[k]))
                            {
                                //fusion de A y B...y borro B
                                //Debug.Log("FUSION");
                                result[a].Add(result[b].pixels);
                                result.RemoveAt(b);
                                b--;
                                not_fus = false;
                                total_markers = result.Count;
                                continue;
                            }
                        }
                    }
                }
            }
        }
        //////////////
        return result;
    }

    ////////////////////////////////////////INFRA////////////////////////////////////////////////////////


    void Update()
    {
        if (_Reader_Infrared != null && _Reader_Depth != null)
        {
            var infrared_frame = _Reader_Infrared.AcquireLatestFrame();
            var depth_frame = _Reader_Depth.AcquireLatestFrame();

            if (infrared_frame != null && depth_frame != null)
            {
                infrared_frame.CopyFrameDataToArray(_Data_Infrared);

                depth_frame.CopyFrameDataToArray(_Data_Depth);
                ushort minDepth = depth_frame.DepthMinReliableDistance;
                maxDepth = depth_frame.DepthMaxReliableDistance;


                ///INFRARED
                int index = 0;

                int column = 0;
                int row = 0;
                List<Coord> blank_spaces = new List<Coord>();

                foreach (var ir in _Data_Infrared)
                {
                    byte intensity = (byte)(ir >> 8);

                    if (column == width)
                    {
                        column = 0;               
                        row++;
                    }

                    try
                    {
                        infrared_matrix[row, column] = intensity;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("ERROR: row" + row + " column" + column);
                    }

                    infrared_matrix[row, column] = intensity;

                    //si el punto es completamente blanco, entonces forma parte de un marker
                    if(intensity == 255)
                    {
                        Coord aux = new Coord(row, column);
                        blank_spaces.Add(aux);
                    }

                    /*if (counting < 100 && intensity == 255)
                    {
                        Debug.Log("intensity: " + intensity);
                        Debug.Log("index: " + index);

                        counting++;
                    }*/

                    _RawData[index++] = intensity;
                    _RawData[index++] = intensity;
                    _RawData[index++] = intensity;
                    _RawData[index++] = 255; // Alpha

                    column++;
                }
                
                _Texture.LoadRawTextureData(_RawData);
                _Texture.Apply();

                infrared_frame.Dispose();
                infrared_frame = null;
                ///INFRARED
                
                ///DEPTH
                column = 0;
                row = 0;

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
                ///DEPTH

                //checkear si hay demasiado blanco
                if (blank_spaces.Count < width*height * 5/100)
                {
                    markers = find_markers(blank_spaces);

                    List<int> tmp_marker_minimum = new List<int>();

                    //algoritmo para llenar una lista con los bordes para cada marcador marcadores
                    for (int i = 0; i < markers.Count; i++)
                    {
                        List<InfraredSourceManager.Coord> marker_pixels = markers[i].pixels;
                        List<InfraredSourceManager.Coord> borders = get_border(marker_pixels);
                        tmp_marker_minimum.Add(get_minimum(borders));
                    }
                    markers_depth = tmp_marker_minimum;
                }
                else
                {
                    markers = null;
                    markers_depth = null;
                }


                //Algoritmo de deteccion de marcadores

                /*if (markers.Count>0)
                    Debug.Log("TOTAL DE MARCADORES:" + markers.Count);*/

                /* index maximo es 868 352 */
                /* van de 4 en 4 ...asi que son 217 088 */
                /* ventana es 512 424 */
                
            }
        }
    }

    void OnApplicationQuit()
    {
        if (_Reader_Infrared != null)
        {
            _Reader_Infrared.Dispose();
            _Reader_Infrared = null;
        }
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
