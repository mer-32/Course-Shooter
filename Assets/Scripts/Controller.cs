using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _player;
    
    private float _inputH;
    private float _inputV;

    private void Update()
    {
        _inputH = Input.GetAxisRaw("Horizontal");
        _inputV = Input.GetAxisRaw("Vertical");
        
        _player.SetInput(_inputH, _inputV);
        
        SendMove();
    }

    private void SendMove()
    {
        _player.GetMoveInfo(out Vector3 position);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"x", position.x},
            {"y", position.z}
        };
        
        MultiplayerManager.Instance.SendMessage("move", data);
    }
}