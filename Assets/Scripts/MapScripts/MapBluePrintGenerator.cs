using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public enum RoomState
{
    Empty,
    StartingRoom,
    PossibleRoom,
    Room,
    EndRoom,
    NoRoom
}

public class PossibleRoom
{
    public Vector2Int coords;
    public Vector2Int previousRoomcoords;

    public PossibleRoom(Vector2Int coords, Vector2Int previousRoomcoords)
    {
        this.coords = coords;
        this.previousRoomcoords = previousRoomcoords;
    }
}

public class Room
{
    public Vector2Int coords;
    public RoomState roomState;
    public Room(Vector2Int coords, RoomState roomState)
    {
        this.coords = coords;
        this.roomState = roomState;
    }
}

public class MapBluePrintGenerator : MonoBehaviour
{
    [SerializeField]
    private int Size = 10; //maximum length and height of rooms
    private int sizeMinusOne; //length of roomsheet - 1
    private RoomState[,] RoomSheet;

    [SerializeField]
    private int minRooms = 0, maxRooms = 0; //the minimum and maximum for rooms to spawn
    private int TotalRooms = 0; //random number between minimum and maximum for Total Number Of Rooms
    private int RoomCount; //local Counter for all the rooms left to spawn

    [SerializeField]
    private bool SeperateCave = true;

    [SerializeField]
    private float SeperateChanceIncrement = 10f;
    private float ChanceToSeperate = 0f;

    [HideInInspector]
    static public List<Room> roomNodes;

    void Awake()
    {
        TotalRooms = UnityEngine.Random.Range(minRooms, maxRooms + 1);
        roomNodes = new List<Room>();
        RoomCount = TotalRooms;
        sizeMinusOne = Size - 1;
        RoomSheet = EmptyAllRooms(Size);
        GenerateRooms(RoomSheet);
        //DebugLogAllRoomPlaces(RoomSheet);
    }

    //Declare all rooms in the roomsheet as empty
    public RoomState[,] EmptyAllRooms(int size)
    {
        RoomState[,] roomSheet = new RoomState[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                roomSheet[x, y] = RoomState.Empty;
            }
        }
        return roomSheet;
    }


    public RoomState[,] GenerateRooms(RoomState[,] roomSheet)
    {
        List<PossibleRoom> PossibleRooms = new List<PossibleRoom>(); //store all possible room locations

        //pick a random corner of the blueprint for a starting room 
        //mark the neighbour rooms as possible locations
        switch (UnityEngine.Random.Range(0, 4))
        {
            case 0:
                //top left
                roomSheet[0, 0] = RoomState.StartingRoom;
                roomNodes.Add(new Room(
                    new Vector2Int(0, 0),
                    RoomState.StartingRoom));
                roomSheet[1, 0] = RoomState.PossibleRoom;
                roomSheet[0, 1] = RoomState.PossibleRoom;
                PossibleRooms.Add(
                    new PossibleRoom(
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 0)));
                PossibleRooms.Add(
                    new PossibleRoom(
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 0)));
                break;
            case 1:
                //top right
                roomSheet[sizeMinusOne, 0] = RoomState.StartingRoom;
                roomNodes.Add(new Room(
                    new Vector2Int(sizeMinusOne, 0),
                    RoomState.StartingRoom));
                roomSheet[sizeMinusOne - 1, 0] = RoomState.PossibleRoom;
                roomSheet[sizeMinusOne, 1] = RoomState.PossibleRoom;
                PossibleRooms.Add(
                    new PossibleRoom(
                    new Vector2Int(sizeMinusOne - 1, 0),
                    new Vector2Int(sizeMinusOne, 0)));
                PossibleRooms.Add(
                    new PossibleRoom(
                    new Vector2Int(sizeMinusOne, 1),
                    new Vector2Int(sizeMinusOne, 0)));
                break;
            case 2:
                //bottom right
                roomSheet[sizeMinusOne, sizeMinusOne] = RoomState.StartingRoom;
                roomNodes.Add(new Room(
                    new Vector2Int(sizeMinusOne, sizeMinusOne),
                    RoomState.StartingRoom));
                roomSheet[sizeMinusOne - 1, sizeMinusOne] = RoomState.PossibleRoom;
                roomSheet[sizeMinusOne, sizeMinusOne - 1] = RoomState.PossibleRoom;
                PossibleRooms.Add(
                    new PossibleRoom(
                    new Vector2Int(sizeMinusOne - 1, sizeMinusOne),
                    new Vector2Int(sizeMinusOne, sizeMinusOne)));
                PossibleRooms.Add(
                    new PossibleRoom(
                    new Vector2Int(sizeMinusOne, sizeMinusOne - 1),
                    new Vector2Int(sizeMinusOne, sizeMinusOne)));
                break;
            case 3:
                //bottom left
                roomSheet[0, sizeMinusOne] = RoomState.StartingRoom;
                roomNodes.Add(new Room(
                    new Vector2Int(0, sizeMinusOne),
                    RoomState.StartingRoom));
                roomSheet[1, sizeMinusOne] = RoomState.PossibleRoom;
                PossibleRooms.Add(
                    new PossibleRoom(
                    new Vector2Int(1, sizeMinusOne),
                    new Vector2Int(0, sizeMinusOne)));
                roomSheet[0, sizeMinusOne - 1] = RoomState.PossibleRoom;
                PossibleRooms.Add(
                    new PossibleRoom(
                    new Vector2Int(0, sizeMinusOne - 1),
                    new Vector2Int(0, sizeMinusOne)));
                break;
        }
        RoomCount--;

        //Loop Til All Rooms Are Generated
        if (!SeperateCave)
        {
            while (RoomCount > 1)
            {
                int test = PickNextRoom(1);
                //Debug.LogError("break outside of function");
                if (test == 0) {
                    if (RoomCount > TotalRooms / 2){
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Restart
                        break;
                    }
                    else 
                        break;
                }
                RoomCount--;
            }
        }
        else
        {
            while (RoomCount > 1)
            {
                int roomsToSpawn = 0;
                if (ChanceToSeperate > 100)
                {
                    roomsToSpawn = Convert.ToInt32(ChanceToSeperate / 100);
                    ChanceToSeperate = ChanceToSeperate - Convert.ToInt32(ChanceToSeperate / 100) * 100;
                }
                roomsToSpawn += UnityEngine.Random.Range(0, 100f) >= ChanceToSeperate ? 1 : 2;
                int roomsSpawned = PickNextRoom(roomsToSpawn);
                if (roomsSpawned > 1)
                {
                    ChanceToSeperate = 0;
                    RoomCount -= roomsSpawned;
                }
                else if (roomsSpawned == 1)
                {
                    ChanceToSeperate += SeperateChanceIncrement;
                    RoomCount--;
                }
                else
                {
                    Debug.LogError("No Room Spawned");
                    if (RoomCount > TotalRooms / 2) {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Restart
                        break;
                    }
                    break;
                }
            }
        }
        PickEndRoom();

        return roomSheet;

        //pick random possible room to turn into a room and the rooms next to the previous room into no room spaces
        int PickNextRoom(int roomCount = 1)
        {
            while (PossibleRooms.Count < roomCount)
            {
                roomCount--;
                if (roomCount > 0) continue;
                return 0;
            }
            int successfullyPickedRooms = 0;

            //create all roooms
            PossibleRoom[] roomsToSpawn = new PossibleRoom[roomCount];
            for (int i = 0; i < roomCount; i++)
            {
                roomsToSpawn[i] = PossibleRooms[UnityEngine.Random.Range(0, PossibleRooms.Count)];
                roomSheet[roomsToSpawn[i].coords.x, roomsToSpawn[i].coords.y] = RoomState.Room;
                roomNodes.Add(new Room(
                    new Vector2Int(roomsToSpawn[i].coords.x, roomsToSpawn[i].coords.y),
                    RoomState.Room));
                successfullyPickedRooms++;
                PossibleRooms.Remove(roomsToSpawn[i]);
            }

            //repeat for every room
            for (int i = 0; i < roomCount; i++)
            {
                //turn all possible rooms adjacent to the previous room into no rooms 
                DisableRoom(new Vector2Int(roomsToSpawn[i].previousRoomcoords.x - 1, roomsToSpawn[i].previousRoomcoords.y));
                DisableRoom(new Vector2Int(roomsToSpawn[i].previousRoomcoords.x + 1, roomsToSpawn[i].previousRoomcoords.y));
                DisableRoom(new Vector2Int(roomsToSpawn[i].previousRoomcoords.x, roomsToSpawn[i].previousRoomcoords.y - 1));
                DisableRoom(new Vector2Int(roomsToSpawn[i].previousRoomcoords.x, roomsToSpawn[i].previousRoomcoords.y + 1));
                //turn all empty adjacent rooms into possible rooms
                MakePossibleRoom(new Vector2Int(roomsToSpawn[i].coords.x - 1, roomsToSpawn[i].coords.y), i);
                MakePossibleRoom(new Vector2Int(roomsToSpawn[i].coords.x + 1, roomsToSpawn[i].coords.y), i);
                MakePossibleRoom(new Vector2Int(roomsToSpawn[i].coords.x, roomsToSpawn[i].coords.y - 1), i);
                MakePossibleRoom(new Vector2Int(roomsToSpawn[i].coords.x, roomsToSpawn[i].coords.y + 1), i);
            }

            void DisableRoom(Vector2Int coords)
            {
                if (coords.x < 0 || coords.x >= Size || coords.y < 0 || coords.y >= Size) return;

                if (roomSheet[coords.x, coords.y] == RoomState.PossibleRoom)
                {
                    roomSheet[coords.x, coords.y] = RoomState.NoRoom;
                    PossibleRooms.RemoveAll(room => room.coords == coords);
                }
            }


            void MakePossibleRoom(Vector2Int coords, int roomNumber)
            {
                if (coords.x < 0 || coords.x >= Size || coords.y < 0 || coords.y >= Size) return;
                if (roomSheet[coords.x, coords.y] == RoomState.Empty)
                {
                    roomSheet[coords.x, coords.y] = RoomState.PossibleRoom;
                    PossibleRooms.Add(
                    new PossibleRoom(
                    new Vector2Int(coords.x, coords.y),
                    new Vector2Int(roomsToSpawn[roomNumber].coords.x, roomsToSpawn[roomNumber].coords.y)));
                }
            }
            return successfullyPickedRooms;
        }

        void PickEndRoom() {
            
            if (PossibleRooms.Count > 0)
            {
                PossibleRoom pEndRoom = PossibleRooms[UnityEngine.Random.Range(0, PossibleRooms.Count)];
                roomSheet[pEndRoom.coords.x, pEndRoom.coords.y] = RoomState.EndRoom;
                roomNodes.Add(new Room(
                    new Vector2Int(pEndRoom.coords.x, pEndRoom.coords.y),
                    RoomState.EndRoom));
                PossibleRooms.Clear();
                return;
            }
            Room endRoom = roomNodes[roomNodes.Count - 1];
            roomNodes.RemoveAt(roomNodes.Count - 1);
            roomSheet[endRoom.coords.x, endRoom.coords.y] = RoomState.EndRoom;
            roomNodes.Add(new Room(
                new Vector2Int(endRoom.coords.x, endRoom.coords.y),
                RoomState.EndRoom));
            PossibleRooms.Clear();

            RoomCount--;
        }
    }

    //Write Debug Logs Of All Rooms and their states
    public void DebugLogAllRoomPlaces(RoomState[,] roomSheet)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                sb.Append(GetRoomSymbol(roomSheet[x, y]));
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());

        char GetRoomSymbol(RoomState state)
        {
            return state switch
            {
                RoomState.Empty => 'E',
                RoomState.StartingRoom => 'S',
                RoomState.PossibleRoom => 'P',
                RoomState.Room => 'R',
                RoomState.EndRoom => 'X',
                RoomState.NoRoom => 'N',
                _ => '?'
            };
        }
    }
}
