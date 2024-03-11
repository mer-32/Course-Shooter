using System.Collections.Generic;
using Colyseus;
using UnityEngine;

namespace Multiplayer
{
    public class MultiplayerManager : ColyseusManager<MultiplayerManager>
    {
        [SerializeField] private PlayerCharacter _player;
        [SerializeField] private EnemyController _enemy;

        private ColyseusRoom<State> _room;

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

        private async void Connect()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"speed", _player.Speed}
            };

            _room = await Instance.client.JoinOrCreate<State>("state_handler", data);

            _room.OnStateChange += OnStateChange;
        }

        private void OnStateChange(State state, bool isFirstState)
        {
            if (!isFirstState) return;
            
            state.players.ForEach((key, player) =>
            {
                if(key == _room.SessionId) CreatePlayer(player);
                else CreateEnemy(key, player);
            });

            _room.State.players.OnAdd += CreateEnemy;
            _room.State.players.OnRemove += RemoveEnemy;
        }

        private void CreatePlayer(Player player)
        {
            Vector3 position = new Vector3(player.pX, player.pY, player.pZ);

            Instantiate(_player, position, Quaternion.identity);
        }

        private void CreateEnemy(string key, Player player)
        {
            Vector3 position = new Vector3(player.pX, player.pY, player.pZ);
            
            EnemyController enemy = Instantiate(_enemy, position, Quaternion.identity);
            enemy.Init(player);
        }

        private void RemoveEnemy(string key, Player player)
        {
            
        }
    }
}