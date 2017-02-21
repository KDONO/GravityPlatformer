using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : ScriptableObject {
    // transform of the object
    public List<Vector2> positions;
    public List<Vector3> rotationEulers;
    public List<Vector3> scales;
    // file path to the prefab of the object
    public List<string> filePaths;
}
