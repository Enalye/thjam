using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator WavePattern() {
		int time = 0;

		while (obj.Active) {
			if (time == 0) {
				float ang = Random.Range(240f, 300f);
				Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[2], EType.NIGHTMARE, EMaterial.BULLET, Colors.yellow,
					                          obj.Position, 50f, ang);

				shot.SpriteAngle = Vector3.forward * ang;
				shot.Radius = 5f;

				StartCoroutine (shot._Change (2, null, Colors.chartreusegreen, type, null, null, 2.5f, 0));
				StartCoroutine (shot._Change (3, null, Colors.chartreusegreen, type, null, null, 0, 0));

				time -= 10;
			}

			time++;
			yield return new WaitForSeconds (GameScheduler.dt);
		}
	}
}