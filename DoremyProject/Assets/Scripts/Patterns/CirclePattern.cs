using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator CirclePattern() {
		yield return new WaitForSeconds(2.0f);
		for(int i = 0; i < 5; i++) {
			EType type = (i % 2 == 0) ? EType.NIGHTMARE : EType.DREAM;
			Circle(type, 45, 200f, 0);
			yield return new WaitForSeconds(0.75f);
		}

		yield return new WaitForSeconds (0.25f);
	}


	public void Circle(EType type, float n, float speed, float offset) {
		for(float i = 0; i < 360; i += 360 / n) {
			float ang = i + offset;

			Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[1], type, EMaterial.BULLET,
				                          obj.Position, speed, ang);

			shot.Position.z = Layering.Bullet;
			shot.Color = type == (EType.NIGHTMARE) ? Colors.firebrick : Colors.chartreusegreen;
			shot.SpriteAngle = Vector3.forward * (ang - 180);
			shot.Radius = 10f;
			shot.Scale = Vector3.one * 1.5f;
			shot.AutoDelete = false;

			bullets.Add(shot);
		}
	}
}