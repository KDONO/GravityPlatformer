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
    public int rectW; // width of the rectangle to make things
    public int rectH; // height of the rectangle to make things

    // get an indictor with lazy loading
    private Transform _indicator = null;
    public Transform indicator
    {
        get
        {
            // if we have a tile selection indicator, use that
            if (GameObject.Find(indicatorPrefab.name) != null)
            {
                Debug.Log("Ever true?");
                GameObject instance = GameObject.Find(indicatorPrefab.name);
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


    // change color if we are in our grid space or not
    Color _inBounds = new Color(0f, 1f, .38f, .3f); // light green, alpha'd heavily
    Color _outOfBounds = new Color(1f, 79, .31f, .3f); // bright red-orange, alpha'd heavily

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
    public void AddObjects(string objPath)
    {
        Rect r = CreateRect();
        AddRect(r, objPath);
    }

    // add objects of the same type in a rect we want
    public void RemoveObjects()
    {
        Rect r = CreateRect();
        RemoveRect(r);
    }

    // add objects and replace anything existing there
    public void AddAndOverwriteObjects(string objPath)
    {
        Rect r = CreateRect();
        AddAndOverwriteRect(r, objPath);
    }

    // lets a user save a level using the LevelData class since it's a ScriptableObject
    public void Save(string levNum)
    {
        uint levelNumber;
        bool check = UInt32.TryParse(levNum, out levelNumber);
        if (!check)
        {
            Debug.Log("Invalid level number");
            return;
        }

        // try to access Resources/Levels in our App
        string filePath = Application.dataPath + "/Resources/Levels";
        if (!Directory.Exists(filePath))
            CreateSaveDirectory();

        // create the data for the board
        LevelData level = ScriptableObject.CreateInstance<LevelData>();
        // instantiate the lists of levelData
        level.objs = new List<Vector2>();
        level.objFilePaths = new List<string>();
        // save tile parameters in those lists
        foreach (GameObject instance in objs.Values)
        {
            level.objs.Add(instance.transform.position);
        }
        foreach(string s in objFilePaths.Values)
        {
            level.objFilePaths.Add(s);
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
        if (levelData.objs.Count != levelData.objFilePaths.Count)
        {
            Debug.LogError("levelData objs count and objFilePaths count do not match for: " + levelData.name);
            return;
        }

        // populate our tile dictionary with data from levelData
        for (int i = 0; i < levelData.objs.Count; i++)
        {
            Vector2 v = levelData.objs[i];
            string s = levelData.objFilePaths[i];
            GameObject instance = CreateContent(v, s);
            objs.Add(instance.transform.position, instance);
            objFilePaths.Add(instance.transform.position, s);
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
    void AddAndOverwriteRect(Rect r, string objPath)
    {
        for (int y = (int)r.yMin; y < (int)r.yMax; y++)
        {
            for (int x = (int)r.xMin; x < (int)r.xMax; ++x)
            {
                Vector2 v = new Vector2(x, y);
                AddAndOverwriteSingle(v, objPath);
            }
        }
    }

    // adds a single object
    void AddSingle(Vector2 v, string objPath)
    {
        // if something exists there, tell us what is is, then don't add anything
        if (objs.ContainsKey(v))
        {
            Debug.Log(objs[v].name + " exists at " + v);
            return;
        }

        GameObject instance = CreateContent(v, objPath);
        objs.Add(v, instance);
        objFilePaths.Add(v, objPath);
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
    void AddAndOverwriteSingle(Vector2 v, string objPath)
    {
        if (objs.ContainsKey(v))
        {
            Debug.Log(objs[v].name + " replaced at " + v);
            RemoveSingle(v);
        }

        GameObject instance = CreateContent(v, objPath);
        objs.Add(v, instance);
        objFilePaths.Add(v, objPath);
    }

    // create the gameobject we want
    GameObject CreateContent(Vector2 v, string objPath)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + objPath);
        GameObject instance = GameObject.Instantiate(prefab);
        instance.transform.position = v;
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
    #endregion
}
