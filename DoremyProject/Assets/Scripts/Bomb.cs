using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bomb : Entity {
	public float shiftTime;
	public AudioClip specialReverse;

	[System.NonSerialized]
	public bool active;

	[System.NonSerialized]
	public bool shifting;

	private float maxRadius;

	public override void Init() {
		base.Init();
		active = false;
		shifting = false;
	}

	public void Fire() {
		radius = 0;
		maxRadius = GetComponent<RectTransform>().rect.width * 0.5f * 0.8f;
		StartCoroutine(_Bomb());
		StartCoroutine(_BindObject());
	}

	public IEnumerator _Bomb() {
		active = true;

		float currentTime = 0;
		shifting = true;

		while(currentTime < shiftTime) {
			transform.localPosition = Player.instance.obj.Position;
			transform.localScale = Vector3.one * (currentTime / shiftTime);
			radius = maxRadius * (currentTime / shiftTime);
			currentTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}

		shifting = false;
		radius = maxRadius;
		currentTime = 0;
		while(!Input.GetButton("Bomb")) {
			transform.localPosition = Player.instance.obj.Position;
			currentTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}

		GameScheduler.instance.audioManager.PlayEffect(specialReverse);
		currentTime = 0;

		while(currentTime < shiftTime) {
			transform.localPosition = Player.instance.obj.Position;
			transform.localScale = Vector3.one * (1 - (currentTime / shiftTime));
			radius = maxRadius * (1 - (currentTime / shiftTime));
			currentTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}
			
		active = false;
		radius = 0.0f;
		transform.localScale = Vector3.zero;
	}
}