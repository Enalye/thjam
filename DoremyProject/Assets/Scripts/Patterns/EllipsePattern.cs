using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator EllipsePattern() {
		float time = 0;
		float angle = 0;
		float angle2 = 10;


		yield return new WaitForSeconds(1.2f);

		while (obj.Active) {
			if ((time % 20 == 0)) {
				float speed = 200f;

				// Play sound

				for (int i = 0; i < 4; ++i) {
					float cos = Mathf.Cos (Mathf.Deg2Rad * angle2);
					float sin = Mathf.Sin (Mathf.Deg2Rad * angle2);

					for (int j = 0; j < 12; ++j) {
						Bullet shot = pool.AddBullet (GameScheduler.instance.sprites [0], EType.DREAM, EMaterial.BULLET, Colors.orange,
							             new Vector3 (obj.Position.x + 100 * cos, obj.Position.y + 20 * sin), speed, angle, -speed / 60);
						StartCoroutine (shot._Change (0.1f, GameScheduler.instance.sprites [0], Color.white, EType.NIGHTMARE, speed, null, 0));

						angle += 360 / 12;
					}

					if (speed > 0) {
						speed -= 1;
					}

					if (speed <= 0) {
						speed += 1;
					}

					angle += 360 / 24;
				}

				angle2 += 20;
				angle += 360 / 48;
			}

			time++;
			yield return new WaitForSeconds (GameScheduler.dt);
		}
	}
}
