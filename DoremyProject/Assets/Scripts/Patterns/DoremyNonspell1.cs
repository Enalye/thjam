using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	IEnumerator DoremyNonspell1() {
		int n = 75;

		float a = 4;
		float b = 9;
		float c = 1;

		float radius = 2;

		while (obj.Active) {
			for (float angle = 0; angle < 360; angle += 360 / n) {
				float radAng = angle * Mathf.Deg2Rad;

				float x = obj.Position.x + (Mathf.Cos (a * radAng) + (Mathf.Cos (b * radAng) / 2) + (Mathf.Sin (c * radAng) / 2)) * radius;
				float y = obj.Position.y + (Mathf.Sin (a * radAng) + (Mathf.Sin (b * radAng) / 2) + (Mathf.Cos (c * radAng) / 2)) * radius;

				Vector3 pos = new Vector3 (x, y);
				float angleToEnemy = Mathf.Atan2 (pos.y - obj.Position.y, pos.x - obj.Position.x) * Mathf.Rad2Deg;

				Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[0], EType.NIGHTMARE, EMaterial.BULLET,
					             pos, 0.1f, angle, 0.01f, 0.2f);
				shot.Color = Color.magenta;
				shot.SetScaleFromRadius (0.2f);
			}

			yield return new WaitForSeconds(0.5f);
		}
	}
}