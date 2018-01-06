using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour {
	private UnityEngine.UI.Text text;

	void Start () {
		text = gameObject.GetComponent<UnityEngine.UI.Text>();
	}

	void Update () {
		text.color = new Color (1f, 1f, 1f, Mathf.Lerp(.35f, 1f, (Mathf.Cos (Time.time * 2f) + 1f) / 2f));

		if(Input.GetButton("Shot1")) {
			Application.LoadLevel(1);
		}
	}
}
