using System;
using System.Collections;
using System.Collections.Generic;
using audience;
using audience.game;
using audience.messages;
using UnityEngine;
using UnityEngine.UI;

namespace audience.game
{
    public class SpellsPanelManager : APanelManager
    {
        // Selection
        [Header("Selection")]
        [SerializeField] private GameObject _SelectionPanel;
        [SerializeField] private ScrollRect _ScrollView;
        [SerializeField] private GameObject _ScrollContent;
        [SerializeField] private GameObject _CardPrefab;
        [SerializeField] private Text _RemainingTimeTextSelection;
        [SerializeField] private Text _SelectionTitle;

        // Mini Game
        [Header("Mini Game")] public string Title;
        [SerializeField] private GameObject _MiniGamePanel;
        [SerializeField] private Text _MiniGameTitle;
        [SerializeField] private Image _PotionImage;
        [SerializeField] private Text _RemainingTextMiniGame;
        private int _NbTouches = 0;

        // Common
        private Text _RemainingText;
        private int _RemainingTime = 20;
        private NetworkManager _NetworkManager;
        public bool AuthorizeCasting;

        //Effect
        private Effects _effects;

        void Start()
        {
            _effects = new Effects(2, 0.8f, 0.8f);
            _NetworkManager = FindObjectOfType<NetworkManager>();
            if (_NetworkManager)
            {
                _NetworkManager.OnMessageReceived += OnMessageReceivedFromServer;
            }

            foreach (var spell in Spells.EventList)
            {
                GenerateCard(spell.Value);
            }

            if (AuthorizeCasting)
            {
                Handheld.Vibrate();
                _RemainingText = _RemainingTextMiniGame;
                _MiniGameTitle.text = Title;
                InvokeRepeating("Timer", 0, 1);
                _SelectionTitle.text = "Choose a spell!";
            }
            else
            {
                _MiniGamePanel.SetActive(false);
                _SelectionPanel.SetActive(true);
                _RemainingTimeTextSelection.text = "You'll be able to cast a spell when a player completes a potion!";
                _SelectionTitle.text = "Browse spells";
            }
        }

        void OnDisable()
        {
            _NetworkManager.OnMessageReceived -= OnMessageReceivedFromServer;
        }

        void Update()
        {
            if (_MiniGamePanel.activeInHierarchy)
            {
                MiniGame();
            }
        }

        #region Custom Methods

        public override void ExitScreen()
        {
            FindObjectOfType<GameManager>().IsChoosingASpell = false;
            base.ExitScreen();
        }

        private void Timer()
        {
            if (_RemainingTime < 0)
            {
                ExitScreen();
            }
            _RemainingText.text = "Remaining time to choose a spell: " + _RemainingTime + " seconds";
            --_RemainingTime;
        }

        void GenerateCard(Type spellType)
        {
            var cardInstance = Instantiate(_CardPrefab);
            cardInstance.transform.SetParent(_ScrollContent.transform, false);
            cardInstance.AddComponent(spellType);

            var spellManager = cardInstance.GetComponent<ISpell>();
            spellManager.SetNetworkManager(_NetworkManager);

            var spellCardManager = cardInstance.GetComponent<SpellCardManager>();
            spellCardManager.AuthorizeCasting = AuthorizeCasting;
            spellCardManager.RectoTitle.text = spellManager.GetTitle();
            spellCardManager.RectoDescription.text = spellManager.GetDescription();
            spellCardManager.CastSpellAction += spellManager.OnCastButtonClick;
            var sprite = Resources.Load<Sprite>(spellManager.GetSpritePath());
            if (sprite)
            {
                spellCardManager.RectoSprite.sprite = sprite;
                spellCardManager.VersoSprite.sprite = sprite;
            }
        }

        public void OnBackButtonClick()
        {
            ExitScreen();
        }

        private void OnMessageReceivedFromServer(Base content)
        {
            if ((int)content.code % 10 == 0) // Success codes always have their unit number equal to 0 (cf. protocol)
            {
                Debug.Log(content.code + ": " + content.content);
                switch (content.code)
                {
                    case Code.spell_casted_success:
                        ExitScreen();
                        break;
                }
            }
        }


        private void MiniGame()
        {
            _effects.GrowShrink(_PotionImage.transform, _NbTouches);

            // NO MORE TOUCHING THIS @VICTOR, I'M FORCED TO GO BACK INTO THIS PIECE OF CODE EACH TIME YOU CHANGED IT.
            // THANKS
            if (Application.isEditor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _NbTouches++;
                    if (_NbTouches >= 3)
                    {
                        EndMinigame();
                        _NbTouches = 0;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began && touch.phase != TouchPhase.Canceled)
                {
                    _NbTouches++;

                    if (_NbTouches >= 3)
                    {
                        EndMinigame();
                        _NbTouches = 0;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void EndMinigame()
        {
            _RemainingTimeTextSelection.gameObject.SetActive(true);
            _RemainingText = _RemainingTimeTextSelection;
            _MiniGamePanel.SetActive(false);
            _SelectionPanel.SetActive(true);
            _SelectionTitle.text = AuthorizeCasting
                ? "You can now choose a spell!"
                : "Browse spells";
        }

        #endregion
    }
}
