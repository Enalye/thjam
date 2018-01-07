using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Sprites/Gauge")]
public class Gauge : MonoBehaviour {
	[Range(0, 100)]
	public float level;  // Between 0 and 100%, starts at 50 by default

	private UnityEngine.UI.Image fluid;

	void Start () {
		fluid = gameObject.GetComponent<UnityEngine.UI.Image>();
		StartCoroutine (_Decrease ());
	}

	public void UpdateLevel(float levelChange) {
		level = Mathf.Clamp(level + levelChange, 0, 100);
		fluid.rectTransform.sizeDelta = new Vector3(fluid.rectTransform.sizeDelta.x,
												    (level / 100) * 400);
	}		

	public IEnumerator _Decrease() {
		while (Application.isPlaying) {
			yield return new WaitForSeconds (0.1f);
			UpdateLevel(-0.1f);
		}
	}
}
