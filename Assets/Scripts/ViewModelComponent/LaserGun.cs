using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : Attachable {
    const float lifetime = 1.5f; // time in seconds the laser lasts
    const float laserSizeX = .5f; // size of the laser in the X scale

    public bool firing; // are we firing
    public bool continuous; // is this a continuous laser

    [SerializeField] GameObject laserBeamPrefab; // holds the prefab of the laser beam
    [SerializeField] LayerMask layerMask; // holds the bit mask of the ActiveAttachable layer

    GameObject _laserChild; // the laser beam object we'll make

    // call awake because the laser has to exist before anything else happens
    void Awake()
    {
        SetFacingsAndZRot();
        AttachToTile();

        // make the laser child
        _laserChild = CreateLaser();
    }

    public override void Start()
    {
        // DO NOT CALL BASE START HERE BECAUSE WE USE AWAKE TO SET UP THE DATA
        // start off firing
        firing = true;
        // start our counting coroutine
        if(!continuous)
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
    void OnCollisionEnter2D(Collision2D col)
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
        RaycastHit2D hit = Physics2D.Raycast(_laserChild.transform.position, facing.ToVector2(), Mathf.Infinity, ~layerMask);

        if(hit.collider != null)
        {
            // calculate where the laser will stop based off the offset edge of the object we hit
            Vector2 endpoint = (Vector2)hit.collider.gameObject.transform.position + PhysicsUtilities.OffsetToEdgeCollider2D(hit.collider, oppositeFacing);
            // distance between the gun beginning of the laser and the endpoint
            float dist = Mathf.Sqrt(Mathf.Pow(_laserChild.transform.position.x - endpoint.x, 2) + Mathf.Pow(_laserChild.transform.position.y - endpoint.y, 2));
            // set localScale of the laser - always in y because of how parent-child works on transforms in Unity
            _laserChild.transform.localScale = new Vector3(laserSizeX, dist, 0);
        }
    }
}
