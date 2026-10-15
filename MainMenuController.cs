using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    [SerializeField] string gameSceneName = "Game";

    UIManager ui;

    void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GameInput.Ensure();
        UIFactory.EnsureEventSystem();

        var canvas = UIFactory.CreateCanvas("MainMenuCanvas", 0);
        UIFactory.CreateBackground(canvas.transform, 1f);
        UIFactory.CreateMushroomIcon(canvas.transform);   // cogumelo no canto inferior

        ui = gameObject.AddComponent<UIManager>();
        var settings = SettingsScreenBuilder.Build(canvas.transform, ui);

        var main = UIScreen.Create(canvas.transform, "Principal", "MARIO HORROR", 110,
            "uma casa. um aspirador. uma vingança.");
        main.Title.gameObject.AddComponent<UIFlicker>();
        main.AddSpacer(40);
        main.AddButton("JOGAR", () => SceneManager.LoadScene(gameSceneName));
        main.AddButton("CONFIGURAÇÕES", () => ui.Push(settings));
        main.AddButton("SAIR", Quit);

        ui.Push(main);
    }

    void Update()
    {
        // Esc / Options também voltam uma tela
        if (GameInput.Instance != null && GameInput.Instance.PausePressed && ui.Count > 1) ui.Pop();
    }

    static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
