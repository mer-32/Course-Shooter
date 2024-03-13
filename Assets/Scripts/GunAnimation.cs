using UnityEngine;

public class GunAnimation: MonoBehaviour
{
    [SerializeField] private Gun _gun;
    [SerializeField] private Animator _animator;
    
    private static readonly int Shoot = Animator.StringToHash("Shoot");

    private void Start()
    {
        _gun.Shooted += OnShoot;
    }
    
    private void OnDestroy()
    {
        _gun.Shooted -= OnShoot;
    }

    private void OnShoot()
    {
        _animator.SetTrigger(Shoot);
    }
}