using UnityEngine;
using System.Collections;

// DrawGizmoGrid.cs
// draws a useful reference grid in the editor in Unity. 
// 09/01/15 - Hayden Scott-Baron
// twitter.com/docky 
// no attribution needed, but please tell me if you like it ^_^

// Modified 2/17/17 by Steven Shing

    [RequireComponent(typeof(LevelCreator))]
public class DrawGizmoGrid : MonoBehaviour
{
    public LevelCreator lc;
    
    // universal grid scale
    public float gridScale = 1f;

    // extents of the grid
    [HideInInspector] public int minX = -15;
    [HideInInspector] public int minY = -15;
    [HideInInspector] public int maxX = 15;
    [HideInInspector] public int maxY = 15;

    // nudges the whole grid rel
    public Vector3 gridOffset = Vector3.zero;

    // is this an XY or an XZ grid?
    public bool topDownGrid = true;

    // choose a colour for the gizmos
    public int gizmoMajorLines = 5;
    public Color gizmoLineColor = new Color(0.4f, 0.4f, 0.3f, 1f);

    // rename + centre the gameobject upon first time dragging the script into the editor. 
    void Reset()
    {
        if (name == "GameObject")
            name = "~~ GIZMO GRID ~~";

        transform.position = Vector3.zero;
    }

    // draw the grid :) 
    void OnDrawGizmos()
    {
        lc = GetComponent<LevelCreator>();
        if(lc.boardWidth % 2 == 0)
        {
            minX = -lc.boardWidth / 2;
            maxX = lc.boardWidth / 2;
        }
        else
        {
            int widthMinusOne = lc.boardWidth - 1;
            minX = -widthMinusOne / 2;
            maxX = widthMinusOne / 2 + 1;
        }

        if (lc.boardHeight % 2 == 0)
        {
            minY = -lc.boardHeight / 2;
            maxY = lc.boardHeight / 2;
        }
        else
        {
            int heightMinusOne = lc.boardHeight - 1;
            minY = -heightMinusOne / 2;
            maxY = heightMinusOne / 2 + 1;
        }

        // orient to the gameobject, so you can rotate the grid independently if desired
        Gizmos.matrix = transform.localToWorldMatrix;

        // set colours
        Color dimColor = new Color(gizmoLineColor.r, gizmoLineColor.g, gizmoLineColor.b, 0.25f * gizmoLineColor.a);
        Color brightColor = Color.Lerp(Color.white, gizmoLineColor, 0.75f);

        // draw the horizontal lines
        for (int x = minX; x < maxX + 1; x++)
        {
            // find major lines
            Gizmos.color = (x % gizmoMajorLines == 0 ? gizmoLineColor : dimColor);
            if (x == 0)
                Gizmos.color = brightColor;

            Vector3 pos1 = new Vector3(x, minY, 0) * gridScale;
            Vector3 pos2 = new Vector3(x, maxY, 0) * gridScale;

            // convert to topdown/overhead units if necessary
            if (topDownGrid)
            {
                pos1 = new Vector3(pos1.x, 0, pos1.y);
                pos2 = new Vector3(pos2.x, 0, pos2.y);
            }

            Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
        }

        // draw the vertical lines
        for (int y = minY; y < maxY + 1; y++)
        {
            // find major lines
            Gizmos.color = (y % gizmoMajorLines == 0 ? gizmoLineColor : dimColor);
            if (y == 0)
                Gizmos.color = brightColor;

            Vector3 pos1 = new Vector3(minX, y, 0) * gridScale;
            Vector3 pos2 = new Vector3(maxX, y, 0) * gridScale;

            // convert to topdown/overhead units if necessary
            if (topDownGrid)
            {
                pos1 = new Vector3(pos1.x, 0, pos1.y);
                pos2 = new Vector3(pos2.x, 0, pos2.y);
            }

            Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
        }
    }
}