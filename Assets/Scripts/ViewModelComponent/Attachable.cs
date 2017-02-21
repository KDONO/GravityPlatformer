using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All objects that can be attached to a surface or steel crate
public abstract class Attachable : MonoBehaviour {
    public abstract void OnCollisionEnter2D(Collision2D col);
}
