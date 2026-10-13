using UnityEngine;

public class Pontuacao : MonoBehaviour
{
    public int pontos = 0;
    public int vidas = 3;

    public void CapturarFantasma()
    {
        pontos += 200;
        Debug.Log("Fantasma capturado! Pontos: " + pontos);
    }

    public void QuebrarBloco()
    {
        pontos += 100;
        Debug.Log("Bloco quebrado! Pontos: " + pontos);
    }

    public void PerderVida()
    {
        vidas--;
        pontos -= 75;

        Debug.Log("Vida perdida! Vidas: " + vidas);
        Debug.Log("Pontuação atual: " + pontos);

        if (vidas <= 0)
        {
            Debug.Log("Fim de jogo!");
        }
    }
}