using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour {
	public SpriteRenderer left_doll;
	public SpriteRenderer left_bubble;
	public SpriteRenderer left_text;

	public List<Sprite> text;

	public SpriteRenderer right_doll;
	public SpriteRenderer right_bubble;
	public SpriteRenderer right_text;

	private int currentDialogue;
	private int textID;

	public IEnumerator StartDialogue() {
		StartCoroutine(_Appear(1.0f, left_doll));
		StartCoroutine(_Appear(1.0f, left_bubble));
		yield return StartCoroutine(_Appear(1.0f, left_text));
		left_text.sprite = text[0];
		currentDialogue = 0;
		textID = 0;

		while (textID < text.Count) {
			if (Input.GetButton("Shot1")) {
				textID++;
				if (textID < text.Count) {
					left_text.sprite = text [textID];
				}
			}
			yield return new WaitForSeconds (0.2f);
		}

		StartCoroutine (_Disappear (1.0f, left_doll));
		StartCoroutine (_Disappear (1.0f, left_bubble));
		yield return StartCoroutine (_Disappear (1.0f, left_text));
	}
		
	public IEnumerator _Appear(float time, SpriteRenderer target) {
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

	public IEnumerator _Disappear(float time, SpriteRenderer target) {
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
