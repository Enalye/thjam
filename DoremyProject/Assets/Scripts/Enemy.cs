﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EPattern { NONE, CIRCLE, ROSACE, HOMING, SPIRAL, PLANT, ELLIPSE, MAGUS, KNIFE, WAVE, LIANE, BOSS };

public partial class Enemy : Entity {
	public Sprite power_item;
	public Sprite point_item;

    public Sprite death_effect;     // The effect used for coroutine DeathEffect
    public Color death_color;       // Color used for coroutine DeathEffect
	public float base_life;              // The amount of life this enemy has
	public float wait_time;

	public EPattern    pattern;
	public BezierCurve curve;
	public bool can_be_damaged;
	public bool debug;

	public int nbPatterns = 1;

	protected bool  dead;
	protected float life;
	protected int   currentPattern;

    public override void Init() {
        base.Init();
		dead = false;
		life = base_life;
		can_be_damaged = false;
		currentPattern = 0;

		obj.Position.z = Layering.Enemy;

        if(obj != null && Application.isPlaying) {
			obj.Radius = 35;
			obj.Scale = Vector3.one;

			StartCoroutine(_Behaviour());
		}
    }

	public virtual void UpdateAt(float dt) {
		pool.QuadTreeHolder.CheckCollision(this);
	}

	public IEnumerator _Behaviour() {
		while (GameScheduler.instance.dialogue.in_dialogue) {
			yield return new WaitForSeconds(GameScheduler.dt);
		}

		yield return new WaitForSeconds(wait_time);

		can_be_damaged = true;

		if (curve != null) {
			if(debug) {
				obj.BoundPosition = curve.GetPoint(0.999f);
			} else {
				StartCoroutine (obj._Follow (curve));
			}
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
			StartCoroutine(SpiralPattern());
		}


		if (pattern == EPattern.ELLIPSE) {
			StartCoroutine(EllipsePattern());
		}
			
		if (pattern == EPattern.KNIFE) {
			obj.Type = EType.DREAM;
			obj.Color = Colors.royalblue;
			StartCoroutine(KnifePattern(15, 50));
		}

		if (pattern == EPattern.WAVE) {
			StartCoroutine(WavePattern());
		}

		if (pattern == EPattern.LIANE) {
			StartCoroutine(LianePattern());
		}

		if (pattern == EPattern.BOSS) {
			StartCoroutine(Boss());
		}

		// BOSS
		if (pattern == EPattern.PLANT) {
			StartCoroutine(PlantPattern());
		}

		if (pattern == EPattern.MAGUS) {
			StartCoroutine(MagusPattern());
		}
	}

	IEnumerator Boss() {
		if(!debug) {
			yield return new WaitForSeconds(3.0f);
		}

		GameScheduler.instance.PlayBossMusic();

		yield return StartCoroutine(Epitrochoid(100, 10, 100));
		yield return StartCoroutine(PlantPattern());
		yield return StartCoroutine(MagusPattern());
		yield return StartCoroutine(LotusSpell());

		Fading fadingObj = GameObject.Find ("Fading").GetComponent<Fading> ();
		fadingObj.BeginFade (1);
		yield return new WaitForSeconds (fadingObj.fadeSpeed);
		SceneManager.LoadScene (3);
	}

	public virtual void TakeDamage(float damage) {
		life -= damage;

		if (life <= 0) {
			NextPattern();
		}
	}

	public Vector3 BulletPos() {
		return new Vector3(obj.Position.x, obj.Position.y, Layering.Bullet);
	}

	public Vector3 BulletSpriteAngle(Bullet bullet) {
		float angle = Mathf.Atan2(bullet.Position.y - obj.Position.y, bullet.Position.x - obj.Position.x) * Mathf.Rad2Deg;
		return Vector3.forward * (angle + 180);
	}

	private void NextPattern() {
		currentPattern++;
		life = base_life;

		if (currentPattern > nbPatterns) {
			Die();
		}
	}

	private void Die() {
		if(!dead) {
			obj.MarkForDeletion();
			dead = true;
		}
	}
}
