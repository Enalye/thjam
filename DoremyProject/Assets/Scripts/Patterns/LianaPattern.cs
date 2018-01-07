using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO
public partial class Enemy : Entity {
	public IEnumerator LianePattern() {
		yield return new WaitForSeconds(0.1f);

		float chose     = 0;
		float n = (obj.Position.x < 0) ? 180f : 0f;
		float speed = 150f; 

		while (obj.Active) {
			Bullet shot = pool.AddBullet (GameScheduler.instance.sprites [5], EType.NIGHTMARE, EMaterial.BULLET, Colors.chartreusegreen,
				                          obj.Position, speed, n);
			shot.SpriteAngle = Vector3.forward * n;
			shot.Radius = 5f;

			n = n + Mathf.Cos (Mathf.Deg2Rad * chose) * 3;
			chose = chose + 25;

			yield return new WaitForSeconds(0.1f);
		}
	}
}