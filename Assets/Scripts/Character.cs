using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected float _speed = 2f;
    [SerializeField] protected float _maxHealth = 10;

    public float Speed => _speed;
    public float MaxHealth => _maxHealth;
    
    public Vector3 Velocity { get; protected set; }
}