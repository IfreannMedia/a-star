using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public class FindPathAStar : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
