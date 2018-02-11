using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTiles : MonoBehaviour {
    [SerializeField]
    GameObject topLeftWall, topRightWall, botLeftWall, botRightWall,
                    midLeftWall, midRightWall, midTopWall, midBotWall,
                    leftDoor, rightDoor, botDoor, topDoor, floor;
    public enum WallPos { top, bot, left, right, topLeft, topRight, botLeft, botRight };
    public enum DoorPos { top, bot, left, right };

    public GameObject GetWall (WallPos p)
    {
        switch (p)
        {
            case WallPos.top:
                return midTopWall;
                break;
            case WallPos.bot:
                return midBotWall;
                break;
            case WallPos.left:
                return midLeftWall;
                break;
            case WallPos.right:
                return midRightWall;
                break;
            case WallPos.topLeft:
                return topLeftWall;
                break;
            case WallPos.topRight:
                return topRightWall;
                break;
            case WallPos.botLeft:
                return botLeftWall;
                break;
            case WallPos.botRight:
                return botRightWall;
                break;
        }
        return floor;
    }

    public GameObject GetDoor(DoorPos d)
    {
        switch(d)
        {
            case DoorPos.top:
                return topDoor;
                break;
            case DoorPos.bot:
                return botDoor;
                break;
            case DoorPos.left:
                return leftDoor;
                break;
            case DoorPos.right:
                return rightDoor;
                break;
        }
        return floor;
    }

    public GameObject GetFloor()
    {
        return floor;
    }


    public void GenerateDungeonTiles(Room r)
    {
        Vector2 roomPos = r.GetPosActual();
        int width = r.GetWidthActual();
        int height = r.GetHeightActual();
        Vector2 botLeft = roomPos - new Vector2(width / 2f, height / 2f) + new Vector2(0.5f, 0.5f);

        for (int w = 0; w < 15; w++)
        {
            for (int h = 0; h < 15; h++)
            {
                Vector2 tilePos = botLeft + new Vector2(w, h);
                if (w == 0) // left side
                {
                    if (h == 0)
                    {
                        // get botLeft sprite
                        GameObject newWall = GameObject.Instantiate(GetWall(WallPos.botLeft));
                        newWall.transform.position = tilePos;
                        print("tiled bottom left");
                    }
                    else if (h == height - 1)
                    {
                        // get topLeft sprite
                    }
                    else
                    {
                        // get mid left sprite
                    }
                }
                else if (w == width - 1)
                {
                    if(h == 0)
                    {
                        // get botRight sprite
                    }
                    else if (h == height - 1)
                    {
                        // get topRight sprite
                    }
                    else
                    {
                        // get midRight sprite
                    }
                }
                else if (height == 0)
                {
                    // get midBot sprite
                }
                else if (height == height - 1)
                {
                    // get midTop sprite
                }
                else
                {
                    // get floor
                }


            }

        }
    }

}
