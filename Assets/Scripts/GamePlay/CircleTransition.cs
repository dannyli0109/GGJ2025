using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class CircleTransition : SingletonMono<CircleTransition>

{
	[Header("References")]
    [Tooltip("Drag in the circle Image that will scale from dot to full screen.")]
    public GameObject circleImagePrefab;
    private Image circleImage;

    [Header("Transition Settings")]
    [Tooltip("Scale value at which the circle completely covers the screen.")]
    public float maxScale = 20f;
    [Tooltip("Duration (seconds) for the circle to scale from dot to full coverage (and vice versa).")]
    public float transitionDuration = 1f;

    public void TransitionToScene(int id, bool shrinkOnNewScene = true)
    {
        StartCoroutine(DoCircleTransition(id, shrinkOnNewScene));
    }

    private IEnumerator DoCircleTransition(int id, bool shrinkOnNewScene)
    {
        // 2) Load the new scene
        // 1) Expand circle from scale=0 to scale=maxScale
        yield return StartCoroutine(ScaleCircle(0f, maxScale, transitionDuration));

        GameManager.Instance.SwitchLevel(id);


        // 3) If desired, shrink circle from scale=maxScale back to 0 in the new scene
        if (shrinkOnNewScene)
        {
            yield return StartCoroutine(ScaleCircle(maxScale, 0f, transitionDuration));
        }
    }

    /// <summary>
    /// Coroutine that scales the circle from startScale to endScale over 'duration' seconds.
    /// </summary>
    private IEnumerator ScaleCircle(float startScale, float endScale, float duration)
    {
        float elapsed = 0f;
        Vector3 initial = new Vector3(startScale, startScale, 1f);
        Vector3 target = new Vector3(endScale, endScale, 1f);

        if (!CircleTransition.Instance.circleImage)
        {
            var circleObj = Instantiate(circleImagePrefab, transform);
            // Make sure our circle Image is enabled and visible
            // if (circleImage != null) circleImage.gameObject.SetActive(true);
            CircleTransition.Instance.circleImage = circleObj.transform.GetChild(0).GetComponent<Image>();
        }
        circleImage = CircleTransition.Instance.circleImage;
        circleImage.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Lerp the scale
            Vector3 newScale = Vector3.Lerp(initial, target, t);
            circleImage.rectTransform.localScale = newScale;

            yield return null;
        }

        // Ensure final scale is exact
        circleImage.rectTransform.localScale = target;

        // If fully shrunk, we can hide the Image if we want
        if (Mathf.Approximately(endScale, 0f) && circleImage != null)
        {
            circleImage.gameObject.SetActive(false);
        }
    }
}
