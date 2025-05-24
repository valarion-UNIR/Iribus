using UnityEngine;

public interface IInteract
{
    void Interact();

    void Hightlight(bool hightlight);
}

public interface IInteractObject : IInteract
{
    string InteractId { get; }
    GameObject GameObject { get; }
}

public interface IGrab : IInteractObject
{
    bool Combine(IGrab objeto);
    void Use();
}

public interface ISocket : IInteractObject
{
    bool PlaceObject(IGrab objeto);
}