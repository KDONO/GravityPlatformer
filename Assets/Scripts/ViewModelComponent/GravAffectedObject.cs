using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravAffectedObject : MonoBehaviour {
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
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Transform groundCheck = col.gameObject.transform.GetChild(0);
            if (GameController.gravTransitionState && !PhysicsUtilities.IsBelow(gameObject.transform, groundCheck))
            {
                Destroy(col.gameObject);
            }
            // to be added: enter gameover state
        }
    }

    // find out if the object is grounded
    bool isGrounded()
    {
        // for position, get the "bottom" edge in terms of the direction of gravity
        Vector2 position = (Vector2)transform.position + PhysicsUtilities.RendererOffsetForRaycasts(_rend);
        float dist = 0.1f;
        RaycastHit2D hit = PhysicsUtilities.RaycastToGravity(position, dist);
        if (hit.collider != null)
            return true;

        return false;
    }
}
