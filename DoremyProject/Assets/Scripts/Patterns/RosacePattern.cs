using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator RosaceSpell(int direction) {
		for (int i = 0; i < 5; ++i) {
			StartCoroutine (RosacePattern(direction, i, 360 / 5 * i));
			yield return new WaitForSeconds(1.5f);
		}

		yield return null;
	}

	public IEnumerator RosacePattern(int direction, int generation, float angOffset) {
		float radius = 0;
		float angleUpdate = 0.25f;
		float nbBranches = 2;

		EType type = (generation % 2 == 0) ? EType.NIGHTMARE : EType.DREAM;
		Color32 color = (type == EType.NIGHTMARE) ? Colors.orchid : Colors.royalblue;

		for(int i = 0; i < 360; i+= 360 / 60) {
			float ang = i;

			Bullet shot = pool.AddBullet (GameScheduler.instance.sprites[0], type, EMaterial.BULLET,
										  obj.Position, 0, ang, 0, 0);
				
			shot.Type = type;
			shot.SpriteAngle = Vector3.forward * ang;
			shot.Color = color;
			shot.Radius = 0.1f;
			shot.Scale = Vector3.one * 1.5f;
			bullets.Add(shot);

			StartCoroutine (shot._Appear(0.2f));
		}

		bool expand = true;
		while(Application.isPlaying) {
			// Update only bullets in the current generation
			for(int i = generation * 60; i < generation * 60 + 60; i++) {
				Bullet shot = bullets [i];

				if(angleUpdate > 0.15f) {
					angleUpdate -= 0.00001f;
				}

				shot.Angle += angleUpdate;

				if (expand == true && radius < 10) {
					radius += 0.00025f;

					if (radius >= 10) {
						expand = false;
					}
				} else if (expand == false && radius > 0) {
					radius -= 0.00025f;
				}

				Rosace(nbBranches, shot.Angle, radius, angOffset * direction, shot);
			}

			yield return new WaitForSeconds(0.01f);
		}

		yield return null;
	}

	public void Rosace(float k, float ang, float radius, float rot, Bullet rosacePart) {
		float radAng = Mathf.Deg2Rad * ang;
		rosacePart.Position.x = Mathf.Cos(k * radAng) * Mathf.Sin(radAng) * radius;
		rosacePart.Position.y = Mathf.Cos(k * radAng) * Mathf.Cos(radAng) * radius;

		float x = rosacePart.Position.x;
		float y = rosacePart.Position.y;

		float radRot = Mathf.Deg2Rad * rot;
		rosacePart.Position.x = obj.Position.x + (x * Mathf.Cos(radRot) - y * Mathf.Sin(radRot));
		rosacePart.Position.y = obj.Position.y + (x * Mathf.Sin(radRot) + y * Mathf.Cos(radRot));
	}
}
