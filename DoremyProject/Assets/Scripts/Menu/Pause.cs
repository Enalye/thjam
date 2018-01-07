using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {
	public GameObject pauseMenu;
	public GameObject continueBtn;
	public GameObject quitBtn;

	private float pauseTime, selectionTime;
	private UnityEngine.UI.Image continueText, quitText;
	private int menuIndex;

	void Start () {
		pauseMenu.SetActive (false);
		pauseTime = Time.realtimeSinceStartup;
		selectionTime = pauseTime;
		continueText = continueBtn.GetComponent<UnityEngine.UI.Image>();
		quitText = quitBtn.GetComponent<UnityEngine.UI.Image>();
	}

	void Update () {
		if (Input.GetButton ("Escape")) {
			if ((Time.realtimeSinceStartup - pauseTime) > .25f) {
				pauseTime = Time.realtimeSinceStartup;
				if (!pauseMenu.activeSelf)
					StartMenu();
				else
					CloseMenu();
			}
		}

		if (pauseMenu.activeSelf) {
			if (Input.GetButton ("Shot1")) {
				switch (menuIndex) {
				case 0:
					CloseMenu();
					break;
				case 1:
					Application.Quit();
					break;
				default:
					break;
				}
			}

			switch(menuIndex) {
			case 0:
				continueText.color = new Color (1f, 1f, 1f, Mathf.Lerp (.35f, 1f, (Mathf.Cos ((Time.realtimeSinceStartup - selectionTime) * 2f) + 1f) / 2f));
				quitText.color = new Color (1f, 1f, 1f, .3f);
				break;
			case 1:
				continueText.color = new Color (1f, 1f, 1f, .3f);
				quitText.color = new Color (1f, 1f, 1f, Mathf.Lerp (.35f, 1f, (Mathf.Cos ((Time.realtimeSinceStartup - selectionTime) * 2f) + 1f) / 2f));
				break;
			default:
				break;
			}

			if ((Time.realtimeSinceStartup - selectionTime) > .25f) {
				if (Input.GetButton("Up")) {
					selectionTime = Time.realtimeSinceStartup;
					menuIndex --;
					if (menuIndex < 0)
						menuIndex = 1;
				} else if (Input.GetButton("Down")) {
					selectionTime = Time.realtimeSinceStartup;
					menuIndex ++;
					if (menuIndex > 1)
						menuIndex = 0;
				}
				if (Input.GetButton ("Shot2")) {
					selectionTime = Time.realtimeSinceStartup;
					menuIndex = 1;
				}
			}
		}
	}

	void StartMenu() {
		pauseMenu.SetActive(true);
		Time.timeScale = 0f;
		menuIndex = 0;
	}

	void CloseMenu() {
		pauseMenu.SetActive(false);
		Time.timeScale = 1f;
	}
}
