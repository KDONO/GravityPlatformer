using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[RequireComponent(typeof(DrawGizmoGrid))]
public class LevelCreator : MonoBehaviour {
    public DrawGizmoGrid grid;
    [SerializeField] GameObject indicatorPrefab;

    public int boardWidth; // number of units in X direction in world space
    public int boardHeight; // number of units in Z direction in world space
    public Vector2 pos; // used to find specific point in board in case we wish to make modifications to the board
    public Vector3 rot = Vector3.zero; // used to alter rotation values - default to 0,0,0
    public float worldUnits = 1f; // used to alter scale values - default to 1 unity unit
    public int rectW; // width of the rectangle to make things
    public int rectH; // height of the rectangle to make things

    // get an indictor with lazy loading
    private Transform _indicator = null;
    public Transform indicator
    {
        get
        {
            // if we have a tile selection indicator, use that
            if (GameObject.Find(indicatorPrefab.name + "(Clone)") != null)
            {
                GameObject instance = GameObject.Find(indicatorPrefab.name + "(Clone)");
                _indicator = instance.transform;
            }
            // else follow the normal patter of lazy loading
            else if (_indicator == null)
            {
                GameObject instance = Instantiate(indicatorPrefab) as GameObject;
                _indicator = instance.transform;
            }
            return _indicator;
        }
    }

    public Dictionary<Vector2, GameObject> objs = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, string> objFilePaths = new Dictionary<Vector2, string>();

    public LevelData levelData;


    // change color of indicator if we are in our grid space or not
    Color _inBounds = new Color(0f, 1f, .38f, .3f); // light green, alpha'd heavily
    Color _outOfBounds = new Color(1f, .31f, .31f, .3f); // bright red-orange, alpha'd heavily

    #region Public
    // resets everything
    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
        objs.Clear();
        objFilePaths.Clear();
    }

    // add objects of the same type in a rect we want
    public void AddObjects(string path)
    {
        Rect r = CreateRect();
        AddRect(r, path);
    }

    // add objects of the same type in a rect we want
    public void RemoveObjects()
    {
        Rect r = CreateRect();
        RemoveRect(r);
    }

    // add objects and replace anything existing there
    public void AddAndOverwriteObjects(string path)
    {
        Rect r = CreateRect();
        AddAndOverwriteRect(r, path);
    }

    // lets a user save a level using the LevelData class since it's a ScriptableObject
    public void Save(string levNum)
    {
        uint levelNumber;
        bool check = UInt32.TryParse(levNum, out levelNumber);
        if (!check)
        {
            Debug.LogError("Invalid level number");
            return;
        }

        // try to access Resources/Levels in our App
        string filePath = Application.dataPath + "/Resources/Levels";
        if (!Directory.Exists(filePath))
            CreateSaveDirectory();

        // create the data for the board
        LevelData level = ScriptableObject.CreateInstance<LevelData>();
        // instantiate the lists of levelData
        level.positions = new List<Vector2>();
        level.rotationEulers = new List<Vector3>();
        level.scales = new List<Vector3>();
        level.filePaths = new List<string>();
        // save tile parameters in those lists
        foreach (GameObject instance in objs.Values)
        {
            level.positions.Add(instance.transform.position);
            level.rotationEulers.Add(instance.transform.rotation.eulerAngles);
            level.scales.Add(instance.transform.localScale);
        }
        foreach(string s in objFilePaths.Values)
        {
            level.filePaths.Add(s);
        }
        
        // name and create the asset
        string fileName = string.Format("Assets/Resources/Levels/Level_{1}.asset", filePath, levelNumber);
        AssetDatabase.CreateAsset(level, fileName);
    }

    // load a level from levelData
    public void Load()
    {
        Clear();
        if (levelData == null)
        {
            Debug.LogError("No level selected");
            return;
        }

        // if this isn't true, something is wrong, so exit the function
        if (levelData.positions.Count != levelData.filePaths.Count ||
            levelData.positions.Count != levelData.rotationEulers.Count ||
            levelData.positions.Count != levelData.scales.Count)
        {
            Debug.LogError("levelData lists sizes do not match: " + levelData.name);
            return;
        }

        // create our game objects with the appropriate transforms and populate our dictionaries
        for (int i = 0; i < levelData.positions.Count; i++)
        {
            Vector2 p = levelData.positions[i];
            Vector3 r = levelData.rotationEulers[i];
            Vector3 s = levelData.scales[i];
            string path = levelData.filePaths[i];
            GameObject instance = CreateLoadedContent(p, r, s, path);
            objs.Add(instance.transform.position, instance);
            objFilePaths.Add(instance.transform.position, path);
        }

    }

    public void UpdateIndicator()
    {
        // move the indicator to the position we want
        indicator.localPosition = pos;

        if (pos.x < grid.minX || pos.x > grid.maxX || pos.y < grid.minY || pos.y > grid.maxY)
            indicator.gameObject.GetComponent<SpriteRenderer>().color = _outOfBounds;
        else
        {
            indicator.gameObject.GetComponent<SpriteRenderer>().color = _inBounds;
        }
    }
    #endregion

    #region Private
    // creates a rect from a width and height with the origin (top-left corner) at our current pos
    Rect CreateRect()
    {
        return new Rect(pos.x, pos.y, rectW, rectH);
    }

    // runs through a rect to add objects
    void AddRect(Rect r, string objPath)
    {
        for (int y = (int)r.yMin; y < (int)r.yMax; y++)
        {
            for (int x = (int)r.xMin; x < (int)r.xMax; ++x)
            {
                Vector2 v = new Vector2(x, y);
                AddSingle(v, objPath);
            }
        }
    }

    // runs through a rect to remove objects
    void RemoveRect(Rect r)
    {
        for (int y = (int)r.yMin; y < (int)r.yMax; y++)
        {
            for (int x = (int)r.xMin; x < (int)r.xMax; ++x)
            {
                Vector2 v = new Vector2(x, y);
                RemoveSingle(v);
            }
        }
    }

    // runs through a rect to add and overwrite objects
    void AddAndOverwriteRect(Rect r, string path)
    {
        for (int y = (int)r.yMin; y < (int)r.yMax; y++)
        {
            for (int x = (int)r.xMin; x < (int)r.xMax; ++x)
            {
                Vector2 v = new Vector2(x, y);
                AddAndOverwriteSingle(v, path);
            }
        }
    }

    // adds a single object
    void AddSingle(Vector2 v, string path)
    {
        // if something exists there, tell us what is is, then don't add anything
        if (objs.ContainsKey(v))
        {
            Debug.Log(objs[v].name + " exists at " + v);
            return;
        }

        GameObject instance = CreateContent(v, path);
        objs.Add(v, instance);
        objFilePaths.Add(v, path);
    }

    // removes a single object
    void RemoveSingle(Vector2 v)
    {
        if(objs.ContainsKey(v) && objFilePaths.ContainsKey(v))
        {
            DestroyImmediate(objs[v]);
            objs.Remove(v);
            objFilePaths.Remove(v);
        }
    }

    // adds and overwrites a single object
    void AddAndOverwriteSingle(Vector2 v, string path)
    {
        if (objs.ContainsKey(v))
        {
            Debug.Log(objs[v].name + " replaced at " + v);
            RemoveSingle(v);
        }

        GameObject instance = CreateContent(v, path);
        objs.Add(v, instance);
        objFilePaths.Add(v, path);
    }

    // create the gameobject we want while editing
    GameObject CreateContent(Vector2 p, string path)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + path);
        GameObject instance = GameObject.Instantiate(prefab);
        instance.transform.position = p;
        instance.transform.rotation = Quaternion.Euler(rot);
        instance.transform.localScale = scaleToWorldUnits(instance, worldUnits);
        instance.transform.parent = transform;
        return instance;
    }

    // create the gameobject from loading
    GameObject CreateLoadedContent(Vector2 p, Vector3 r, Vector3 s, string path)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + path);
        GameObject instance = GameObject.Instantiate(prefab);
        instance.transform.position = p;
        instance.transform.rotation = Quaternion.Euler(r);
        instance.transform.localScale = s;
        instance.transform.parent = transform;
        return instance;
    }

    // makes a save directory if we do not have one
    void CreateSaveDirectory()
    {
        string filePath = Application.dataPath + "/Resources";
        // if Resources doesn't exist, make the folder
        if (!Directory.Exists(filePath))
            AssetDatabase.CreateFolder("Assets", "Resources");
        filePath += "/Levels";
        // if Levels doesnt exist in Resources, make the folder
        if (!Directory.Exists(filePath))
            AssetDatabase.CreateFolder("Assets/Resources", "Levels");
        // update our changes
        AssetDatabase.Refresh();
    }

    // scale an object to a desired world unit based off of the object's sprite renderer
    // NOTE ONLY WORKS IF OBJECT HAS A SPRITERENDERER
    Vector2 scaleToWorldUnits(GameObject g, float wu)
    {
        Bounds bounds = g.GetComponent<SpriteRenderer>().sprite.bounds;
        if (bounds == null)
            return Vector3.one;
        float xSize = bounds.size.x;
        float ySize = bounds.size.y;

       return new Vector2(wu / xSize, wu / ySize);
    }
    #endregion
}
