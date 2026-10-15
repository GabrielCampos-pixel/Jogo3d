using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// O aspirador: segure R2 (PS4) / botão esquerdo do mouse para sugar tudo que tenha o script Sugavel
/// dentro do cone à frente do "bocal". Gasta munição enquanto suga.
public class VacuumCleaner : MonoBehaviour
{
    [SerializeField] Transform bocal;               // normalmente a câmera
    [SerializeField] float alcance = 6f;
    [SerializeField] float anguloCone = 40f;
    [SerializeField] float municaoMaxima = 100f;
    [SerializeField] float consumoPorSegundo = 8f;

    public float Municao { get; private set; }
    public event Action<float> MunicaoMudou;

    IPlayerInput input;
    bool vibrando;
    readonly HashSet<Sugavel> alvos = new HashSet<Sugavel>();

    void Start()
    {
        GameInput.Ensure();
        input = GameInput.Instance;
        Municao = municaoMaxima;
        if (bocal == null) bocal = transform;
    }

    void Update()
    {
        bool sugando = !PauseMenuController.IsPaused && input.SuckHeld && Municao > 0f;

        if (sugando)
        {
            Sugar();
            Municao = Mathf.Max(0f, Municao - consumoPorSegundo * Time.deltaTime);
            MunicaoMudou?.Invoke(Municao);
        }
        SetVibracao(sugando);
    }

    void Sugar()
    {
        alvos.Clear();
        foreach (var hit in Physics.OverlapSphere(bocal.position, alcance))
        {
            var s = hit.GetComponentInParent<Sugavel>();
            if (s == null || alvos.Contains(s)) continue;
            Vector3 para = s.transform.position - bocal.position;
            if (Vector3.Angle(bocal.forward, para) > anguloCone * 0.5f) continue;
            alvos.Add(s);
        }
        foreach (var s in alvos) s.ReceberSucao(bocal.position, Time.deltaTime);
    }

    /// Chame isto quando o jogador pegar um cogumelo.
    public void AdicionarMunicao(float quantidade)
    {
        Municao = Mathf.Min(municaoMaxima, Municao + quantidade);
        MunicaoMudou?.Invoke(Municao);
    }

    void SetVibracao(bool ligar)
    {
        bool quer = ligar && GameSettings.Vibration;
        if (quer == vibrando || Gamepad.current == null) return;
        vibrando = quer;
        Gamepad.current.SetMotorSpeeds(quer ? 0.15f : 0f, quer ? 0.15f : 0f);
    }

    void OnDisable()
    {
        if (vibrando && Gamepad.current != null) Gamepad.current.SetMotorSpeeds(0f, 0f);
        vibrando = false;
    }

    // HUD provisório só para testar (depois vira o medidor 3D no aspirador)
    void OnGUI() => GUI.Label(new Rect(20, 20, 300, 30), "Munição: " + Mathf.CeilToInt(Municao));

    void OnDrawGizmosSelected()
    {
        var o = bocal != null ? bocal : transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(o.position, alcance);
    }
}
