using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoidData
{
    public Vector3 position;
    public Vector3 foward;

    public Vector3 aliAcc;
    public Vector3 sepAcc;
    public Vector3 cohAcc;

    public int perceptNum;

    public static int GetSize()
    {
        return sizeof(float) * 3 * 5 + sizeof(int);
    }
}
