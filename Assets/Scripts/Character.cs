using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected float _speed = 2f;
    
    public float Speed => _speed;
    public Vector3 Velocity { get; protected set; }
}