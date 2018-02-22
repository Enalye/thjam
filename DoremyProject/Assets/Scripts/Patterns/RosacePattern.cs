using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator RosaceSpell(int direction) {
		for (int i = 0; i < 5; ++i) {
			yield return new WaitForSeconds(1.5f);

			if (obj.Active) {
				StartCoroutine (RosacePattern (direction, i, 360 / 5 * i));
			}
		}

		yield return null;
	}

	public IEnumerator RosacePattern(int direction, int generation, float angOffset) {
		float radius = 0;
		float angleUpdate = 0.25f;
		float nbBranches = 2;

		int a = 0;
		for(int i = 0; i < 360; i+= 360 / 60) {
			float ang = i;

			EType type = (a % 4 == 0) ? EType.DREAM : EType.NIGHTMARE;
			Color32 color = (type == EType.NIGHTMARE) ? Colors.orchid : Colors.royalblue;

			Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[0], type, EMaterial.BULLET,
				                          color, obj.Position, 0, ang, 0, 0);
				
			shot.Type = type;
			shot.SpriteAngle = Vector3.forward * ang;
			shot.Radius = 10f;
			shot.Scale = Vector3.one * 1.5f;
			bullets.Add(shot);

			StartCoroutine (shot._Appear(0.2f));
			a++;
		}

		bool expand = true;
		bool isAtLeastOneActive = true;
		while(isAtLeastOneActive) {
			isAtLeastOneActive = false;

			// Update only bullets in the current generation
			for(int i = generation * 60; i < generation * 60 + 60; i++) {
				Bullet shot = bullets [i];

				if (shot.Active) {
					isAtLeastOneActive = true;
				}

				if (angleUpdate > 0.15f) {
					angleUpdate -= 0.00001f;
				}

				shot.Angle += angleUpdate;

				if (expand == true && radius < 1000) {
					radius += 0.025f;

				if (radius >= 1000) {
						expand = false;
					}
				} else if (expand == false && radius > 0) {
					radius -= 0.025f;
				}

				if(!shot.Magnetized) {
					Rosace (nbBranches, shot.Angle, radius, angOffset * direction, shot);
				}
			}

			yield return new WaitForSeconds(0.01f);
		}

		yield return null;
	}

	public void Rosace(float k, float ang, float radius, float rot, Bullet rosacePart) {
		float radAng = Mathf.Deg2Rad * ang;
		float posX = Mathf.Cos(k * radAng) * Mathf.Sin(radAng) * radius;
		float posY = Mathf.Cos(k * radAng) * Mathf.Cos(radAng) * radius;

		float radRot = Mathf.Deg2Rad * rot;
		rosacePart.BoundPosition = new Vector3(obj.Position.x + (posX * Mathf.Cos(radRot) - posY * Mathf.Sin(radRot)),
										       obj.Position.y + (posX * Mathf.Sin(radRot) + posY * Mathf.Cos(radRot)));
	}
}
