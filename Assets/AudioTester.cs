using System.Collections;
using UnityEngine;

public class AudioTester : SubGameAudioSource
{
    [SerializeField] private AudioClip loopClip;
    [SerializeField] private float loopClipVolume = 1;
    [SerializeField] private AudioClip periodicClip;
    [SerializeField] private float periodicClipVolume = 1;
    [SerializeField] private float periodicClipPeriod = 3;

    public override SubGame SubGame => SubGame.Iribus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlayPeriodicAudio());
        Source.PlayLoop(loopClip, loopClipVolume);
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator PlayPeriodicAudio()
    {
        while (true)
        {
            Source.PlayOneShot(periodicClip, periodicClipVolume);
            yield return new WaitForSeconds(periodicClipPeriod);
        }
    }
}
