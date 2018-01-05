using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Sprites/Gauge")]
public class Gauge : Bullet {
	[Range(0, 100)]
	public float level = 50;  // Between 0 and 200%, starts at 100 by default
	public float height = 10; // Height of the gauge

	// No collision handling here allows for a simplified code
	public override void SetupVertices(Vector3[] vertices, Color32[] colors) {
		int offset = Index * 4;
		Vector3 bullet_ext = Vector3.Scale(Bounds.extents, Scale);

		float min_x = Position.x - bullet_ext.x;
		float min_y = Position.y;
		float max_x = Position.x + bullet_ext.x;
		float max_y = Position.y + (height * level / 100);

		SetupVertices(offset, min_x, min_y, max_x, max_y, vertices, colors);
	}

	public void UpdateLevel(float levelChange) {
		level = Mathf.Max(0, level + levelChange);
	}		

	public IEnumerator _Decrease() {
		while (Application.isPlaying) {
			yield return new WaitForSeconds (0.1f);
			UpdateLevel(-0.5f);
		}
	}
}
