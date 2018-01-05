using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Enemy : Entity {
	public Sprite power_item;
	public Sprite point_item;

    public Sprite death_effect;     // The effect used for coroutine DeathEffect
    public Color death_color;       // Color used for coroutine DeathEffect
	public float life;              // The amount of life this enemy has

	public Sprite bullet_sprite;

    private bool dead;

    public override void Init() {
        base.Init();
		dead = false;

        if(obj != null && Application.isPlaying) {
			StartCoroutine(Pattern());
        }
    }

    public void Die() {
		Debug.Log("Dying");
        if(!dead) {
            pool.RemoveBullet(obj);

			StopAllCoroutines();
			StartCoroutine(_DropItems(1, 1));;
            dead = true;
        }
    }

	public void UpdateAt(float dt) {
		pool.QuadTreeHolder.CheckCollision(this);
	}

	public IEnumerator _DropItems(int nbPowerItems, int nbPointItems) {
		int spriteAngularSpeed = 2;
		for (int i = 0; i < nbPowerItems; ++i) {
			SpawnItem (power_item);
		}

		for (int i = 0; i < nbPowerItems; ++i) {
			SpawnItem (point_item);
		}

		yield return new WaitForFixedUpdate ();
	}

	public IEnumerator Pattern() {
		for(int i = 0; i < 5; i++) {
			Circle(15, 2f, 0);
			yield return new WaitForSeconds(0.75f);
		}

		yield return new WaitForSeconds (0.25f);
	}

	public void Circle(float n, float speed, float offset) {
		for(float i = 0; i < 360; i += 360 / n) {
			float ang = i + offset;

			if (bullet_sprite) { // The mesh pool for bullets != null) {
				Bullet shot = pool.AddBullet(bullet_sprite, EType.BULLET, EMaterial.BULLET,
					             			 obj.Position, speed, ang, 0, 0);

				shot.Radius = 0.2f;
				shot.Scale = Vector3.one * shot.Radius * 2;
				shot.SpriteAngle = new Vector3 (0, 0, ang - 90);
				shot.AutoDelete = false;

				bullets.Add(shot);
			}
		}
	}

	private void SpawnItem(Sprite sprite) {
		float ang = 90;
		Vector3 pos = obj.Position + Vector3.forward * 10;
		Bullet item = pool.AddBullet(sprite, EType.ITEM, EMaterial.BULLET,
								     pos, 50, ang, -1);
		item.SpriteAngle = Vector3.forward * (ang - 90);
		item.SpriteAngularVelocity = Vector3.forward * 2;;
		StartCoroutine(item._Change(0.75f, null, null, 0, null));
	}
}
