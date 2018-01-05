using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator RosaceSpell() {
		for (int i = 0; i < 5; ++i) {
			StartCoroutine (RosacePattern(i, 360 / 5 * i));
			yield return new WaitForSeconds(1.5f);
		}

		yield return null;
	}

	public IEnumerator RosacePattern(int generation, float angOffset) {
		float radius = 0;
		float angleUpdate = 0.25f;
		float nbBranches = 2;

		EType type = (generation % 2 == 0) ? EType.NIGHTMARE : EType.DREAM;
		Color color = (type == EType.NIGHTMARE) ? Color.magenta : Color.cyan;

		if (bullet_sprite) { // The mesh pool for bullets != null) {
			for(int i = 0; i < 360; i+= 360 / 60) {
				float ang = i;

				Bullet shot = pool.AddBullet (bullet_sprite, type, EMaterial.BULLET,
					obj.Position, 0, ang, 0, 0);
				shot.Type = type;
				shot.Color = color;
				shot.SetScaleFromRadius(0.2f);
				bullets.Add(shot);
			}
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

				Rosace(nbBranches, shot.Angle, radius, angOffset, shot);
			}

			yield return new WaitForSeconds(0.01f);
		}

		yield return null;
	}
}
