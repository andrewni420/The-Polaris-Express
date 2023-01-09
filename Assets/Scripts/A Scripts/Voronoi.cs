using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Voronoi Diagram", menuName = "PCG/Voronoi")]
public class Voronoi : ScriptableObject
{
    //todo: 
    //      put in fog gates
    //      make resources more scarce as levels go on
    //      maybe decrease neighborradius and make it randomize whether to place things down -> more random item generation
    //      create item prefabs to interact with player inventory (depends on prefabs being made)
    //      test navmeshagents on surface
    //      create navmeshobstacles at caves and mountains 
    private float mtnThickness = 10;
    private int levelSize;
    private float groundMountainRatio;
    private Vector2 center;
    private bool centerFound = false;
    private function falloff;

    //prevents infinite loops
    private int loopCounter = 0;



    private int count = 0;

    public delegate float function(float z, float x);

    public Vector2[] points;
    public Vector2[] caveEntrances;
    public Vector2[] gates;
    public bool generatePoints=true;
    public List<Bisector> bisectors= new List<Bisector>();

    public Vector2[][] starLocations;
    public Vector2[][] starDestinations;
    public Vector2 endLocation;
    public GameObject endGate;
    public Vector2 startLocation;

    public void setParams(float mtnThickness, float gmr, function f, int levelSize, int seed)
    {
        Random.InitState(seed);
        this.mtnThickness = mtnThickness;
        this.levelSize = levelSize;
        setGMR(gmr);
        setFalloff(f);
        if (generatePoints) points = choosePoints();
        points = choosePointsIfNone();
        chooseEntrances();
        
        centerFound = circumCenter(out center);
        if (centerFound && !inBox(center, 50f/levelSize))
        {
            if (Vector2.Dot(points[2] - points[0], points[1] - points[0]) < 0)
            {
                Vector2 temp = points[0];
                points[0] = points[1];
                points[1] = temp;
            }
        }
        
        calcBisectors();

        endLocation = chooseEndLocation();
        loopCounter = 0;

        chooseStarLocations();
        chooseStarDestinations();
        
    }
    public void setGMR(float gmr) { groundMountainRatio = gmr; }
    public void setFalloff(function f) { falloff = f; }

    public void chooseEntrances()
    {
        caveEntrances = new Vector2[3];
        for(int i=0;i<3;i++)
        {
            Vector2 r = Random.insideUnitCircle;
            caveEntrances[i] = points[i] + 8f*r/levelSize;
        }
    }

    public Vector2[] choosePointsIfNone()
    {
        if (points == null || points.Length < 3) return choosePoints();
        return points;
    }

    public Vector2[] choosePoints()
    {
        Vector2[] chosenPoints = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            //entire mountain range = 2 thickness. Cave = 1 thickness. 1 more thickness for walking space
            float zPos = Random.Range(4 * mtnThickness / levelSize, 1 - 4 * mtnThickness / levelSize);
            float xPos = Random.Range(4 * mtnThickness / levelSize, 1 - 4 * mtnThickness / levelSize);
            chosenPoints[i] = new Vector2(zPos, xPos);
        }

        //entire mountain range = 2 thickness. Cave at each end = 2 thickness, and 2 more thicknesses for ability to walk between cave and mountain
        float minDistance = 6 * mtnThickness;

        if (Vector2.Distance(chosenPoints[0], chosenPoints[1]) < minDistance/levelSize ||
            Vector2.Distance(chosenPoints[1], chosenPoints[2]) < minDistance/levelSize ||
            Vector2.Distance(chosenPoints[0], chosenPoints[2]) < minDistance/levelSize)
        {
            count++;
            return choosePoints();
        }

        if (Vector2.Distance(chosenPoints[2], chosenPoints[0]) > Vector2.Distance(chosenPoints[1], chosenPoints[0]))
        {
            Vector2 temp = chosenPoints[2];
            chosenPoints[2] = chosenPoints[1];
            chosenPoints[1] = temp;
        }

        return chosenPoints;
    }

    public bool circumCenter(out Vector2 center)
    {
        center = new Vector2();
        float[] angles = new float[3];
        for (int i = 0; i < 3; i++)
        {
            angles[i] = Vector2.Angle(points[(i+1)%3] - points[i], points[(i+2)%3] - points[i]);
            if (angles[i] < 0.001 || angles[i]>179.999) return false;
        }

        Vector3 sin = new Vector3(Mathf.Sin(angles[0] * Mathf.PI / 90), Mathf.Sin(angles[1] * Mathf.PI / 90), Mathf.Sin(angles[2] * Mathf.PI / 90));
        Vector3 x = new Vector3(points[0].x, points[1].x, points[2].x);
        Vector3 y = new Vector3(points[0].y, points[1].y, points[2].y);

        center= new Vector2(Vector3.Dot(sin, x), Vector3.Dot(sin, y)) / (sin.x + sin.y + sin.z);
        return true;
    }

    public bool circumRadius(out float radius)
    {
        radius = -1;
        Vector2 center;
        if (circumCenter(out center))
        {
            radius = Vector2.Distance(center, points[0]);
            return true;
        }
        return false;
    }

    public Bisector perpBisector(int i)
    {
        Vector2 one = points[i];
        Vector2 two = points[(i + 1) % 3];
        Vector2 three = points[(i + 2) % 3];

        Vector2 point = (one + two) / 2;
        Vector2 normal = (one - two).normalized;
        Bisector b = new Bisector(i, point, normal, three, Vector2.Dot(point, normal));
        if (centerFound && inBox(center,0))
        {
            b.start = center;
            Vector2 dir = new Vector2(normal.y, -normal.x);
            if (Vector2.Dot(point - three, dir) < 0) dir = -dir;

            float[] t = new float[4];
            float minPos = float.MaxValue;
            t[0] = (1 - center.x) / dir.x;
            t[1] = (-center.x) / dir.x;
            t[2] = (1 - center.y) / dir.y;
            t[3] = (-center.y) / dir.y;
            foreach (float ti in t) if (ti > 0) minPos = Mathf.Min(ti, minPos);

            b.end = center + minPos * dir;
            return b;
        }
        else
        {
            Vector2 dir = new Vector2(normal.y, -normal.x);
            float[] t = new float[4];
            t[0] = (1 - point.x) / dir.x;
            t[1] = (-point.x) / dir.x;
            t[2] = (1 - point.y) / dir.y;
            t[3] = (-point.y) / dir.y;

            float maxNeg = float.MinValue;
            float minPos = float.MaxValue;

            foreach (float ti in t)
            {
                if (ti < 0)
                {
                    maxNeg = Mathf.Max(ti, maxNeg);
                }
                else
                {
                    minPos = Mathf.Min(ti, minPos);
                }
            }

            b.start = point + maxNeg * dir;
            b.end = point + minPos * dir;
            return b;
        }
    }


    public float edgeToMountain((Vector2 point, Vector2 normal, float dot) b, Vector2 pos, float noise)
    {
        float mtn = mtnThickness / levelSize;
        float gmr = groundMountainRatio;
        float dist = Vector2.Dot(pos, b.normal) - b.dot;
        if (dist > mtn) return gmr*noise;
        dist = (mtn - dist)/ mtn;
        return falloff(dist,noise);
    }

    public bool inBox(Vector2 point, float offset)
    {
        return point.x > 0+offset && point.x < 1-offset && point.y > 0+offset && point.y < 1-offset;
    }

    public int closestEdge(Vector2 pos)
    {
        int to_return = 0;
        float minDist = float.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            float dist = Mathf.Abs(Vector2.Dot(pos, bisectors[i].normal) - bisectors[i].dot);
            if (dist < minDist)
            {
                minDist = dist;
                to_return = i;
            }
        }
        return to_return;
    }

    public void calcBisectors()
    {
        bisectors = new List<Bisector>();
        gates = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            bisectors.Add(perpBisector(i));
            if (generatePoints) bisectors[i].chooseFogGate();
            gates[i] = bisectors[i].fogGate;
        }
    }

    public float heightFunction(float noise, float z, float x)
    {
        return Mathf.Max(dividerHeight(noise, z, x), caveHeight(noise, z, x));
    }

    public float closestEdgeDistance(float z, float x)
    {
        Vector2 pos = new Vector2(z, x);
        float minDist = float.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            float dist = Mathf.Abs(Vector2.Dot(pos, bisectors[i].normal) - bisectors[i].dot);
            if (centerFound)
            {
                float rightSide = Vector2.Dot(bisectors[i].point - bisectors[i].opposite, pos - center);
                if (rightSide < 0) dist = Vector2.Distance(pos, center);
            }
            if (dist < minDist)
            {
                minDist = dist;
            }
        }
        return minDist;
    }

    public float dividerHeight(float noise, float z, float x)
    {
        float mtn = mtnThickness / levelSize;
        Vector2 pos = new Vector2(z, x);

        foreach (Bisector b in bisectors)
        {
            if (Vector2.Distance(b.fogGate, pos) < mtn) return 0;
        }
        if (Vector2.Distance(pos, endLocation) < mtn * 6 / 5) return 0;


        //Find closest edge
        float minDist = closestEdgeDistance(z, x);

        //Apply falloff function
        
        minDist = (mtn - minDist) / mtn;
        return falloff(minDist, noise);
    }

    public int section(float z, float x)
    {
        Vector2 pos = new Vector2(z, x);
        int to_return = 0;
        float minDist = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            float dist = Vector2.Distance(pos, points[i]);
            if (dist<minDist)
            {
                minDist = dist;
                to_return = i;
            }
        }
        return to_return;
    }

    public int section(Vector2 pos)
    {
        return section(pos.x, pos.y);
    }

    float caveHeight(float noise, float z, float x)
    {
        float cr = mtnThickness / levelSize;
        Vector2 pos = new Vector2(z, x);

        foreach (Vector2 e in caveEntrances) if (Vector2.Distance(e, pos) < cr/2) return 0;

        float minDist = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            float dist = Vector2.Distance(pos, points[i]);
            if (dist < minDist)
            {
                minDist = dist;
            }
        }

        if (minDist < cr)
        {
            //minDist goes from 0 at the edge of the hill to 1 at the center
            minDist = (cr - minDist) / cr;
            //shape minDist to go to 0.9 early and slowly increase to 1
            minDist = Mathf.Min(minDist * 8 / 6, minDist / 2f + 0.5f);

            //d = 2 * Mathf.Min(d, 0.75f);

            //Sin produces a nice rounded curve appropriate for a small hill. Cap it at 1/3 since it's a small hill
            //float r = Mathf.Sin(d * Mathf.PI / 2)/4f;
            //linearly increase noise amplitude
            //return Mathf.Pow(7, d) / 10 - 0.1f + (gmr + (0.4f - gmr) * d) * noise;//
            return (groundMountainRatio + 0.2f * minDist) * noise + minDist / 4;
        }

        return groundMountainRatio * noise;
    }

    public int nearestGate(Vector2 pos)
    {
        float minDist = float.MaxValue;
        int index = -1;
        for (int i = 0; i < bisectors.Count; i++)
        {
            if (bisectors[i].fogGate == new Vector2(-1, -1)) continue;
            float dist = Vector2.Distance(pos, bisectors[i].fogGate);
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }
        return index;
    }

    public void chooseStarLocations()
    {

        List<Vector2>[] starLocLists = new List<Vector2>[3] { new List<Vector2>(), new List<Vector2>(), new List<Vector2>()};
        while(starLocLists[0].Count<3 || starLocLists[1].Count < 3 || starLocLists[2].Count < 3)
        {
            loopCounter++;
            if (loopCounter>200)
            {
                Debug.Log(("first while loop", starLocLists[0].Count, starLocLists[1].Count, starLocLists[2].Count));
                return;
            }
            Vector2 pos = randomSafePos();
            int sec = section(pos);
            if (starLocLists[sec].Count < 3) starLocLists[sec].Add(pos);
        }

        starLocations = new Vector2[3][];
        for (int i = 0; i < 3; i++)
        {
            starLocations[i] = starLocLists[i].ToArray();
        }

        //starLocations = new Vector2[3][];
        //for (int i = 0; i < 3; i++)
        //{
        //    starLocations[i] = new Vector2[3];
        //    for (int j = 0; j < 3; j++)
        //    {
        //        starLocations[i][j] = chooseStarHelper(i);
        //        loopCounter = 0;
        //    }
        //}
    }

    public Vector2 randomPos()
    {
        float mtn = mtnThickness / levelSize;
        return new Vector2(Random.Range(mtn, 1 - mtn), Random.Range(mtn, 1 - mtn));
    }

    public Vector2 randomSafePos()
    {
        float mtn = mtnThickness / levelSize;
        int rspCounter = 0;
        Vector2 pos = randomPos();
        while(closestPointDistance(pos) < mtn || closestEdgeDistance(pos.x, pos.y) < mtn)
        {
            pos = randomPos();
            rspCounter++;
            if (rspCounter > 100)
            {
                Debug.Log("rsp failed");
                return new Vector2();
            }
        }
        return pos;
    }

    public Vector2 chooseStarHelper(int s)
    {
        float mtn = mtnThickness / levelSize;
        loopCounter++;
        if (loopCounter > 100)
        {
            Debug.Log(("loop failed starhelper", s));
            return new Vector2(0, 0);
        }

        Vector2 pos = new Vector2(Random.Range(mtn, 1 - mtn), Random.Range(mtn, 1 - mtn));
        if (section(pos) != s) return chooseStarHelper(s);
        if (closestPointDistance(pos) < mtn) return chooseStarHelper(s);
        if (closestEdgeDistance(pos.x, pos.y) < mtn) return chooseStarHelper(s);
        return pos;
    }

    public void chooseStarDestinations()
    {
        float mtn = mtnThickness / levelSize;
        starDestinations = new Vector2[3][];
        Vector2[] dir = new Vector2[3];
        dir[0] = bisectors[0].fogGate-points[0];
        dir[1] = bisectors[1].fogGate - points[1];
        dir[2] = endLocation - points[2];

        float length = 1 - mtn;
        float[] values = new float[] { mtn + length / 6, mtn + length / 2, mtn + 5 * length / 6 };

        for (int i = 0; i < 3; i++)
        {
            starDestinations[i] = new Vector2[] { points[i] + dir[i]*values[0], points[i] + dir[i] * values[1], points[i] + dir[i] * values[2] };
        }

    }

    public Vector2 chooseEndLocation()
    {
        float mtn = mtnThickness / levelSize*4/5;
        loopCounter += 1;
        if (loopCounter > 100)
        {
            Debug.Log("loop failed endlocation");
            return new Vector2(0, 0);
        }

        Vector2 loc;
        float temp = Random.Range(mtn, 1 - mtn);

        loc = new Vector2(mtn, temp);
        if (section(loc) == 2) return loc;

        loc = new Vector2(1 - mtn, temp);
        if (section(loc) == 2) return loc;

        loc = new Vector2(temp, mtn);
        if (section(loc) == 2) return loc;

        loc = new Vector2(temp, 1 - mtn);
        if (section(loc) == 2) return loc;

        Debug.Log(("section", section(loc)));
        return chooseEndLocation();
    }

    

    public void chooseStartLocation()
    {
        //maybe spawn player in cave 1 and have tutorial there
    }


    public float closestPointDistance(Vector2 pos)
    {
        float dist = float.MaxValue;
        foreach (Vector2 p in points)
        {
            dist = Mathf.Min(dist, Vector2.Distance(p, pos));
        }
        return dist;
    }

    public Vector2 getDir(Vector2 pos)
    {
        if (Mathf.Min(pos.x, 1 - pos.x) < Mathf.Min(pos.y, 1 - pos.y)) return new Vector2(0, 1);
        return new Vector2(1, 0);
        //float mtn = mtnThickness / levelSize * 4 / 5;
        //if (pos.x == mtn|| pos.x==1-mtn) return new Vector2(0, 1);
        //return new Vector2(1, 0);
    }

}

public class Bisector
{
    public int i;
    public Vector2 start;
    public Vector2 end;

    public Vector2 fogGate;
    public GameObject gateObject;

    public Vector2 point;
    public Vector2 normal;
    public Vector2 opposite;
    public float dot;

    public Bisector(int _i, Vector2 p, Vector2 n, Vector2 o, float d)
    {
        i = _i;
        point = p;
        normal = n;
        opposite = o;
        dot = d;
    }

    public void chooseFogGate()
    {
        
        float pos = Random.Range(0.3f, 0.7f);
        fogGate = start + pos * (end - start);
        if (i == 2) fogGate = new Vector2(-1, -1);
    }
}
