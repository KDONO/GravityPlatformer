using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {
    public bool isGlass;
    Glass glass;
    SpriteRenderer rend;

    // check if we're glass
    void Start () {
        if (isGlass)
        {
            glass = new Glass();
            glass.SetSprites(GetType().Name);
            rend = GetComponent<SpriteRenderer>();
            rend.sprite = glass.glassSprite;
            gameObject.layer = LayerMask.NameToLayer("Glass");
        }
	}
	
    // if we are glass, handle glass collisions
	void OnCollisionEnter2D()
    {
        if (isGlass)
            if (glass.CollisionHandler(rend))
                Destroy(gameObject);
    }
}
