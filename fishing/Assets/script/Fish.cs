using UnityEngine;

public class Fish : MonoBehaviour
{
    public enum Raridade
    {
        Comum,
        Incomum,
        Raro,
        �pico,
        Lend�rio
    }

    public Raridade raridade = Raridade.Comum;
}