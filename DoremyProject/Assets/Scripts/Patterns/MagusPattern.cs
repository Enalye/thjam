using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator MagusPattern() {
		can_be_damaged = false;
		yield return new WaitForSeconds (3.0f);
		can_be_damaged = true;

		// Variables
		float bway = 5;
		float bnum = 3;
		float bitAnglebase = 90;
		float tcount = 0;
		float wTime = 12;
		float bspd = 120f;
		float angacc = 0;
		float tRange = 400f;
		float bAngle = 90;

		int patternID = currentPattern;
		while (obj.Active && (currentPattern == patternID)) {
			if (tRange > 80) {
				for (int i = 0; i < bnum; ++i) {
					float bitAngle = bitAnglebase + 360 / bnum * i;
					float tX = obj.Position.x + tRange * Mathf.Cos (Mathf.Deg2Rad * bitAngle);
					float tY = obj.Position.y + tRange * Mathf.Sin (Mathf.Deg2Rad * bitAngle);

					angacc = bspd * 10 * Mathf.Sin (Mathf.Deg2Rad * (360 / bnum * i + tcount * wTime * 1.2f)) / 60;
					for (int j = 0; j < bway; ++j) {
						float angle = bAngle + 360 / bway * j;
						Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[4], EType.NIGHTMARE, EMaterial.BULLET, Color.white,
							             new Vector3 (tX, tY), 125f, angle, 0, angacc);

						shot.MaxSpeed = bspd;
						shot.SpriteAngle = Vector3.forward * angle;
						shot.Radius = 10f;

						// Nullify angular velocity after one second
						Color32 color;
						EType type = EType.NIGHTMARE;

						if (tcount % 4 == 0) {
							if (j % 3 == 0) {
								color = Colors.royalblue;
								type = EType.DREAM;
							} else {
								color = Colors.orchid;
							}
						} else {
							if (j % 3 == 0) {
								color = Colors.mediumpurple;
							} else {
								color = Colors.hotpink;
							}
						}

						StartCoroutine (shot._Change (1, null, color, type, null, null, 0, 0));

						// Play sound
					}
				}
			}

			bitAnglebase += bspd * 2.5f * Mathf.Sin(Mathf.Deg2Rad * (tcount * wTime / 4));
			bAngle += bspd * Mathf.Sin(Mathf.Deg2Rad * (tcount * wTime / 4));

			if(bspd < 450f){
				bspd += 105f / (60 * 30) * wTime;
			}

			tRange = 200 + 200 * Mathf.Cos(Mathf.Deg2Rad * (tcount * wTime / 1.3f));
			tcount++;
			yield return new WaitForSeconds(1 / wTime);
		}
	}
}