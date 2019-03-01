﻿using System.Collections;
using System.Collections.Generic;
using audience.messages;
using SocketIO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.UI;
using WebSocketSharp;

namespace audience.lobby
{

    public class LobbyManager : MonoBehaviour
    {
        #region Private Attributes

        [SerializeField]
        private InputField _RoomPinInputField;

        [SerializeField]
        private InputField _NameInputField;

        [SerializeField]
        private Button[] _ColorButtons;

        [SerializeField]
        private Canvas _Canvas;

        [SerializeField]
        private GameObject _ErrorOverlayPrefab;

        private NetworkManager _NetworkManager;

        #endregion

        #region Unity API

        void Start()
        {
            _NetworkManager = FindObjectOfType<NetworkManager>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadSceneAsync(SceneNames.TitleScreen);
            }
        }

        #endregion

        #region Custom Methods

        private void InstantiateErrorOverlay(string error)
        {
            var instance = Instantiate(_ErrorOverlayPrefab, _Canvas.transform);
            var errorOverlay = instance.GetComponent<Overlay>();
            errorOverlay.Description = error;
            errorOverlay.Primary += () => { Destroy(instance.gameObject); };
        }

        public void OnJoinButtonClick()
        {
            if (_NameInputField.text.IsNullOrEmpty())
            {
                InstantiateErrorOverlay(StringLitterals.ERROR_NO_NAME);
                return;
            }
            ViewerInfo.Name = _NameInputField.text;

            if (_RoomPinInputField.text.IsNullOrEmpty())
            {
                InstantiateErrorOverlay(StringLitterals.ERROR_NO_PIN);
                return;
            }

            var asInt = int.Parse(_RoomPinInputField.text);
            _NetworkManager.EmitJoinGame(asInt);
        }


        // TODO: Refactor this (dynamically add buttons and their listeners)
        // once the UIs are established.
        public void OnRedClick() { ViewerInfo.Color = Color.red; }
        public void OnBlueClick() { ViewerInfo.Color = Color.blue; }
        public void OnGreenClick() { ViewerInfo.Color = Color.green; }
        public void OnYellowClick() { ViewerInfo.Color = Color.yellow; }

        #endregion
    }

}
