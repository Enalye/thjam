﻿using UnityEngine;
using System.Collections;

public enum EOwner { ENEMY, PLAYER, GROUP, NONE };
public enum EType { DEFAULT, PLAYER, OPTION, SHOT, ENEMY, NIGHTMARE, DREAM, BOMB, ITEM, EFFECT, CUSTOM };
public enum EStyle { SHOT, LASER, CURVE, TEXT };

// Add as many items as materials / fonts (in rendering order)
public enum EMaterial { BULLET, PLAYER, ENEMY, GUI, COUNT, NEUTRAL };

[CreateAssetMenu(fileName = "Bullet", menuName = "Sprites/Bullet")]
public partial class Bullet : ScriptableObject
{
    // Index in the queue
    public int Index { get; set; }

    // Lifetime (optional !)
    public float? Lifetime { get; set; }

    // Clamping (optional !)
    public Bounds? Clamping { get; set; }

    // Type and owner
    public EType Type;
    public EOwner Owner;

    // Material ID used for rendering
    public EMaterial Material;

    // Is it an active bullet (for coroutines)
    public bool Active = false;
	public bool Removing = false;

	// Was the bullet grazed ?
	public bool Grazed = false;

    // Standard Position, Rotation, Scale
    public Vector3 Position;
    public Vector3 PreviousPosition;
	public Vector3? BoundPosition;
    public Vector3 Scale = new Vector3(1, 1);
    public Quaternion Rotation { get; set; }

    // Width and Height of dipslayed quad
    public float Width {
        get { return Vector3.Distance(OBB.BL, OBB.BR); }
    }

    public float Height {
        get { return Vector3.Distance(OBB.BL, OBB.FL); }
    }

    // Cartesian Information
    private Vector3 _direction;
    public Vector3 Direction {
        get { return _direction; }
        set
        {
            _direction = value;
            float rad_angle = Mathf.Atan2(_direction.y, _direction.x);
            _angle = rad_angle * Mathf.Rad2Deg;
        }
    }

    // Polar information
    [SerializeField]
    private float _angle;
    public float Angle {
        get { return _angle; }
        set
        {
            _angle = value;
            float rad_angle = _angle * Mathf.Deg2Rad;
            _direction = new Vector3(Mathf.Cos(rad_angle), Mathf.Sin(rad_angle));
        }
    }

    // Sprite angle
    public Vector3 SpriteAngle;
	public Vector3 SpriteAngularVelocity;

    public float Speed;
	public float? MinSpeed;
	public float? MaxSpeed;
    public float Acceleration;
    public float AngularVelocity;
	public Vector3 Force;

    // Vertices and UV information
    public Sprite Sprite; // Usualy Bounds, UVs are set from the sprite ref
    public Bounds Bounds { get; set; }
    public Rect UVs { get; set; }

    // Coordinate rect
    public Rect AABB { get; set; }
    public OBB  OBB { get; set; }    

    [SerializeField]
    private Color32 _color = new Color32(255, 255, 255, 255);
    public Color32 Color {
        get { return _color; }
        set { _color = value; }
    }

    // Radius of the hitbox
    public float Radius;

    // Delay before appearing
    public float Delay;

	// Damage / Heal value
    public float Damage;

    // Deletion conditions
    public bool SpellResist;
    public bool AutoDelete;

	// Status
	public bool Magnetized;

    // Usually not to be modified by hand
    public float CurrentTime { get; set; }

	// Reset all variables but index
	public void Init() {
		Lifetime = null;
		Clamping = null;

		Rotation = Quaternion.identity;
		Scale = Vector3.one;

		BoundPosition = null;

		Speed = 0;
		MinSpeed = null;
		MaxSpeed = null;

		Direction = Vector3.zero;
		Angle = 0;
		Acceleration = 0;
		AngularVelocity = 0;
		Force = Vector3.zero;
		Radius = 0;
		Delay = 0;
		Damage = 0;
		CurrentTime = 0;

		Sprite = null;

		Grazed = false;
		Active = true;
		Removing = false;
		SpellResist = false;
		AutoDelete = true;
		Magnetized = false;

		AABB = Rect.zero;
	}

	public void CopyData(float ? speed, float ? angle, float ? acc, float ? ang_vec) {
		if (speed.HasValue) { Speed = speed.Value; }
		if (angle.HasValue) { Angle = angle.Value; }
		if (acc.HasValue) { Acceleration = acc.Value; }
		if (ang_vec.HasValue) { AngularVelocity = ang_vec.Value; }
	}

	public void CopyData(Sprite sprite, EType type, EMaterial material, Color32 color, Vector3 position, Vector3 scale,
                         float speed = 0, float angle = 0, float acc = 0, float ang_vec = 0) {
        Position = position;
        PreviousPosition = position;
		Scale = scale;
        Direction = new Vector3(0, 0);
        Sprite = sprite;
        Type = type;
        Material = material;
		Color = color;
        Speed = speed;
        Angle = angle;
        Acceleration = acc;
        AngularVelocity = ang_vec;

        if(sprite != null) { 
            UVs = sprite.rect;
            Bounds = sprite.bounds;
        }
    }

    public virtual void SetupTriangles(int[] _indices) {
        int xdx = Index * 6;
        int ydx = Index * 4;

        _indices[xdx] = ydx + 0;
        _indices[xdx + 1] = ydx + 2;
        _indices[xdx + 2] = ydx + 1;
        _indices[xdx + 3] = ydx + 3;
        _indices[xdx + 4] = ydx + 2;
        _indices[xdx + 5] = ydx + 0;
    }

    public virtual void SetupUVs(Vector2[] _uvs, Texture tex) {
        int offset = Index * 4;
        Texture texture = tex;

        float width = texture.width;
        float height = texture.height;

        float min_x = UVs.xMin;
        float min_y = UVs.yMin;
        float max_x = UVs.xMax;
        float max_y = UVs.yMax;

        _uvs[offset] = new Vector2(min_x / width, min_y / height);
        _uvs[offset + 1] = new Vector2(max_x / width, min_y / height);
        _uvs[offset + 2] = new Vector2(max_x / width, max_y / height);
        _uvs[offset + 3] = new Vector2(min_x / width, max_y / height);
    }

    // This function updates the internal bullet data
    protected void UpdateState(float dt = 0) {
        CurrentTime += dt;
        PreviousPosition = Position;

		/* No update if CurrentTime lower than delay to fire */
		if(CurrentTime < Delay) {
			return;
		}
			
		Speed = Speed + Acceleration;

		/* If needed tune speed value */
		if (MinSpeed.HasValue) {
			Speed = Mathf.Max(MinSpeed.Value, Speed);
		} 

		if (MaxSpeed.HasValue) {
			Speed = Mathf.Min (MaxSpeed.Value, Speed);
		}
			
		if (BoundPosition.HasValue) {
			Position = BoundPosition.Value;
		} else {
			Position += dt * Speed * Direction + dt * Force;
		}

        Angle += AngularVelocity;
		SpriteAngle += SpriteAngularVelocity;
    }

    // This is used for physics
    public virtual void ComputePosition(Vector3[] vertices, Color32[] colors, float dt = 0) {
        UpdateState(dt);
        SetupVertices(vertices, colors);     
    }

    // This is the real function setting up vertices
	public virtual void SetupVertices(Vector3[] vertices, Color32[] colors) {
        int offset = Index * 4;
        Vector3 bullet_ext = Vector3.Scale(Bounds.extents, Scale);

        // Clamp
        if (Clamping != null) {
            Vector3 clamping_ext = Clamping.Value.extents;
            Vector3 clamping_pos = Clamping.Value.center;
            Vector3 max_boundary = new Vector3(clamping_pos.x + clamping_ext.x - bullet_ext.x, clamping_pos.y - clamping_ext.y - bullet_ext.y);
            Vector3 min_boundary = new Vector3(clamping_pos.x - clamping_ext.x + bullet_ext.x - 0.5f, clamping_pos.y + clamping_ext.y + bullet_ext.y);
            Position = Maths.Clamping(Position, min_boundary, max_boundary);
        }

        float min_x = Position.x - bullet_ext.x;
        float min_y = Position.y - bullet_ext.y;
        float max_x = Position.x + bullet_ext.x;
        float max_y = Position.y + bullet_ext.y;

		SetupVertices(offset, min_x, min_y, max_x, max_y, vertices, colors);

        // Update colliders box
		float rect_min_x = Mathf.Min(vertices[offset].x, vertices[offset + 1].x, vertices[offset + 2].x, vertices[offset + 3].x);
		float rect_min_y = Mathf.Min(vertices[offset].y, vertices[offset + 1].y, vertices[offset + 2].y, vertices[offset + 3].y);
		float rect_max_x = Mathf.Max(vertices[offset].x, vertices[offset + 1].x, vertices[offset + 2].x, vertices[offset + 3].x);
		float rect_max_y = Mathf.Max(vertices[offset].y, vertices[offset + 1].y, vertices[offset + 2].y, vertices[offset + 3].y);
        AABB = Rect.MinMaxRect(rect_min_x, rect_min_y, rect_max_x, rect_max_y);
		OBB = new OBB(vertices[offset + 3], vertices[offset + 2], vertices[offset], vertices[offset + 1]);
    }

	public void SetupVertices(int offset, float min_x, float min_y, float max_x, float max_y, Vector3[] vertices, Color32[] colors) {
		vertices[offset] = Maths.Rotate(new Vector3(min_x, min_y, Position.z), Position, SpriteAngle);          // Low left
		vertices[offset + 1] = Maths.Rotate(new Vector3(max_x, min_y, Position.z), Position, SpriteAngle);      // Up left
		vertices[offset + 2] = Maths.Rotate(new Vector3(max_x, max_y, Position.z), Position, SpriteAngle);      // Up right
		vertices[offset + 3] = Maths.Rotate(new Vector3(min_x, max_y, Position.z), Position, SpriteAngle);      // Low right

		colors[offset] = colors[offset + 1] = colors[offset + 2] = colors[offset + 3] = Color;
	}

	public void MarkForDeletion() {
		Lifetime = 0;
	}

	public void MarkInvincible() {
		Lifetime = null;
		AutoDelete = false;
	}

	private Vector3 ComputeForce(float forceAngle, float forceSpeed) {
		float radAng = Mathf.Deg2Rad * forceAngle;
		float cos = Mathf.Cos(radAng);
		float sin = Mathf.Sin(radAng);
		return new Vector3(forceSpeed * cos, forceSpeed * sin);
	}

	private void SetForce(float forceAngle, float forceSpeed) {
		BoundPosition = null;
		Force = ComputeForce(forceAngle, forceSpeed);
	}

	private void SetForce(Vector3 force) {
		BoundPosition = null;
		Force = force;
	}

	public IEnumerator _Follow(BezierCurve curve) {
        float elapsedTime = 0;
        while (elapsedTime < 1) {
			BoundPosition = curve.GetPoint (elapsedTime);
			Direction = curve.GetDirection (elapsedTime);
			elapsedTime += GameScheduler.dt * curve.GetSpeed (elapsedTime);

            yield return new WaitForSeconds(GameScheduler.dt);
        }

		// Turn on AutoDelete once moved to the end
		AutoDelete = true;
    }

	public IEnumerator _Magnetize() {
		Magnetized = true;
		float steeringSpeed = 0;

		while(Active && !Removing) {
			Vector3 playerPos = Player.instance.obj.Position;
			float angle = Mathf.Atan2(playerPos.y - Position.y, playerPos.x - Position.x) * Mathf.Rad2Deg;
			SpriteAngle = Vector3.forward * angle;

			Vector3 desiredDirection = Vector3.Normalize(playerPos - Position) * steeringSpeed;
			Vector3 steering = desiredDirection - Direction; 
			SetForce(steering);

			steeringSpeed += 4f;
			yield return new WaitForSeconds(GameScheduler.dt);
		}
	}

	public IEnumerator _RotateAround(Bullet other, float angle) {
		yield return null; // Exit the first time since other not set yet

		while (Active && other.Active) {
			Speed = other.Speed;

			float radAng = Mathf.Deg2Rad * angle;
			float cos = Mathf.Cos (radAng);
			float sin = Mathf.Sin (radAng);

			Position.x -= other.Position.x;
			Position.y -= other.Position.y;

			float xnew = Position.x * cos - Position.y * sin;
			float ynew = Position.x * sin + Position.y * cos;

			Position.x = xnew + other.Position.x;
			Position.y = ynew + other.Position.y;

			yield return new WaitForSeconds(GameScheduler.dt);
		}

		// Ask for deletion of this bullet
		Lifetime = 0;
	}

	public IEnumerator _Appear(float time) {
		Vector3 originalScale = Scale;
		Vector3 biggerScale = originalScale * 2.25f;
		Color = Colors.ChangeAlpha(Color, 0);
		Scale = biggerScale;

		float elapsedTime = 0;
		while (elapsedTime < time) {
			float timeRatio = elapsedTime / time;

			byte alpha = (byte)Mathf.Min(255, 255 * timeRatio);
			Color = Colors.ChangeAlpha(Color, alpha);
			Scale = Vector3.Lerp(biggerScale, originalScale, timeRatio);
			elapsedTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}
	}

	public IEnumerator _Disappear(float time) {
		Vector3 originalScale = Scale;
		Vector3 biggerScale = originalScale * 2.25f;
		byte oriAlpha = Color.a;

		float elapsedTime = 0;
		while (elapsedTime < time) {
			float timeRatio = elapsedTime / time;

			byte alpha = (byte)Mathf.Max(0, oriAlpha * (1 - timeRatio));
			Scale = Vector3.Lerp(originalScale, biggerScale, timeRatio);
			Color = Colors.ChangeAlpha(Color, alpha);
			elapsedTime += GameScheduler.dt;
			yield return new WaitForSeconds(GameScheduler.dt);
		}

		Color = Colors.ChangeAlpha(Color, 0);
	}

	public IEnumerator _Change(float timeToWait, Sprite sprite, Color color, EType type, float ? speed, float ? angle, float ? acc = 0, float ? ang_vec = 0) {
		yield return new WaitForSeconds(timeToWait);

		if (Active) {
			CopyData (speed, angle, acc, ang_vec);
			Type = type;
			Color = color;

			if (sprite != null) {
				Sprite = sprite;
			}
		}
	}
}