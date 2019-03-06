﻿using System;
using System.Collections;
using System.Collections.Generic;
using audience.game;
using audience.messages;
using Newtonsoft.Json;
using SocketIO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace audience
{

    public partial class NetworkManager : MonoBehaviour
    {

        private SocketIOComponent _Socket;

        /// <summary>
        /// Ensures that messages are contextualized.
        /// Only relevant messages are received in the current scene.
        /// </summary>
        private Dictionary<string, Delegate> _MessageFunctionMapper;
        
        #region Unity API

        void Start()
        {
            var socketComponent = GetComponent<SocketIOComponent>();
            var hostAddress = PlayerPrefs.GetString(PlayerPrefsKeys.HOST_ADDRESS) + SocketInfo.SUFFIX_ADDRESS;
            Debug.Log("Host address is: " + hostAddress);
            socketComponent.url = hostAddress;
            socketComponent.Start();
            socketComponent.Connect();

            _MessageFunctionMapper = new Dictionary<string, Delegate>()
            {
                { SceneNames.Lobby, (Action<Base>)OnLobbyMessage },
                { SceneNames.Game, (Action<Base>)OnGameMessage },
            };

            _Socket = GetComponent<SocketIOComponent>();
            
            _Socket.On(Command.MESSAGE, OnMessage);

            LobbyStart();
            GameStart();
        }

        #endregion

        #region Emit

        public void ExitRoom()
        {
            _Socket.Emit(Command.DISCONNECT);
        }

        #endregion

        #region Receive

        private void OnMessage(SocketIOEvent e)
        {
            var content = JsonConvert.DeserializeObject<Base>(e.data.ToString());
            var currentSceneName = SceneManager.GetActiveScene().name;
            
            _MessageFunctionMapper[currentSceneName]?.DynamicInvoke(content);
        }

        #endregion
    }
}

