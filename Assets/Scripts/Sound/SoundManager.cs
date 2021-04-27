using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGSoundName
{
    ArScene = 0,
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private int currentBGM = -1;
    private Dictionary<int, AudioClip> bgAudioDict;
    private string defaultBGMparam = "bgmChild";

    [SerializeField]
    public AudioMixer audioMixer;
    [SerializeField]
    private AudioSource bgAudioSource;
    [SerializeField]
    private AudioSource walkSound;

    private void Awake()
    {
        Instance = this;
        bgAudioDict = new Dictionary<int, AudioClip>();
    }
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        if (bgAudioDict.Count <= 0)
        {
            bgAudioDict.Add((int)BGSoundName.ArScene, Resources.Load<AudioClip>("sound/bgm_main"));
        }
    }
    public void OnNewGame()
    {
        currentBGM = -1;
    }
    public void PlayWalkSound() => walkSound.Play();

    public void TurnOnBGM(BGSoundName audio)
    {
        if ((int)audio == currentBGM) return;
        if (currentBGM == -1)
        {
            bgAudioSource.clip = bgAudioDict[0];
            bgAudioSource.loop = true;
            bgAudioSource.Play();
            currentBGM = (int)audio;
            return;
        }
        StartCoroutine(FadeAndPlayBGM((int)audio));
        Debug.Log("Playing : " + audio);
    }
    private IEnumerator FadeAndPlayBGM(int audio)
    {
        yield return StartCoroutine(StopBGM());
        bgAudioSource.clip = bgAudioDict[(int)audio];
        bgAudioSource.Play();
        currentBGM = audio;
    }
    public void ChangeBgmValue(float value)
    {
        audioMixer.SetFloat("BgmMaster", value);
    }
    public void ChangeSfxValue(float value)
    {
        audioMixer.SetFloat("SfxMaster", value);
    }
    public IEnumerator StopBGM()
    {
        float currentVol;
        audioMixer.GetFloat(defaultBGMparam, out currentVol);
        yield return StartCoroutine(FadeMixerGroup.StartFade(audioMixer, defaultBGMparam, 1.5f, 0));
        bgAudioSource.Stop();
        audioMixer.SetFloat(defaultBGMparam, currentVol);
    }
}
