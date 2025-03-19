using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoIntro : MonoBehaviour
{
    // Reference to the VideoPlayer component
    public VideoPlayer videoPlayer;

    void Start()
    {
        // Ensure that we subscribe to the event only once
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            // Optionally start playback via script
            // videoPlayer.Play();
        }
        else
        {
            Debug.LogError("VideoPlayer component not assigned!");
        }
    }

    // Callback that fires when the video finishes playing
    void OnVideoFinished(VideoPlayer vp)
    {
        // Optionally, you might want to add a fade-out or delay here
        SceneManager.LoadScene("BusScene", LoadSceneMode.Single);
    }
}