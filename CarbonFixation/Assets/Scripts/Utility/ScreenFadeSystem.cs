using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utility
{
    /// <summary>
    ///     This class is a singleton class that lazy-loads itself when called from resources
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
	public class ScreenFadeSystem : MonoBehaviourPlus
    {
        private static ScreenFadeSystem instance;

        public enum State { CLEAR, OPAQUE }

        private static CanvasGroup myCanvasGroup;
        private static CancellationTokenSource cancellationTokenSource = new();

        private void OnDestroy()
        {
            cancellationTokenSource.Dispose();
        }

        public static async UniTask FadeAsync(State type, float duration, float preFadeDelay = 0f, float postFadeDelay = 0f)
        {
            Debug.Log($"ScreenFadeSystem.FadeAsync start screen fade {type}");
            PokeAwake();
            float startTime = Time.time;
            // before yield
            
            switch (type)
            {
                case State.OPAQUE:
                    myCanvasGroup.blocksRaycasts = true;
                    break;
            }

            try
            {
                // pre fade delay
                while (Time.time < startTime + preFadeDelay)
                    await UniTask.Yield(cancellationTokenSource.Token, true);
                
                // yield while
                while (Time.time < startTime + preFadeDelay + duration)
                {
                    switch (type)
                    {
                        case State.CLEAR:
                            myCanvasGroup.alpha = Mathf.Lerp(1, 0, Mathf.Clamp01((Time.time - (startTime + preFadeDelay)) / duration));
                            break;
                        case State.OPAQUE:
                            myCanvasGroup.alpha = Mathf.Lerp(0, 1, Mathf.Clamp01((Time.time - (startTime + preFadeDelay)) / duration));
                            break;
                    }

                    await UniTask.Yield(cancellationTokenSource.Token, true);
                }
                
                // post fade delay
                while (Time.time < startTime + preFadeDelay + duration + postFadeDelay)
                    await UniTask.Yield(cancellationTokenSource.Token, true);
            }
            // Break from while loop when canceled
            catch (Exception ex) when (ex is not OperationCanceledException) // when (ex is not OperationCanceledException) at C# 9.0
            {
                Debug.LogException(ex);
            } 

            Debug.Log($"ScreenFadeSystem.FadeAsync complete screen fade {type}");
            // after yield
            switch (type)
            {
                case State.CLEAR:
                    myCanvasGroup.alpha = 0;
                    myCanvasGroup.blocksRaycasts = false;
                    break;
                case State.OPAQUE:
                    myCanvasGroup.alpha = 1;
                    myCanvasGroup.blocksRaycasts = true;
                    break;
            }
        }

        private static void PokeAwake()
        {
            if (instance == null)
            {
                var go = new GameObject("ScreenFadeSystem");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<ScreenFadeSystem>();
                var canvas = (GameObject)GameObject.Instantiate(Resources.Load("CanvasScreenFade"), go.transform);
                myCanvasGroup = canvas.GetComponentInChildren<CanvasGroup>();
            }
        }
    }
}