using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All objects that can be attached to a surface or steel crate
public abstract class Attachable : MonoBehaviour {
    public LayerMask canAttachToLayerMask; // what kinds of objects can we be attached to

    protected int zRot; // euler angle of the z rotation as an int
    protected Directions facing; // direction the object is facing
    protected Directions oppositeFacing; // opposite the direction is facing aka into the object

    protected string parentLayer; // stores the layer of object we're on

    // always call these at start
    public virtual void Start()
    {
        SetFacingsAndZRot();
        AttachToTile();
    }

    // set up facings and zRot values
    protected void SetFacingsAndZRot()
    {
        zRot = (int)transform.eulerAngles.z; // set z rotation

        // set facing and opposite facing depending on the z rotation
        facing = PhysicsUtilities.GetFacingDirection(zRot);
        oppositeFacing = PhysicsUtilities.GetFacingDirection(zRot, true);
    }

    // child the object to any valid object that it can be on and store what kind of object that is
    protected void AttachToTile()
    {
        Vector3 offset = PhysicsUtilities.OffsetToEdgeRenderer(GetComponent<SpriteRenderer>(), oppositeFacing);
        RaycastHit2D hit = Physics2D.Raycast(transform.position + offset, oppositeFacing.ToVector2(), 0.1f, canAttachToLayerMask);
        if (hit.collider != null)
        {
            transform.parent = hit.transform;
            parentLayer = LayerMask.LayerToName(hit.transform.gameObject.layer);
        }
    }
}
