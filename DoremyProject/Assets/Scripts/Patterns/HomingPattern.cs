using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator HomingPattern() {
		yield return new WaitForSeconds (0.5f);
		while (obj.Active) {
			yield return new WaitForSeconds (1.0f);

			Vector3 playerPos = Player.instance.obj.Position;
			float angle = Mathf.Atan2 (playerPos.y - obj.Position.y, playerPos.x - obj.Position.x) * Mathf.Rad2Deg;
			Vector3 pos = obj.Position;

			int n = 8;
			for (int i = 0; i < n; ++i) {
				Bullet shot = pool.AddBullet(GameScheduler.instance.sprites[2], EType.NIGHTMARE, EMaterial.BULLET,
										     Colors.firebrick,
											 pos, 200f, angle);
				shot.Radius = 10f;
				shot.Scale = Vector3.one * 1.5f;
				shot.SpriteAngle = Vector3.forward * angle;

				Bullet shot1 = pool.AddBullet(GameScheduler.instance.sprites[2], EType.NIGHTMARE, EMaterial.BULLET,
					Colors.firebrick,
					pos, 200f, angle - 16, -1f);
				shot1.Radius = 10f;
				shot1.MinSpeed = 150f;
				shot1.Scale = Vector3.one * 1.5f;
				shot1.SpriteAngle = Vector3.forward * (angle - 16);

				Bullet shot2 = pool.AddBullet(GameScheduler.instance.sprites[2], EType.NIGHTMARE, EMaterial.BULLET,
					Colors.firebrick,
					pos, 200f, angle + 16, -1f);
				shot2.Radius = 10f;
				shot2.MinSpeed = 150f;
				shot2.Scale = Vector3.one * 1.5f;
				shot2.SpriteAngle = Vector3.forward * (angle + 16);

				yield return new WaitForSeconds (0.1f);
			}
		}
	}
}