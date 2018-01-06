using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator HomingPattern() {
		while (obj.Active) {
			Vector3 playerPos = Player.instance.obj.Position;
			float angle = Mathf.Atan2 (playerPos.y - obj.Position.y, playerPos.x - obj.Position.x) * Mathf.Rad2Deg;
			Vector3 pos = obj.Position;

			int n = 8;
			for (int i = 0; i < n; ++i) {
				Bullet shot = pool.AddBullet(GameScheduler.instance.sprites[0], EType.NIGHTMARE, EMaterial.BULLET,
											 pos, 4f, angle);
				shot.Color = Color.magenta;

				shot.SetScaleFromRadius(0.2f);
				yield return new WaitForSeconds (0.05f);
			}

			yield return new WaitForSeconds (1.5f);
		}
	}
}