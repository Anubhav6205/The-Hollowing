using System;
using System.Collections.Generic;

/// <summary>
/// Global static event dispatcher for key game events.
/// Enables decoupled communication between systems like UI, gameplay logic, and triggers.
/// </summary>
public static class EventManager
{
    // Fired when a door is locked (e.g., after a key event or story step)
    public static event Action DoorLocked;

    // Fired when the player picks up the phone (e.g., to trigger a monologue or cutscene)
    public static event Action PhonePickedUp;

    // Fired when a battery is picked up, passes reference to the specific blinking object
    public static event Action<ObjectBlinkLight> BatteryCollected;

    // Fired when a unique navigation waypoint should appear (e.g., ghost guidance)
    public static event Action<string> OnWaypointPop;

    // Fired when flashlight is toggled on or off
    public static event Action<bool> FlashlightToggled;

    // Fired whenever the player's battery count changes (e.g., update UI)
    public static event Action<int> BatteryCountChanged;

    // Fired when the flashlight is picked up (separate from toggle)
    public static event Action FlashlightPickedUp;

    // Fired when an artifact is picked up (e.g., 1st, 2nd...5th), passes the ID
    public static event Action<int> OnArtifactPicked;

    // Internally tracks which waypoints have already been popped to avoid repetition
    static HashSet<string> _poppedKeys = new HashSet<string>();

    /// <summary>
    /// Call this when an artifact is collected to trigger associated game events.
    /// </summary>
    public static void ArtifactPicked(int id)
    {
        OnArtifactPicked?.Invoke(id);
    }

    /// <summary>
    /// Fires the battery collected event for the specific blinking object.
    /// </summary>
    public static void RaiseBatteryCollected(ObjectBlinkLight b) => BatteryCollected?.Invoke(b);

    /// <summary>
    /// Fires the flashlight toggle event with current state (on/off).
    /// </summary>
    public static void RaiseFlashlightToggled(bool isOn) => FlashlightToggled?.Invoke(isOn);

    /// <summary>
    /// Fires a waypoint popup event once per unique key. Prevents duplicates.
    /// </summary>
    public static void PopWaypoint(string key)
    {
        // Only trigger the event if the key hasn't been used before
        if (_poppedKeys.Add(key))
            OnWaypointPop?.Invoke(key);
    }

    /// <summary>
    /// Fires when the flashlight is picked up for the first time.
    /// </summary>
    public static void RaiseFlashlightPickedUp() => FlashlightPickedUp?.Invoke();

    /// <summary>
    /// Updates listeners with the new battery count (e.g., for HUD)
    /// </summary>
    public static void RaiseBatteryCountChanged(int newCount) =>
        BatteryCountChanged?.Invoke(newCount);

    /// <summary>
    /// Fires the event that a door has been locked (could trigger sound/UI/cutscene).
    /// </summary>
    public static void RaiseDoorLocked() => DoorLocked?.Invoke();

    /// <summary>
    /// Fires the event when phone is picked up (e.g., to play audio or show message).
    /// </summary>
    public static void RaisePhonePickedUp() => PhonePickedUp?.Invoke();
}
