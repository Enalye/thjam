using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMainMenu : MonoBehaviour {
	void Start () {
		
	}

	void Update () {
		if (Input.GetButton ("Shot1") || Input.GetButton ("Shot2") || Input.GetButton ("Shot3")) {
			Application.LoadLevel (0);
		}
	}
}
