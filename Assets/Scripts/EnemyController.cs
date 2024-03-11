using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyCharacter _character;

    private List<float> _receiveTimeInterval = new List<float>() {0,0,0,0,0};
    private float _lastReceiveTime = 0f;
    private Player _player;
    private float AverageInterval
    {
        get
        {
            int receiveIntervalCount = _receiveTimeInterval.Count;
            float summ = 0;

            for (int i = 0; i < receiveIntervalCount; i++)
            {
                summ += _receiveTimeInterval[i];
            }

            return summ / receiveIntervalCount;
        }
    }

    public void Init(Player player)
    {
        _player = player;
        _character.SetSpeed(player.speed);
        player.OnChange += OnChange;
    }

    public void Destroy()
    {
        _player.OnChange -= OnChange;
        
        Destroy(gameObject);
    }
    
    public void OnChange(List<DataChange> changes)
    {
        SaveReceiveTime();
        
        Vector3 position = _character.TargetPosition;
        Vector3 velocity = Vector3.zero;

        foreach (DataChange dataChange in changes)
        {
            switch (dataChange.Field)
            {
                case "pX":
                    position.x = (float) dataChange.Value;
                    break;
                case "pY":
                    position.y = (float) dataChange.Value;
                    break;
                case "pZ":
                    position.z = (float) dataChange.Value;
                    break;
                case "vX":
                    velocity.x = (float) dataChange.Value;
                    break;
                case "vY":
                    velocity.y = (float) dataChange.Value;
                    break;
                case "vZ":
                    velocity.z = (float) dataChange.Value;
                    break;
                case "rX":
                    _character.SetRotateX((float) dataChange.Value);
                    break;
                case "rY":
                    _character.SetRotateY((float) dataChange.Value);
                    break;
                default:
                    Debug.LogWarning("Не обрабатывается изменение поля " + dataChange.Field);
                    break;
            }
        }

        _character.SetMovement(position, velocity, AverageInterval);
    }

    private void SaveReceiveTime()
    {
        float interval = Time.time - _lastReceiveTime;
        _lastReceiveTime = Time.time;
        
        _receiveTimeInterval.Add(interval);
        _receiveTimeInterval.RemoveAt(0);
    }

}