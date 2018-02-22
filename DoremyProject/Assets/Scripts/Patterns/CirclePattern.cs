using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator CirclePattern() {
		yield return new WaitForSeconds(2.0f);
		for(int i = 0; i < 5; i++) {
			EType type = (i % 2 == 0) ? EType.NIGHTMARE : EType.DREAM;

			if (obj.Active && !obj.Removing) {
				Circle(25, 200f, 0);
			}

			yield return new WaitForSeconds(0.75f);
		}

		yield return new WaitForSeconds (0.25f);
	}


	public void Circle(float n, float speed, float offset) {
		int a = 0;
		for(float i = 0; i < 360; i += 360 / n) {
			float ang = i + offset;

			EType type = (a % 3 == 0) ? EType.DREAM : EType.NIGHTMARE;
			Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[1], type, EMaterial.BULLET,
				                          type == (EType.NIGHTMARE) ? Colors.chartreusegreen : Colors.royalblue,
										  obj.Position, Vector3.one * 1.5f, speed, ang);

			shot.SpriteAngle = Vector3.forward * (ang - 180);
			shot.Radius = 10f;

			bullets.Add(shot);
			a++;
		}
	}
}