using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator CirclePattern() {
		yield return new WaitForSeconds(2.0f);
		for(int i = 0; i < 5; i++) {
			EType type = (i % 2 == 0) ? EType.NIGHTMARE : EType.DREAM;
			Circle(type, 45, 2f, 0);
			yield return new WaitForSeconds(0.75f);
		}

		yield return new WaitForSeconds (0.25f);
	}


	public void Circle(EType type, float n, float speed, float offset) {
		for(float i = 0; i < 360; i += 360 / n) {
			float ang = i + offset;

			if (bullet_sprite) { // The mesh pool for bullets != null) {
				Bullet shot = pool.AddBullet (bullet_sprite, type, EMaterial.BULLET,
					              		      obj.Position, 1.5f, ang);

				shot.Color = type == (EType.NIGHTMARE) ? Color.magenta : Color.cyan;
				shot.SetScaleFromRadius(0.2f);
				shot.SpriteAngle = new Vector3 (0, 0, ang - 90);
				shot.AutoDelete = false;

				bullets.Add(shot);
			}
		}
	}
}