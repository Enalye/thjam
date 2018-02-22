using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator LotusSpell() {
		//StartCoroutine(CirclePattern());
		yield return StartCoroutine(Lotus());
	}

	public IEnumerator Lotus() {
		yield return new WaitForSeconds (0.5f);

		int count = 0;
		float angle = 0;
		float angvec = 0;
		int patternID = currentPattern;

		while (obj.Active && !obj.Removing && (patternID == currentPattern)) {
			while(angle < 360) {
				float radAngle = Mathf.Deg2Rad * angle;

				float acos3 = Mathf.Abs(Mathf.Cos(radAngle * 3));
				float acos3pi2 = Mathf.Abs(Mathf.Cos(radAngle * 3 + Mathf.PI * 0.5f));
				float acos6pi2 = Mathf.Abs(Mathf.Cos(radAngle * 6 + Mathf.PI * 0.5f));

				// TODO : Add rotation coroutine

				if(count % 2 == 0) {
					float[] forbiddenSeeds = {210, 270, 330};

					if(!shouldSkip(angle, forbiddenSeeds)) {
						float r1 = ((acos3 + (0.25f - acos3pi2) * 500) / (2 + acos6pi2 * 8));
						Vector3 pos = new Vector3(obj.Position.x + r1 * Mathf.Cos(radAngle), obj.Position.y + r1 * Mathf.Sin(radAngle), Layering.Bullet);

						EType type = (angle % 60 == 0) ? EType.DREAM : EType.NIGHTMARE;
						Color32 color = Colors.mediumpurple;
						color = (type == EType.DREAM) ? Colors.royalblue : color;
						Sprite sprite = (type == EType.DREAM) ? GameScheduler.instance.sprites[0] : GameScheduler.instance.sprites[1];

						Bullet shot = pool.AddBullet(sprite, type, EMaterial.BULLET,
							color, pos, Vector3.one * 1.25f, 200f, angle, -acos3 * 0.25f);
						shot.Radius = 5;
						shot.SpriteAngle = BulletSpriteAngle(shot);
					}
				} else {
					float[] forbiddenSeeds = {15, 165, 195, 225, 255, 285, 315, 345};

					if(!shouldSkip(angle, forbiddenSeeds)) {
						float acos6 = Mathf.Abs(Mathf.Cos(radAngle * 6));
						float acos12pi2 = Mathf.Abs(Mathf.Cos(radAngle * 12 + Mathf.PI * 0.5f));

						float r2 = ((acos3 + (0.25f - acos6pi2) * 500) / (2 + acos12pi2 * 8));
						Vector3 pos = new Vector3(obj.Position.x + r2 * Mathf.Cos(radAngle), obj.Position.y + r2 * Mathf.Sin(radAngle), Layering.Bullet);

						EType type = (angle % 30 == 0) ? EType.DREAM : EType.NIGHTMARE;
						Color32 color = Colors.orchid;
						color = (type == EType.DREAM) ? Colors.royalblue : color;
						Sprite sprite = (type == EType.DREAM) ? GameScheduler.instance.sprites[0] : GameScheduler.instance.sprites[1];

						Bullet shot = pool.AddBullet(sprite, type, EMaterial.BULLET,
							color, pos, Vector3.one * 1.25f, 200f, angle, -acos6 * 0.25f);
						shot.Radius = 5;
						shot.SpriteAngle = BulletSpriteAngle(shot);
					}
				}

				angle++;
			}
				
			angle = 0;
			count++;
			yield return new WaitForSeconds(1.0f);
		}
	}

	bool shouldSkip(float value, float[] forbiddenSeeds) {
		bool skip = false;
		for(int i = 0; (i < forbiddenSeeds.Length) && (skip == false); ++i) {
			skip = (value == forbiddenSeeds[i] - 1) || (value == forbiddenSeeds[i] + 1);
		}
		return skip;
	}
}