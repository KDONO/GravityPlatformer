using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// add a custom editor for board creation
[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorInspector : Editor {
    string[] _objectFilePathOptions = { "Floor", "Player", "SteelCrate", "VictoryGate", "SpikeBall", "Spikes" };
    int _index = 0;
    string _levelNumber;

    public LevelCreator current
    {
        get
        {
            return (LevelCreator)target;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Label("Grid Params:");
        GUILayout.Label("MinX: " + current.grid.minX);
        GUILayout.Label("MaxX: " + current.grid.maxX);
        GUILayout.Label("MinY: " + current.grid.minY);
        GUILayout.Label("MaxY: " + current.grid.maxY);

        if (GUILayout.Button("Clear"))
            current.Clear();

        _index = EditorGUILayout.Popup(_index, _objectFilePathOptions);
        if (GUILayout.Button("Add Objects"))
            current.AddObjects(_objectFilePathOptions[_index]);
        if (GUILayout.Button("Add and Overwrite Objects"))
            current.AddAndOverwriteObjects(_objectFilePathOptions[_index]);
        if (GUILayout.Button("Remove Objects"))
            current.RemoveObjects();

        _levelNumber = EditorGUILayout.TextField("Level Number: ", _levelNumber);
        if (GUILayout.Button("Save"))
            current.Save(_levelNumber);
        if (GUILayout.Button("Load"))
            current.Load();

        if (GUI.changed)
            current.UpdateIndicator();
    }
}
