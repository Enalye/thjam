using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class to be used for any GameObject holding a MeshPool and a sprite
[ExecuteInEditMode]
public class Entity : MonoBehaviour {
    public MeshPool     pool;          // Mesh pool representing the entity data
    public Sprite       sprite;        // Sprite to animate
	public EType		type;		   // Type of the object 
	public EMaterial    material;      // Material to use
    public List<Bullet> bullets;       // List of objects fired by the entity

    [System.NonSerialized]
    public Bullet obj;                 // Bullet object to manipulate

	protected float radius;

    [ExecuteInEditMode]
    public virtual void Init() {
		if(gameObject.activeInHierarchy && sprite != null) {
            bullets = new List<Bullet>();
			obj = pool.AddBullet(sprite, type, material, Color.white, transform.position);
			obj.AutoDelete = false;
        }
    }

    public IEnumerator _Wait(float n) {
        yield return new WaitForSeconds(n);
    }


	public IEnumerator _BindObject() {
		while (obj.Active) {
			obj.Radius = radius;
			obj.Scale = Vector3.one * radius * 2;
			obj.Position = transform.localPosition;
			yield return new WaitForSeconds(GameScheduler.dt);
		}
	}
}
