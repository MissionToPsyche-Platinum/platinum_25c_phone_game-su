using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform camera;
    //Modify camera position over time
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = camera.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * .1f * magnitude;
            float y = Random.Range(-1f, 1f) * .1f * magnitude;
            camera.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        camera.localPosition = originalPos; //reset to original position when done shaking
    }
}