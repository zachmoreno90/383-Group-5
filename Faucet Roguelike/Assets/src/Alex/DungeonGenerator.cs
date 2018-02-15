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

        For simplifying generation, I am using Unit Position in this class.
        The room class then handles scaling these unit positions to the size of a room. For example, a 1x1 room is 15x15 units in unity scene.
        As far as this class is concerned, a room that is actually 15x15 centered at position (0, 30) in scene looks like a 1x1 room at (0,2)
        i'm calling this 1x1 a unit size, and this (0,2) the unit position
    */

    int mRoomCount = 0;  // this increments with each new room and is used to assign room IDs
    Vector2 mNorth = new Vector2(0,1), mSouth = new Vector2(0,-1), mEast = new Vector2(1,0), mWest = new Vector2(-1, 0);

    public GameObject mDefaultRoom;

    List<Room> DungeonRooms = new List<Room>();
    List<Door> ExpandingDoors = new List<Door>();
    List<Vector2> mMainPathTraveledDirs = new List<Vector2>();

    DungeonTiles tileGenerator;

    Room mStartRoom;
    int mMainPathMin = 8, mMainPathMax = 12;                                        // eventually needs to be modified by difficulty/level
    float minMainPathPortion = 0.25f;
    private void Awake()
    {
        SetupMainPathDirs();
        GenerateDungeon();
        tileGenerator = GetComponent<DungeonTiles>();
        foreach (Room r in DungeonRooms)
        {
//          print("Tiling Room " + r);
            tileGenerator.GenerateDungeonTiles(r);
        }
    }

    void SetupMainPathDirs()
    {   // set up mMainPathTraveledDirs to include all cardinal directions
        // these directions are removed when used along the main path
        // when only one is left, main path can't travel that direction
        mMainPathTraveledDirs.Add(mNorth);
        mMainPathTraveledDirs.Add(mSouth);
        mMainPathTraveledDirs.Add(mWest);
        mMainPathTraveledDirs.Add(mEast);
    }

    void GenerateDungeon()
    {
        int countToMid, countToEnd;
        int dungeonSpan = Random.Range(mMainPathMin, mMainPathMax);
        int minDist = Mathf.CeilToInt(minMainPathPortion * dungeonSpan);

        countToMid = Random.Range(minDist, dungeonSpan - minDist);
        countToEnd = dungeonSpan - countToMid;
        bool goingRight = (Random.value > 0.5);               // choose left or right as the primary direction for the start to mid direction
        //create starting room, add it to dungeon rooms list
        mStartRoom = CreateRoom(Vector2.zero);

        // TESTING BUILD BIG ROOM ABOVE
        // add doors to start room, then call expand making sure rooms only expand 'away' from start room
        ExpandMainPathDoors(mStartRoom);
        for (int i = 0; i < countToMid; i++)
        {                                                      // until a distance to middle room has been reached
            Room newRoom = ExpandRoom();
         
            if(i != countToMid - 1)
            ExpandMainPathDoors(newRoom);
        }

        BuildBigRoom();

        // then build the middle room, which should be a minimum of four room units combined.
       // call build big room, passing height/depth, and that this is not the end room
       // the DungeonRooms list can be used to get the last room added to know where the big room is coming from

        // continue expanding from middle room, choosing a side other side main path entered middle room, and other than
        // direction denied to main path if a single direction still exists in mMainPathTraveledDirs
        // once distance to end room is reached, built end room
        // call build big room, passing height/depth, and that this is the end room

        // randomly shuffle the doors remaining in the expandingDoors list
        // then starting at the first door in the list, start expanding branches from those doors.

        // might need to refine dungeon, removing some percentage of superfluous rooms.
        
    }
    void BuildBigRoom()
    {
        List<Room> mergingRooms = new List<Room>();
        Vector2[] dirs = GetRandomDirections();
        int chosenDir = 0;
        Room lastRoom = DungeonRooms[DungeonRooms.Count - 1];

        Vector2 perp = new Vector2(dirs[0].y, -dirs[0].x);
        bool bigRoomCreated = true;

        do
        {
            bigRoomCreated = true;
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = dirs[chosenDir];
                if (i == 2)
                    dir = perp;
                else if (i == 3)
                    dir *= -1;
                AddDoorToFront(lastRoom, dir);
                lastRoom = ExpandRoom();
                if (lastRoom == null)
                    bigRoomCreated = false;
                else
                    mergingRooms.Add(lastRoom);

                if (!bigRoomCreated)
                {
                    Application.LoadLevel(Application.loadedLevelName);
                    foreach (Room r in mergingRooms)
                    {
                        DungeonRooms.Remove(r);
                    }
                    mergingRooms.Clear();
                    chosenDir++;
                    lastRoom = DungeonRooms[DungeonRooms.Count - 1];
                }
            }
        } while (!bigRoomCreated && chosenDir < 4);
        MergeRooms(mergingRooms);

        // get random exit door position/direction for continuing main path, add to front of door list

    }

   
    Room CreateRoom (Room curRoom, Door door)
    {
        // curRoom give it a door at position based on door
        Room newRoom = CreateRoom(door.GetNewPos());
        // give newRoom a door at position inverse of door
        return newRoom;
    }

    Room CreateRoom(Room curRoom, Vector2 dir)
    {
        Vector2 newPos = curRoom.GetUnitPos() + dir;
        Room newRoom = CreateRoom(newPos);
        return newRoom;
    }

    Room CreateRoom(Vector2 rPos)
    {
        if (isSpaceAvailable(rPos))
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


    void MergeRooms(List<Room> R)
    {   // merge all rooms in the passed array by giving them all the same roomID. this room ID will be used when adding sprites
        foreach (Room r in R)
        {
            if (r != R[0])
            {
                R[0].MergeWithRoom(r);
                DungeonRooms.Remove(r);
            }
        }
    }


    void ExpandMainPathDoors(Room r)
    {
        Vector2[] dirs = GetRandomDirections();
        for (int i = 0; i < 4; i++)
        {
            if(isSpaceAvailable(r, dirs[i]) && (mMainPathTraveledDirs.Count > 1 || !mMainPathTraveledDirs.Contains(dirs[i])))
            {
                AddDoorToFront(r, dirs[i]);
                if (mMainPathTraveledDirs.Count > 1)
                    mMainPathTraveledDirs.Remove(dirs[i]);

                for(i++; i < 4; i++)
                {   // add remaining doors to end
                    AddDoorToEnd(r, dirs[i]);
                }
                return;
            }

        }
        print("FAILED TO ADD MAIN PATH DOOR");
        /*
        // what I used to do
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
        */
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

    Room ExpandRoom()
    {
        Door nextDoor = ExpandingDoors[0];                                                    // get most recent door from expanding doors list
        ExpandingDoors.RemoveAt(0);

        Room newRoom = CreateRoom(nextDoor.GetNewPos());    // place room based on door retrieved
        
        return newRoom;
  
    }

    Room ExpandBranch()
    {
        Door nextDoor = ExpandingDoors[0];                                                    // get most recent door from expanding doors list
        ExpandingDoors.RemoveAt(0);
        // If position of new room will be adjacent to 2 or less other rooms, then create new room but not two rooms that are diagonally adjacent?
        Room newRoom = CreateRoom(nextDoor.GetNewPos());    // place room based on door retrieved
        return newRoom;

//        else return null
    }


    bool isSpaceAvailable(Room r, Vector2 dir)
    {
        return isSpaceAvailable(r.GetUnitPos() + dir);
    }

    bool isSpaceAvailable(Vector2 pos)
    {   
        foreach (Room r in DungeonRooms)
        {
            if (r.ContainsPos(pos))                 // returns true if room containst he Unit Position pos
                return false;
        }
        return true;
    }

    Room GetRoomAtPos(Vector2 pos)
    {
        foreach (Room r in DungeonRooms)
        {
            if (r.ContainsPos(pos))                 // returns true if room containst he Unit Position pos
                return r;
        }
        return null;
    }

    int adjacentRoomCount(Vector2 pos)
    {
        int adjacencyCount = 0;
        foreach (Room r in DungeonRooms)
        {   // incremenet adjacencyCount for each room that exists north, south, east, or west from pos
            adjacencyCount = (r.ContainsPos(pos + mNorth)) ? adjacencyCount + 1 : adjacencyCount;
            adjacencyCount = (r.ContainsPos(pos + mSouth)) ? adjacencyCount + 1 : adjacencyCount;
            adjacencyCount = (r.ContainsPos(pos + mEast)) ? adjacencyCount + 1 : adjacencyCount;
            adjacencyCount = (r.ContainsPos(pos + mWest)) ? adjacencyCount + 1 : adjacencyCount;
        }
        return adjacencyCount;
    }

    Vector2[] GetRandomDirections()         // returns array of 4 cardinal directions in a random order
    {
        Vector2[] dirs = { mNorth, mSouth, mEast, mWest};
        for(int i = 0; i < 3; i ++)
        {   // randomly shuffle the dirs array
            Vector2 temp = dirs[i];
            int val = Random.Range(i, 3);
            dirs[i] = dirs[val];
            dirs[val] = temp;
        }
        return dirs;    
    }

    int[] GetRandomOrder(int n)
    {
        int[] N = new int[n];
        for(int i = 0; i < n; i++)
        { n = i; }

        for(int r = 0; r < n; r++)
        {
            int temp = n;
            int val = Random.Range(r, n-1);
            N[r] = N[val];
            N[val] = temp;
        }
        return N;
    }
}
