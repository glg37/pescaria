using UnityEngine;

public interface IFishing
{
    void Pegar();
    void Soltar();

    Vector3 GetPosicaoOriginal();
    Quaternion GetRotacaoOriginal();
    Vector3 GetEscalaOriginal();
}
