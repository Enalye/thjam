using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour {
	public GameObject lookObject;

	void Update () {
		transform.LookAt (lookObject.transform);
	}
}
