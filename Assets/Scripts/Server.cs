using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts
{
    public class Server : MonoBehaviour
    {

        public int Port = 1300;
        private bool _isServerStarted;
        
        void Update()
        {
            if (!_isServerStarted)
            {
                return;
            }










        }

        public void StartGame()
        {
            DisplayBoardSpaces();

            //TODO: server
            //            if (_isHostPlayer)
            //            {
            AssignMonstersToPlayers();

        }

        private void AssignMonstersToPlayers()
        {
            /*
            List<Node> player1StartingNodes = new List<Node>
            {
                GameGraph.Nodes[23],
                GameGraph.Nodes[24],
                GameGraph.Nodes[13],
                GameGraph.Nodes[14]
            };

            List<Node> player2StartingNodes = new List<Node>
            {
                GameGraph.Nodes[17],
                GameGraph.Nodes[18],
                GameGraph.Nodes[19],
                GameGraph.Nodes[20]
            };

            List<Monster> availableMonsters = new List<Monster>(MonsterPrefabs);
            while (availableMonsters.Any())
            {
                int monsterIndex = _random.Next(0, availableMonsters.Count);

                Monster currentMonster = availableMonsters[monsterIndex];

                Monster monsterPrefab = MonsterPrefabs[MonsterPrefabs.IndexOf(currentMonster)];

                Quaternion monsterQuaternion;

                if (availableMonsters.Count % 2 == 0)
                {
                    currentMonster.CurrentNode = player1StartingNodes[0];
                    player1StartingNodes.RemoveAt(0);
                    monsterQuaternion = Quaternion.Euler(monsterPrefab.transform.rotation.eulerAngles.x, monsterPrefab.transform.rotation.eulerAngles.y + 180, monsterPrefab.transform.rotation.eulerAngles.z);
                    Monster monsterInstance =
                        Instantiate(monsterPrefab,
                            new Vector3(currentMonster.CurrentNode.XPosition, 0, currentMonster.CurrentNode.YPosition),
                            monsterQuaternion) as Monster;
                    if (monsterInstance != null)
                    {
                        monsterInstance.BelongsToHost = true;
                        monsterInstance.CurrentNode = currentMonster.CurrentNode;
                        Player1Monsters.Add(monsterInstance);
                    }
                }
                else
                {
                    currentMonster.CurrentNode = player2StartingNodes[0];
                    player2StartingNodes.RemoveAt(0);
                    monsterQuaternion = monsterPrefab.transform.rotation;
                    Monster monsterInstance =
                        Instantiate(monsterPrefab,
                            new Vector3(currentMonster.CurrentNode.XPosition, 0, currentMonster.CurrentNode.YPosition),
                            monsterQuaternion) as Monster;
                    if (monsterInstance != null)
                    {
                        monsterInstance.BelongsToHost = false;
                        monsterInstance.CurrentNode = currentMonster.CurrentNode;
                        Player2Monsters.Add(monsterInstance);
                    }
                }


                availableMonsters.RemoveAt(monsterIndex);

            }
            */
        }

        private void DisplayBoardSpaces()
        {
            /*

            for (int i = 0; i < SpacePrefabs.Count; i++)
            {
                BoardSpace spacePrefab = SpacePrefabs[i];
                float yAngleOffset = 30 * ((i - 1) % 12);
                Quaternion spaceQuaternion = Quaternion.Euler(spacePrefab.transform.rotation.eulerAngles.x, spacePrefab.transform.rotation.eulerAngles.y + yAngleOffset, spacePrefab.transform.rotation.eulerAngles.z);
                if (!BoardSpaces.ContainsKey(i))
                {
                    BoardSpace space =
                        Instantiate(spacePrefab,
                            new Vector3(spacePrefab.transform.position.x, spacePrefab.transform.position.y - .005f,
                                spacePrefab.transform.position.z), spaceQuaternion) as BoardSpace;
                    if (space != null)
                    {
                        space.Node = GameGraph.Nodes[i];

                        BoardSpaces.Add(i, space);
                    }
                }
            }
            */
        }


        public void Init(string ipAddress = "127.0.0.1")
        {
            DontDestroyOnLoad(gameObject);

            try
            {
                NetworkServer.Listen(ipAddress, Port);

                _isServerStarted = true;
            }
            catch (Exception e)
            {
                Debug.Log("Server error: " + e.Message);
            }
        }

        public void SendToAll(string data, short messageTypeId = 0)
        {
            NetworkServer.SendToAll(messageTypeId, new StringMessage(data));
        }

        public void SendToClient(string data, Client client, short messageTypeId = 0)
        {
            try
            {
                NetworkServer.SendToClient(client.connection.connectionId, messageTypeId, new StringMessage(data));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ShutDown()
        {
            NetworkServer.DisconnectAll();
            NetworkServer.Shutdown();
        }

        private void OnConnected(NetworkMessage netMsg)
        {
            Debug.Log("Client has been connected to host");
            if (NetworkServer.connections.Count == 2)
            {
                NetworkServer.SendToAll(MsgType.Scene, new StringMessage("dejarik"));
            }

        }

    }
}
