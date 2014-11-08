﻿using UnityEngine;
using System.Collections;
using System;


public class EventArgs<T> : EventArgs
{
    public T Value { get; private set; }

    public EventArgs(T val)
    {
        Value = val;
    }
}

public class GeneralHelpers 
{
    public static Vector2 Perpendicular(Vector2 v)
    {
        return new Vector2(v.y, -v.x);
    }
    public static float PosifyRotation(float rotation)
    {
        return rotation > 0 ? rotation : rotation + Mathf.PI * 2f;
    }
    public static float AngleBetweenVectors(Vector2 p1, Vector2 p2)
    {
        float theta = Mathf.Atan2(Mathf.Abs(p2.y - p1.y), Mathf.Abs(p2.x - p1.x));
        //Debug.Log("Theta:" + "(" + (p2.y - p1.y) + ") / (" + (p2.x - p1.x) + ")");
        if (p2.y > p1.y)
        {
            if (p2.x > p1.x)
            {
                return theta;
            }
            else
            {
                return Mathf.PI - theta;
            }
        }
        else
        {
            if (p2.x > p1.x)
            {
                return Mathf.PI * 2 - theta;
            }
            else
            {
                return Mathf.PI + theta;
            }
        }
    }
}