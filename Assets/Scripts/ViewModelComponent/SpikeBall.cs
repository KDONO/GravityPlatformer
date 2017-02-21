using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : Attachable {
    public override void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // To Death State
            Destroy(col.gameObject);
        }
    }
}
