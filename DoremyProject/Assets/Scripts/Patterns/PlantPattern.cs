using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	IEnumerator PlantPattern() {
		while (obj.Active) {
			yield return new WaitForSeconds (1.0f);

			float angle = Random.Range (0, 360);

			int nbSpirals = 3;
			int nbBulletsPerSpiral = 20;

			for (int i = 0; i < nbSpirals; ++i) {
				float angle2 = Random.Range (0, 360);
				float speed2 = Random.Range (-1.25f, -1.75f);
				float radius2 = 0;


				Vector3 pos0 = Vector3.zero;
				Vector3 pos10 = Vector3.zero;

				for (int j = 0; j < nbBulletsPerSpiral; ++j) {
					radius2 += 0.08f;
					float radAng2 = Mathf.Deg2Rad * angle2;

					Vector3 pos = new Vector3 (obj.Position.x + radius2 * Mathf.Cos (radAng2),
						             obj.Position.y + radius2 * Mathf.Sin (radAng2));

					if (j == 0) {
						pos0 = pos;
					}

					if (j == 10) {
						pos10 = pos;
					}

					Bullet shot = pool.AddBullet (bullet_sprite, EType.NIGHTMARE, EMaterial.BULLET,
						             pos, 5.0f, angle, -0.15f);

					shot.MinSpeed = speed2;
					shot.SetScaleFromRadius (0.1f);
					shot.Color = Color.magenta;
					bullets.Add(shot);

					angle2 += 360 / 18;
				}

				Bullet dream = pool.AddBullet (bullet_sprite, EType.DREAM, EMaterial.BULLET,
					((pos0 + pos10) / 2), 5.0f, angle,  -0.15f);

				dream.MinSpeed = speed2;
				dream.SetScaleFromRadius (0.15f);
				dream.Color = Color.cyan;

				angle += (360 / 3) + Random.Range (-10, 10);

				/*for (int bulletIdx = i * nbBulletsPerSpiral; bulletIdx < i * nbBulletsPerSpiral + nbBulletsPerSpiral; ++bulletIdx) {
					StartCoroutine(RotateAroundDream (dream, bullets[bulletIdx]));
				}*/
			}
		}
	}

	/*IEnumerator RotateAroundDream(Bullet dream, Bullet bullet) {
		float rot = 0.5f;
		while (bullet.Active) {
			float radRot = Mathf.Deg2Rad * rot;

			float x = bullet.Position.x;
			float y = bullet.Position.y;

			bullet.Position.x = dream.Position.x + (x * Mathf.Cos (radRot) - y * Mathf.Sin (radRot));
			bullet.Position.y = dream.Position.y + (x * Mathf.Sin (radRot) + y * Mathf.Cos (radRot));
			yield return new WaitForSeconds(0.1f);
		}
	}*/
}