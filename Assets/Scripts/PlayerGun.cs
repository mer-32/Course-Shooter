using UnityEngine;
using System;

public class PlayerGun: Gun
{
    [SerializeField] private int _damage;
    [SerializeField] private Transform _bulletPoint;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _shootDelay;

    private float _lastShootDelay;
    
    public bool TryShoot(out ShootInfo info)
    {
        info = new ShootInfo();
        
        if(Time.time - _lastShootDelay < _shootDelay) return false;

        Vector3 position = _bulletPoint.position;
        Vector3 velocity = _bulletPoint.forward * _bulletSpeed;

        _lastShootDelay = Time.time;

        Bullet bullet = Instantiate(_bulletPrefab, position, _bulletPoint.rotation);
        bullet.Init(velocity, _damage);
        
        Shooted?.Invoke();

        info.pX = position.x;
        info.pY = position.y;
        info.pZ = position.z;
        info.dX = velocity.x;
        info.dY = velocity.y;
        info.dZ = velocity.z;
        
        return true;
    }
}

public abstract class Gun : MonoBehaviour
{
    [SerializeField] protected Bullet _bulletPrefab;

    public Action Shooted;
}