using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    /*	*Expand – means to generate a connecting room with its own exits, adding those exits to a list for further expansion.
        Step 1: Generate starting room and exits
        Step 2: Expand dungeon through one valid exit, add other exits to a list. 
        Step 3: Continue expanding dungeon through a single new valid exit until randomly chosen distance to middle is reached.
        Step 4: Generate middle room with exits.
        Step 5: Continue expanding dungeon through a single new valid exit until 	randomly chosen distance from middle to end is reached.
        Step 6: Continue expanding dungeon from all other exits added to original list.
        Step 7: choose and delete some percentage of superfluous rooms that lead to 	the same end room.
        Step 8: Generate dungeon room visuals (doors, decorations, etc.)	
        Step 9: populate rooms with interacting items.
    Step 10: populate rooms with enemies.
    Step 11: spawn player in start room. 
    */

    int mRoomCount = 0;  // this increments with each new room and is used to assign room IDs
    Vector2 mNorth = new Vector2(0,1), mSouth = new Vector2(0,-1), mEast = new Vector2(1,0), mWest = new Vector2(-1, 0);

    public GameObject mDefaultRoom;

    List<Room> DungeonRooms = new List<Room>();
    List<Door> ExpandingDoors = new List<Door>();
    Dictionary<Vector2, Room> roomPositions = new Dictionary<Vector2, Room>();
    DungeonTiles tileGenerator;

    Room mStartRoom;
    int mMainPathMin = 10, mMainPathMax = 15;

    private void Awake()
    {
        GenerateDungeon();
        tileGenerator = GetComponent<DungeonTiles>();
        foreach (Room r in DungeonRooms)
        {
//          print("Tiling Room " + r);
            tileGenerator.GenerateDungeonTiles(r);
        }
    }

    void GenerateDungeon()
    {
        int countToMid, countToEnd;
        int dungeonSpan = Random.Range(mMainPathMin, mMainPathMax);
        countToMid = Random.Range(mMainPathMin / 2, mMainPathMax / 2);
        countToEnd = dungeonSpan - countToMid;
        bool goingRight = (Random.value > 0.5);               // choose left or right as the primary direction for the start to mid direction
        //create starting room, add it to dungeon rooms list
        mStartRoom = CreateRoom(Vector2.zero);
        // add doors to start room, then call expand making sure rooms only expand 'away' from start room
        ExpandMainPathDoors(mStartRoom, goingRight);
        for (int i = 0; i < countToMid; i++)
        {                                                      // until a distance to middle room has been reached
            Room newRoom = ExpandRoom();
         
            ExpandMainPathDoors(newRoom, goingRight);
        }
        // then build the middle room, which should be a minimum of four room units combined.
        BuildBigRoom(4, 4, goingRight, false);
        // then continue expanding from middle room, making sure rooms only expand 'away' from middle
        goingRight = (Random.value > 0.5);
        for(int i = 0; i < countToEnd; i++)                     // once a distance to end room is reached, build end room
        {
            Room newRoom = ExpandRoom();

            ExpandMainPathDoors(newRoom, goingRight);
        }

        BuildBigRoom(3, 3, goingRight, true);
        // then continue expanding from doors that have not yet been visited, adding branches
        // until a number of iterations has been met.

        // might need to refine dungeon, removing some percentage of superfluous rooms.
    }

    
    void BuildBigRoom(int width, int height, bool goingRight, bool isEndRoom)
    {
        Room[,] rList = new Room[width, height];
        // get last door on expanding door list
        // pathToEndRoom = randomly choose an int between 0 and width. this will be the  room the path to end starts at
        int pathToEnd = Random.Range(0, width-1);   // randomly chosen index for the room that will begin the path to end room

        for(int w = 0; w < width; w++)
        {

            for(int h = 0; h < height; h++)
            {
                // expand room
                rList[w, h] = ExpandRoom();
                // if h == 0, add door (goingRight) ? right : left
                if (h == 0)
                    AddDoorToFront(rList[w, h], (goingRight) ? mEast : mWest);
                // if(h < height - 1, add door going up
                if (h < height - 1)
                    AddDoorToFront(rList[w, h], mNorth);
            }
        }
        // NEED TO DO : merge rooms
        if (!isEndRoom)
        {
            // get exit room to be more random
            AddDoorToFront(rList[pathToEnd, height - 1], mNorth);   // adding main path exit to front of expanding door list
            // go through rList and add doors to end of expanding door list on exterior

            // add a big room from expanding room of size height/width
            // add doors on the exterior of the big room to end of expandingDoors list
            // adding one to the front of the list
        }
    }
    Room CreateRoom(Vector2 rPos)
    {
        if (IsSpaceAvailable(rPos))
        {
            //Room newRoom = new Room(mDefaultRoom, rPos, mRoomCount++) as Room; // room count used to set the room id, and then increment room count so next set id will be different
            GameObject newRoomObj = new GameObject();
            Room newRoom = newRoomObj.AddComponent<Room>() as Room;
            newRoom.SetupRoom(mDefaultRoom, rPos, mRoomCount++);
            DungeonRooms.Add(newRoom);
            return newRoom;
        }
        else
            return null;
    }

    void MergeRooms(Room [] R)
    {   // merge all rooms in the passed array by giving them all the same roomID. this room ID will be used when adding sprites
        foreach (Room r in R)
        {
            r.SetRoomID(R[0].GetRoomID());
        }
    }

    void ExpandMainPathDoors(Room r, bool goingRight)
    {
        if(Random.value > 0.5)
        {
            AddDoorToFront(r, (goingRight) ? mEast : mWest);
            AddDoorToEnd(r, mNorth);
        }
        else
        {
            AddDoorToFront(r, mNorth);
            AddDoorToEnd(r, (goingRight) ? mEast : mWest);
        }
        AddDoorToEnd(r, (goingRight) ? mWest : mEast);
        AddDoorToEnd(r, mSouth);
    }

    void RandomAddDoorsToEnd(Room r, Vector2[] dirs)
    {
        // randomize order of dirs
        for(int i = 0; i < dirs.Length; i++)
        {
            AddDoorToEnd(r, dirs[i]);
        }
    }
    void AddDoorToFront(Room r, Vector2 dir)
    {
        Door newDoor = CreateDoor(r, dir);
        ExpandingDoors.Insert(0, newDoor);
    }

    void AddDoorToEnd(Room r, Vector2 dir)
    {
        Door newDoor = CreateDoor(r, dir);
        ExpandingDoors.Insert(ExpandingDoors.Count, newDoor);
    }

    Door CreateDoor(Room r, Vector2 dir)
    {
        GameObject newDoorObj = new GameObject();
        Door newDoor = newDoorObj.AddComponent<Door>() as Door;
        newDoor.Setup(r, dir);
        return newDoor;
    }
    void ExpandDoors(Room r, Room origin)
    {
        // origin room passes either the starting room, or the mid room
        // this origin room position is used to make sure main path doesn't backtrack
        // r is the room being expanded
    
    }

    void ExpandDoors(Room r)
    {
        //expand doors from given room r

    }

    Room ExpandRoom()
    {
        Door nextDoor = ExpandingDoors[0];                                                    // get most recent door from expanding doors list
  
        Room newRoom = CreateRoom(nextDoor.GetNewPos());    // place room based on door retrieved
        ExpandingDoors.RemoveAt(0);
        return newRoom;
  
    }

    void Refine()
    {
        // this may be unnecessary if I don't allow expanding rooms to generate if they would be adjacent to 3 other rooms.
    }

    bool IsSpaceAvailable(Vector2 pos)
    {   // whenever a room is generated, it's unit position is added tot he roomPositions dictionary
        if (roomPositions.ContainsKey(pos)) // if unit position exists, we know that particular space is taken
            return false;
        return true;
    }

}
