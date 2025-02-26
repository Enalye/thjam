﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class MeshPool : MonoBehaviour {
    public int BulletCount;
    public MeshRenderer[] MeshRenderers;
    public MeshFilter[] MeshFilters;

    // Collision stuff
    public QuadTreeHolder QuadTreeHolder;

    public List<Bullet> _active = new List<Bullet>();
    private List<Bullet> _temp = new List<Bullet>();

    // Number of bullets to pre-allocate
    public int MaxBullets = 5000;
	private int MaxItems;

    // Mesh and bullets pre-allocated
    private Mesh[] _meshs;
    private Bullet[] _bullets;

	// Dream gauge
	public DreamGauge _gauge;

	// GUI elements
	public  Sprite  _border_sprite;
	private Bullet  _border;

    // Vertices data
    private int[][] _indices;
    private Vector3[][] _vertices;
    private Vector3[][] _normals;
    private Vector2[][] _uvs;
    private Color32[][] _colors;

    private Material[] _materials;

    // Queue to reference available bullets
    private Queue<int> _available;

	public bool _debug;

    public void Init() {
        _active.Clear();

		MaxItems = MaxBullets;
		_bullets = new Bullet[MaxItems];
        for(int i = 0; i < MaxBullets; i++) {
            _bullets[i] = ScriptableObject.CreateInstance("Bullet") as Bullet;
        }

        // Dirty init stuff here
		_available = new Queue<int>(Enumerable.Range(0, MaxBullets));
		if (_debug) {
			StartCoroutine (DisplayAvailableSize ());
		}

        int NbMaterials = (int)EMaterial.COUNT;

        // Vertices per material / mesh
        _vertices = new Vector3[NbMaterials][];

        // Indices per material / mesh
        _indices = new int[NbMaterials][];

        // Normals per material / mesh
        _normals = new Vector3[NbMaterials][];

        // UVs per material / mesh
        _uvs = new Vector2[NbMaterials][];

        // Colors per material / mesh
        _colors = new Color32[NbMaterials][];

        // Material array aligned with EMaterial enum
		_materials = new Material[NbMaterials];
			
        //_mesh = new Mesh {subMeshCount = (int)EMaterial.COUNT, vertices = _vertices, normals = _normals, uv = _uvs, colors32 = _colors};
        _meshs = new Mesh[NbMaterials];

        // Allocating for each material @TODO maybe set MaxBullets to less for some meshs. Or use sprites for them !
        for (int i = 0; i < NbMaterials; ++i) {
			_vertices[i] = new Vector3[MaxItems * 4];
			_indices[i] = new int[MaxItems * 6];
			_normals[i] = Enumerable.Repeat(Vector3.back, MaxItems * 4).ToArray();
			_uvs[i] = new Vector2[MaxItems * 4];
			_colors[i] = Enumerable.Repeat((Color32)Color.white, MaxItems * 4).ToArray();
            _meshs[i] = new Mesh { vertices = _vertices[i], normals = _normals[i], uv = _uvs[i], colors32 = _colors[i] };
			_materials[i] = MeshRenderers[i].sharedMaterial;
			MeshRenderers [i].sortingLayerName = "Default";
        }

		// GUI elements
		_border = AddBullet(_border_sprite, EType.EFFECT, EMaterial.GUI, Color.white);
		_border.Position = new Vector3(0, 0, Layering.Background);
		_border.Scale = new Vector3(1.55f, 1.6f, 1);
    }

    public List<Bullet> GetBullets() {
        return _active;
    }

    public Bullet PullBullet(EType type, EMaterial material) {
        if (_available.Count == 0) {
            Debug.LogWarning("No available quads, failed to add bullet");
            return null;
        }

        int index = _available.Dequeue();
        Bullet bullet = _bullets[index];
		bullet.Init();

        // Get an available Index and set Time to 0
        bullet.Index = index;
        bullet.CurrentTime = 0;

        bullet.Type = type;
        bullet.Material = material;

        return bullet;
    }

    public void SetupBullet(Bullet bullet) {
        int MaterialIdx = (int)bullet.Material;

        // Update bullet appearance (UVs)
        bullet.SetupUVs(_uvs[MaterialIdx], _materials[MaterialIdx].mainTexture);

        // Setup triangles according to render mode
        bullet.SetupTriangles(_indices[MaterialIdx]);

        // Update bullet once (vertices)
        bullet.SetupVertices(_vertices[MaterialIdx], _colors[MaterialIdx]);
    }

	public Bullet AddBullet(Sprite sprite, EType type, EMaterial material, Color32 color, Vector3 position, Vector3 scale,
							float speed = 0, float angle = 0, float acc = 0, float ang_vec = 0) {
		Bullet bullet = PullBullet(type, material);
		bullet.CopyData(sprite, type, material, color, position, scale, speed, angle, acc, ang_vec);

		if (type == EType.NIGHTMARE || type == EType.DREAM) {
			//bullet.Position.z = Layering.Bullet;
			StartCoroutine(bullet._Appear(0.5f));
		}

		// Update some of the bullet data
		if(sprite != null) { 
			bullet.UVs = sprite.rect;
			bullet.Bounds = sprite.bounds;
		}

		SetupBullet(bullet);

		// Add the bullet to active list
		bullet.Active = true;
		_active.Add(bullet);

		return bullet;
	}

	public Bullet AddBullet(Sprite sprite, EType type, EMaterial material, Color32 color, Vector3 position,
							float speed = 0, float angle = 0, float acc = 0, float ang_vec = 0) {
		return AddBullet (sprite, type, material, color, position, Vector3.one, speed, angle, acc, ang_vec);
	}

	public Bullet AddBullet(Sprite sprite, EType type, EMaterial material, Color32 color) {
		return AddBullet (sprite, type, material, color, Vector3.zero);
    }
		
    public void CleanBullet(Bullet bullet) {
        // We only clean indice data because laziness / optimisation
        int xdx = bullet.Index * 6;
        int matidx = (int)bullet.Material;

        _indices[matidx][xdx] = 0;
        _indices[matidx][xdx + 1] = 0;
        _indices[matidx][xdx + 2] = 0;
        _indices[matidx][xdx + 3] = 0;
        _indices[matidx][xdx + 4] = 0;
        _indices[matidx][xdx + 5] = 0;
    }

    private void RemoveBullet(Bullet bullet) {
		bullet.Active = false;
		CleanBullet(bullet);
		_available.Enqueue(bullet.Index);
    }

	public IEnumerator _DeleteAfterFade(Bullet bullet) {
		bullet.Removing = true;
		if (bullet.Type == EType.NIGHTMARE || bullet.Type == EType.DREAM || bullet.Type == EType.ENEMY) {
			yield return StartCoroutine(bullet._Disappear(0.5f));
		}

		RemoveBullet(bullet);
	}

    public void ChangeBulletAppearance(Bullet bullet, Sprite sprite, EMaterial material) {
        bullet.Bounds = sprite.bounds;
        bullet.UVs = sprite.rect;
        bullet.Material = material;
        SetupBullet(bullet);
    }

	public void ChangeBulletColor(Bullet bullet, Color32 color) {
		bullet.Color = color;
		SetupBullet(bullet);
	}

    public void UpdateBulletAppearance(Bullet bullet) {
        int MaterialIdx = (int)bullet.Material;
        bullet.SetupUVs(_uvs[MaterialIdx], _materials[MaterialIdx].mainTexture);
    }

	public void UpdateGaugeLevel(float levelChange) {
		_gauge.UpdateLevel(levelChange);
	}

    public void UpdateAt(float dt) {
        _temp = new List<Bullet>();

        foreach (Bullet bullet in _active) {
			// Check if the bullet has expired
			HandleBulletLifeTime(bullet);

			if (bullet.Active) {
				// Update bullets position if they are not part of a group
				if (bullet.Owner != EOwner.GROUP) {
					int MaterialIdx = (int)bullet.Material;
					bullet.ComputePosition(_vertices[MaterialIdx], _colors[MaterialIdx], dt);
				}

				_temp.Add(bullet);
			}
        }

        _active = _temp;
        BulletCount = _active.Count;

        // Maybe only call that once before rendering
        SetMesh();
    }
		
    public void ReferenceBullets() {
		QuadTreeHolder.ReferenceBullets(_active);
    }

    void SetMesh() {
        for (int i = 0; i < (int)EMaterial.COUNT; ++i) {
            _meshs[i].vertices = _vertices[i];
            if (_indices != null) {
                _meshs[i].SetTriangles(_indices[i], 0, false);
            } 

            _meshs[i].uv = _uvs[i];
            _meshs[i].colors32 = _colors[i];
            _meshs[i].RecalculateBounds();
            MeshFilters[i].mesh = _meshs[i];
        }
    }

	private void HandleBulletLifeTime(Bullet bullet) {
		if ((bullet.Removing == false) &&
			((bullet.Lifetime.HasValue && bullet.CurrentTime >= bullet.Lifetime.Value) ||
			 (bullet.AutoDelete && (bullet.Type == EType.NIGHTMARE || bullet.Type == EType.DREAM || bullet.Type == EType.SHOT || bullet.Type == EType.ENEMY) &&
			 !bullet.AABB.Overlaps(QuadTreeHolder.quadtree.rect)))) {
			StartCoroutine(_DeleteAfterFade(bullet));
		}
	}

	public IEnumerator DisplayAvailableSize() {
		while (Application.isPlaying) {
			Debug.Log ("Size of available queue : " + _available.Count);
			yield return new WaitForSeconds (10.0f);
		}
	}
}

