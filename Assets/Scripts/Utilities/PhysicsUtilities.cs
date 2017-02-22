using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsUtilities {
    public static float gravity = 9.81f;
    
    // apply the force of gravity
    public static void ApplyGravity(Rigidbody2D rb)
    {
        switch (GameController.Dir)
        {
            case Directions.North:
                rb.AddForce(new Vector2(0, gravity));
                break;
            case Directions.East:
                rb.AddForce(new Vector2(gravity, 0));
                break;
            case Directions.South:
                rb.AddForce(new Vector2(0, -gravity));
                break;
            case Directions.West:
                rb.AddForce(new Vector2(-gravity, 0));
                break;
            default:
                break;
        }
    }

    // check if a is below b
    public static bool IsBelow(Vector2 a, Vector2 b)
    {
        switch (GameController.Dir)
        {
            case Directions.North:
                if (a.y > b.y)
                    return true;
                break;
            case Directions.South:
                if (a.y < b.y)
                    return true;
                break;
            case Directions.East:
                if (a.x > b.x)
                    return true;
                break;
            case Directions.West:
                if (a.x < b.x)
                    return true;
                break;
            default:
                break;
        }
        return false;
    }

    // the following cast a Raycast in the direction of gravity
    public static RaycastHit2D RaycastToGravity(Vector2 position, float dist, LayerMask layer)
    {
        if(layer > int.MinValue)
            return Physics2D.Raycast(position, GameController.Dir.ToVector2(), dist, layer);
        return Physics2D.Raycast(position, GameController.Dir.ToVector2(), dist);
    }

    // overload in case we don't care about a LayerMask
    public static RaycastHit2D RaycastToGravity(Vector2 position, float dist)
    {
        return RaycastToGravity(position, dist, int.MinValue);
    }

    // find the distance to the edge of the Renderer in the desired direction
    public static Vector2 OffsetToEdgeRenderer(Renderer rend, Directions dir)
    {
        switch (dir)
        {
            case Directions.North:
                return new Vector2(0, rend.bounds.size.y/2f);
            case Directions.South:
                return new Vector2(0, -rend.bounds.size.y/2f);
            case Directions.East:
                return new Vector2(rend.bounds.size.x/2f, 0);
            case Directions.West:
                return new Vector2(-rend.bounds.size.x/2f, 0);
            default:
                break;
        }

        return Vector2.zero;
    }

    // overload OffsetToEdgeRenderer if we only care about the world direction
    public static Vector2 OffsetToEdgeRenderer(Renderer rend)
    {
        return OffsetToEdgeRenderer(rend, GameController.Dir);
    }

    // find the distance to the edge of the Collider in the desired direction
    public static Vector2 OffsetToEdgeCollider2D(Collider2D col, Directions dir)
    {
        switch (dir)
        {
            case Directions.North:
                return new Vector2(0, col.bounds.size.y/2f);
            case Directions.South:
                return new Vector2(0, -col.bounds.size.y/2f);
            case Directions.East:
                return new Vector2(col.bounds.size.x/2f, 0);
            case Directions.West:
                return new Vector2(-col.bounds.size.x/2f, 0);
            default:
                break;
        }

        return Vector2.zero;
    }

    // overload OffsetToEdgeCollider2D if we only care about the world direction
    public static Vector2 OffsetToEdgeCollider2D(Collider2D col)
    {
        return OffsetToEdgeCollider2D(col, GameController.Dir);
    }
}
