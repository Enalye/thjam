﻿using UnityEngine;
using System;

public class BezierSpline : MonoBehaviour {
    public Path path;
    public int linesteps;

    public Vector3 GetPoint(float t) {
        return transform.TransformPoint(Bezier.GetPoint(path.points[0], path.points[1], path.points[2], path.points[3], t));
    }

    public Vector3 GetVelocity(float t) {
        return transform.TransformPoint(Bezier.GetFirstDerivative(path.points[0], path.points[1], path.points[2], path.points[3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t) {
        return GetVelocity(t).normalized;
    }

    public void Reset() {
		if(path == null) {
			path = new Path(4);
		}

        path.points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
    }
}