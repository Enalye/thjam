using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator PlantPattern() {
		//yield return new WaitForSeconds (1.6f);
		while (obj.Active) {
			//yield return new WaitForSeconds(0.3f);
			float nAngle = -180;

			while (nAngle < 180) {
				StartCoroutine(_Stems(nAngle, 0.05f));
				nAngle += 45;
				yield return null;
			}

			yield return new WaitForSeconds(4.2f);
		}
	}

	public IEnumerator _Stems(float angle, float dec) {
		Bullet trim = pool.AddBullet (bullet_sprite, EType.DREAM, EMaterial.BULLET,
									  obj.Position, 2.0f, angle, 0.025f, 0.2f);
		trim.Color = Color.cyan;
		trim.SetScaleFromRadius(0.2f);

		float count = 0;
		while(trim.Active) {
			// Distance to player bigger than 20 
			if(count % 3 == 0) {
				if(count % 6 == 0) {
					float radAngle2 = Mathf.Deg2Rad * (trim.Angle + 90);
					float radAngle3 = Mathf.Deg2Rad * (trim.Angle - 90);

					float sin2 = Mathf.Sin (radAngle2);
					float cos2 = Mathf.Cos (radAngle2);
					float sin3 = Mathf.Sin (radAngle3);
					float cos3 = Mathf.Cos (radAngle3);

					float angDiff = Random.Range(30.0f, 45.0f);

					Bullet stem1 = pool.AddBullet (bullet_sprite, EType.NIGHTMARE, EMaterial.BULLET,
												   trim.Position, 0.0f, trim.Angle);
					stem1.Lifetime = 2.4f;
					stem1.Delay = 0.25f;
					stem1.Color = Color.magenta;
					stem1.SetScaleFromRadius(0.1f);
					StartCoroutine(stem1._Change(1.6f, bullet_sprite, Color.cyan, EType.DREAM, 0.0f, stem1.Angle));

					Vector3 pos2 = new Vector3 (trim.Position.x + dec * cos2, trim.Position.y + dec * sin2); 
					Bullet stem2 = pool.AddBullet (bullet_sprite, EType.NIGHTMARE, EMaterial.BULLET,
						                           pos2, 0.0f, trim.Angle + angDiff);
					stem2.Lifetime = 2.4f;
					stem2.Delay = 0.25f;
					stem2.Color = Color.magenta;
					stem2.SetScaleFromRadius(0.1f);
					StartCoroutine(stem2._Change(1.6f, bullet_sprite, Color.cyan, EType.DREAM, 0.0f, trim.Angle + Random.Range(30.0f, 45.0f)));

					Vector3 pos3 = new Vector3 (trim.Position.x + dec * cos3, trim.Position.y + dec * sin3);
					Bullet stem3 = pool.AddBullet (bullet_sprite, EType.NIGHTMARE, EMaterial.BULLET,
						                           pos3, 0.0f, trim.Angle - angDiff);
					stem3.Lifetime = 2.4f;
					stem3.Delay = 0.25f;
					stem3.Color = Color.magenta;
					stem3.SetScaleFromRadius(0.1f);
					StartCoroutine(stem3._Change(1.6f, bullet_sprite, Color.cyan, EType.DREAM, 0.0f, trim.Angle - Random.Range(30.0f, 45.0f)));
				}
			}

			if(count % 40 == 15) {
				Flowers(trim);
				count = 0;
			}
				
			count++;
			yield return null;
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

			Vector3 pos = new Vector3(trim.Position.x + nGap * 0.01f * cosAngleGap + 0.2f * cosAngleB,
				                      trim.Position.y + nGap * 0.01f * sinAngleGap + 0.2f * sinAngleB);
			Bullet petal = pool.AddBullet(bullet_sprite, EType.DREAM, EMaterial.BULLET,
				                          pos, 0, nAngle + nAngle_b);
			petal.Delay = 0.2f;
			petal.SetScaleFromRadius(0.1f);
			petal.Color = Color.green;

			StartCoroutine(petal._Change(1.2f, bullet_sprite, Color.red, EType.NIGHTMARE, null, null, 0.04f));
			petal.MaxSpeed = 1.5f;
			petal.Lifetime = 4.0f;
			nAngle += 60;
		}
	}
}