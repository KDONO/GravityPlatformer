using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelCrate : MonoBehaviour {
    public bool gravEnabled; // is gravity enabled on the crate
    public bool isGlass;     // is the crate glass?

    Glass _glass;             // object to hold Glass behavior
    Rigidbody2D _rb;
    SpriteRenderer rend;
    bool _grounded;
    bool _isColliding;

	void Start () {
        // set up glass properties if we're a glass object
        if (isGlass)
        {
            _glass = new Glass();
            _glass.SetSprites(GetType().Name);
            rend = GetComponent<SpriteRenderer>();
            rend.sprite = _glass.glassSprite;
            gameObject.layer = LayerMask.NameToLayer("Glass");
        }
        gravEnabled = true;
        _rb = GetComponent<Rigidbody2D>();
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation; // steel crates cannot rotate
        rend = GetComponent<SpriteRenderer>();
        _grounded = false;
	}
	
	void FixedUpdate () {
        _isColliding = false; // reset isColliding

        // if we're moving gravity and not grounded
        if (gravEnabled && (GameController.gravTransitionState || !_grounded))
        {
            _rb.isKinematic = false; // we are now a dynamic rigidbody again
            PhysicsUtilities.ApplyGravity(_rb);
        }
        else
        {
            _rb.isKinematic = true; // set rigidbody to kinematic to prevent physics when not in motion
            _rb.velocity = Vector2.zero; // freeze the object
        }
        _grounded = isGrounded();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // do this check so the collision is only called once - failsafe against unity physics engine
        if (_isColliding)
            return;

        _isColliding = true;

        // only check for collisions with the player is the steel crate's gravity is enabled
        if (gravEnabled && col.gameObject.CompareTag("Player"))
        {
            Transform groundCheck = col.gameObject.transform.GetChild(0);
            Vector2 posOffset = (Vector2)transform.position + PhysicsUtilities.OffsetToEdgeRenderer(rend);
            // NEED TO FIND OUT HOW TO NOT DESTROY INSTANTLY WHILE IN GRAV TRANSITION
            // FOR SOME REASON THE BELOW DOES NOT WORK
            if (!GameController.gravTransitionState && PhysicsUtilities.IsBelow(groundCheck.position, posOffset))
            {
                Destroy(col.gameObject);
            }
            // to be added: enter gameover state
        }

        // handle check for glass
        if (isGlass && GameController.gravTransitionState)
            if (_glass.CollisionHandler(rend))
                Destroy(gameObject);
    }

    // find out if the object is grounded
    bool isGrounded()
    {
        // for position, get the "bottom" edge in terms of the direction of gravity
        Vector2 position = (Vector2)transform.position + PhysicsUtilities.OffsetToEdgeRenderer(rend);
        //Debug.DrawLine(transform.position, position);
        float dist = 0.1f;
        RaycastHit2D hit = PhysicsUtilities.RaycastToGravity(position, dist, GameController.terrainLayer);
        if (hit.collider != null)
            return true;

        return false;
    }
}
