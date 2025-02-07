﻿using System;
using audience.messages;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;
using Key = audience.PlayerPrefsKeys;

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
        private GameObject _JoinInputField;

        [SerializeField]
        private Canvas _Canvas;

        [SerializeField]
        private GameObject _ErrorOverlayPrefab;

        private NetworkManager _NetworkManager;

        private Effects _effects;

        #endregion

        #region Unity API

        void Start()
        {
            _effects = new Effects(2, 0.95f, 0.9f);
            _NetworkManager = FindObjectOfType<NetworkManager>();
            _NetworkManager.OnMessageReceived += OnMessageReceivedFromServer;
            _NetworkManager.OnReceivedVoteForIngredient += OnReceivedVoteForIngredient;
            if (PlayerPrefs.HasKey(Key.USER_NICKNAME))
            {
                _NameInputField.text = PlayerPrefs.GetString(Key.USER_NICKNAME);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadSceneAsync(SceneNames.TitleScreen);
            }

            if (_NameInputField.text.IsNullOrEmpty() == false && _RoomPinInputField.text.IsNullOrEmpty() == false)
            {
                _effects.GrowShrink(_JoinInputField.transform);
            }
        }

        void OnDisable()
        {
            _NetworkManager.OnMessageReceived -= OnMessageReceivedFromServer;
            _NetworkManager.OnReceivedVoteForIngredient -= OnReceivedVoteForIngredient;
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
            else
            {
                PlayerPrefs.SetString(Key.USER_NICKNAME, _NameInputField.text);
            }
            ViewerInfo.Name = _NameInputField.text;

            if (_RoomPinInputField.text.IsNullOrEmpty())
            {
                InstantiateErrorOverlay(StringLitterals.ERROR_NO_PIN);
                return;
            }

            if (!_NetworkManager.IsConnectedToServer)
            {
                InstantiateErrorOverlay(StringLitterals.ERROR_SERVER_UNREACHABLE);
            }

            try
            {
                var asInt = int.Parse(_RoomPinInputField.text);
                _NetworkManager.EmitJoinGame(asInt);
            }
            catch (Exception e)
            {
                InstantiateErrorOverlay(StringLitterals.ERROR_WRONG_PIN);
            }
        }

        public void OnBackButtonClick()
        {
            SceneManager.LoadSceneAsync(SceneNames.TitleScreen);
        }

        void OnReceivedVoteForIngredient(IngredientPoll ingredientPoll)
        {
            TransmitIngredientPoll.Instance = ingredientPoll;
            TransmitIngredientPoll.WasAskedToVote = true;
        }

        void OnMessageReceivedFromServer(Base message)
        {
            if ((int)message.code % 10 == 0) // Success codes always have their unit number equal to 0 (cf. protocol)
            {
                Debug.Log(message.content);
                switch (message.code)
                {
                    case Code.register_viewer_success:
                        SceneManager.LoadSceneAsync(SceneNames.Game);
                        break;
                }
            }
            else
            {
                Debug.LogError(message.content);
                switch (message.code)
                {
                    case Code.join_game_error:
                        InstantiateErrorOverlay(StringLitterals.ERROR_WRONG_PIN);
                        break;
                }
            }
        }

        #endregion
    }

}
