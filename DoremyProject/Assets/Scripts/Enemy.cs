using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EPattern { NONE, CIRCLE, ROSACE, HOMING, SPIRAL, PLANT, ELLIPSE, MAGUS, KNIFE, WAVE, LIANE, BOSS };

public partial class Enemy : Entity {
	public Sprite power_item;
	public Sprite point_item;

    public Sprite death_effect;     // The effect used for coroutine DeathEffect
    public Color death_color;       // Color used for coroutine DeathEffect
	public float life;              // The amount of life this enemy has
	public float wait_time;

	public EPattern    pattern;
	public BezierCurve curve;
	public bool can_be_damaged;

	public int nbPatterns = 1;

	// States
    private bool dead;
	private int currentPattern;

    public override void Init() {
        base.Init();
		dead = false;
		can_be_damaged = false;
		currentPattern = 0;

        if(obj != null && Application.isPlaying) {
			obj.Radius = 35;
			obj.Scale = Vector3.one;

			StartCoroutine(_Behaviour());
		}
    }

    public void Die() {
        if(!dead) {
            pool.RemoveBullet(obj);
            dead = true;
        }
    }

	public void UpdateAt(float dt) {
		pool.QuadTreeHolder.CheckCollision(this);
	}

	public IEnumerator _Behaviour() {
		while (GameScheduler.instance.dialogue.in_dialogue) {
			yield return  new WaitForSeconds(GameScheduler.dt);
		}

		yield return GameScheduler.dt;

		can_be_damaged = true;

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

		GameScheduler.instance.audioManager.PlayMusic(GameScheduler.instance.bossMusic);

		yield return StartCoroutine(PlantPattern());
		yield return StartCoroutine(MagusPattern());

		Fading fadingObj = GameObject.Find ("Fading").GetComponent<Fading> ();
		fadingObj.BeginFade (1);
		yield return new WaitForSeconds (fadingObj.fadeSpeed);
		SceneManager.LoadScene (3);
	}

	public void NextPattern() {
		nbPatterns--;
		currentPattern++;
	}
}
