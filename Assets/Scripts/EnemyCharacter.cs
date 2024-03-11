using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private Transform _head;
    
    private Vector3 _targetPosition = Vector3.zero;
    private float _velocityMagnitude = 0;

    public Vector3 TargetPosition => _targetPosition;

    private void Start()
    {
        _targetPosition = transform.position;
    }

    private void Update()
    {
        if (_velocityMagnitude > 0.1f)
        {
            float maxDistance = _velocityMagnitude * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, maxDistance);
        }
        else
        {
            transform.position = _targetPosition;
        }
    }

    public void SetSpeed(float value) => _speed = value;

    public void SetRotateX(float value)
    {
        _head.localEulerAngles = new Vector3(value, 0, 0);
    }

    public void SetRotateY(float value)
    {
        transform.localEulerAngles = new Vector3(0, value, 0);
    }

    public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval)
    {
        _targetPosition = position + (velocity * averageInterval);
        _velocityMagnitude = velocity.magnitude;
        
        Velocity = velocity;
    }
}