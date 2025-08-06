using UnityEngine;
using System.Collections.Generic;

public class Vector3Comparer : IEqualityComparer<Vector3>
{
    private readonly float tolerance;

    public Vector3Comparer(float tolerance = 0.00001f)
    {
        this.tolerance = tolerance;
    }

    public bool Equals(Vector3 v1, Vector3 v2)
    {
        return Vector3.SqrMagnitude(v1 - v2) < tolerance * tolerance;
    }

    public int GetHashCode(Vector3 v)
    {
        int x = Mathf.RoundToInt(v.x / tolerance);
        int y = Mathf.RoundToInt(v.y / tolerance);
        int z = Mathf.RoundToInt(v.z / tolerance);
        return x ^ (y << 2) ^ (z >> 2);
    }
}