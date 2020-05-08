using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{

    public Image Img;
    public AnimationCurve FadeInCurve;
    public AnimationCurve FadeOutCurve;

    private void Start()
    {
        StartCoroutine(FadeIn());

    }

    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }

    IEnumerator FadeIn()
    {

        float iIntensity = 1f;

        while (iIntensity > 0f) {

            iIntensity -= Time.deltaTime;
            Img.color = new Color(0f, 0f, 0f, FadeInCurve.Evaluate(iIntensity));
            yield return 0;

        }

    }

    IEnumerator FadeOut(string scene)
    {

        float iIntensity = 0f;

        while (iIntensity < 1f) {

            iIntensity += Time.deltaTime;
            Img.color = new Color(0f, 0f, 0f, FadeInCurve.Evaluate(iIntensity));
            yield return 0;

        }

        SceneManager.LoadScene(scene);

    }

}