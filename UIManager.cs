using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// Controla as telas de UI usando uma PILHA (Stack<UIScreen>).
/// Push(tela)  -> esconde a tela do topo e mostra a nova (ex.: Menu -> Configurações)
/// Pop()       -> remove o topo e mostra de novo a tela anterior (botão Voltar / Círculo)
public class UIManager : MonoBehaviour
{
    readonly Stack<UIScreen> stack = new Stack<UIScreen>();

    public int Count => stack.Count;
    public UIScreen Current => stack.Count > 0 ? stack.Peek() : null;

    /// Chamado quando o jogador aperta "voltar" estando na tela raiz (ex.: fechar o pause).
    public Action OnBackAtRoot;

    public void Push(UIScreen screen)
    {
        if (stack.Count > 0) stack.Peek().Hide();
        stack.Push(screen);
        screen.Show();
    }

    public void Pop()
    {
        if (stack.Count == 0) return;
        stack.Pop().Hide();
        if (stack.Count > 0) stack.Peek().Show();
    }

    public void Clear()
    {
        while (stack.Count > 0) stack.Pop().Hide();
    }

    void Update()
    {
        if (stack.Count == 0 || GameInput.Instance == null) return;

        if (GameInput.Instance.BackPressed)
        {
            if (stack.Count > 1) Pop();
            else OnBackAtRoot?.Invoke();
        }

        // Se o jogador clicou no vazio com o mouse, o controle perde a seleção: devolve ao mexer no D-pad/analógico.
        var es = EventSystem.current;
        var pad = Gamepad.current;
        if (es != null && pad != null && es.currentSelectedGameObject == null &&
            Current != null && Current.FirstSelected != null &&
            (pad.dpad.ReadValue().sqrMagnitude > 0.25f || pad.leftStick.ReadValue().sqrMagnitude > 0.25f))
        {
            es.SetSelectedGameObject(Current.FirstSelected);
        }
    }
}
