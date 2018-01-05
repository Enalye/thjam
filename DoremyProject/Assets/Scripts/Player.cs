﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Entity {
    public static Player instance = null;     // Singleton
	public float hitbox_radius = 0.15f;

    public Animator animator;
	public SpriteRenderer animatorRenderer;

    public AudioClip death_hit; // TODO : create an audio handler class
    public AudioClip death;     // TODO : create an audio handler class
    public AudioClip game_over; // TODO : create an audio handler class

    public Bounds clamping;

	public Bullet playerSprite;
    public GameObject bomb;

    public float focus_speed = 5;
    public float unfocus_speed = 10;
	public float collectHitboxRadius = 50;
	public float secondsOfInvicibilityOnHit = 1.2f;

    public int power_level;
    public Sprite option;
    public float option_scale;
    public List<OptionData> default_option_data;

    public List<Transform> bomb_components;

    // Experimental
    public BezierCurve curve;
    public Sprite laser_sprite;

    private float moveHorizontal;
    private float moveVertical;

    [System.NonSerialized]
    public bool can_move;        // TODO : create a status class
    [System.NonSerialized]
    public bool can_be_damaged;  // TODO : create a status class
    [System.NonSerialized]
    public bool moving;          // TODO : create a status class
    [System.NonSerialized]
    public bool bombing;         // TODO : create a status class
    [System.NonSerialized]
    public bool dead;            // TODO : create a status class

    static int skill_state = Animator.StringToHash("Skill");
    static int idle_state = Animator.StringToHash("Idle");

    [System.NonSerialized]
    public bool render;

    private OptionData option_data;

    [System.Serializable]
    public class OptionData {
        public List<Vector3> positions;
        public List<Bullet> options;
    }

    public override void Init() {
        // Set instance to this object
        if (instance == null) {
            instance = this;
        }

        // Enforce there can be only one instance
        else if (instance != this) {
            Destroy(gameObject);
        }

        base.Init();

		if (obj != null) {
			obj.Clamping = clamping;
			obj.Radius = hitbox_radius;
			obj.Scale = Vector3.one * hitbox_radius * 2;

			// Object is invisible and only used for collision
			obj.Color = Color.white; // Temporary visible as missing the sprites

			/* Init options lists */
			option_data = new OptionData();
			option_data.options = new List<Bullet>();
			option_data.positions = new List<Vector3>();

			if (animatorRenderer != null) {
				playerSprite = pool.AddBullet(animatorRenderer.sprite, EType.PLAYER, EMaterial.PLAYER, obj.Position);
				playerSprite.Scale = transform.lossyScale;
			}

			/* Init collection hitbox */
        
			if (power_level >= 1) { 
				SpawnOptions ();
			}

			// Init status
			can_be_damaged = true;
			can_move = true;
			moving = false;
			dead = false;
			render = true;
		}
    }

    public void UpdateAt() {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        // Update appearance (if driven by sprite)
		if(playerSprite != null) {
			pool.ChangeBulletAppearance(playerSprite, animatorRenderer.sprite, EMaterial.PLAYER);
			playerSprite.Position = obj.Position;
        }

        if (!dead) {
            ManageMovement();
			pool.QuadTreeHolder.CheckCollision(this);
            if (power_level >= 1) { // Wrong logic
                UpdateOptions();
            }
        }
    }

    void Destroy() {
        instance = null;
        Destroy(gameObject);
    }

    public void Die() {
        dead = true;
    }

	private void ManageMovement() {
        // If I can't move, no movement handling
        if(!can_move) {
            moving = false;
            return;
        }

        Vector3 move = new Vector3(moveHorizontal, moveVertical, 0);
        obj.Direction = move.normalized;
        transform.position = obj.Position;

        if(move != Vector3.zero) {
            moving = true;
            if (Input.GetKey("left shift")) {
                obj.Speed = focus_speed;
            } else {
				obj.Speed = unfocus_speed;
            }
        } else {
            moving = false;
            obj.Speed = 0;
        }

		if (animator != null) {
			UpdateAnimations();
		}
    }

	void UpdateAnimations() {
		if (Input.GetKey ("x") && bombing == false) {
			animator.SetBool ("IsGoingRight", false);
			animator.SetBool ("IsGoingLeft", false);           
		} else if (bombing == false) { 
			if (obj.Direction.x < 0) {
				animator.SetBool ("IsGoingLeft", true);
				animator.SetBool ("IsGoingRight", false);            
			} else if (obj.Direction.x > 0) {
				animator.SetBool ("IsGoingLeft", false);
				animator.SetBool ("IsGoingRight", true);
			} else {
				animator.SetBool ("IsGoingLeft", false);
				animator.SetBool ("IsGoingRight", false);
			}
		}
	}

    void SpawnOptions() {
        foreach(Vector3 position in default_option_data[power_level-1].positions) {
			Bullet option_bullet = pool.AddBullet(option, EType.OPTION, EMaterial.PLAYER,
                                                  obj.Position + position, 0, 0, 0, (position.x < 0) ? 1 : -1);
            option_bullet.Color = new Color32(255, 255, 255, 125);
            option_bullet.Scale = new Vector3(option_scale, option_scale);

            option_data.options.Add(option_bullet);
            option_data.positions.Add(position);
        }
    }

    void UpdateOptions() {
        if(obj != null) { 
            for (int i = 0; i < option_data.options.Count; i++) {
                option_data.options[i].Position = obj.Position + option_data.positions[i];
            }
        }
    }

	public IEnumerator _HitDisplay() {
		can_be_damaged = false;
		pool.UpdateGaugeLevel(-15.0f);
		pool.ChangeBulletColor(obj, Color.red);
		yield return new WaitForSeconds(secondsOfInvicibilityOnHit);
		pool.ChangeBulletColor(obj, Color.white);
		can_be_damaged = true;
	}
}
