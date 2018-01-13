using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMainMenu : MonoBehaviour {
	void Update () {
		if (Input.anyKey) {
			float fadeTime = GameObject.Find("Fading").GetComponent<Fading>().BeginFade (1);
			StartCoroutine (LoadAfter(fadeTime));
		}
	}

	private IEnumerator LoadAfter(float time) {
		yield return new WaitForSeconds (time);
		SceneManager.LoadScene (0);
	}
}
