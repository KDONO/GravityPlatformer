using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelCrate : MonoBehaviour {
    Rigidbody2D _rb;
    Renderer _rend;
    bool _grounded;

	void Start () {
        _rb = GetComponent<Rigidbody2D>();
        _rend = GetComponent<Renderer>();
        _grounded = false;
	}
	
	void FixedUpdate () {
        if (GameController.gravTransitionState || !_grounded)
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            PhysicsUtilities.ApplyGravity(_rb);
        }
        else
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _grounded = isGrounded();
        //Debug.Log("Grav Object grounded: " + _grounded);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Transform groundCheck = col.gameObject.transform.GetChild(0);
            Vector2 posOffset = (Vector2)transform.position + PhysicsUtilities.OffsetToEdgeRenderer(_rend);
            // NEED TO FIND OUT HOW TO NOT DESTROY INSTANTLY WHILE IN GRAV TRANSITION
            // FOR SOME REASON THE BELOW DOES NOT WORK
            if (!GameController.gravTransitionState && PhysicsUtilities.IsBelow(groundCheck.position, posOffset))
            {
                Destroy(col.gameObject);
            }
            // to be added: enter gameover state
        }
    }

    // OBJECT SOMETIMES PENETRATES OTHERS
    // find out if the object is grounded
    bool isGrounded()
    {
        // for position, get the "bottom" edge in terms of the direction of gravity
        Vector2 position = (Vector2)transform.position + PhysicsUtilities.OffsetToEdgeRenderer(_rend);
        Debug.DrawLine(transform.position, position);
        float dist = 0.1f;
        RaycastHit2D hit = PhysicsUtilities.RaycastToGravity(position, dist, GameController.terrainLayer);
        if (hit.collider != null)
            return true;

        return false;
    }
}
