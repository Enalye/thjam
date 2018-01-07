using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCutIn : MonoBehaviour {
	public float offset = 0f;

	private UnityEngine.UI.RawImage image;
	private Vector3 origin, dest;
	private bool isCuttingIn = true;
	private float startTime;

	void Start () {
		image = gameObject.GetComponent<UnityEngine.UI.RawImage>();
		dest = transform.position;
		origin = dest + new Vector3(offset, 0f, 0f);
		startTime = Time.time;
	}

	void Update () {
		if (isCuttingIn) {
			float t = Time.time - startTime;
			t = Mathf.InverseLerp (0f, 2f, t);

			if ((Time.time - startTime) > 2f) {
				isCuttingIn = false;
				transform.position = dest;
			} else {
				image.color = new Color (1f, 1f, 1f, easeInOut (t));
				transform.position = Vector3.Lerp (origin, dest, easeOut(t));
			}
		}
	}

	private float easeInOut(float t) {
		return (1f - Mathf.Cos(t * Mathf.PI)) / 2f;
	}

	private float easeOut(float t) {
		t = (t - 1f);
		t = (t * t * t + 1f);
		return t;
	}
}
