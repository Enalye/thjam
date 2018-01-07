using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator PlantPattern() {
		//yield return new WaitForSeconds (1.6f);
		while (obj.Active && (currentPattern == 0)) {
			//yield return new WaitForSeconds(0.3f);
			float nAngle = -180;

			while (nAngle < 180) {
				StartCoroutine(_Stems(nAngle, 25f));
				nAngle += 45;
				yield return new WaitForSeconds(GameScheduler.dt);
			}

			yield return new WaitForSeconds(6.2f);
		}
	}

	public IEnumerator _Stems(float angle, float dec) {
		Bullet trim = pool.AddBullet (GameScheduler.instance.sprites[3], EType.DREAM, EMaterial.ENEMY,
			                          Colors.royalblue, obj.Position, 100f, angle, 1.25f, 0.2f);
		trim.Radius = 15f;

		float count = 0;
		while(trim.Active && !trim.Removing) {
			if(count % 3 == 0) {
				if(count % 6 == 0) {
					float radAngle2 = Mathf.Deg2Rad * (trim.Angle + 90);
					float radAngle3 = Mathf.Deg2Rad * (trim.Angle - 90);

					float sin2 = Mathf.Sin (radAngle2);
					float cos2 = Mathf.Cos (radAngle2);
					float sin3 = Mathf.Sin (radAngle3);
					float cos3 = Mathf.Cos (radAngle3);

					float angDiff = Random.Range(30.0f, 45.0f);

					Bullet stem1 = pool.AddBullet (GameScheduler.instance.sprites[0], EType.NIGHTMARE, EMaterial.BULLET,
						                           Colors.chartreusegreen, trim.Position, 0.0f, trim.Angle);
					stem1.Lifetime = 2.4f;
					stem1.Delay = 0.25f;
					StartCoroutine(stem1._Change(1.6f, GameScheduler.instance.sprites[0], Color.white, EType.DREAM, 0.0f, stem1.Angle));

					Vector3 pos2 = new Vector3 (trim.Position.x + dec * cos2, trim.Position.y + dec * sin2); 
					Bullet stem2 = pool.AddBullet (GameScheduler.instance.sprites[0], EType.NIGHTMARE, EMaterial.BULLET,
						                           Colors.chartreusegreen, pos2, 0.0f, trim.Angle + angDiff);
					stem2.Lifetime = 2.4f;
					stem2.Delay = 0.25f;
					StartCoroutine(stem2._Change(1.6f, GameScheduler.instance.sprites[0], Color.white, EType.DREAM, 0.0f, trim.Angle + Random.Range(30.0f, 45.0f)));

					Vector3 pos3 = new Vector3 (trim.Position.x + dec * cos3, trim.Position.y + dec * sin3);
					Bullet stem3 = pool.AddBullet (GameScheduler.instance.sprites[0], EType.NIGHTMARE, EMaterial.BULLET,
						                           Colors.chartreusegreen, pos3, 0.0f, trim.Angle - angDiff);
					stem3.Lifetime = 2.4f;
					stem3.Delay = 0.25f;
					StartCoroutine(stem3._Change(1.6f, GameScheduler.instance.sprites[0], Color.white, EType.DREAM, 0.0f, trim.Angle - Random.Range(30.0f, 45.0f)));
				}
			}

			if(count % 40 == 15) {
				Flowers(trim);
				count = 0;
			}
				
			count++;
			yield return new WaitForSeconds(GameScheduler.dt);
		}
	}

	public void Flowers(Bullet trim) {
		float nAngle_Gap = Random.Range(0.0f, 360.0f);
		float nGap = Random.Range(0.0f, 30.0f);
		Vector3 playerPos = Player.instance.obj.Position;

		float nAngle_b = Mathf.Atan2(playerPos.y - trim.Position.y, playerPos.x - trim.Position.x) * Mathf.Rad2Deg;
		float nAngle = -180.0f;

		float cosAngleGap = Mathf.Cos(nAngle_Gap * Mathf.Deg2Rad);
		float sinAngleGap = Mathf.Sin(nAngle_Gap * Mathf.Deg2Rad);

		while(nAngle < 180) {
			float cosAngleB = Mathf.Cos((nAngle + nAngle_b) * Mathf.Deg2Rad);
			float sinAngleB = Mathf.Sin((nAngle + nAngle_b) * Mathf.Deg2Rad);

			Vector3 pos = new Vector3(trim.Position.x + nGap * cosAngleGap + 10f * cosAngleB,
				                      trim.Position.y + nGap * sinAngleGap + 10f * sinAngleB);
			float angle = nAngle + nAngle_b;
			Bullet petal = pool.AddBullet(GameScheduler.instance.sprites[1], EType.DREAM, EMaterial.BULLET,
				                          Colors.yellow, pos, 0, angle);
			petal.Delay = 0.2f;
			petal.SpriteAngle = Vector3.forward * angle;

			StartCoroutine(petal._Change(1.2f, GameScheduler.instance.sprites[1], Colors.firebrick, EType.NIGHTMARE, null, null, 2f));
			petal.MaxSpeed = 150f;
			petal.Lifetime = 4.0f;

			StartCoroutine(petal._Change(3.8f, GameScheduler.instance.sprites[1], Colors.yellow, EType.DREAM, null, null, -2f));

			nAngle += 60;
		}
	}
}