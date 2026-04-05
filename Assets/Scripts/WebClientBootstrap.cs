using System;
using UnityEngine;

public class WebClientBootstrap : MonoBehaviour
{
    public enum ClientMode
    {
        Auto,
        Desktop,
        Mobile
    }

    [Serializable]
    private class ClientProfile
    {
        public string mode;
        public bool hasTouchPoints;
        public bool coarsePointer;
        public int viewportWidth;
        public int viewportHeight;
        public float devicePixelRatio;
    }

    [SerializeField] private ClientMode editorFallbackMode = ClientMode.Desktop;

    public static WebClientBootstrap Instance { get; private set; }

    public static event Action<ClientMode, string> ClientModeChanged;

    public ClientMode CurrentMode { get; private set; } = ClientMode.Auto;
    public string LastProfileJson { get; private set; } = string.Empty;
    public bool HasRuntimeClientMode { get; private set; }

    public bool IsMobileMode
    {
        get
        {
            if (HasRuntimeClientMode)
            {
                return CurrentMode == ClientMode.Mobile;
            }

            return editorFallbackMode == ClientMode.Mobile;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (!HasRuntimeClientMode && Application.isEditor)
        {
            CurrentMode = editorFallbackMode;
        }
    }

    public void SetClientMode(string rawMode)
    {
        ClientMode parsedMode;
        if (!TryParseMode(rawMode, out parsedMode))
        {
            Debug.LogWarning("[WebClientBootstrap] Unsupported client mode: " + rawMode);
            return;
        }

        ApplyClientMode(parsedMode, LastProfileJson, true);
    }

    public void SetClientProfileJson(string json)
    {
        string profileJson = json ?? string.Empty;

        if (string.IsNullOrEmpty(profileJson))
        {
            return;
        }

        ClientProfile profile = null;

        try
        {
            profile = JsonUtility.FromJson<ClientProfile>(profileJson);
        }
        catch (ArgumentException)
        {
            Debug.LogWarning("[WebClientBootstrap] Invalid client profile JSON.");
        }

        if (profile == null)
        {
            return;
        }

        ClientMode parsedMode;
        if (!TryParseMode(profile.mode, out parsedMode))
        {
            Debug.LogWarning("[WebClientBootstrap] Client profile is missing a valid mode.");
            return;
        }

        ApplyClientMode(parsedMode, profileJson, true);
    }

    private void ApplyClientMode(ClientMode mode, string profileJson, bool markRuntimeValue)
    {
        bool hasChanged = CurrentMode != mode || LastProfileJson != profileJson || HasRuntimeClientMode != markRuntimeValue;

        CurrentMode = mode;
        LastProfileJson = profileJson ?? string.Empty;
        HasRuntimeClientMode = markRuntimeValue;

        if (!hasChanged)
        {
            return;
        }

        Debug.Log("[WebClientBootstrap] Client mode set to " + CurrentMode + ".");
        ClientModeChanged?.Invoke(CurrentMode, LastProfileJson);
    }

    private static bool TryParseMode(string rawMode, out ClientMode parsedMode)
    {
        parsedMode = ClientMode.Auto;

        if (string.IsNullOrWhiteSpace(rawMode))
        {
            return false;
        }

        if (string.Equals(rawMode, "desktop", StringComparison.OrdinalIgnoreCase))
        {
            parsedMode = ClientMode.Desktop;
            return true;
        }

        if (string.Equals(rawMode, "mobile", StringComparison.OrdinalIgnoreCase))
        {
            parsedMode = ClientMode.Mobile;
            return true;
        }

        if (string.Equals(rawMode, "auto", StringComparison.OrdinalIgnoreCase))
        {
            parsedMode = ClientMode.Auto;
            return true;
        }

        return false;
    }
}
