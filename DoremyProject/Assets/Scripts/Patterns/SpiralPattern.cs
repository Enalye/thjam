﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	IEnumerator SpiralPattern() {
		while (obj.Active) {
			yield return new WaitForSeconds (2.5f);
			float angle = Random.Range (0, 360);

			int nbSpirals = 3;
			int nbBulletsPerSpiral = 20;

			for (int i = 0; i < nbSpirals; ++i) {
				float angle2 = Random.Range (0, 360);
				float speed2 = Random.Range (-60f, -90f);
				float radius2 = 0;

				Vector3 pos0 = Vector3.zero;
				Vector3 pos10 = Vector3.zero;
				Bullet dream = pool.AddBullet (GameScheduler.instance.sprites[0], EType.DREAM, EMaterial.BULLET, Colors.royalblue);

				for (int j = 0; j < nbBulletsPerSpiral; ++j) {
					radius2 += 6f;
					float radAng2 = Mathf.Deg2Rad * angle2;

					Vector3 pos = new Vector3 (obj.Position.x + radius2 * Mathf.Cos (radAng2),
						                       obj.Position.y + radius2 * Mathf.Sin (radAng2));

					if (j == 0) {
						pos0 = pos;
					}

					if (j == 10) {
						pos10 = pos;
					}

					Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[0], EType.NIGHTMARE, EMaterial.BULLET,
												  Colors.orchid, pos, 0.0f, angle);

					shot.MinSpeed = speed2;
					shot.Scale = Vector3.one * 1.15f;
					shot.Radius = 5f;
					bullets.Add(shot);
					StartCoroutine (shot._RotateAround(dream, 0.5f));

					angle2 += 360 / 18;
				}
					
				dream.Position = ((pos0 + pos10) / 2);
				dream.Speed = 0.0f;
				dream.Angle = angle;
				dream.Acceleration = -7.5f;
				dream.MinSpeed = speed2;
				dream.Scale = Vector3.one * 1.5f;
				dream.Radius = 10f;

				angle += (360 / 3) + Random.Range (-10, 10);
			}
		}
	}
}