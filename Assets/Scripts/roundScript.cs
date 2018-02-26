using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class roundScript : MonoBehaviour {


    [Header("Round Settings")]
    [Tooltip("Round Number")]
    public string roundNumber;
    Button playGame;
    Image panelImage;

    Text roundText;

    public AudioClip letterAppearance;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(animateText(roundNumber));
    }

    private IEnumerator animateText(string strComplete)
    {
        int i = 0; 
        string str = "";
        while (i < strComplete.Length)
        {
            str += strComplete[i++];
            yield return new WaitForSeconds(0.5F);
            roundText = GetComponent<Text>();
            roundText.text = str;
            if(i < 7) audioSource.PlayOneShot(letterAppearance, 0.7F);
        }

        float fadeTime = GameObject.Find("GameController").GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);


        //StartCoroutine(gamePause(1.0F));    // short pause before starting the next scene 
    }

    //private IEnumerator playGameAnimation(float v)
    //{
    //    yield return new WaitForSeconds(v);
    //    panelImage.color = new Color(0, 0, 0, panelImage.color.a + v);
    //    if (panelImage.color.a != 1)
    //        StartCoroutine(playGameAnimation(0.025F));
    //}

    //private IEnumerator gamePause(float v)
    //{
    //    yield return new WaitForSecondsRealtime(v);
    //    if(v == 1.0F) animationStarterFunc();
    //    else ChangeToNextScene();
    //}

    //private void animationStarterFunc()
    //{
    //    roundText.enabled = false;
    //    panelImage = GameObject.Find("panel").GetComponentInChildren<CanvasRenderer>().GetComponent<Image>();
    //    if (panelImage.color.a != 1)
    //    {
    //        StartCoroutine(playGameAnimation(0.025F));
    //    }
    //    StartCoroutine(gamePause(0.75F));
    //}

    //public void ChangeToNextScene()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}
}
