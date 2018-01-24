﻿	using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
	private AudioSource musicSource;         // Reference to the audio source which will play the music.
	private List<AudioSource> efxSources;    // Reference to the audio source which will play the music.

	public void Init() {
		musicSource = gameObject.AddComponent<AudioSource>();
		efxSources = new List<AudioSource>();
	}

	public void Destroy() {
		Destroy(gameObject);
	}

	public AudioSource GetMusicSource() {
		return musicSource;
	}

	public void PlaySingle(AudioClip clip) {
		if(clip == null) {
			return;
		}

		int found_it = -1;
		int i = 0;

		foreach(AudioSource source in efxSources) {
			if(source.clip == clip) {
				found_it = i;
			}
			i++;
		}

		AudioSource efxSource = null;
		// If not found
		if (found_it == -1) {
			efxSource = gameObject.AddComponent<AudioSource>();
			efxSource.playOnAwake = false;
			efxSources.Add(efxSource);
		} else {
			efxSource = efxSources[found_it];
		}

		// Set the clip of our efxSource audio source to the clip passed in as a parameter.
		efxSource.clip = clip;

		// Play the clip.
		efxSource.Play();
	}

	public void PlayMusic(AudioClip clip) {
		// Set the clip of our musicSource audio source to the clip passed in as a parameter.
		musicSource.clip = clip;
		musicSource.volume = 0.25f;
		musicSource.loop = true;

		// Play the clip.
		musicSource.Play();
	}

	public void StopMusic() {
		musicSource.Stop();
	}

	IEnumerator CR_CountTime(AudioSource source)
	{
		float t = 0;

		while(t < source.clip.length) {
			t += Time.deltaTime;
		}
		source.Pause();

		yield break;
	}
}
