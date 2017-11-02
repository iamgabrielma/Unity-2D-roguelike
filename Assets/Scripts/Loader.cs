using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script will check if GameManager has been instantiated, and if is not, do it from the Prefabs
public class Loader : MonoBehaviour {

	public GameObject gameManager;
	// Use this for initialization
	void Awake () {
		if (GameManager.instance == null)
			Instantiate (gameManager);
			Debug.Log("Hello World!");
	}
}
