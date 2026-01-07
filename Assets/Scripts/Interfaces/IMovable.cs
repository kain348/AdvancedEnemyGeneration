using UnityEngine;

public interface IMovable
{
    void Move(Vector3 direction);
    void Stop();
    bool IsMoving { get; }
    float Speed { get; }
}