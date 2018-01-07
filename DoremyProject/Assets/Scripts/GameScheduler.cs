using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GameScheduler : MonoBehaviour
{
    public static GameScheduler instance = null;     // Singleton

    public MeshPool meshpool;
	private Enemy[] enemies;
	public List<Sprite> sprites;
    public Player player;
    public QuadTreeHolder quadtree;
	public AudioManager audioManager;
	public Dialogue dialogue;
	public SplineController splineController;
	public SplineController splineController2;

    private Camera cam;
    private float t = 0f;
    private float accumulator = 0;
    private float interpolation = 0;

    private static Vector3 default_resolution = new Vector3(640, 480);

    public static float dt = 0.01f; // Simulation delta time
    public static float cam_height;
    public static float cam_width;
    public static Vector3 origin;
    public static float scale_factor;

	public AudioClip stageMusic;
	public AudioClip bossMusic;

    public bool InDialogue;

    // Charged to initialize every other script in the right order.
    void OnEnable() {
		StartCoroutine(Begin());
    }

	IEnumerator Begin() {
		// Set instance to this object
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		// Initialize quadtree and meshpool
		quadtree.Init();
		meshpool.Init();

		// Initialize player
		player.Init();
		dialogue.StartDialogue();
		quadtree.player = player;

		if (Application.isPlaying) {
			audioManager.Init ();
			audioManager.PlayMusic (stageMusic);
		}

		// Wait for dialogue end
		InDialogue = true;
		yield return StartCoroutine(dialogue.StartDialogue());
		InDialogue = false;

		//Start camera travelling
		splineController.FollowSpline();
		splineController2.FollowSpline();

		// Initialize enemies
		enemies = FindObjectsOfType(typeof(Enemy)) as Enemy[];
		for (int i = 0; i < enemies.Length; ++i){
			enemies[i].Init();
		}

		meshpool.UpdateAt(0);
	}

    void Update() {
		player.UpdateAt (1f);

		float frameTime = Time.deltaTime;
		accumulator += frameTime;

		while (accumulator >= dt) {
			meshpool.UpdateAt (dt);			// Movement
			meshpool.ReferenceBullets ();	// Reference bullets for collisions

			// Collisions checks
			player.UpdateAt (dt);

			if (InDialogue == false) {
				for (int i = 0; i < enemies.Length; ++i) {
					enemies [i].UpdateAt (dt);
				}
			}

			accumulator -= dt;
			t += dt;
		}
    }
}
