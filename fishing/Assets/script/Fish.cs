using UnityEngine;

public class Fish : MonoBehaviour
{
    public enum Raridade
    {
        Comum,
        Incomum,
        Raro,
        Épico,
        Lendário
    }

    public Raridade raridade = Raridade.Comum;
}