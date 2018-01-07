using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour {
	private UnityEngine.UI.RawImage image;
	private Vector3 origin, dest;
	private float startTime;
	private bool isStarting = true;

	void Start () {
		image = gameObject.GetComponent<UnityEngine.UI.RawImage>();
		dest = transform.position;
		origin = dest + new Vector3(0f, 800f, 0f);
		startTime = Time.time;
	}

	void Update () {
		if (isStarting) {
			float t = Time.time - startTime;
			t = Mathf.InverseLerp (0f, 5f, t);

			if ((Time.time - startTime) > 5f) {
				isStarting = false;
				transform.position = dest;
			} else {
				image.color = new Color (1f, 1f, 1f, easeInOut (t));
				transform.position = Vector3.Lerp (origin, dest, easeOutBounce(t));
			}
		}
		image.color = new Color (1f, 1f, 1f, Mathf.Lerp (.35f, 1f, (Mathf.Cos (Time.time * 2f) + 1f) / 2f));
		if (Input.GetButton ("Shot1") || Input.GetButton ("Shot2") || Input.GetButton ("Shot3")) {
			Application.LoadLevel (1);
		}
	}

	private float easeInOut(float t) {
		return (1f - Mathf.Cos(t * Mathf.PI)) / 2f;
	}

	private float easeOutBounce(float p) {
		if(p < 4f/11f)
			return (121f * p * p)/16f;
		else if(p < 8f/11f)
			return (363f/40f * p * p) - (99f/10f * p) + 17f/5f;
		else if(p < 9f/10f)
			return (4356f/361f * p * p) - (35442f/1805f * p) + 16061f/1805f;
		return (54f/5f * p * p) - (513f/25f * p) + 268f/25f;
	}
}
