using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Plays a cutscene video when a specific artifact is picked up.
/// Freezes gameplay, shows a video using RawImage UI, allows skipping via space key.
/// </summary>
[Serializable]
public struct ArtifactVideo
{
    [Tooltip("ID of the artifact associated with the video")]
    public int artifactId;

    [Tooltip("Video clip to play for this artifact")]
    public VideoClip clip;
}

public class ArtifactCutsceneManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("RawImage where the video will be rendered")]
    public RawImage displayUI;

    [Tooltip("Optional skip prompt (e.g., 'Press SPACE to skip')")]
    public TMPro.TextMeshProUGUI skipPrompt;

    [Header("Mappings")]
    [Tooltip("List of artifact ID to video mappings")]
    public List<ArtifactVideo> clips;

    [Header("Freeze")]
    [Tooltip("All components that should be disabled during cutscene")]
    public MonoBehaviour[] disableDuringCutscene;

    private Dictionary<int, VideoClip> _clipMap;
    private bool _isPlaying = false;

    void Awake()
    {
        _clipMap = new Dictionary<int, VideoClip>();
        foreach (var e in clips)
        {
            if (e.clip != null && !_clipMap.ContainsKey(e.artifactId))
                _clipMap[e.artifactId] = e.clip;
        }

        displayUI.gameObject.SetActive(false);
        if (skipPrompt != null)
            skipPrompt.gameObject.SetActive(false);
    }

    void OnEnable() => EventManager.OnArtifactPicked += OnPicked;

    void OnDisable() => EventManager.OnArtifactPicked -= OnPicked;

    /// <summary>
    /// Handles artifact pickup event and triggers cutscene if mapped.
    /// </summary>
    void OnPicked(int id)
    {
        if (_isPlaying || !_clipMap.TryGetValue(id, out var clip))
            return;

        StartCoroutine(PlayOneShot(clip));
    }

    /// <summary>
    /// Plays a single video clip as a cutscene.
    /// Pauses game time, shows video, allows skipping, and restores state after.
    /// </summary>
    IEnumerator PlayOneShot(VideoClip clip)
    {
        _isPlaying = true;

        // Freeze game
        Time.timeScale = 0f;
        AudioListener.pause = true;
        foreach (var mb in disableDuringCutscene)
            if (mb != null)
                mb.enabled = false;

        // Show video UI
        displayUI.gameObject.SetActive(true);
        if (skipPrompt != null) skipPrompt.gameObject.SetActive(true);

        // Setup video
        var rt = new RenderTexture((int)clip.width, (int)clip.height, 0);
        displayUI.texture = rt;

        var vpGO = new GameObject("CutscenePlayer");
        vpGO.transform.SetParent(transform, false);

        var vp = vpGO.AddComponent<VideoPlayer>();
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.targetTexture = rt;
        vp.clip = clip;
        vp.isLooping = false;
        vp.audioOutputMode = VideoAudioOutputMode.None; // Can be changed if audio is needed

        // Wait for preparation
        vp.Prepare();
        while (!vp.isPrepared)
            yield return new WaitForSecondsRealtime(0.05f);

        vp.Play();

        bool done = false;
        VideoPlayer.EventHandler handler = null;
        handler = (_) =>
        {
            done = true;
            vp.loopPointReached -= handler;
        };
        vp.loopPointReached += handler;

        // Check for skip
        while (!done)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                vp.Stop();
                vp.loopPointReached -= handler;
                done = true;
            }
            yield return null;
        }

        // Cleanup
        vp.Stop();
        Destroy(vpGO);
        rt.Release();
        Destroy(rt);

        displayUI.gameObject.SetActive(false);
        if (skipPrompt != null) skipPrompt.gameObject.SetActive(false);

        // Unfreeze game
        foreach (var mb in disableDuringCutscene)
            if (mb != null)
                mb.enabled = true;

        AudioListener.pause = false;
        Time.timeScale = 1f;
        _isPlaying = false;
    }
}
