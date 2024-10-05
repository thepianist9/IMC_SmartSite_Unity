using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Game.ServerAuthoritativeSynchronousSpawning
{
    /// <summary>
    /// A dynamic prefab loading use-case where the server instructs all clients to load a single network prefab, and
    /// will only invoke a spawn once all clients have successfully completed their respective loads of said prefab. The
    /// server will initially send a ClientRpc to all clients, begin loading the prefab on the server, will await
    /// acknowledgement of a load via ServerRpcs from each client, and will only spawn the prefab over the network once
    /// it has received an acknowledgement from every client, within m_SynchronousSpawnTimeoutTimer seconds.
    /// </summary>
    /// <remarks>
    /// This use-case is recommended for scenarios where you'd want to guarantee the same world version across all
    /// connected clients. Since the server will wait until all clients have loaded the same dynamic prefab, the spawn
    /// of said dynamic prefab will be synchronous. Thus, we recommend using this technique for spawning game-changing
    /// gameplay elements, assuming you'd want all clients to be able to interact with said gameplay elements from the
    /// same point forward. For example, you wouldn't want to have an enemy only be visible (network side and/or
    /// visually) to some clients and not others -- you'd want to delay the enemy's spawn until all clients have
    /// dynamically loaded it and are able to see it before spawning it server side.
    /// </remarks>
    public sealed class ServerAuthoritativeSynchronousSpawning : NetworkBehaviour
    {
        [SerializeField]
        NetworkManager m_NetworkManager;

        [SerializeField] List<AssetReferenceGameObject> m_DynamicPrefabReferences;

        const int k_MaxConnectedClientCount = 4;

        const int k_MaxConnectPayload = 1024;

        float m_SynchronousSpawnTimeoutTimer;

        int m_SynchronousSpawnAckCount = 0;

        [SerializeField] List<GameObject> m_DynamicSpawnedPrefabs;
        [SerializeField] SelectTransformGizmo m_SelectTransformGizmo;
        [SerializeField] private GameObject m_messageBox;
        [SerializeField] private AppController m_AppController;

        void Start()
        {
            DynamicPrefabLoadingUtilities.Init(m_NetworkManager);

            // In the use-cases where connection approval is implemented, the server can begin to validate a user's
            // connection payload, and either approve or deny connection to the joining client.
            // Note: we will define a very simplistic connection approval below, which will effectively deny all
            // late-joining clients unless the server has not loaded any dynamic prefabs. You could choose to not define
            // a connection approval callback, but late-joining clients will have mismatching NetworkConfigs (and  
            // potentially different world versions if the server has spawned a dynamic prefab).
            m_NetworkManager.NetworkConfig.ConnectionApproval = true;

            // Here, we keep ForceSamePrefabs disabled. This will allow us to dynamically add network prefabs to Netcode
            // for GameObject after establishing a connection.
            m_NetworkManager.NetworkConfig.ForceSamePrefabs = false;

            // This is a simplistic use-case of a connection approval callback. To see how a connection approval should
            // be used to validate a user's connection payload, see the connection approval use-case, or the
            // APIPlayground, where all post-connection techniques are used in harmony.
            m_NetworkManager.ConnectionApprovalCallback += ConnectionApprovalCallback;

        }

        public override void OnDestroy()
        {
            m_NetworkManager.ConnectionApprovalCallback -= ConnectionApprovalCallback;
            DynamicPrefabLoadingUtilities.UnloadAndReleaseAllDynamicPrefabs();
            base.OnDestroy();
        }

        void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            Debug.Log("Client is trying to connect " + request.ClientNetworkId);
            var connectionData = request.Payload;
            var clientId = request.ClientNetworkId;

            if (clientId == m_NetworkManager.LocalClientId)
            {
                // allow the host to connect
                Approve();
                return;
            }

            // A sample-specific denial on clients after k_MaxConnectedClientCount clients have been connected
            if (m_NetworkManager.ConnectedClientsList.Count >= k_MaxConnectedClientCount)
            {
                ImmediateDeny();
                return;
            }

            if (connectionData.Length > k_MaxConnectPayload)
            {
                // If connectionData is too big, deny immediately to avoid wasting time on the server. This is intended as
                // a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
                ImmediateDeny();
                return;
            }

            // simple approval if the server has not loaded any dynamic prefabs yet
            if (DynamicPrefabLoadingUtilities.LoadedPrefabCount == 0)
            {
                Approve();
            }
            else
            {
                ImmediateDeny();
            }

            void Approve()
            {
                Debug.Log($"Client {clientId} approved");
                response.Approved = true;
                response.CreatePlayerObject = false; //we're not going to spawn a player object for this sample
            }

            void ImmediateDeny()
            {
                Debug.Log($"Client {clientId} denied connection");
                response.Approved = false;
                response.CreatePlayerObject = false;
            }
        }

        // invoked by UI
        public void OnClickedTrySpawnSynchronously(int index)
        {
            Debug.Log($"AppController private space: {m_AppController.privateSpace}");
            if(!m_AppController.privateSpace)
            {
                var position = GameObject.FindGameObjectWithTag("Shared Space").transform.localPosition;
                var rotation = GameObject.FindGameObjectWithTag("Shared Space").transform.localRotation;

                //calculate offset and send to all clients
                if (!m_NetworkManager.IsServer)
                {
                    TrySpawnServerRpc
                        (index, position, rotation);
                }


                TrySpawnSynchronously(index, position, rotation);
            }
            else
            {
                m_messageBox.SetActive(true);
                m_messageBox.GetComponentInChildren<TMP_Text>().text = "You are in private space. Please switch to shared space to spawn objects";
            }

        }


        [ServerRpc(RequireOwnership = false)]
        void TrySpawnServerRpc(int index, Vector3 position, Quaternion rotation)
        {
            Debug.Log("Runnning ServerRpc on Server");
            if (!IsServer)
            {
                return;
            }

            TrySpawnSynchronously(index, position, rotation);
        }   


        async void TrySpawnSynchronously(int index, Vector3 position, Quaternion rotation)
        {
            var randomPrefab = m_DynamicPrefabReferences[index];
            await TrySpawnDynamicPrefabSynchronously(randomPrefab.AssetGUID, position, rotation);
        }


        /// <summary>
        /// This call attempts to spawn a prefab by it's addressable guid - it ensures that all the clients have loaded the prefab before spawning it,
        /// and if the clients fail to acknowledge that they've loaded a prefab - the spawn will fail.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        async Task<(bool Success, NetworkObject Obj)> TrySpawnDynamicPrefabSynchronously(string guid, Vector3 position, Quaternion rotation)
        {
            if (IsServer)
            {
                var assetGuid = new AddressableGUID()
                {
                    Value = guid
                };

                if (DynamicPrefabLoadingUtilities.IsPrefabLoadedOnAllClients(assetGuid))
                {
                    Debug.Log("Prefab is already loaded by all peers, we can spawn it immediately");
                    var obj = Spawn(assetGuid, position, rotation);
                    return (true, obj);
                }

                m_SynchronousSpawnAckCount = 0;
                m_SynchronousSpawnTimeoutTimer = 0;

                Debug.Log("Loading dynamic prefab on the clients...");
                LoadAddressableClientRpc(assetGuid);


                //load the prefab on the server, so that any late-joiner will need to load that prefab also
                await DynamicPrefabLoadingUtilities.LoadDynamicPrefab(assetGuid, 0);

                // server loaded a prefab, update UI with the loaded asset's name
                DynamicPrefabLoadingUtilities.TryGetLoadedGameObjectFromGuid(assetGuid, out var loadedGameObject);

                var requiredAcknowledgementsCount = IsHost ? m_NetworkManager.ConnectedClients.Count - 1 :
                    m_NetworkManager.ConnectedClients.Count;

                while (m_SynchronousSpawnTimeoutTimer < 3000)
                {
                    if (m_SynchronousSpawnAckCount >= requiredAcknowledgementsCount)
                    {
                        Debug.Log($"All clients have loaded the prefab in {m_SynchronousSpawnTimeoutTimer} seconds, spawning the prefab on the server...");
                        var obj = Spawn(assetGuid, position, rotation);
                        return (true, obj);
                    }

                    m_SynchronousSpawnTimeoutTimer += Time.deltaTime;
                    await Task.Yield();
                }

                // left to the reader: you'll need to be reactive to clients failing to load -- you should either have
                // the offending client try again or disconnect it after a predetermined amount of failed attempts
                Debug.LogError("Failed to spawn dynamic prefab - timeout");
                return (false, null);
            }

            return (false, null);

            NetworkObject Spawn(AddressableGUID assetGuid, Vector3 position, Quaternion rotation)
            {
                Transform sharedSpace = GameObject.FindGameObjectWithTag("Shared Space").transform;
                Vector3 pos = new Vector3(sharedSpace.position.x+position.x, sharedSpace.position.y+position.y + 0.5f, sharedSpace.position.z + position.z);
                if (!DynamicPrefabLoadingUtilities.TryGetLoadedGameObjectFromGuid(assetGuid, out var prefab))
                {
                    Debug.LogWarning($"GUID {assetGuid} is not a GUID of a previously loaded prefab. Failed to spawn a prefab.");
                    return null;
                }
                var obj = Instantiate(prefab.Result, sharedSpace);
                obj.transform.localPosition = pos;
                obj.transform.localRotation = rotation;  
                var networkObj = obj.GetComponent<NetworkObject>();
                networkObj.Spawn();
                networkObj.TrySetParent(sharedSpace);

                m_DynamicSpawnedPrefabs.Add(obj);


                Debug.Log("Spawned dynamic prefab");

                // every client loaded dynamic prefab, their respective ClientUIs in case they loaded first

                return networkObj;
            }
        }

        [ClientRpc]
        void LoadAddressableClientRpc(AddressableGUID guid, ClientRpcParams rpcParams = default)
        {
            if (!IsHost)
            {
                Load(guid);
            }

            async void Load(AddressableGUID assetGuid)
            { 
                Debug.Log("Loading dynamic prefab on the client...");
                await DynamicPrefabLoadingUtilities.LoadDynamicPrefab(assetGuid, 0);
                Debug.Log("Client loaded dynamic prefab");

                DynamicPrefabLoadingUtilities.TryGetLoadedGameObjectFromGuid(assetGuid, out var loadedGameObject);

                AcknowledgeSuccessfulPrefabLoadServerRpc(assetGuid.GetHashCode()); 
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void AcknowledgeSuccessfulPrefabLoadServerRpc(int prefabHash, ServerRpcParams rpcParams = default)
        {
            m_SynchronousSpawnAckCount++;
            Debug.Log($"Client acknowledged successful prefab load with hash: {prefabHash}");
            DynamicPrefabLoadingUtilities.RecordThatClientHasLoadedAPrefab(prefabHash,
                rpcParams.Receive.SenderClientId);

            // a quick way to grab a matching prefab reference's name via its prefabHash
            var loadedPrefabName = "Undefined";
            foreach (var prefabReference in m_DynamicPrefabReferences)
            {
                var prefabReferenceGuid = new AddressableGUID() { Value = prefabReference.AssetGUID };
                if (prefabReferenceGuid.GetHashCode() == prefabHash)
                {
                    // found the matching prefab reference
                    if (DynamicPrefabLoadingUtilities.LoadedDynamicPrefabResourceHandles.TryGetValue(
                            prefabReferenceGuid,
                            out var loadedGameObject))
                    {
                        // if it is loaded on the server, update the name on the ClientUI
                        loadedPrefabName = loadedGameObject.Result.name;
                    }
                    break;
                }
            }

        }

        public bool RequestOwnership (ulong clientId)
        {
            if (!IsServer && clientId != null)
            {
                Debug.Log("Requesting ownership from server");
                GrantOwnershipServerRPC(clientId);
                m_SelectTransformGizmo.ActivateEdit();
                return true;
                
            }
            else 
                return false;

        }

        public void RevokeOwnership()
        {
            if (IsServer)
            {
                foreach (var obj in m_DynamicSpawnedPrefabs)
                {
                    NetworkObject no = obj.GetComponent<NetworkObject>();
                    no.RemoveOwnership();
                }
                m_SelectTransformGizmo.ActivateEdit();
                
            }
            else
            {
                DisableEditClientRPC();
            }
        }

        [ClientRpc]
        private void DisableEditClientRPC()
        {
            m_SelectTransformGizmo.DeactivateEdit();
        }

        [ServerRpc(RequireOwnership = false)]

        private void GrantOwnershipServerRPC(ulong clientId)
        {
            if (IsServer)
            {
                Debug.Log($"Granting ownership to client: {clientId}");
                foreach (var obj in m_DynamicSpawnedPrefabs)
                {
                    NetworkObject no = obj.GetComponent<NetworkObject>();
                    no.ChangeOwnership(clientId);
                }
            }
        }
    }
}
