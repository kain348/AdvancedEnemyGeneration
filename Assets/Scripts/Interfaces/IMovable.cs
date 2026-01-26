using UnityEngine;

public interface IMovable
{
    bool IsMoving { get; }
    float Speed { get; }

    void Move(Vector3 direction);
    void Stop();
}