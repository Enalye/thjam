using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : Enemy {
	public GameObject lifeCircle;
	private Material lifeCircleMaterial;

	public override void Init() {
		base.Init();
		lifeCircleMaterial = lifeCircle.GetComponent<UnityEngine.UI.Image>().material;
		UpdateLevel(1.0f);
	}

	public override void UpdateAt(float dt) {
		base.UpdateAt(dt);
		lifeCircle.transform.localPosition = new Vector3(obj.Position.x, obj.Position.y, Layering.BossLifebar);
	}

	public override void TakeDamage(float damage) {
		base.TakeDamage(damage);
		UpdateLevel(life / base_life);
	}

	private void UpdateLevel(float cutoff) {
		lifeCircleMaterial.SetFloat("_Cutoff", cutoff);
	}
}
