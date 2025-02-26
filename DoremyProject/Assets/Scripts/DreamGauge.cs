﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamGauge : MonoBehaviour {
	[Range(0, 100)]
	public float level;  // Between 0 and 100%, starts at 50 by default

	private UnityEngine.UI.Image fluid;

	void Start () {
		fluid = gameObject.GetComponent<UnityEngine.UI.Image>();
		UpdateLevel (0);
		StartCoroutine (_Decrease ());
	}

	public void UpdateLevel(float levelChange) {
		level = Mathf.Clamp(level + levelChange, 0, 100);
		fluid.rectTransform.sizeDelta = new Vector3(fluid.rectTransform.sizeDelta.x,
												    (level / 100) * 395);

		if (level == 0 && !Player.instance.debug_invincible) {
			float fadeTime = GameObject.Find("Fading").GetComponent<Fading>().BeginFade (1);
			StartCoroutine (LoadAfter(fadeTime));
		}
	}		

	public IEnumerator _Decrease() {
		while (Application.isPlaying) {
			yield return new WaitForSeconds (0.1f);

			if (!GameScheduler.instance.dialogue.in_dialogue) {
				float decreaseLevel = -0.15f;
				UpdateLevel(decreaseLevel);
			}
		}
	}

	private IEnumerator LoadAfter(float time) {
		yield return new WaitForSeconds (time);
		SceneManager.LoadScene(2);
	}
}
