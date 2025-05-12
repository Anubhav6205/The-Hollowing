using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a binding between an artifact ID and a GameObject
/// that should be activated when that artifact is picked up.
/// </summary>
[Serializable]
public struct ArtifactActivationBinding
{
    [Tooltip("Artifact ID (e.g., 1 for toy truck, 2 for ribbon, etc.)")]
    public int artifactId;

    [Tooltip("GameObject to activate when this artifact is picked up")]
    public GameObject objectToActivate;
}

/// <summary>
/// Manages story progression by enabling objects when specific artifacts are collected.
/// Also handles navigation system activation after phone pickup.
/// </summary>
public class StoryManager : MonoBehaviour
{
    [Tooltip("Reference to the navigation UI system (e.g., arrows, compass)")]
    public GameObject navigationSystem;

    // Internal lookup mapping artifact IDs to a list of GameObjects to activate
    private Dictionary<int, List<GameObject>> _map;

    [Tooltip("List of bindings between artifact IDs and objects to activate")]
    [SerializeField]
    private List<ArtifactActivationBinding> _bindings;

    /// <summary>
    /// Builds a dictionary mapping artifact IDs to their corresponding GameObjects.
    /// Disables all bound GameObjects at startup.
    /// </summary>
    void Awake()
    {
        _map = new Dictionary<int, List<GameObject>>();

        foreach (var b in _bindings)
        {
            if (!_map.TryGetValue(b.artifactId, out var list))
            {
                list = new List<GameObject>();
                _map[b.artifactId] = list;
            }

            list.Add(b.objectToActivate);

            // Ensure all trigger objects are initially inactive
            if (b.objectToActivate != null)
                b.objectToActivate.SetActive(false);
        }
    }

    /// <summary>
    /// Subscribes to artifact pickup events.
    /// </summary>
    void OnEnable()
    {
        EventManager.OnArtifactPicked += OnArtifactPicked;
    }

    /// <summary>
    /// Unsubscribes from artifact pickup events.
    /// </summary>
    void OnDisable()
    {
        EventManager.OnArtifactPicked -= OnArtifactPicked;
    }

    /// <summary>
    /// Called when an artifact is picked. Activates any GameObjects bound to that artifact ID.
    /// </summary>
    /// <param name="id">ID of the picked artifact</param>
    private void OnArtifactPicked(int id)
    {
        Debug.Log("Artifact picked");
        Debug.Log(id);

        if (_map.TryGetValue(id, out var triggers))
        {
            foreach (var t in triggers)
            {
                t.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Call this (e.g. after phone pickup) to enable the navigation UI system.
    /// </summary>
    public void ActivateNavigationSystem()
    {
        if (navigationSystem != null)
            navigationSystem.SetActive(true);
    }
}
