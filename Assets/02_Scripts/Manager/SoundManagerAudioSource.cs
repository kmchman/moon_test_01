using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SoundManagerAudioSource : MonoBehaviour
{
	private static Dictionary<string, AudioClip> AudioClipCache = new Dictionary<string, AudioClip>();

	public bool isPlaying { get; private set; }
	private AudioSource _audioSource;

	public static void Clear()
	{
		AudioClipCache.Clear();
	}

	private void Awake()
	{
		_audioSource = this.gameObject.AddComponent<AudioSource>();
	}

	public bool IsReady()
	{
		return !_audioSource.isPlaying;
	}

	public void SetSpeed(float speed_)
	{
		_audioSource.pitch = speed_;
	}

	public void SetVolume(float value)
	{
		_audioSource.volume = value;
	}

	public void Play(AudioClip clip_, float volume_, bool isLoop_) { Play(clip_, volume_, isLoop_, 0, null); }
	public void Play(AudioClip clip_, float volume_, bool isLoop_, float delay_) { Play(clip_, volume_, isLoop_, delay_, null); }
	public void Play(AudioClip clip_, float volume_, bool isLoop_, float delay_, Action<SoundManagerAudioSource> endDelegate_)
	{
		Stop();

		isPlaying = true;
		StartCoroutine(PlayClip(clip_, volume_, isLoop_, delay_, endDelegate_));
	}

	public void Play(string path_, float volume_, bool isLoop_, bool isTemp_) { Play(path_, volume_, isLoop_, 0, null, isTemp_); }
	public void Play(string path_, float volume_, bool isLoop_, float delay_, bool isTemp_) { Play(path_, volume_, isLoop_, delay_, null, isTemp_); }
	public void Play(string path_, float volume_, bool isLoop_, float delay_, Action<SoundManagerAudioSource> endDelegate_, bool isTemp_)
	{
		Stop();

		isPlaying = true;
		StartCoroutine(PlayClip(path_, volume_, isLoop_, delay_, endDelegate_, isTemp_));
	}

	public void Continue()
	{
		_audioSource.Play();
	}

	public void Pause()
	{
		_audioSource.Pause();
	}

	public void Stop()
	{
		StopAllCoroutines();

		_audioSource.Stop();
		_audioSource.clip = null;

		isPlaying = false;
	}

	private IEnumerator PlayClip(string path_, float volume_, bool isLoop_, float delay_, Action<SoundManagerAudioSource> endDelegate_, bool isTemp_)
	{
		yield return this.StartCoroutine(LoadClip(path_, isTemp_));

		AudioClip clip_ = null;
		if (AudioClipCache.TryGetValue(path_, out clip_))
			yield return StartCoroutine(PlayClip(clip_, volume_, isLoop_, delay_, endDelegate_));
		else
			Debug.LogError("Not Found Sound Path[" + path_ + "]");
	}

	private IEnumerator PlayClip(AudioClip clip_, float volume_, bool isLoop_, float delay_, Action<SoundManagerAudioSource> endDelegate_)
	{
		if (delay_ > 0)
			yield return new WaitForSeconds(delay_);

		_audioSource.clip = clip_;
		_audioSource.loop = isLoop_;
		_audioSource.volume = volume_;
		_audioSource.Play();

		if (!isLoop_)
		{
			while (_audioSource.isPlaying)
				yield return null;

			Stop();

			if (endDelegate_ != null)
				endDelegate_(this);
		}
	}

	private IEnumerator LoadClip(string path_, bool isTemp_)
	{
//		Action<UnityEngine.Object> cb = (obj) => {
//			AudioClip clip_ = obj as AudioClip;
//			if (clip_ == null)
//				return;
//			AudioClipCache[path_] = clip_;
//		};
//
//		if (!AudioClipCache.ContainsKey(path_) || AudioClipCache[path_] == null)
//		{
//			bool isLoaded = false;
//			if (isTemp_)
//			{
//				AssetBundleManager.Inst.AsyncLoadAssetBundleTemp<UnityEngine.Object>(path_, path_, (assetBundle, obj) => {
//					isLoaded = true;
//					cb(obj);
//					if (assetBundle != null)
//						assetBundle.Unload(false);
//				});
//			}
//			else
//			{
//				AssetBundleManager.Inst.AsyncLoadAssetBundle(path_, (obj) => {
//					isLoaded = true;
//					cb(obj);
//				});
//			}
//			
//			while (!isLoaded)
//				yield return null;
//		}
		yield return null;
	}
}
