using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : Attachable
{
    Directions _activeDirection; // direction when the spikes are active
    int _activeAttachableLayer; // the layer for ActiveAttachable
    int _zRot; // rotation z value

    void Start()
    {
        _zRot = (int) transform.rotation.eulerAngles.z;
        _activeAttachableLayer = LayerMask.NameToLayer("ActiveSpike");

        // in what direction are the spikes active
        // spikes can only be in vary degrees of 90
        switch (_zRot)
        {
            case 0:
                _activeDirection = Directions.South;
                break;
            case 90:
                _activeDirection = Directions.East;
                break;
            case 180:
                _activeDirection = Directions.North;
                break;
            case 270:
                _activeDirection = Directions.West;
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        // if in grav transition and going in the direction where the spikes would be active, set it to the ActiveAttachable layer
        if (GameController.gravTransitionState)
        {
            if (GameController.Dir == _activeDirection)
                gameObject.layer = _activeAttachableLayer;
        }
        // otherwise, it is InactiveAttachable
        else
            gameObject.layer = LayerMask.NameToLayer("Inactive");
    }

    public override void OnCollisionEnter2D(Collision2D col)
    {
        // only destroy the player if the spikes are active
        if (_activeAttachableLayer == gameObject.layer && col.gameObject.CompareTag("Player"))
        {
            // Death State
            Destroy(col.gameObject);
        }
    }
}
