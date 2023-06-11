using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkHelp : MonoBehaviour {
    // Start is called before the first frame update
    public Button host;
    public Button client;
    void Start() {
        host.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        client.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }

}
