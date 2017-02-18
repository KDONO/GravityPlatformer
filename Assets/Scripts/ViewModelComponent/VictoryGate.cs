using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryGate : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Sup");
        if (col.gameObject.CompareTag("Player"))
            SceneManager.LoadScene("Test2");
    }
}
