using UnityEngine;
using System.Collections;

public class Path : MonoBehaviour{
    //The List of Points in the path
    public System.Collections.Generic.List<Vector3> path;
    //The current point we're at
    public Vector3 current;
    //The index of the next point
    public int nPoint;
   // public float length;
    bool isLooped;
    
    public Path()
    {
        path = new System.Collections.Generic.List<Vector3>();
    }
    public Path(int test_code)
    {
        path = new System.Collections.Generic.List<Vector3>();
        switch(test_code)
        {
            case 0:
                path.Add(new Vector3(0, 5f, 0));
                path.Add(new Vector3(15f, 5, 15f));
                path.Add(new Vector3(15f, 5f, -15f));
                path.Add(new Vector3(-15f, 100f, -15f));
                path.Add(new Vector3(-15f, 5f, 15f));
                current = path[0];
                nPoint = 1;
                isLooped = true;
                break;
            case 1:
                path.Add(new Vector3(0, 5f, 0));
                path.Add(new Vector3(15f, 5, 15f));
                path.Add(new Vector3(15f, 5f, -15f));
                path.Add(new Vector3(-15f, 5f, -15f));
                path.Add(new Vector3(-15f, 5f, 15f));
                current = path[0];
                nPoint = 1;
                isLooped = false;
                break;

        }
    }

    public Path(System.Collections.Generic.List<Vector3> path, bool isLooped = false)
    {
        this.path = path;
        current = path[0];
        nPoint = 1;
        this.isLooped = isLooped;
    }

    public Path(string csv_filename)
    {
        Debug.Log("This function is currently unavailable");
    }




    //Returns the Length of the total path
    public float length()
    {
        float length = 0;
        if (path == null)
            return length;
        else
        {
            if (isLooped) {
                for (int i = 0; i < path.Count; i++)
                {
                    int j = (i + 1) % path.Count;
                    Vector3 block = path[j] - path[i];
                    length += block.magnitude;
                }
            }
            else
            {
                for (int i = 0; i < path.Count-1; i++)
                {
                    int j = (i + 1);
                    Vector3 block = path[j] - path[i];
                    length += block.magnitude;
                }
            }
            
        }
        return length;
    }


    public float travelled()
    {
        float path_length = 0;
        float length= 0;
        System.Collections.Generic.List<Vector3> path;

        if (nPoint == 0)
            path = this.path;
        else 
            path = this.path.GetRange(0, nPoint);


            for (int i = 0; i < path.Count - 1; i++)
            {
                int j = (i + 1);
                Vector3 block = path[j] - path[i];
                length += block.magnitude;
            }

        if (nPoint != 0)
            path_length = (this.path[nPoint - 1] - current).magnitude;
        else
            path_length = (this.path[this.path.Count -1] - current).magnitude;

        return length + path_length;
    }

    public Vector3 next()
    {
        current = path[nPoint];
        nPoint++;
        return current;
    }
    
    public Vector3 step(float dist)
    {
        Vector3 point = current;
        Vector3 path = this.path[nPoint] - point;
        while (dist >= 0)
        {
            //If we're looping
            if (nPoint >= (this.path.Count))
            {
                if (isLooped)
                    nPoint = 0;
                else
                    nPoint = this.path.Count - 1;
            }

            path = this.path[nPoint] - current;

            if (dist > path.magnitude && isLooped)
            {
                dist = dist - path.magnitude;
                current = new Vector3(this.path[nPoint].x, this.path[nPoint].y, this.path[nPoint].z);
                nPoint++;
            }
            else if(dist > path.magnitude && !isLooped && nPoint == this.path.Count -1)
            {
                current = this.path[nPoint];
                return current;
            }
            else if (dist < path.magnitude)
            {
                path.Normalize();
                path.Scale(new Vector3(dist, dist, dist));
                current = current + path;
                return current;
            }
            else
            {
                current = current + path;
                nPoint++;
                return current;
            }
        }
        return current;
    }

    public Vector3 jump_to(int index)
    {
        current = path[index];
        nPoint = (index + 1) % path.Count;
        return current;
    }

    public Vector3 jump_to(float distance)
    {
        current = path[0];
        nPoint = 1;
        step(distance);
        return current;
    }

    public void smooth(int smoothness)
    {
        for (int j = 0; j < smoothness; j++)
        {
            System.Collections.Generic.List<Vector3> newPoints = new System.Collections.Generic.List<Vector3>();

            for (int i = 0; i < path.Count - 1; i++)
            {

                Vector3 spath = path[i + 1] - path[i];

                spath.Scale(new Vector3(0.25f, 0.25f, 0.25f));
                newPoints.Add(path[i] + spath);

                spath = path[i + 1] - path[i];
                spath.Scale(new Vector3(0.25f, 0.25f, 0.25f));
                newPoints.Add(path[i] + spath);
            }
            path = newPoints;
        }
        current = path[0];
    }


}
