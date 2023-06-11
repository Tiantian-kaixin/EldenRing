using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour {
    // Start is called before the first frame update
    private NetworkVariable<Vector3> pos = new NetworkVariable<Vector3>(new Vector3(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> test = new NetworkVariable<bool>(false);
    private float speed = 5;
    void Start() {

    }
    // Update is called once per frame
    void Update() {
        // use rpc
        if (IsOwner) {
            UpdateTransform();
        }
        // use variable
        //if (IsOwner) {
        //    UpdateTransform();
        //    pos.Value = transform.position;
        //} else {
        //    transform.position = pos.Value;
        //}
    }

    private void UpdateTransform() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        transform.position += new Vector3(x, 0, z) * Time.deltaTime * speed;
        testServerRpc(NetworkManager.Singleton.LocalClientId, z);
    }

    [ServerRpc]
    private void testServerRpc(ulong id, float z) {
        if (IsServer) {
            testClientRpc(id, z);
        }
    }
    [ClientRpc]
    private void testClientRpc(ulong id, float z) {
        if (id != NetworkManager.Singleton.LocalClientId) {
            transform.position += new Vector3(0, 0, z) * Time.deltaTime * speed;
        }
    }
}
