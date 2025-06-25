using UnityEngine;

public interface IInteractMV
{
    GameObject GameObject { get; }

    public void InteractMV();
    public void Highlight();
    public void Unhighlight();
}
