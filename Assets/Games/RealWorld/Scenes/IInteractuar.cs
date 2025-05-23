using UnityEngine;

public interface IInteractuar
{
    string InteractuarID { get; }
    GameObject GameObject { get; }
    void Interactuar();
}

public interface IAgarrar : IInteractuar
{
    bool Combinar(IAgarrar objeto);
    void Usar();
}

public interface ISocket : IInteractuar
{
    bool PlacearObjeto(IAgarrar objeto);
}