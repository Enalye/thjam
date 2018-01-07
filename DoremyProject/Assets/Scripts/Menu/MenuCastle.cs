using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCastle : MonoBehaviour {
	private UnityEngine.UI.RawImage image;
	private Vector3 origin, dest, originRot, destRot;
	private bool isMovingUp = true;
	private float startTime;

	void Start () {
		image = gameObject.GetComponent<UnityEngine.UI.RawImage>();
		dest = transform.position + new Vector3(0f, 15f, 0f);
		origin = transform.position - new Vector3(0f, 15f, 0f);
		destRot = transform.rotation.eulerAngles + new Vector3(0f, 0f, 5f);
		originRot = transform.rotation.eulerAngles - new Vector3(0f, 0f, 7f);
		startTime = Time.time;
	}

	void Update () {
		float t = Time.time - startTime;
		t = Mathf.InverseLerp (0f, 8f, t);

		if (isMovingUp) {
			if ((Time.time - startTime) > 8f) {
				isMovingUp = false;
				transform.position = dest;
				transform.rotation = Quaternion.Euler(destRot);
				startTime = Time.time;
			} else {
				transform.position = Vector3.Lerp (origin, dest, easeInOutBack (t));
				transform.rotation = Quaternion.Euler(Vector3.Lerp (originRot, destRot, easeInOut (t)));
			}
		} else {
			if ((Time.time - startTime) > 8f) {
				isMovingUp = true;
				transform.position = origin;
				transform.rotation = Quaternion.Euler(originRot);
				startTime = Time.time;
			} else {
				transform.position = Vector3.Lerp (dest, origin, easeInOutBack (t));
				transform.rotation = Quaternion.Euler(Vector3.Lerp (destRot, originRot, easeInOut (t)));
			}
		}
	}

	private float easeInOut(float t) {
		return (1f - Mathf.Cos(t * Mathf.PI)) / 2f;
	}

	private float easeInOutBack(float t) {
		if(t < .5f) {
			t *= 2f;
			return(t * t * t - t * Mathf.Sin(t * Mathf.PI)) / 2f;
		}
		t = (1f - (2f*t - 1f));
		return (1f - (t * t * t - t * Mathf.Sin(t * Mathf.PI))) / 2f + .5f;
	}
}
