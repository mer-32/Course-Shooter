using UnityEngine;

public class CheckFly : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _radius;
    [SerializeField] private float _coyoteTime = 0.15f;

    private float _flyTimer = 0f;
    
    public bool IsFly { get; private set; }

    private void Update()
    {
        if (Physics.CheckSphere(transform.position, _radius, _layerMask))
        {
            IsFly = false;
            _flyTimer = 0f;
        }
        else
        {
            _flyTimer += Time.deltaTime;
            
            if(_flyTimer >= _coyoteTime) IsFly = true;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
#endif

}