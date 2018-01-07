using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Entity {
    public static Player instance = null;     // Singleton
	public float hitbox_radius = 0.15f;
	public float grazebox_radius = 0.3f;

	public Sprite shot_sprite;
	public Sprite option_sprite;

    public Animator animator;
	public SpriteRenderer animatorRenderer;

    public AudioClip death_hit; // TODO : create an audio handler class
    public AudioClip death;     // TODO : create an audio handler class
    public AudioClip game_over; // TODO : create an audio handler class

    public Bounds clamping;

	public Bullet playerSprite;
	public Bullet grazeObj;
    public GameObject bomb;

	public float option_distance;
    public float focus_speed;
    public float unfocus_speed;
	public float collectHitboxRadius;
	public float secondsOfInvicibilityOnHit;

    public int power_level;	

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

    private List<OptionData> options;

    [System.Serializable]
    public class OptionData {
        public Vector3 position;
        public Bullet bullet;

		public OptionData(Bullet nBullet) {
			bullet = nBullet;

			bullet.Color = Color.blue;
			bullet.Radius = .4f;
			bullet.Scale = Vector3.one * bullet.Radius;
			bullet.AutoDelete = false;
		}
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
			// Obj represents hitbox here */
			obj.Clamping = clamping;
			obj.Color = new Color32(255, 255, 255, 155);
			obj.Scale = Vector3.one * 0.5f;
			obj.Radius = hitbox_radius;
			obj.Position.z = Layering.Hitbox;

			/* Init options lists */
			CreateOptions();

			if (animatorRenderer != null) {
				playerSprite = pool.AddBullet(animatorRenderer.sprite, EType.PLAYER, EMaterial.PLAYER, Color.white, obj.Position);
				playerSprite.Scale = transform.lossyScale;
			}

			grazeObj = pool.AddBullet(sprite, EType.EFFECT, EMaterial.PLAYER, Colors.invisible, obj.Position);
			grazeObj.Scale = Vector3.one * 1.2f;
			grazeObj.Radius = grazebox_radius;
			grazeObj.BoundPosition = obj;

			/* Init collection hitbox */

			// Init status
			can_be_damaged = true;
			can_move = true;
			moving = false;
			dead = false;
			render = true;

			StartCoroutine(SpreadShot());
			StartCoroutine(OptionsShot());
		}
    }

	public void UpdateAt(float deltaTime) {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        // Update appearance (if driven by sprite)
		if(playerSprite != null) {
			pool.ChangeBulletAppearance(playerSprite, animatorRenderer.sprite, EMaterial.PLAYER);
			playerSprite.Position = new Vector3(obj.Position.x - 5, obj.Position.y, 10);
        }

        if (!dead) {
            ManageMovement();
			pool.QuadTreeHolder.CheckCollision(this);
			UpdateOptions(deltaTime);
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

        if(move != Vector3.zero) {
            moving = true;
            if (Input.GetButton("Focus")) {
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

	public void CreateOptions() {
		options = new List<OptionData> ();
	
		options.Add(new OptionData(pool.AddBullet(option_sprite, EType.OPTION, EMaterial.PLAYER, Color.white, obj.Position)));
		options.Add(new OptionData(pool.AddBullet(option_sprite, EType.OPTION, EMaterial.PLAYER, Color.white, obj.Position)));
		options.Add(new OptionData(pool.AddBullet(option_sprite, EType.OPTION, EMaterial.PLAYER, Color.white, obj.Position)));
		options.Add(new OptionData(pool.AddBullet(option_sprite, EType.OPTION, EMaterial.PLAYER, Color.white, obj.Position)));
	}

	private float optionAngleOffset = 0f;
	public void UpdateOptions(float deltaTime) {
		int newPowerLevel = (int)((pool._gauge.level * 4f) / 100f);
		if (newPowerLevel > power_level) {
			power_level = newPowerLevel;
		} else if (newPowerLevel < power_level) {
			power_level = newPowerLevel;
		}

		if (Input.GetButton("Focus")) {
			for (int i = 0; i < options.Count; i++) {
				if (i >= power_level) {
					options[i].position = obj.Position;
					options[i].bullet.Color = new Color(1f, 1f, 1f, 0f);
				} else {
					float optionAngle = ((-90f + (power_level - 1) * -10f) + (i * 20f)) * Mathf.Deg2Rad;
					Vector3 pos = obj.Position + new Vector3(Mathf.Cos(optionAngle) * option_distance, Mathf.Sin(optionAngle) * option_distance, 0f);
					options[i].position = Vector3.Lerp(options[i].position, pos, deltaTime * .25f);
					options[i].bullet.SpriteAngle += new Vector3(0f, 0f, deltaTime * 2f);
					options[i].bullet.Color = new Color(1f, 1f, 1f, 1f);
				}
				options[i].bullet.Position = options[i].position;
				options[i].bullet.Scale = Vector3.one * 0.75f;
			}
		} else {
			for (int i = 0; i < options.Count; i++) {
				if (i >= power_level) {
					options[i].position = obj.Position;
					options[i].bullet.Color = new Color(1f, 1f, 1f, 0f);
				} else {
					float optionAngle = ((i * 360f + optionAngleOffset) / power_level) * Mathf.Deg2Rad;
					Vector3 pos = obj.Position + new Vector3(Mathf.Cos(optionAngle) * option_distance, Mathf.Sin(optionAngle) * option_distance, 0f) * 1.5f;
					options[i].position = Vector3.Lerp(options[i].position, pos, deltaTime * .25f);
					options[i].bullet.SpriteAngle += new Vector3(0f, 0f, deltaTime * 2f);
					options[i].bullet.Color = new Color(1f, 1f, 1f, 1f);
					optionAngleOffset += deltaTime * 5f;
				}
				options[i].bullet.Position = options[i].position;
				options[i].bullet.Scale = Vector3.one * 0.75f;
			}
		}
	}

	public IEnumerator OptionsShot() {
		while(Application.isPlaying) {
			if(dead)
				continue;
			
			if (Input.GetButton("Focus")) {
				if (Input.GetButton("Shot1")) {
					for (int i = 0; i < power_level; i++) {
						for (int y = 0; y < 4; y++) {
							Bullet shot = pool.AddBullet(shot_sprite, EType.SHOT, EMaterial.PLAYER,
														 Colors.firebrick,
														 options[i].position,
														 1000f,
														 (90f - 10f) + (y * 20f / 4f),
														 0f, 0f);

							shot.Radius = 0.25f;
							shot.Scale = Vector3.one * shot.Radius;
							shot.SpriteAngle = new Vector3(0f, 0f, shot.Angle + 90);
							shot.Lifetime = 1f;
							shot.AutoDelete = true;
							shot.Damage = 0.2f;
							bullets.Add(shot);
						}
					}
				}
				yield return new WaitForSeconds(.25f);
			} else if(Input.GetButton ("Shot1")) {
				for (int i = 0; i < power_level; i++) {
					for (int y = 0; y < 3; y++) {
						Bullet shot = pool.AddBullet(shot_sprite, EType.SHOT, EMaterial.PLAYER,
													 Color.green,
										             options[i].position,
										             1000f,
										             (90f - 20f) + (y * 40f / 3f),
										             0f, 0f);

						shot.Radius = 0.25f;
						shot.Scale = Vector3.one * shot.Radius;
						shot.SpriteAngle = new Vector3(0f, 0f, shot.Angle + 90);
						shot.Lifetime = 1f;
						shot.AutoDelete = true;
						shot.Damage = 0.1f;
						bullets.Add(shot);
					}
				}
				yield return new WaitForSeconds(.4f);
			}
			else yield return new WaitForSeconds(.2f);
		}
	}

	public IEnumerator SpreadShot() {
		while (Application.isPlaying) {
			if (dead)
				continue;

			if(Input.GetButton("Focus")) {
				if (Input.GetButton ("Shot1")) {
					//Focus fire.
					for (int i = 0; i < 3; i++) {
						Bullet shot = pool.AddBullet (shot_sprite, EType.SHOT, EMaterial.PLAYER,
													  Color.white,
										              obj.Position,
										              1500f,
										              (90f - 10f) + (i * 20f / 3f),
										              .2f, 0f);

						shot.Radius = 0.2f;
						shot.Scale = Vector3.one * shot.Radius * 2f;
						shot.SpriteAngle = new Vector3 (0f, 0f, shot.Angle + 90);
						shot.Lifetime = 1f;
						shot.AutoDelete = true;
						shot.Damage = 0.3f;
						bullets.Add (shot);
					}
				}
				yield return new WaitForSeconds (.1f);
			}
			else if (Input.GetButton ("Shot1")) {
				//Unfocus fire.
				for (int i = 0; i < 6; i++) {
					Bullet shot = pool.AddBullet (shot_sprite, EType.SHOT, EMaterial.PLAYER,
												  Color.white,
												  obj.Position,
												  1000f,
												  (90f - 25f) + (i * 50f / 6f),
												  .1f, 0f);
					
					shot.Radius = 0.2f;
					shot.Scale = Vector3.one * shot.Radius * 2f;
					shot.SpriteAngle = new Vector3 (0f, 0f, shot.Angle + 90);
					shot.Lifetime = 1.5f;
					shot.AutoDelete = true;
					shot.Damage = 0.3f;
					bullets.Add (shot);
				}
				yield return new WaitForSeconds (.2f);
			}
			else
				yield return new WaitForSeconds (.3f);
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

	public IEnumerator _EatDisplay() {
		pool.UpdateGaugeLevel(5.0f);
		pool.ChangeBulletColor(obj, Color.green);
		yield return new WaitForSeconds(0.1f);
		pool.ChangeBulletColor(obj, Color.white);
	}

	public IEnumerator _GrazeDisplay() {
		pool.UpdateGaugeLevel(5.0f);
		pool.ChangeBulletColor(obj, Color.yellow);
		yield return new WaitForSeconds(0.1f);
		pool.ChangeBulletColor(obj, Color.white);
	}
}	