using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MapBluePrintGenerator))]
public class RoomSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] startingRoomTypes;
    [SerializeField]
    private GameObject[] roomTypes;
    [SerializeField]
    private GameObject[] endRoomTypes;

    public float time;

    void Awake()
    {
        StartCoroutine(GenerateRooms());
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    public IEnumerator GenerateRooms()
    {
        foreach (Room room in MapBluePrintGenerator.roomNodes)
        {
            switch (room.roomState)
            {
                case RoomState.StartingRoom:
                    Instantiate(
                        startingRoomTypes[UnityEngine.Random.Range(0, startingRoomTypes.Length)],
                        new Vector3(room.coords.x, 0, -room.coords.y),
                        Quaternion.identity
                        );
                    yield return new WaitForSeconds(time);
                    break;
                case RoomState.Room:
                    Instantiate(
                            roomTypes[UnityEngine.Random.Range(0, startingRoomTypes.Length)],
                            new Vector3(room.coords.x, 0, -room.coords.y),
                            Quaternion.identity
                            );
                        yield return new WaitForSeconds(time);
                    break;
                case RoomState.EndRoom:
                    Instantiate(
                            endRoomTypes[UnityEngine.Random.Range(0, startingRoomTypes.Length)],
                            new Vector3(room.coords.x, 0, -room.coords.y),
                            Quaternion.identity
                            );
                        yield return new WaitForSeconds(time);
                    break;
            }
        }
    }
}
