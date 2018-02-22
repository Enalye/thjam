using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator PlantPattern() {
		int generation = 0;
		int patternID = currentPattern;
		while (obj.Active && (currentPattern == patternID)) {
			float nAngle = -180;

			while (nAngle < 180) {
				StartCoroutine(_Stems(nAngle, 10f, generation));
				nAngle += 45;
				yield return new WaitForSeconds(GameScheduler.dt);
			}

			generation++;
			yield return new WaitForSeconds(6.2f);
		}
	}

	public IEnumerator _Stems(float angle, float dec, int mainGen) {
		float angVec = (mainGen % 2 == 0) ? 0.2f : -0.2f;
		Bullet trim = pool.AddBullet(GameScheduler.instance.sprites[3], EType.DREAM, EMaterial.ENEMY,
									 Colors.royalblue, BulletPos(), 100f, angle, 1.25f, angVec);
		trim.Radius = 15f;

		float time = 0;
		int generation = 0;
		while(trim.Active && !trim.Removing && !trim.Magnetized) {
			float radAngle2 = Mathf.Deg2Rad * (trim.Angle + 120);
			float radAngle3 = Mathf.Deg2Rad * (trim.Angle - 120);

			float sin2 = Mathf.Sin (radAngle2);
			float cos2 = Mathf.Cos (radAngle2);
			float sin3 = Mathf.Sin (radAngle3);
			float cos3 = Mathf.Cos (radAngle3);

			float angDiff = Random.Range(30.0f, 45.0f);

			Bullet stem1 = pool.AddBullet (GameScheduler.instance.sprites[0], EType.NIGHTMARE, EMaterial.BULLET,
				                           Colors.chartreusegreen, trim.Position, 0.0f, trim.Angle);
			stem1.Delay = 0.25f;
			stem1.Lifetime = 2.6f;
			StartCoroutine(stem1._Change(1.6f, GameScheduler.instance.sprites[0], Color.white, EType.DREAM, 0.0f, stem1.Angle));

			Vector3 pos2 = new Vector3 (trim.Position.x + dec * cos2, trim.Position.y + dec * sin2, Layering.Bullet); 
			Bullet stem2 = pool.AddBullet (GameScheduler.instance.sprites[0], EType.NIGHTMARE, EMaterial.BULLET,
				                           Colors.chartreusegreen, pos2, 0.0f, trim.Angle + angDiff);
			stem2.Delay = 0.25f;
			stem2.Lifetime = 2.6f;
			StartCoroutine(stem2._Change(1.6f, GameScheduler.instance.sprites[0], Color.white, EType.DREAM, 0.0f, trim.Angle + Random.Range(30.0f, 45.0f)));

			Vector3 pos3 = new Vector3 (trim.Position.x + dec * cos3, trim.Position.y + dec * sin3, Layering.Bullet);
			Bullet stem3 = pool.AddBullet (GameScheduler.instance.sprites[0], EType.NIGHTMARE, EMaterial.BULLET,
				                           Colors.chartreusegreen, pos3, 0.0f, trim.Angle - angDiff);
			stem3.Delay = 0.25f;
			stem3.Lifetime = 2.6f;
			StartCoroutine(stem3._Change(1.6f, GameScheduler.instance.sprites[0], Color.white, EType.DREAM, 0.0f, trim.Angle - Random.Range(30.0f, 45.0f)));

			if(time == 0.2f) {
				Flowers(trim, generation);
				time = 0;
				generation++;
			}
				
			time += 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
	}

	public void Flowers(Bullet trim, int generation) {
		float nAngle_Gap = Random.Range(0.0f, 360.0f);
		float nGap = Random.Range(0.0f, 30.0f);
		Vector3 playerPos = Player.instance.obj.Position;

		float nAngle_b = Mathf.Atan2(playerPos.y - trim.Position.y, playerPos.x - trim.Position.x) * Mathf.Rad2Deg;
		float nAngle = -180.0f;

		float cosAngleGap = Mathf.Cos(nAngle_Gap * Mathf.Deg2Rad);
		float sinAngleGap = Mathf.Sin(nAngle_Gap * Mathf.Deg2Rad);

		int count = 0;
		while(nAngle < 180) {
			float cosAngleB = Mathf.Cos((nAngle + nAngle_b) * Mathf.Deg2Rad);
			float sinAngleB = Mathf.Sin((nAngle + nAngle_b) * Mathf.Deg2Rad);

			Vector3 pos = new Vector3(trim.Position.x + nGap * cosAngleGap + 10f * cosAngleB,
				                      trim.Position.y + nGap * sinAngleGap + 10f * sinAngleB,
									  Layering.Bullet);
			
			float angle = nAngle + nAngle_b;
			Bullet petal = pool.AddBullet(GameScheduler.instance.sprites[1], EType.NIGHTMARE, EMaterial.BULLET,
				                          Colors.yellow, pos, 0, angle);
			petal.Delay = 0.2f;
			petal.SpriteAngle = Vector3.forward * angle;
			petal.MaxSpeed = 150f;
			petal.MinSpeed = -200f;

			StartCoroutine(petal._Change(1.2f, GameScheduler.instance.sprites[1], Colors.orange, EType.NIGHTMARE, null, null, 2f));

			EType finalType = ((count == 2 || count == 4) && (generation % 3 == 0)) ? EType.DREAM : EType.NIGHTMARE;
			Color32 finalColor = (finalType == EType.DREAM) ? Colors.royalblue : Colors.firebrick;
			StartCoroutine(petal._Change(2.8f, GameScheduler.instance.sprites[1], finalColor, finalType, null, null, -2f));

			if(finalType == EType.DREAM) {
				petal.MinSpeed = -100f;
			}

			nAngle += 60;
			count++;
		}
	}
}