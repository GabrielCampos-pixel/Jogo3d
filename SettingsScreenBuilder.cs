using UnityEngine;

/// Tela de Configurações (usada tanto no menu inicial quanto no pause).
public static class SettingsScreenBuilder
{
    public static UIScreen Build(Transform canvas, UIManager ui)
    {
        var s = UIScreen.Create(canvas, "Configuracoes", "CONFIGURAÇÕES", 64);
        s.AddSpacer(20);

        s.AddSlider("Volume", 0f, 1f, GameSettings.Volume, v =>
        {
            GameSettings.Volume = v;
            GameSettings.Apply();
        });

        s.AddSlider("Sensibilidade", 0.2f, 3f, GameSettings.Sensitivity, v => GameSettings.Sensitivity = v);

#if !UNITY_ANDROID && !UNITY_IOS
        s.AddToggle("Tela cheia", GameSettings.Fullscreen, on =>
        {
            GameSettings.Fullscreen = on;
            GameSettings.Apply();
        });
#endif

        s.AddToggle("Vibração do controle", GameSettings.Vibration, on => GameSettings.Vibration = on);

        s.AddSpacer(20);
        s.AddButton("VOLTAR", () => ui.Pop());

        s.Hidden += GameSettings.Save;   // salva sempre que sair da tela
        return s;
    }
}
