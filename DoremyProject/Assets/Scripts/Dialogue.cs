using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour {
	public Image left_doll;
	public Image left_bubble;
	public Image left_text;

	public List<Sprite> text;

	public Image right_doll;
	public Image right_bubble;
	public Image right_text;

	public bool in_dialogue;

	private int currentDialogue;
	private int textID;

	public IEnumerator StartDialogue() {
		in_dialogue = true;
		StartCoroutine(_Appear(1.0f, left_doll));
		StartCoroutine(_Appear(1.0f, left_bubble));
		yield return StartCoroutine(_Appear(1.0f, left_text));

		left_text.sprite = text[0];
		currentDialogue = 0;
		textID = 0;

		while (textID < text.Count) {
			if (Input.GetButtonDown("Shot1")) {
				textID++;
				if (textID < text.Count) {
					left_text.sprite = text[textID];
				}
			}
			yield return null;
		}

		StartCoroutine (_Disappear (1.0f, left_doll));
		StartCoroutine (_Disappear (1.0f, left_bubble));
		yield return StartCoroutine (_Disappear (1.0f, left_text));
		in_dialogue = false;
	}
		
	public IEnumerator _Appear(float time, Image target) {
		target.color = Colors.ChangeAlpha(target.color, 0);

		float elapsedTime = 0;
		while (elapsedTime < time) {
			float timeRatio = elapsedTime / time;

			byte alpha = (byte)Mathf.Min(255, 255 * timeRatio);
			target.color = Colors.ChangeAlpha(target.color, alpha);
			elapsedTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}
	}

	public IEnumerator _Disappear(float time, Image target) {
		float oriAlpha = target.color.a * 255;

		float elapsedTime = 0;
		while (elapsedTime < time) {
			float timeRatio = elapsedTime / time;

			byte alpha = (byte)Mathf.Max(0, oriAlpha * (1 - timeRatio));
			target.color = Colors.ChangeAlpha(target.color, alpha);
			elapsedTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}

		target.color = Colors.ChangeAlpha(target.color, 0);
	}
}
