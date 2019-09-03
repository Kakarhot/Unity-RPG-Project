using UnityEngine;
using System.Collections;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActiveFade;

        private void Awake() 
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediately()
        {
            if(canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1;
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1,time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        Coroutine Fade(float target, float time)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFade;
        }
        
        IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}