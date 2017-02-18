using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteScaleToWorldUnits : MonoBehaviour {
    public float worldUnits = 1f;

    void Awake()
    {
        Bounds bounds = GetComponent<SpriteRenderer>().sprite.bounds;
        float xSize = bounds.size.x;
        float ySize = bounds.size.y;

        transform.localScale = new Vector2(worldUnits / xSize, worldUnits / ySize);
    }
}
