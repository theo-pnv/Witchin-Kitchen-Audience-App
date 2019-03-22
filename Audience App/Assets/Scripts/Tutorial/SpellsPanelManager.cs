﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace audience.tutorial
{
    public class SpellsPanelManager : ATutorialPanelManager
    {
        [SerializeField] private GameObject _PrimaryPanel;
        [SerializeField] private GameObject _EventPanel;

        #region Custom Methods

        public void NextPage()
        {
            Instantiate(_EventPanel, _Canvas.transform).GetComponent<EventPanelManager>();
        }

        public void PreviousPage()
        {
            Instantiate(_PrimaryPanel, _Canvas.transform).GetComponent<PrimaryPanelManager>();
            Destroy(gameObject);
        }

        #endregion
    }
}