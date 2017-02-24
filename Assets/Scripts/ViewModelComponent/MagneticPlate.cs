using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticPlate : Attachable {
    GameObject attachedToPlate; // stores the object attached to the plate
    Directions storedDir; // stores the direction gravity was in when the player crossed over it

    // releases any attached object - is public so buttons can access it
    public void ReleaseAttachedObject()
    {
        if (attachedToPlate != null)
        {
            // release steel crates and unfreeze them
            if (attachedToPlate.gameObject.layer == LayerMask.NameToLayer("SteelCrates"))
            {
                attachedToPlate.GetComponent<SteelCrate>().gravEnabled = true;
                attachedToPlate = null;
            }
            // allow the player to be affected by gravity again
            else if (attachedToPlate.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                attachedToPlate.GetComponent<PlayerController>().gravEnabled = true;
                if (storedDir != GameController.Dir)
                    StartCoroutine(attachedToPlate.GetComponent<PlayerController>().Turn(GameController.Dir));
                attachedToPlate = null;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // check for collisions if the plate does not have an object
        if (attachedToPlate == null)
        {
            // attach our steel crate only if gravity is going into the plate, reposition it, and freeze all physics on it
            if (col.gameObject.layer == LayerMask.NameToLayer("SteelCrates") && GameController.Dir == oppositeFacing)
            {
                attachedToPlate = col.gameObject;
                StartCoroutine(AdjustPosition(attachedToPlate.transform));
                attachedToPlate.GetComponent<SteelCrate>().gravEnabled = false;
            }

            // NEED TO CHANGE INPUT TO STAY ON STORED AXIS - DO THAT HERE
            // attach our player and prevent gravity from being applied to it
            if(col.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                attachedToPlate = col.gameObject;
                StartCoroutine(AdjustPosition(attachedToPlate.transform));
                attachedToPlate.GetComponent<PlayerController>().gravEnabled = false;
                storedDir = GameController.Dir;
            }
        }
    }

    // release the object when it leaves the trigger
    void OnTriggerExit2D()
    {
        ReleaseAttachedObject();
    }
  
    // move the player to the transform we desire
    // this will artificially slow down the player too as they just attempt to walk over it
    IEnumerator AdjustPosition(Transform t)
    {
        Tweener tweener = t.MoveTo(transform.position, 0.25f, EasingEquations.Linear);
        while (tweener != null)
            yield return null;
    }
}
