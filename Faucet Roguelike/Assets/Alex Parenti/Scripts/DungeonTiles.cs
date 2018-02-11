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
        if (width > 15 || height > 15)
        {
            print("width = " + width);
            print("height = " + height);
        }
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                Vector2 tilePos = botLeft + new Vector2(w, h);
                GameObject newTile = null;
            
                if (w == 0) // left side
                {
                    if (h == 0)
                    {
                        // get botLeft sprite
                        newTile = GameObject.Instantiate(GetWall(WallPos.botLeft));
                    }
                    else if (h == height - 1)
                    {
                        // get topLeft sprite
                        newTile = GameObject.Instantiate(GetWall(WallPos.topLeft));
                    }
                    else
                    {
                        // get mid left sprite
                        newTile = GameObject.Instantiate(GetWall(WallPos.left));
                    }
                }
                else if (w == width - 1)
                {
                    if(h == 0)
                    {
                        // get botRight sprite
                        newTile = GameObject.Instantiate(GetWall(WallPos.botRight));
                    }
                    else if (h == height - 1)
                    {
                        // get topRight sprite
                        newTile = GameObject.Instantiate(GetWall(WallPos.topRight));
                    }
                    else
                    {
                        // get midRight sprite
                        newTile = GameObject.Instantiate(GetWall(WallPos.right));
                    }
                }
                else if (h == 0)
                {
                    // get midBot sprite
                    newTile = GameObject.Instantiate(GetWall(WallPos.bot));
                }
                else if (h == height - 1)
                {
                    // get midTop sprite
                    newTile = GameObject.Instantiate(GetWall(WallPos.top));
                }
                else
                {
                    // get floor
                    newTile = GameObject.Instantiate(GetFloor());
                }
                if(newTile != null)
                    newTile.transform.position = tilePos;

            }

        }
    }

}
