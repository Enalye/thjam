using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class QuadTreeHolder : MonoBehaviour {
    public static QuadTree quadtree = null;         // Singleton
    public Rect startingBounds;                     // Use this for initialization

    private List<Bullet> bullets_buffer;
    private List<Bullet> bullets_close_player;

    public MeshPool bullet_pool; // to remove bullets
	public Player	player;

	private AudioManager audioManager;
	private Bomb 	     bomb;

    public bool trap;

    [ExecuteInEditMode]
    public void Init() {
        ResetQuadTree();
		bomb = player.bomb;
		audioManager = GameScheduler.instance.audioManager;
        bullets_close_player = new List<Bullet>();
        bullets_buffer = new List<Bullet>();
    }

    public void ResetQuadTree() {
        if(quadtree == null) {
            quadtree = new QuadTree(0, startingBounds, Color.red);
        } else {
            quadtree.Clear();
        }
    }
	
	// Clear and re-add bullets to the QuadTree (maybe logic needs to be sorted out of the quadtree to avoid complexity)
	public void ReferenceBullets(List<Bullet> pool_bullets) {
		ResetQuadTree();

		foreach (Bullet bullet in pool_bullets) {
			quadtree.Insert (bullet);
		}
	}

	public void CheckCollision(Player player) {
		bullets_close_player.Clear();
		if(player.obj != null && player.can_be_damaged && !player.debug_invincible) { // Optimisation
			bullets_close_player = quadtree.Get(player.grazeObj.AABB);

			// For each bullet close to the ennemy
			for(int i = 0; i < bullets_close_player.Count; ++i) {
				Bullet bullet = bullets_close_player[i];
				Vector3 playerPos = player.obj.Position;
				Vector3 bulletPos = bullet.Position;

				// Enemy collision => player loses a life
				if((bullet.Type == EType.NIGHTMARE ||
					bullet.Type == EType.ENEMY) && player.can_be_damaged) {

					if(CircleCollision(playerPos, bulletPos, player.grazebox_radius, bullet.Radius)) {
						if (bullet.Grazed == false) {
							StartCoroutine(player._GrazeDisplay());
							bullet.Grazed = true;
						}

						if(CircleCollision(playerPos, bulletPos, player.hitbox_radius, bullet.Radius)) {
							StartCoroutine(player._HitDisplay());
							bullets_close_player[i].MarkForDeletion();
						}
					}
				}

				if((bullets_close_player[i].Type == EType.DREAM && bullets_close_player[i].Removing == false) &&
					CircleCollision(playerPos, bulletPos, player.hitbox_radius, bullet.Radius)) {
					StartCoroutine(player._EatDisplay());
					bullets_close_player[i].MarkForDeletion();
				}
			}
		}
			
		bool playerInsideBomb = bomb.active && (CircleCollision (player.obj.Position, bomb.transform.localPosition, player.obj.Radius, bomb.radius));
		if (playerInsideBomb) {
			audioManager.SwitchMusicToSpiritVersion(0.75f);
		} else {
			audioManager.SwitchMusicToMainVersion(0.75f);
		}
	}

	public void CheckCollision(Enemy enemy) {
		if(enemy.obj != null && enemy.obj.Active && !enemy.obj.Removing && enemy.can_be_damaged) {
			// Get a list of bullets overlapping the current enemy bounding rect
			bullets_buffer = quadtree.Get(enemy.obj.AABB);

			// For each bullet in the list we just got
			for (int j = 0; j < bullets_buffer.Count; ++j) {
				if (bullets_buffer[j].Type == EType.SHOT) {
					bullets_buffer[j].MarkForDeletion();
					enemy.TakeDamage(bullets_buffer [j].Damage);
				}
			}
		}
	}

	// Circle collision
	public static bool CircleCollision(Vector3 pos1, Vector3 pos2, float rad1, float rad2) {
		float diff_x = pos1.x - pos2.x;
		float diff_y = pos1.y - pos2.y;

        float dist_sqrt = (diff_x * diff_x) + (diff_y * diff_y);
		float radius_sum = (rad1 + rad2);

        return dist_sqrt <= radius_sum * radius_sum;
    }

    // AABB (Axis Aligned Bounding Box) collision (non-oriented rectangles)
    public static bool AABBCollision(float r1_minX, float r1_maxX, float r1_minZ, float r1_maxZ,
                                     float r2_minX, float r2_maxX, float r2_minZ, float r2_maxZ) {
        // If the min of one box in one dimension is greater than the max of another box then the boxes are not intersecting
        return !(r1_minX > r2_maxX || r2_minX > r1_maxX || r1_minZ > r2_maxZ || r2_minZ > r1_maxZ);
    }

    // OBB (Oriented Bounding Box) collision (oriented rectangles)
    public static bool OBBCollision(OBB r1,
                                    OBB r2) {
        // Check AABB then do a SAT
        return AABBFromOBBCollision(r1, r2) && RectangleSATCollision(r1, r2);
    }

    // Find out if OBB rectangles are intersecting by using AABB
    private static bool AABBFromOBBCollision(OBB r1,
                                             OBB r2) {
        // Find the min/max values for the AABB algorithm
        float r1_minX = Mathf.Min(r1.FL.x, Mathf.Min(r1.FR.x, Mathf.Min(r1.BL.x, r1.BR.x)));
        float r1_maxX = Mathf.Max(r1.FL.x, Mathf.Max(r1.FR.x, Mathf.Max(r1.BL.x, r1.BR.x)));
        float r2_minX = Mathf.Min(r2.FL.x, Mathf.Min(r2.FR.x, Mathf.Min(r2.BL.x, r2.BR.x)));
        float r2_maxX = Mathf.Max(r2.FL.x, Mathf.Max(r2.FR.x, Mathf.Max(r2.BL.x, r2.BR.x)));

        float r1_minZ = Mathf.Min(r1.FL.y, Mathf.Min(r1.FR.y, Mathf.Min(r1.BL.y, r1.BR.y)));
        float r1_maxZ = Mathf.Max(r1.FL.y, Mathf.Max(r1.FR.y, Mathf.Max(r1.BL.y, r1.BR.y)));
        float r2_minZ = Mathf.Min(r2.FL.y, Mathf.Min(r2.FR.y, Mathf.Min(r2.BL.y, r2.BR.y)));
        float r2_maxZ = Mathf.Max(r2.FL.y, Mathf.Max(r2.FR.y, Mathf.Max(r2.BL.y, r2.BR.y)));

        return AABBCollision(r1_minX, r1_maxX, r1_minZ, r1_maxZ, r2_minX, r2_maxX, r2_minZ, r2_maxZ);
    }

    // Use SAT (separating axis theorem) to test if to OBB rectangles collide
    public static bool RectangleSATCollision(OBB r1, OBB r2) {
        // Get the normal of r1 left face and check 
        Vector3 normal1 = GetNormal(r1.BL, r1.FL);
        if(!Overlapping(normal1, r1, r2)) {
            return false;
        }

        // Get the normal of r1 forward face and check
        Vector3 normal2 = GetNormal(r1.FL, r1.FR);
        if(!Overlapping(normal2, r1, r2)) {
            return false;
        }

        // Get the normal of r2 left face and check 
        Vector3 normal3 = GetNormal(r2.BL, r2.FL);
        if(!Overlapping(normal3, r1, r2)) {
            return false;
        }

        // Get the normal of r2 forward face and check 
        Vector3 normal4 = GetNormal(r2.FL, r2.FR);
        if (!Overlapping(normal4, r1, r2)) {
            return false;
        }

        // If we get past all the tests, then there is collision
        return true;
    }

    // Get the normal of a face from 2 points
    private static Vector3 GetNormal(Vector3 startPos, Vector3 endPos) {
        // Direction (sense is arbitrary)
        Vector3 dir = endPos - startPos;

        // Normal, just flip x and y and make one negative (don't need to normalize it)
        Vector3 normal = new Vector3(-dir.y, dir.x, dir.z);

        // Draw the normal from the center of the rectangle's side (TODO : add trap)
        Debug.DrawRay(startPos + (dir * 0.5f), normal.normalized * 2f, Color.red);

        return normal;
    }

    // Is this side overlapping ?
    private static bool Overlapping(Vector3 normal, OBB r1, OBB r2) {
        // Project the corners of rectangle 1 onto the normal
        float dot1 = DotProduct(normal, r1.FL);
        float dot2 = DotProduct(normal, r1.FR);
        float dot3 = DotProduct(normal, r1.BL);
        float dot4 = DotProduct(normal, r1.BR);

        // Find the range
        float min1 = Mathf.Min(dot1, Mathf.Min(dot2, Mathf.Min(dot3, dot4)));
        float max1 = Mathf.Max(dot1, Mathf.Max(dot2, Mathf.Max(dot3, dot4)));

        // Project the corners of rectangle 2 onto the normal
        float dot5 = DotProduct(normal, r2.FL);
        float dot6 = DotProduct(normal, r2.FR);
        float dot7 = DotProduct(normal, r2.BL);
        float dot8 = DotProduct(normal, r2.BR);

        // Find the range
        float min2 = Mathf.Min(dot5, Mathf.Min(dot6, Mathf.Min(dot7, dot8)));
        float max2 = Mathf.Max(dot5, Mathf.Max(dot6, Mathf.Max(dot7, dot8)));

        // Are the ranges overlapping ?
        return (min1 <= max2 && min2 <= max1);
    }

    // Get the dot product
    private static float DotProduct(Vector3 v1, Vector3 v2) {
        return v1.x * v2.x + v1.y * v2.y;
    }

    // Project a 2D polygon (unused)
    public void ProjectPolygon(Vector2 axis, Vector2[] polygon,
                               ref float min, ref float max)
    {
        // To project a point on an axis use the dot product
        float dotProduct = Vector2.Dot(axis, polygon[0]);
        min = dotProduct;
        max = dotProduct;

        for (int i = 0; i < polygon.Length; i++) {
            dotProduct = Vector2.Dot(polygon[i], axis);
            if (dotProduct < min) {
                min = dotProduct;
            } else if (dotProduct > max) {
                max = dotProduct;
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startingBounds.center, startingBounds.size);

        List<Bullet> bullets = bullet_pool.GetBullets();

        for (int i = 0; i < bullets.Count; i++) {
			if (bullets [i].Type == EType.ENEMY || bullets [i].Type == EType.NIGHTMARE || bullets [i].Type == EType.DREAM || bullets [i].Type == EType.SHOT ) {
				Gizmos.color = Color.yellow;
				OBB OBB = bullets [i].OBB;
				Gizmos.DrawLine (OBB.FL, OBB.FR);
				Gizmos.DrawLine (OBB.FR, OBB.BR);
				Gizmos.DrawLine (OBB.BR, OBB.BL);
				Gizmos.DrawLine (OBB.BL, OBB.FL);

				if (bullets [i].AABB != Rect.zero) {
					Gizmos.color = Color.green;
					Gizmos.DrawWireCube (bullets [i].AABB.center, bullets [i].AABB.size);

					Gizmos.color = Color.red;
					if (bullets [i].Radius != 0) {
						Gizmos.DrawWireSphere (bullets [i].AABB.center, bullets [i].Radius);
					}
				}
			}
        }

		if (player.obj != null) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (player.obj.Position, player.hitbox_radius);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere (player.obj.Position, player.grazebox_radius);
		}
    }
}
