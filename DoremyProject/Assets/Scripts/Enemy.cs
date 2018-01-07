using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EPattern { NONE, CIRCLE, ROSACE, HOMING, SPIRAL, PLANT, ELLIPSE, MAGUS, KNIFE, WAVE, LIANE, BOSS };

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
	public int nbPatterns = 1;

    private bool dead;
	private int currentPattern;

    public override void Init() {
        base.Init();
		dead = false;
		currentPattern = 0;

        if(obj != null && Application.isPlaying) {
			obj.Radius = 35;
			obj.Scale = Vector3.one;
			obj.AutoDelete = false;

			StartCoroutine(_Behaviour());
		}
    }

    public void Die() {
        if(!dead) {
            pool.RemoveBullet(obj);
			bullets.Clear();

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
			StartCoroutine(SpiralPattern());
		}


		if (pattern == EPattern.ELLIPSE) {
			StartCoroutine(EllipsePattern());
		}
			
		if (pattern == EPattern.KNIFE) {
			obj.Type = EType.DREAM;
			obj.Color = Colors.royalblue;
			obj.AutoDelete = false;
			StartCoroutine(KnifePattern(15, 75));
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
		yield return new WaitForSeconds(2.5f);

		yield return StartCoroutine(PlantPattern());
		yield return StartCoroutine(MagusPattern());
	}

	public void NextPattern() {
		nbPatterns--;
		currentPattern++;

		if (nbPatterns > 0) {
			life = 50;
		}
	}
}
