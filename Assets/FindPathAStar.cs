using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PathMarker
{
    public MapLocation mapLocation;
    public float G;
    public float H;
    public float F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation l, float g, float h, float f, GameObject marker, PathMarker parent)
    {
        this.mapLocation = l;
        this.G = g;
        this.H = h;
        this.F = f;
        this.marker = marker;
        this.parent = parent;
    }

    public override bool Equals(object obj)
    {
        return obj != null && obj.GetType().Equals(GetType())
            && ((PathMarker)obj).mapLocation.Equals(mapLocation);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class FindPathAStar : MonoBehaviour
{

    public Maze maze;
    public Material closedMat;
    public Material openMat;

    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();

    public GameObject start;
    public GameObject end;
    public GameObject pathP;

    PathMarker goalNode;
    PathMarker startNode;
    PathMarker lastPos;
    bool done = false;

    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject go in markers)
        {
            Destroy(go);
        }
    }

    void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();

        List<MapLocation> locations = new List<MapLocation>();
        for (int x = 1; x < maze.width - 1; x++)
            for (int z = 1; z < maze.depth - 1; z++)
            {
                if (maze.map[x, z] != 1)
                    locations.Add(new MapLocation(x, z));
            }

        locations.Shuffle();

        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].x), 0, 0, 0,
                                    Instantiate(start, startLocation, Quaternion.identity), null);

        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        goalNode = new PathMarker(new MapLocation(locations[1].x, locations[1].x), 0, 0, 0,
                                    Instantiate(end, goalLocation, Quaternion.identity), null);

        open.Clear();
        closed.Clear();

        open.Add(startNode);
        lastPos = startNode;

    }

    void Search(PathMarker thisNode)
    {
        if (thisNode.Equals(goalNode))
        {
            done = true;
            return;
        }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbour = dir + thisNode.mapLocation;
            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            if (neighbour.x < 1 || neighbour.x >= maze.width || neighbour.z < 1 || neighbour.z >= maze.depth) continue;
            if (IsClosed(neighbour)) continue;

            float G = Vector2.Distance(thisNode.mapLocation.ToVector(), neighbour.ToVector()) + thisNode.G;
            float H = Vector2.Distance(neighbour.ToVector(), goalNode.mapLocation.ToVector());
            float F = G + H;

            GameObject pathBlock = Instantiate(pathP, new Vector3(neighbour.x * maze.scale, 0, neighbour.z * maze.scale), Quaternion.identity);

            TextMesh[] values = pathBlock.GetComponentsInChildren<TextMesh>();
            values[0].text = "G: " + G.ToString("0.00");
            values[1].text = "H: " + H.ToString("0.00");
            values[2].text = "F: " + F.ToString("0.00");

            if (!UpdateMarker(neighbour, G, H, F, thisNode))
                open.Add(new PathMarker(neighbour, G, H, F, pathBlock, thisNode));
        }

        open = open.OrderBy(p => p.F).ToList<PathMarker>();
        PathMarker pm = open.ElementAt(0);
        closed.Add(pm);
        open.RemoveAt(0);
        pm.marker.GetComponent<Renderer>().material = closedMat;

        lastPos = pm;
    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {
        foreach (PathMarker path in open)
        {
            if (path.mapLocation.Equals(pos))
            {
                path.G = g;
                path.H = h;
                path.F = f;
                path.parent = prt;
                return true;
            }
        }
        return false;
    }

    bool IsClosed(MapLocation marker)
    {
        foreach (PathMarker p in closed)
        {
            if (p.mapLocation == marker) return true;
        }
        return false;
    }
    private void GetPath()
    {
        RemoveAllMarkers();
        PathMarker begin = lastPos;

        while(!startNode.Equals(begin) && begin != null)
        {
            Instantiate(pathP, new Vector3(begin.mapLocation.x * maze.scale, 0, begin.mapLocation.z * maze.scale), Quaternion.identity);
            begin = begin.parent;
        }
        Instantiate(pathP, new Vector3(startNode.mapLocation.x * maze.scale, 0, startNode.mapLocation.z * maze.scale), Quaternion.identity);
        // PlacePathMarker(begin);

    }

    //private void PlacePathMarker(PathMarker currentMarker)
    //{
    //    Vector3 loc = new Vector3(currentMarker.mapLocation.x * maze.scale, 0, currentMarker.mapLocation.z * maze.scale);
    //    Instantiate(pathP, loc, Quaternion.identity);
    //    if (currentMarker.parent != null)
    //        PlacePathMarker(currentMarker.parent);

    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            BeginSearch();
        if (Input.GetKeyDown(KeyCode.C) && !done)
            Search(lastPos);
        if (Input.GetKeyDown(KeyCode.M))
            GetPath();
    }

}
