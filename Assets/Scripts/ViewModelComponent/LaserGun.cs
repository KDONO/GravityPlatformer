using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : Attachable {
    const float lifetime = 1.5f; // time in seconds the laser lasts
    const float laserSizeX = .5f; // size of the laser in the X scale

    public bool firing; // are we firing

    [SerializeField] GameObject laserBeamPrefab; // holds the prefab of the laser beam
    [SerializeField] LayerMask layerMask; // holds the bit mask of the ActiveAttachable layer

    GameObject _laserChild; // the laser beam object we'll make
    Directions _facing; // what direciton is the gun facing
    Directions _oppositeFacing; // the opposite of facing
    int _zRot; // the zrotation of the gun

    void Awake()
    {
        _zRot = (int)transform.rotation.eulerAngles.z; // set z rotation

        // set facing and opposite facing depending on the z rotation
        switch (_zRot)
        {
            case 0:
                _facing = Directions.North;
                _oppositeFacing = Directions.South;
                break;
            case 90:
                _facing = Directions.West;
                _oppositeFacing = Directions.East;
                break;
            case 180:
                _facing = Directions.South;
                _oppositeFacing = Directions.North;
                break;
            case 270:
                _facing = Directions.East;
                _oppositeFacing = Directions.West;
                break;
            default:
                break;
        }

        // make the laser child
        _laserChild = CreateLaser();
    }

    void Start()
    {
        // start off firing
        firing = true;
        // start our counting coroutine
        StartCoroutine("Tick");
    }

    void FixedUpdate()
    {
        // if the laser is firing, the gun is now a laser and enable the laser beam
        if (firing)
        {
            gameObject.layer = LayerMask.NameToLayer("Laser");
            AdjustLaserDistance(); // adjust the laser
            _laserChild.SetActive(true);
        }
        // otherwise, it is InactiveAttachable and disable the laser beam
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Inactive");
            _laserChild.SetActive(false);
        }
    }

    // if the player collids with the laser, they die
    // no need to check if the laser is firing due to the layer mask changing in FixedUpdate
    public override void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // To Death State
            Destroy(col.gameObject);
        }
    }

    // coroutine counter
    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(lifetime); // wait, in realtime seconds, for lifetime seconds
            firing = !firing; // toggle firing
        }
    }

    // create the laser beam object
    GameObject CreateLaser()
    {
        GameObject laserInstance = Instantiate(laserBeamPrefab) as GameObject;
        laserInstance.transform.parent = transform; // child it to the laser gun
        laserInstance.transform.position = transform.position; // set it's position to the laser gun
        laserInstance.transform.rotation = transform.rotation; // set it's rotation to be the same as the laser gun

        return laserInstance;
    }
    
    // adjust the size of the laser depending on objects in the way
    void AdjustLaserDistance()
    {
        // escape if we don't have a laser child
        if (_laserChild == null)
            return;

        // check for any object that's not an ActiveAttachable 
        RaycastHit2D hit = Physics2D.Raycast(_laserChild.transform.position, _facing.ToVector2(), Mathf.Infinity, ~layerMask);

        if(hit.collider != null)
        {
            //Debug.Log(hit.collider.gameObject.name);
            //Debug.Log(hit.collider.bounds);
            //Debug.Log(transform.position.x);
            // calculate where the laser will stop based off the offset edge of the object we hit
            Vector2 endpoint = (Vector2)hit.collider.gameObject.transform.position + PhysicsUtilities.OffsetToEdgeCollider2D(hit.collider, _oppositeFacing);
            // distance between the gun beginning of the laser and the endpoint
            float dist = Mathf.Sqrt(Mathf.Pow(_laserChild.transform.position.x - endpoint.x, 2) + Mathf.Pow(_laserChild.transform.position.y - endpoint.y, 2));
            // set localScale of the laser - always in y because of how parent-child works on transforms in Unity
            _laserChild.transform.localScale = new Vector3(laserSizeX, dist, 0);
        }
    }
}
