using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// Esc / Options (PS4) abre e fecha o pause.
public class PauseMenuController : MonoBehaviour
{
    [SerializeField] string mainMenuSceneName = "MainMenu";

    public static bool IsPaused { get; private set; }

    UIManager ui;
    UIScreen pauseScreen;
    GameObject canvasGo;
    bool paused;

    void Awake()
    {
        IsPaused = false;
        GameInput.Ensure();
        UIFactory.EnsureEventSystem();

        var canvas = UIFactory.CreateCanvas("PauseCanvas", 100);
        canvasGo = canvas.gameObject;
        UIFactory.CreateBackground(canvas.transform, 0.88f);
        UIFactory.CreateMushroomIcon(canvas.transform);

        ui = gameObject.AddComponent<UIManager>();
        ui.OnBackAtRoot = Resume;   // Círculo na tela de pause = continuar

        var settings = SettingsScreenBuilder.Build(canvas.transform, ui);
        pauseScreen = UIScreen.Create(canvas.transform, "Pausa", "PAUSADO", 96);
        pauseScreen.AddSpacer(30);
        pauseScreen.AddButton("CONTINUAR", Resume);
        pauseScreen.AddButton("CONFIGURAÇÕES", () => ui.Push(settings));
        pauseScreen.AddButton("MENU PRINCIPAL", ToMainMenu);

        canvasGo.SetActive(false);

        // Celular não tem Esc/Options: botão "II" no canto
        if (Application.isMobilePlatform)
        {
            var touch = UIFactory.CreateCanvas("TouchPauseCanvas", 50);
            var b = UIFactory.CreateButton(touch.transform, "II", Pause, 90f);
            var r = (RectTransform)b.transform;
            r.anchorMin = r.anchorMax = r.pivot = new Vector2(1f, 1f);
            r.anchoredPosition = new Vector2(-30f, -30f);
            r.sizeDelta = new Vector2(110f, 90f);
        }
    }

    void Start() => LockCursor(true);

    void Update()
    {
        if (!GameInput.Instance.PausePressed) return;
        if (!paused) Pause();
        else if (ui.Count > 1) ui.Pop();
        else Resume();
    }

    void Pause()
    {
        if (paused) return;
        paused = true;
        IsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(0f, 0f);
        canvasGo.SetActive(true);
        LockCursor(false);
        ui.Push(pauseScreen);
    }

    void Resume()
    {
        if (!paused) return;
        ui.Clear();
        canvasGo.SetActive(false);
        paused = false;
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        LockCursor(true);
    }

    void ToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        IsPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDestroy()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    static void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
