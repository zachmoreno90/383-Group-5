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
    public List<Vector2> unitPositions = new List<Vector2>();

    public void SetupRoom(GameObject roomObj, Vector2 pos, int id)
    {
        roomObject = roomObj;
        roomPos = pos * unitScale;
        SetRoomID(id);
        GenerateRoom();
        height = unitScale;
        width = unitScale;
        unitPositions.Add(pos);
    }

    public void SetupRoom(GameObject roomObj, Vector2 pos, int h, int w, int id)
    {
        roomObject = roomObj;
        roomPos = pos * unitScale;
        SetRoomID(id);
        GenerateRoom();
        height = h * unitScale;
        width = w * unitScale;
        Vector2 bottomLeftPos = pos - new Vector2(w / 2, h / 2);
        for(int i = 0; i < w; i++)
        {
            for(int n = 0; n < h; n++)
            {
                Vector2 nextPos = bottomLeftPos + new Vector2(i, n);
                unitPositions.Add(nextPos);
            }
        }
    }

    public void GenerateRoom()
    {
        GameObject newRoom = Instantiate(roomObject) as GameObject;
        //        newRoom.transform.localScale = new Vector3(roomWidth, roomHeight, 1);
        //        newRoom.transform.position = new Vector3(roomPos.x + roomWidth / 2f, roomPos.y + roomHeight / 2f, 0);
        newRoom.transform.position = roomPos;
        newRoom.transform.position += new Vector3(0, 0, 2);
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
   
    public void MergeWithRoom(Room r)
    {
        Vector2 rPos = r.GetPosActual();

        float minX = Mathf.Min(roomPos.x - width / 2f, rPos.x - r.width / 2f);
        float maxX = Mathf.Max(roomPos.x + width / 2f, rPos.x + r.width / 2f);
        float minY = Mathf.Min(roomPos.y - height / 2f, rPos.y - r.height / 2f);
        float maxY = Mathf.Max(roomPos.y + height / 2f, rPos.y + r.height / 2f);

        roomPos = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
        width = (int)(maxX - minX);
        height = (int)(maxY - minY);
    }

    public void ExpandRoomInDir(Vector2 dir)
    {   // requires a cardinal direction to work correctly
        // for each position p in unitPositions, if p + dir doesn't
        // exist in unitPositions, add the new position
        int unitCount = unitPositions.Count;
        for(int i = 0; i < unitCount; i++)
        {
            Vector2 newPos = unitPositions[i] + dir;
            if (!unitPositions.Contains(newPos))
                unitPositions.Add(newPos);
        }
   
        float minX = Mathf.Infinity, maxX = 0, minY = Mathf.Infinity, maxY = 0;
        // get the min and max X and y values for the positions in unitPositions, and set roomPos to their average.
        foreach(Vector2 p in unitPositions)
        {
            minX = Mathf.Min(p.x, minX);
            maxX = Mathf.Max(p.x, maxX);
            minY = Mathf.Min(p.y, minY);
            maxY = Mathf.Max(p.y, maxY);
        }
        roomPos = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f) * unitScale;
        // increase width or height by dir based on its value.
        if (dir.x == 0)
            height += unitScale;
        else // if dir.y == 0
            width += unitScale;

    }

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

    public bool ContainsPos(Vector2 p)
    {
        foreach(Vector2 v in unitPositions)
        {
            if (v == p)
                return true;            
        }
        return false;
    }

    public List<Vector2> GetUnitPositions()
    { return unitPositions; }

}
