using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// Interface isolada: Criar outra implementação para VR e para o touch do celular
public interface IPlayerInput
{
    Vector2 Move { get; }
    Vector2 Look { get; }      // já em graus neste frame
    bool JumpPressed { get; }
    bool SuckHeld { get; }
    bool SprintHeld { get; }
}


/// Controle PS4 (DualShock 4): Cruz=pular, Círculo=voltar no menu, R2/R1=sugar,
/// L3=correr, Options=pausar, analógico esquerdo=andar, direito=olhar.
public class GameInput : MonoBehaviour, IPlayerInput
{
    public static GameInput Instance { get; private set; }

    public static void Ensure()
    {
        if (Instance == null) new GameObject("GameInput").AddComponent<GameInput>();
    }

    const float StickDegPerSec = 160f;
    const float MouseDegPerPixel = 0.1f;

    InputAction move, lookStick, lookMouse, jump, suck, sprint, pause, back;
    InputAction[] all;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Build();
    }

    void Build()
    {
        move = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
        move.AddBinding("<Gamepad>/leftStick");

        lookStick = new InputAction("LookStick", InputActionType.Value, expectedControlType: "Vector2");
        lookStick.AddBinding("<Gamepad>/rightStick");

        lookMouse = new InputAction("LookMouse", InputActionType.Value, expectedControlType: "Vector2");
        lookMouse.AddBinding("<Mouse>/delta");

        jump = new InputAction("Jump", InputActionType.Button);
        jump.AddBinding("<Gamepad>/buttonSouth");
        jump.AddBinding("<Keyboard>/space");

        suck = new InputAction("Suck", InputActionType.Button);
        suck.AddBinding("<Gamepad>/rightTrigger");
        suck.AddBinding("<Gamepad>/rightShoulder");
        suck.AddBinding("<Mouse>/leftButton");

        sprint = new InputAction("Sprint", InputActionType.Button);
        sprint.AddBinding("<Gamepad>/leftStickPress");
        sprint.AddBinding("<Keyboard>/leftShift");

        pause = new InputAction("Pause", InputActionType.Button);
        pause.AddBinding("<Gamepad>/start");      // botão Options do PS4
        pause.AddBinding("<Keyboard>/escape");

        back = new InputAction("Back", InputActionType.Button);
        back.AddBinding("<Gamepad>/buttonEast");  // Círculo

        all = new[] { move, lookStick, lookMouse, jump, suck, sprint, pause, back };
        foreach (var a in all) a.Enable();
    }

    void OnDestroy()
    {
        if (Instance != this || all == null) return;
        foreach (var a in all) { a.Disable(); a.Dispose(); }
        Instance = null;
    }

    // ---- IPlayerInput ----
    public Vector2 Move => move.ReadValue<Vector2>();
    public Vector2 Look =>
        lookStick.ReadValue<Vector2>() * StickDegPerSec * GameSettings.Sensitivity * Time.deltaTime +
        lookMouse.ReadValue<Vector2>() * MouseDegPerPixel * GameSettings.Sensitivity;
    public bool JumpPressed => jump.WasPressedThisFrame();
    public bool SuckHeld => suck.IsPressed();
    public bool SprintHeld => sprint.IsPressed();

    // ---- Menus ----
    public bool PausePressed => pause.WasPressedThisFrame();
    public bool BackPressed => back.WasPressedThisFrame();
}
