using UnityEngine;

/// Configurações do jogador salvas em PlayerPrefs.
public static class GameSettings
{
    const string KVol = "cfg_volume", KSens = "cfg_sens", KFull = "cfg_full", KVib = "cfg_vib";

    public static float Volume { get; set; } = 0.8f;
    public static float Sensitivity { get; set; } = 1f;
    public static bool Fullscreen { get; set; } = true;
    public static bool Vibration { get; set; } = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() { Load(); Apply(); }

    public static void Load()
    {
        Volume = PlayerPrefs.GetFloat(KVol, 0.8f);
        Sensitivity = PlayerPrefs.GetFloat(KSens, 1f);
        Fullscreen = PlayerPrefs.GetInt(KFull, 1) == 1;
        Vibration = PlayerPrefs.GetInt(KVib, 1) == 1;
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat(KVol, Volume);
        PlayerPrefs.SetFloat(KSens, Sensitivity);
        PlayerPrefs.SetInt(KFull, Fullscreen ? 1 : 0);
        PlayerPrefs.SetInt(KVib, Vibration ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void Apply()
    {
        AudioListener.volume = Volume;
#if !UNITY_ANDROID && !UNITY_IOS
        Screen.fullScreen = Fullscreen;
#endif
    }
}
