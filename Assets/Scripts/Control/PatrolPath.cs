using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var j = GetNextIndex(i);
            Gizmos.DrawSphere(GetWayPoint(i), 0.5f);
            Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
        }
    }

    public int GetNextIndex(int i)
    {
        if (i + 1 == transform.childCount)
            return 0;
        
        return i + 1;
    }

    public Vector3 GetWayPoint(int i)
    {
        return transform.GetChild(i).position;
    }
}
