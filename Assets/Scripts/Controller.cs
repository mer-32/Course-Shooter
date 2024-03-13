using System;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private PlayerGun _gun;
    [SerializeField] private float _mouseSensetivity = 2f;

    private MultiplayerManager _multiplayerManager;

    private void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
    }

    private void Update()
    {
        float inputH = Input.GetAxisRaw("Horizontal");
        float inputV = Input.GetAxisRaw("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        bool isShoot = Input.GetMouseButton(0);

        bool space = Input.GetKeyDown(KeyCode.Space);

        _player.SetInput(inputH, inputV, mouseX * _mouseSensetivity);
        _player.RotateX(-mouseY * _mouseSensetivity);
        
        if(space) _player.Jump();
        
        if (isShoot && _gun.TryShoot(out ShootInfo shootInfo)) SendShoot(ref shootInfo);
        
        SendMove();
    }

    private void SendShoot(ref ShootInfo shootInfo)
    {
        shootInfo.Key = _multiplayerManager.GetSessionId();
        string json = JsonUtility.ToJson(shootInfo);
        
        _multiplayerManager.SendMessage("shoot", json);
    }

    private void SendMove()
    {
        _player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"pX", position.x},
            {"pY", position.y},
            {"pZ", position.z},
            {"vX", velocity.x},
            {"vY", velocity.y},
            {"vZ", velocity.z},
            {"rX", rotateX},
            {"rY", rotateY},
        };

        _multiplayerManager.SendMessage("move", data);
    }
}

[Serializable]
public struct ShootInfo
{
    public string Key;
    public float pX;
    public float pY;
    public float pZ;
    public float dX;
    public float dY;
    public float dZ;
}