using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    const float speed = 0.1f;
    const float jumpForce = 5f;
    //const float gravity = 9.81f;

    [HideInInspector] public bool grounded;

    Transform _groundChecker;
    Rigidbody2D _rb;
    Renderer _rend;
    bool _idleOverride;
    float _idleCheck;
    Vector3 _lastPos;

    void Start () {
        _groundChecker = transform.GetChild(0);
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;
        _rend = GetComponent<Renderer>();
        _idleOverride = false;
    }
	
	void FixedUpdate () {
        PhysicsUtilities.ApplyGravity(_rb);
        // only check for Grounded if we didn't override it with the idle check
        if(!_idleOverride)
            grounded = isGrounded();
        // Do the following to resolve any times colliders hit, and the raycasts fail
        // if we do so, override the normal grounded check
        Vector3 currentPos = transform.position;
        _idleCheck += Time.deltaTime;
        if((!grounded || GameController.gravTransitionState) && _idleCheck >= .25f && currentPos == _lastPos)
        {
            //Debug.Log("In idle check");
            
            grounded = true;
            GameController.gravTransitionState = false;
            _idleCheck = 0;
            _idleOverride = true;
        }
        _lastPos = currentPos;

        //Debug.Log("grounded: " + grounded);
        //Debug.Log("gravTrans: " + GameController.gravTransitionState);

        HandleInput();
	}

    void HandleInput()
    {
        // any valid input will reset _idleOverride
        
        // you can only change gravity if you are on the ground and not in ground transition
        // use the arrow keys
        if (grounded && !GameController.gravTransitionState)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && GameController.Dir != Directions.North)
            {
                GameController.Dir = Directions.North;
                GameController.gravTransitionState = true;
                StartCoroutine(Turn(GameController.Dir));
                _idleOverride = false;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && GameController.Dir != Directions.East)
            {
                GameController.Dir = Directions.East;
                GameController.gravTransitionState = true;
                StartCoroutine(Turn(GameController.Dir));
                _idleOverride = false;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && GameController.Dir != Directions.South)
            {
                GameController.Dir = Directions.South;
                GameController.gravTransitionState = true;
                StartCoroutine(Turn(GameController.Dir));
                _idleOverride = false;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && GameController.Dir != Directions.West)
            {
                GameController.Dir = Directions.West;
                GameController.gravTransitionState = true;
                StartCoroutine(Turn(GameController.Dir));
                _idleOverride = false;
            }
        }

        /*
        // you can only jump if you're grounded and not in GameController.gravTransitionState
        // do this check again in case we changed in for gravity changing input
        if (_grounded && !_GameController.gravTransitionState)
        {
            // jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _rb.velocity = -GameController.Dir.ToVector2() * jumpForce;
            }
        }
        */

        // you can move if you are not in grav transition
        // use WASD
        // maps DA if grav is vert, maps WS if grav is horz
        if (!GameController.gravTransitionState)
        {
            switch (GameController.Dir)
            {
                case Directions.North:
                case Directions.South:
                    if (Input.GetKey(KeyCode.D))
                    {
                        transform.position += new Vector3(speed, 0, 0);
                        _idleOverride = false;
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        transform.position -= new Vector3(speed, 0, 0);
                        _idleOverride = false;
                    }
                    break;
                case Directions.East:
                case Directions.West:
                    if (Input.GetKey(KeyCode.W))
                    {
                        transform.position += new Vector3(0, speed, 0);
                        _idleOverride = false;
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        transform.position -= new Vector3(0, speed, 0);
                        _idleOverride = false;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    // turn so that you are standing correctly
    IEnumerator Turn(Directions dir)
    {
        TransformLocalEulerTweener t = (TransformLocalEulerTweener)transform.RotateToLocal(dir.ToEuler(), 0.25f, EasingEquations.EaseInOutQuad);

        float roundedStart = Mathf.Round(t.startValue.z);
        float roundedEnd = Mathf.Round(t.endValue.z);

        // When rotating between South and West, we must make an exception so it looks like the unit
        // rotates the most efficient way (since 0 and 360 are treated the same)
        if (roundedStart == 0 && roundedEnd == 270)
        {
            t.startValue = new Vector3(t.startValue.x, t.startValue.y, 360f);
        }
        else if (roundedStart == 270 && roundedEnd == 0)
        {
            t.endValue = new Vector3(t.startValue.x, t.startValue.y, 360f);
        }

        while (t != null)
        {
            GameController.gravTransitionState = true; // make sure _GameController.gravTransitionState remains true while we're animating
            grounded = false;
            yield return null;
        }
    }

    // check if we're grounded
    bool isGrounded()
    {
        Vector2 position = _groundChecker.position; // get our groundChecker object
        float dist = 0.1f;                           // how far are we checking
        // check both ends and the center of the game object
        RaycastHit2D[] hits = new RaycastHit2D[] {
            PhysicsUtilities.RaycastToGravity(position - new Vector2(_rend.bounds.size.x/2f, 0), dist, GameController.terrainLayer),
            PhysicsUtilities.RaycastToGravity(position, dist, GameController.terrainLayer),
            PhysicsUtilities.RaycastToGravity(position + new Vector2(_rend.bounds.size.x/2f, 0), dist, GameController.terrainLayer)
        };

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                GameController.gravTransitionState = false; // if we hit ground, we're not in GameController.gravTransitionState
                return true;
            }
        }

        return false;
    }
}
