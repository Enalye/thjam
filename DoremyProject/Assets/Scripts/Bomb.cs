using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bomb : MonoBehaviour {
	public float time;
	public float radius;
	public bool active;

	private float maxRadius;

	public void Fire() {
		radius = 0;
		maxRadius = GetComponent<RectTransform>().rect.width;
		StartCoroutine(_Bomb());
	}

	public IEnumerator _Bomb() {
		active = true;
		float currentTime = 0;
		float partTime = time;

		while(currentTime < partTime) {
			transform.localPosition = Player.instance.obj.Position;
			transform.localScale = Vector3.one * (currentTime / partTime);
			radius = maxRadius * (currentTime / partTime);
			currentTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}

		radius = maxRadius;
		currentTime = 0;
		while(!Input.GetButton("Bomb")) {
			transform.localPosition = Player.instance.obj.Position;
			currentTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}

		currentTime = 0;
		while(currentTime < partTime) {
			transform.localPosition = Player.instance.obj.Position;
			transform.localScale = Vector3.one * (1 - (currentTime / partTime));
			radius = maxRadius * (1 - (currentTime / partTime));
			currentTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}

		active = false;
		radius = 0.0f;
		transform.localScale = Vector3.zero;
	}
}