using System.Collections;
using UnityEngine;

public static class DirectionsExtensions {
    public static Vector2 ToVector2(this Directions d)
    {
        switch (d)
        {
            case Directions.North:
                return Vector2.up;
            case Directions.East:
                return Vector2.right;
            case Directions.South:
                return Vector2.down;
            case Directions.West:
                return Vector2.left;
            default:
                return Vector2.zero;
        }
    }

    public static Vector3 ToVector3(this Directions d)
    {
        switch (d)
        {
            case Directions.North:
                return Vector3.up;
            case Directions.East:
                return Vector3.right;
            case Directions.South:
                return Vector3.down;
            case Directions.West:
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    }

    public static Vector3 ToEuler(this Directions d)
    {
        return new Vector3(0, 0, (int)d * 90);
    }
}
