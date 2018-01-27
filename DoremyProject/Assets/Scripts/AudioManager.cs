using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
	private List<AudioSource> mainMusicSources;  // Reference to the audio sources which will play the game music.
	private List<AudioSource> spiritMusicSources;  // Reference to the audio sources which will play the game music.
	private List<AudioSource> efxSources;    // Reference to the audio sources which will play the game effects.

	public void Init() {
		mainMusicSources = new List<AudioSource>();
		spiritMusicSources = new List<AudioSource>();
		efxSources = new List<AudioSource>();
	}

	public void Destroy() {
		Destroy(gameObject);
	}

	private AudioSource PlaySingle(AudioClip clip, List<AudioSource> owner) {
		if(clip == null) {
			Debug.LogError("Clip passed to PlaySingle was null");
			return null;
		}

		int found_it = -1;
		int i = 0;

		foreach(AudioSource source in owner) {
			if(source.clip == clip) {
				found_it = i;
			}
			i++;
		}

		AudioSource audioSource = null;

		// If not found
		if (found_it == -1) {
			audioSource = gameObject.AddComponent<AudioSource>();
			owner.Add(audioSource);
		} else {
			audioSource = owner[found_it];
		}

		// Set the clip of our efxSource audio source to the clip passed in as a parameter.
		audioSource.clip = clip;

		// Play the clip.
		audioSource.Play();

		return audioSource;
	}

	public void PlayEffect(AudioClip clip) {
		AudioSource audioSource = PlaySingle(clip, efxSources);
		audioSource.playOnAwake = false;
	}

	public void PlayMusic(AudioClip mainClip, AudioClip spiritClip, int startLoop, float volume) {
		AudioSource mainSource = PlaySingle(mainClip, mainMusicSources);
		mainSource.volume = volume;

		AudioSource spiritSource = PlaySingle(spiritClip, spiritMusicSources);
		spiritSource.volume = 0;

		StartCoroutine(Loop(mainSource, spiritSource, startLoop));
	}

	/* Loops the themes with custom satrt and end times */
	public IEnumerator Loop(AudioSource mainSource, AudioSource spiritSource, int startLoop) {
		while(mainSource.volume != 0) {
			if(mainSource.isPlaying == false) {
				mainSource.timeSamples = startLoop;
				spiritSource.timeSamples = startLoop;

				mainSource.Play();
				spiritSource.Play();
			}

			yield return new WaitForSeconds(GameScheduler.dt);
		}
	}

	/* Prerequisite : both musics already playing, only one duo is active */
	public IEnumerator SwitchMusic(float time) {
		AudioSource currSource = null;
		AudioSource newSource = null;

		for(int i = 0; i < mainMusicSources.Count; ++i) {
			if (mainMusicSources[i].volume != 0) {
				currSource = mainMusicSources[i];
				newSource = spiritMusicSources[i];
				break;
			}

			if(spiritMusicSources[i].volume != 0) {
				currSource = spiritMusicSources[i];
				newSource = mainMusicSources[i];
				break;
			}
		}

		if ((currSource != null) && (newSource != null)) {
			float elapsedTime = 0;
			while (elapsedTime < time) {
				float timeRatio = elapsedTime / time;
				currSource.volume = Mathf.Lerp (1.0f, 0, timeRatio);
				newSource.volume = Mathf.Lerp (0, 1.0f, timeRatio);
				elapsedTime += GameScheduler.dt;
				yield return new WaitForSeconds (GameScheduler.dt);
			}

			currSource.volume = 0;
		}
	}
}
