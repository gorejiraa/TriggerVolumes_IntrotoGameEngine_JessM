using UnityEngine;
using UnityEngine.UI;

public class GifPlayer : MonoBehaviour
{
    public Sprite[] frames;
    public float framesPerSecond = 10f;
    Image img; int i; float t;

    void Awake() { img = GetComponent<Image>(); }
    void Update()
    {
        if (frames == null || frames.Length == 0) return;
        t += Time.unscaledDeltaTime;
        if (t >= 1f / framesPerSecond)
        {
            t -= 1f / framesPerSecond;
            i = (i + 1) % frames.Length;
            img.sprite = frames[i];
        }
    }
}