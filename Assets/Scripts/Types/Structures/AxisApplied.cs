using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AxisApplied
{
    public Axis axis;
    public float units;

    public Space space;

    public AxisApplied(Axis axis, float units, Space space = Space.Self)
    {
        this.axis = axis;
        this.units = units;

        this.space = space;
    }
}
