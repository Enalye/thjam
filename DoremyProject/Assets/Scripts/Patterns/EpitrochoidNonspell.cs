using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Entity {
	public IEnumerator Epitrochoid(float a, float b, float c) {
		yield return new WaitForSeconds (0.5f);

		float min_a = a / 25f;
		float min_b = b / 10f;

		float angle = 0;
		float change = 2.5f;

		float speed = 200f;

		float side_latency = 1;
		float min_latencyspeed = 0.003f;
		float latencyspeed = 0.001f;

		float side = 1;

		int count = 0;
		int patternID = currentPattern;
		while (obj.Active && !obj.Removing && currentPattern == patternID) {
			float radAngle = Mathf.Deg2Rad * angle;
			float xTerm = Mathf.Deg2Rad * (((a / b) + 1) * angle);
			float yTerm = Mathf.Deg2Rad * (((a / b) + 1) * angle);

			float x = (a + b) * Mathf.Cos(radAngle) - c * Mathf.Cos(xTerm);
			float y = (a + b) * Mathf.Sin(radAngle) - c * Mathf.Sin(yTerm); 
			Vector3 pos = new Vector3(obj.Position.x + x, obj.Position.y + y, Layering.Bullet);

			Color32 color = (count % 2 == 0) ? Colors.hotpink : Colors.orchid;
			EType type = (count % 10 == 0) ? EType.DREAM : EType.NIGHTMARE;
			color = (type == EType.DREAM) ? Colors.royalblue : color;
			float acceleration = (type == EType.DREAM) ? 0.5f : 0;

			Bullet shot = pool.AddBullet(GameScheduler.instance.sprites[4], type, EMaterial.BULLET,
										 color, pos, 200f, angle, acceleration);
			shot.Scale = ((count % 2 == 0) ? 0.8f : 1.2f) * Vector3.one;

			yield return new WaitForSeconds(latencyspeed);
			angle += change;

			if(a * side > min_a) {
				a -= side * 0.05f;
				speed += side * 2f;
			} else {
				side *= -1;
			}

			if(b * side > min_b) {
				b -= side * 0.02f;
			} else {
				side *= -1;
			}

			if(latencyspeed > min_latencyspeed) {
				latencyspeed -= side * 0.0001f;
			} else {
				side_latency *= -1;
			}

			count++;
		}
	}
}
