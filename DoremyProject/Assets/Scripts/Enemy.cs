using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public partial class Enemy : Entity {
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
			obj.SetScaleFromRadius(0.75f);
			//StartCoroutine(CirclePattern());
			StartCoroutine(RosaceSpell());
		}
    }

    public void Die() {
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

	public IEnumerator CirclePattern() {
		for(int i = 0; i < 5; i++) {
			EType type = (i % 2 == 0) ? EType.NIGHTMARE : EType.DREAM;
			Circle(type, 15, 2f, 0);
			yield return new WaitForSeconds(0.75f);
		}

		yield return new WaitForSeconds (0.25f);
	}


	public void Circle(EType type, float n, float speed, float offset) {
		for(float i = 0; i < 360; i += 360 / n) {
			float ang = i + offset;

			if (bullet_sprite) { // The mesh pool for bullets != null) {
				Bullet shot = pool.AddBullet(bullet_sprite, type, EMaterial.BULLET,
					             			 obj.Position, 0, ang, 0, 0);

				shot.Color = type == (EType.NIGHTMARE) ? Color.magenta : Color.cyan;
				shot.SetScaleFromRadius(0.2f);
				shot.SpriteAngle = new Vector3 (0, 0, ang - 90);
				shot.AutoDelete = false;

				bullets.Add(shot);
			}
		}
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
