using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
  
    GameObject roomObject;
    Vector2 roomPos; // roomPos is the bottom left corner of a room
    //int roomWidth, roomHeight;
    int unitScale = 15;
    int roomID = 0;
    int height, width;
    public void SetupRoom(GameObject roomObj, Vector2 pos, int id)
    {
        roomObject = roomObj;
        roomPos = pos * unitScale;
        SetRoomID(id);
        GenerateRoom();
        height = unitScale;
        width = unitScale;
    }

    public void GenerateRoom()
    {
        GameObject newRoom = Instantiate(roomObject) as GameObject;
        //        newRoom.transform.localScale = new Vector3(roomWidth, roomHeight, 1);
        //        newRoom.transform.position = new Vector3(roomPos.x + roomWidth / 2f, roomPos.y + roomHeight / 2f, 0);
        newRoom.transform.position = roomPos;
    }

    public Vector2 GetUnitPos()
    {
        return roomPos / unitScale;
    }
    /*
    public int GetUnitWidth()
    {
        return roomWidth / unitScale;
    }
    public int GetUnitHeight()
    {
        return roomHeight / unitScale;
    }*/
   

    public Vector2 GetPosActual()
    { return roomPos;}

    public int GetWidthActual()
    { return width; }
    public int GetHeightActual()
    { return height; }
    public void SetRoomID(int id)
    { roomID = id; }
    public int GetRoomID()
    { return roomID; }
}
