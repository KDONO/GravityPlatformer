using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : Attachable
{
    void FixedUpdate()
    {
        // SPIKES NOT ALWAYS ACTIVE DUE TO GRAV TRANSITION CURRENTLY ENDING WHEN THE PLAYER LANDS
        // if in grav transition and going in the direction where the spikes would be active, set it to the Active layer
        if (GameController.gravTransitionState)
        {
            // is gravity going into the spikes?
            if (GameController.Dir == oppositeFacing)
                gameObject.layer = LayerMask.NameToLayer("Active");
        }
        // otherwise, it is Inactive
        else
            gameObject.layer = LayerMask.NameToLayer("Inactive");
    }

    // NEED TO MAKE IT SO THAT SPIKES WORK WHEN ON STEEL CRATES
    void OnCollisionEnter2D(Collision2D col)
    {
        // only destroy the player if the spikes are active
        if (gameObject.layer != LayerMask.NameToLayer("Inactive") && col.gameObject.CompareTag("Player"))
        {
            // Death State
            Destroy(col.gameObject);
        }
    }
}
