
// This file is provided under The MIT License as part of RiptideNetworking.
// Copyright (c) 2022 Tom Weiland
// For additional information please see the included LICENSE.md file or view it on GitHub: https://github.com/tom-weiland/RiptideNetworking/blob/main/LICENSE.md

using UnityEngine;
using UnityEngine.UI;

namespace RiptideDemos.RudpTransport.Unity.PlayerHosted
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _singleton;
        public static UIManager Singleton
        {
            get => _singleton;
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else if (_singleton != value)
                {
                    Debug.Log($"{nameof(UIManager)} instance already exists, destroying object!");
                    Destroy(value);
                }
            }
        }

        string ip;
        int port;

        private void Awake()
        {
            Singleton = this;
            HostClicked();
        }

        public void HostClicked()
        {

            NetworkManager.Singleton.StartHost();
        }

        public void JoinClicked()
        {


            NetworkManager.Singleton.JoinGame(Settings.ip);

        }

        public void LeaveClicked()
        {
            NetworkManager.Singleton.LeaveGame();
        }

    }
}
