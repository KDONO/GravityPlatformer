using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    static Directions _dir;
    PlayerController _player;
    public static bool gravTransitionState;
    public static LayerMask terrainLayer;

    public static Directions Dir { get; set; }
    public static PlayerController Player { get; set; }

    void Awake () {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gravTransitionState = true;
        _dir = Directions.South;
        terrainLayer = LayerMask.GetMask("Terrain");
	}
    // add states here - make classes
    // Play, Load, Pause, Victory, Gameover, Menu, 
}
