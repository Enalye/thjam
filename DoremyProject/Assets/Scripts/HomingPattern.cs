using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator HomingPattern() {
		Vector3 playerPos = Player.instance.obj.Position;
		float angle = Mathf.Atan2(playerPos.y - obj.Position.y, playerPos.x - obj.Position.x) * Mathf.Rad2Deg;
		int n = 8;
		for (int i = 0; i < n; ++i) {
			Bullet shot = pool.AddBullet(bullet_sprite, type, EMaterial.BULLET,
										 obj.Position, 2f, angle);
			yield return new WaitForSeconds(0.05f);
		}

		yield return new WaitForSeconds(0.5f);
	}
}