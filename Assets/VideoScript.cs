using UnityEngine;
using UnityEngine.Video;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Bringme : MonoBehaviour
{
    public VideoPlayer videoPlayer;   // assign on the cube
    public Camera targetCamera;       // drag Main Camera here (or auto if null)

    void Start()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        if (targetCamera == null && Camera.main != null) targetCamera = Camera.main;

        if (videoPlayer == null)
        {
            Debug.LogError("[Bringme] No VideoPlayer component found.");
            enabled = false; return;
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = targetCamera;
        videoPlayer.targetCameraAlpha = 0f; // will fade to 1 on play
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.skipOnDrop = true;

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.errorReceived += (vp, msg) => Debug.LogError("[Bringme] Video error: " + msg);
        videoPlayer.loopPointReached += (vp) => Debug.Log("[Bringme] Video finished.");
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.prepareCompleted -= OnPrepared;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("[Bringme] Manual play (P) pressed.");
            StartCoroutine(PlayFlow());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("[Bringme] Player entered trigger, preparing video…");
        StartCoroutine(PlayFlow());
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("[Bringme] Player left trigger, stopping video.");
        StopVideo();
    }

    IEnumerator PlayFlow()
    {
        if (videoPlayer.clip == null)
        {
            Debug.LogError("[Bringme] No Video Clip assigned to VideoPlayer.");
            yield break;
        }

        // Prepare (some platforms need this)
        videoPlayer.Prepare();
        float t = 0f;
        while (!videoPlayer.isPrepared && t < 5f)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        if (!videoPlayer.isPrepared)
        {
            Debug.LogError("[Bringme] Prepare timed out. Check codec/clip path.");
            yield break;
        }

        // Show on camera and play
        videoPlayer.targetCamera = targetCamera;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCameraAlpha = 1f;
        videoPlayer.Play();

        Debug.Log("[Bringme] Playing. isPrepared=" + videoPlayer.isPrepared + ", isPlaying=" + videoPlayer.isPlaying);
    }

    void StopVideo()
    {
        if (videoPlayer == null) return;
        if (videoPlayer.isPlaying) videoPlayer.Stop();
        videoPlayer.targetCameraAlpha = 0f;
    }

    void OnPrepared(VideoPlayer vp)
    {
        Debug.Log("[Bringme] Prepared callback fired.");
    }
}
