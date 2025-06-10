using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AudioSourcePool
{
    private class EmptyComponent : MonoBehaviour { }

    private readonly SubGame subgame;
    private readonly List<AudioSource> audioSources = new();
    private readonly ObjectPool<AudioSource> pool;
    private readonly GameObject corroutinesObject;
    private readonly MonoBehaviour coroutinesManager;

    public AudioSourcePool(SubGame subgame)
    {
        this.subgame = subgame;
        pool = new (CreateAudioSource);
        corroutinesObject = new GameObject($"{nameof(AudioSourcePool)}-{nameof(corroutinesObject)}");
        coroutinesManager = corroutinesObject.AddComponent<EmptyComponent>();
    }

    private AudioSource CreateAudioSource()
    {
        var screen = SubGameScreenManager.instance[subgame];
        if (screen != null)
        {
            var template = screen.AudioSourceTemplate;
            audioSources.Add(template);
            var ret = UnityEngine.Object.Instantiate(template, Vector3.zero, Quaternion.identity, template.transform);
            audioSources.Add(ret);
            return ret;
        }
        else
        {
            var camera = Camera.main;
            var template = camera.GetComponentsInChildren<AudioSource>().Where(c => c.CompareTag(TagsIribus.Speakers)).FirstOrDefault();
            if (template == null)
            {
                var speakers = new GameObject("Speakers");
                speakers.tag = TagsIribus.Speakers;
                speakers.transform.SetParent(camera.transform, false);
                template = speakers.AddComponent<AudioSource>();
            }
            audioSources.Add(template);
            var ret = UnityEngine.Object.Instantiate(template, Vector3.zero, Quaternion.identity, template.transform);
            audioSources.Add(ret);
            return ret;
        }
    }

    public void Alter(Action<AudioSource> alteration)
    {
        var screen = SubGameScreenManager.instance[subgame];
        var template = screen != null ? screen.AudioSourceTemplate : null;
        foreach (var audioSource in audioSources.Concat(new[] { template }))
            if(audioSource != null)
                alteration(audioSource);
    }

    public PlayingAudio PlayOneShot(AudioClip clip, float volumeScale = 1.0f)
    {
        var source = pool.Get();
        
        var coroutine = coroutinesManager.StartCoroutine(PlayOneShotCoroutine(source, clip, volumeScale, 0));

        return new PlayingAudio(pool, coroutinesManager, source, coroutine);
    }

    public PlayingAudio PlayDelayed(AudioClip clip, float delay, float volumeScale = 1.0f)
    {
        var source = pool.Get();
        
        var coroutine = coroutinesManager.StartCoroutine(PlayOneShotCoroutine(source, clip, volumeScale, delay));

        return new PlayingAudio(pool, coroutinesManager, source, coroutine);
    }

    public PlayingAudio PlayLoop(AudioClip clip, float volumeScale = 1.0f)
    {
        var source = pool.Get();

        source.clip = clip;
        source.loop = true;

        source.volume *= volumeScale;
        source.Play();

        return new PlayingAudio(pool, coroutinesManager, source, volumeScale: volumeScale);
    }

    private IEnumerator PlayOneShotCoroutine(AudioSource source, AudioClip clip, float volumeScale, float delaySeconds)
    {
        if(delaySeconds > 0)
            yield return new WaitForSecondsRealtime(delaySeconds);
        source.loop = false;
        source.PlayOneShot(clip, volumeScale);
        yield return new WaitForSecondsRealtime(clip.length);
        while (source.isPlaying)
            yield return new WaitForNextFrameUnit();
        pool.Release(source);
    }
}