using UnityEngine;
using UnityEngine.Pool;

public class PlayingAudio
{
    private readonly AudioSource source;
    private readonly Coroutine coroutine;
    private readonly MonoBehaviour monoBehaviour;
    private readonly ObjectPool<AudioSource> pool;
    private readonly float? volumeScale;

    public PlayingAudio(ObjectPool<AudioSource> pool, MonoBehaviour monoBehaviour, AudioSource source, Coroutine coroutine = null, float? volumeScale = null)
    {
        this.source = source;
        this.coroutine = coroutine;
        this.monoBehaviour = monoBehaviour;
        this.pool = pool;
        this.volumeScale = volumeScale;
    }

    public void Stop()
    {
        if(coroutine != null)
            monoBehaviour.StopCoroutine(coroutine);
        source.Stop();

        if (volumeScale.HasValue)
            source.volume /= volumeScale.Value;

        pool.Release(source);
    }
}
