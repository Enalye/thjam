using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTitle : MonoBehaviour {
	private UnityEngine.UI.RawImage image;
	private Vector3 origin, dest, originRot, destRot;
	private bool hasAppeared = false, isAppearing = false;
	private float startTime;

	void Start () {
		image = gameObject.GetComponent<UnityEngine.UI.RawImage>();
		startTime = Time.time;
	}

	void Update () {
		float t = Time.time - startTime;

		if (hasAppeared)
			return;

		if (isAppearing) {
			t = Mathf.InverseLerp (0f, 3f, t);

			if ((Time.time - startTime) > 3f) {
				hasAppeared = true;
				isAppearing = false;
				startTime = Time.time;
			} else {
				image.color = new Color (1f, 1f, 1f, easeInOut (t));
			}
		} else {
			image.color = new Color (1f, 1f, 1f, 0f);
			if ((Time.time - startTime) > 5f) {
				isAppearing = true;
				startTime = Time.time;
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
