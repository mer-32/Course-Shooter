using System.Collections.Generic;
using Colyseus;
using UnityEngine;

namespace Multiplayer
{
    public class MultiplayerManager : ColyseusManager<MultiplayerManager>
    {
        [SerializeField] private Skins _skins;
        [SerializeField] private SpawnPoints _spawnPoints;
        [SerializeField] private LossCounter _lossCounter;
        [SerializeField] private PlayerCharacter _player;
        [SerializeField] private EnemyController _enemy;

        private ColyseusRoom<State> _room;
        private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();

        public LossCounter LossCounter => _lossCounter;
        public SpawnPoints SpawnPoints => _spawnPoints;

        protected override void Start()
        {
            Instance.InitializeClient();
            Connect();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _room.Leave();
        }

        public void SendMessage(string key, Dictionary<string, object> data)
        {
            _room.Send(key, data);
        }

        public void SendMessage(string key, string data)
        {
            _room.Send(key, data);
        }

        public string GetSessionId() => _room.SessionId;

        private async void Connect()
        {
            _spawnPoints.GetPoint(Random.Range(0, _spawnPoints.Length), out Vector3 spawnPosition,
                out Vector3 spawnRotation);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"skins", _skins.Length},
                {"points", _spawnPoints.Length},
                {"speed", _player.Speed},
                {"hp", _player.MaxHealth},
                {"pX", spawnPosition.x},
                {"pY", spawnPosition.y},
                {"pZ", spawnPosition.z},
                {"rY", spawnRotation.y}
            };

            _room = await Instance.client.JoinOrCreate<State>("state_handler", data);

            _room.OnStateChange += OnStateChange;

            _room.OnMessage<string>("Shoot", ApplyShoot);
        }

        private void ApplyShoot(string jsonShootInfo)
        {
            ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(jsonShootInfo);

            if (_enemies.TryGetValue(shootInfo.Key, out EnemyController enemy))
            {
                enemy.Shoot(shootInfo);
            }
            else
            {
                Debug.LogError("Противника нет, а он пытался стрелять");
            }
        }

        private void OnStateChange(State state, bool isFirstState)
        {
            if (!isFirstState) return;

            state.players.ForEach((key, player) =>
            {
                if (key == _room.SessionId) CreatePlayer(player);
                else CreateEnemy(key, player);
            });

            _room.State.players.OnAdd += CreateEnemy;
            _room.State.players.OnRemove += RemoveEnemy;
        }

        private void CreatePlayer(Player player)
        {
            Vector3 position = new Vector3(player.pX, player.pY, player.pZ);

            Quaternion rotation = Quaternion.Euler(0, player.rY, 0);

            PlayerCharacter character = Instantiate(_player, position, rotation);
            player.OnChange += character.OnChange;

            Controller controller = character.GetComponent<Controller>();

            _room.OnMessage<int>("Restart", controller.Restart);
            
            character.GetComponent<SetSkin>().Set(_skins.GetMaterial(player.skin));
        }

        private void CreateEnemy(string key, Player player)
        {
            Vector3 position = new Vector3(player.pX, player.pY, player.pZ);

            EnemyController enemy = Instantiate(_enemy, position, Quaternion.identity);
            enemy.Init(key, player);
            enemy.GetComponent<SetSkin>().Set(_skins.GetMaterial(player.skin));

            _enemies.Add(key, enemy);
        }

        private void RemoveEnemy(string key, Player player)
        {
            if (_enemies.TryGetValue(key, out EnemyController enemy))
            {
                enemy.Destroy();
                _enemies.Remove(key);
            }
        }
    }
}