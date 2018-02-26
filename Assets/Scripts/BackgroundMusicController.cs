using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class BackgroundMusicController : MonoBehaviour {

    private AudioSource backgroundAudio;
    public AudioClip firstAudioClip;
    public AudioClip secondAudioClip;

    public static int menuSceneBuildIndex = 1; // The buildindex for menuScene

    private static BackgroundMusicController instance = null;
    public static BackgroundMusicController Instance
    {
        get { return instance; }
    }
    // Use this for initialization
    void Start ()
    {
        backgroundAudio  = GetComponent<AudioSource>();
        if(!backgroundAudio.isPlaying)
        {
            backgroundAudio.PlayOneShot(firstAudioClip, 0.4F);
            backgroundAudio.PlayOneShot(secondAudioClip, 0.1F);
        }
	}
	
	// Update is called once per frame
	void Awake ()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        SceneManager.activeSceneChanged += DestroyOnMenuScreen;
    }
    void Update()
    {
        if (!MainScript.playerIsAlive && backgroundAudio.isPlaying)
        {
            backgroundAudio.volume = 0.1F;
        }
        else
        {
            backgroundAudio.volume = 1.0F;
        }

    }

    void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == menuSceneBuildIndex) //could compare Scene.name instead
        {
            Destroy(this.gameObject); //change as appropriate
        }
    }
}
