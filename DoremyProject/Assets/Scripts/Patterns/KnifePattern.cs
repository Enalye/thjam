using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator KnifePattern(int n, float radius) {
		for (int i = 0; i < n; ++i) {
			Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[1], EType.NIGHTMARE, EMaterial.BULLET, Colors.yellow);
			bullets.Add (shot);
		}

		float count = 0;
		float ang = 0;
		while (obj.Active) {
			float angle = 90;
			for (int i = 0; i < n; ++i) {
				float x = obj.Position.x + radius * Mathf.Cos (Mathf.Deg2Rad * angle);
				float y = obj.Position.y + radius * Mathf.Sin (Mathf.Deg2Rad * angle);

				bullets [i].Position = new Vector3 (x, y);
				bullets [i].Speed = 0;
				bullets [i].Angle = angle;
				bullets [i].SpriteAngle = Vector3.forward * angle;
				bullets [i].Radius = 5f;

				angle += 360 / n;
			}

			if (count % 10 == 0) { 
				Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[2], EType.NIGHTMARE, EMaterial.BULLET, Colors.yellow,
											  obj.Position, 150f, ang);
				shot.SpriteAngle = Vector3.forward * ang;
				shot.Radius = 5f;

				ang += 25;
			}

			yield return new WaitForSeconds (GameScheduler.dt);
			count++;
		}

		for (int i = 0; i < bullets.Count; ++i) {
			pool.RemoveBullet (bullets [i]);
		}
	}
}