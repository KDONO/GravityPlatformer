using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class denotes the behavior of glass
public class Glass {
    public int threshold = 2; // how many collisions does it take to be destroyed - default to 2, but can be changed

    public Sprite glassSprite; // the non-cracked sprite
    public Sprite crackedSprite; // the cracked glass sprite

    int count = 0;
    
    // set the appropriate sprites for glass objects based on name
    // should this be in each individual class instead of here???
    public void SetSprites(string className)
    {
        if (className.Equals("SteelCrate"))
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Glass_Spritesheet");
            glassSprite = sprites[0];
            crackedSprite = sprites[1];
        }
        else if (className.Equals("Floor"))
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Glass_Spritesheet");
            glassSprite = sprites[1];
            crackedSprite = sprites[0];
        }
    }

    // this resolves what happens with a collision
    // returns true if it should be destroyed
    public bool CollisionHandler(SpriteRenderer rend)
    {
        count++;
        if (count == threshold / 2)
        {
            rend.sprite = crackedSprite;
            return false;
        }
        if (count == threshold)
            return true;
        return false;
    }
}
