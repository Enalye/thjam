using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bomb : MonoBehaviour {
	public float time;

	public void Fire() {
		StartCoroutine(_Follow());
	}

	public IEnumerator _Follow() {
		float currentTime = 0;
		while(currentTime < time) {
			transform.localPosition = Player.instance.obj.Position;
			currentTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}
	}
}