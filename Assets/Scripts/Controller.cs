using System;
using System.Collections;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private PlayerGun _gun;
    [SerializeField] private float _mouseSensetivity = 2f;

    private MultiplayerManager _multiplayerManager;
    private bool _hold = false;
    private bool _hideCursor;

    private void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
        _hideCursor = true;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _hideCursor = !_hideCursor;

            Cursor.lockState = _hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        if(_hold) return;
        
        float inputH = Input.GetAxisRaw("Horizontal");
        float inputV = Input.GetAxisRaw("Vertical");

        float mouseX = 0;
        float mouseY = 0;
        bool isShoot = false;
        
        if (_hideCursor)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            isShoot = Input.GetMouseButton(0); 
        }

        bool space = Input.GetKeyDown(KeyCode.Space);

        _player.SetInput(inputH, inputV, mouseX * _mouseSensetivity);
        _player.RotateX(-mouseY * _mouseSensetivity);
        
        if(space) _player.Jump();
        
        if (isShoot && _gun.TryShoot(out ShootInfo shootInfo)) SendShoot(ref shootInfo);
        
        SendMove();
    }

    public void Restart(int spawnIndex)
    {
        _multiplayerManager.SpawnPoints.GetPoint(spawnIndex, out Vector3 position, out  Vector3 rotation);
        
        StartCoroutine(HoldRoutine());

        _player.transform.position = position;

        rotation.x = 0;
        rotation.z = 0;
        _player.transform.eulerAngles = rotation;
        _player.SetInput(0,0,0);
        
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"pX", position.x},
            {"pY", position.y},
            {"pZ", position.z},
            {"vX", 0},
            {"vY", 0},
            {"vZ", 0},
            {"rX", 0},
            {"rY", rotation.y},
        };

        _multiplayerManager.SendMessage("move", data);
    }
    
    private IEnumerator HoldRoutine()
    {
        _hold = true;
        yield return new WaitForSecondsRealtime(_restartDelay);
        _hold = false;
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

[Serializable]
public struct RestartInfo
{
    public float X;
    public float Z;
}