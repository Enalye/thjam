using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EPattern { CIRCLE, ROSACE, HOMING, SPIRAL };

[ExecuteInEditMode]
public partial class Enemy : Entity {
	public Sprite power_item;
	public Sprite point_item;

    public Sprite death_effect;     // The effect used for coroutine DeathEffect
    public Color death_color;       // Color used for coroutine DeathEffect
	public float life;              // The amount of life this enemy has
	public float wait_time;

	public EPattern    pattern;
	public BezierCurve curve;

	public Sprite bullet_sprite;

    private bool dead;

    public override void Init() {
        base.Init();
		dead = false;

        if(obj != null && Application.isPlaying) {
			obj.SetScaleFromRadius(0.35f);

			StartCoroutine(_Behaviour());
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

	public IEnumerator _Behaviour() {
		yield return new WaitForSeconds(wait_time);

		if (curve != null) {
			StartCoroutine (obj._Follow (curve));
		}

		if (pattern == EPattern.ROSACE) {
			StartCoroutine(RosaceSpell(obj.Position.x > 0 ? 1 : -1));
		}

		if (pattern == EPattern.CIRCLE) {
			StartCoroutine(CirclePattern());
		}

		if (pattern == EPattern.HOMING) {
			obj.Lifetime = wait_time + 8;
			StartCoroutine(HomingPattern());
		}

		if (pattern == EPattern.SPIRAL) {
			StartCoroutine(PlantPattern());
		}
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
