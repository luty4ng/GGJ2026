using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    [Header("房间参数")]
    [Tooltip("房间长宽的最大值（米）")]
    [Range(2f, 20f)]
    public float maxRoomSize = 5f;
    
    [Tooltip("房间高度（米）")]
    public float roomHeight = 3f;
    
    [Tooltip("墙壁厚度（米）")]
    [Range(0.05f, 0.5f)]
    public float wallThickness = 0.2f;
    
    [Header("迷宫参数")]
    [Tooltip("迷宫的行数")]
    public int mazeRows = 5;
    
    [Tooltip("迷宫的列数")]
    public int mazeColumns = 5;
    
    [Header("材质")]
    public Material wallMaterial;
    public Material floorMaterial;
    public Material ceilingMaterial;
    
    // 迷宫单元格数据结构
    private class Cell
    {
        public bool visited = false;
        public bool[] walls = new bool[4] { true, true, true, true }; // 北、东、南、西
        public int row, col;
        
        public Cell(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    
    private Cell[,] maze;
    private List<GameObject> roomObjects = new List<GameObject>();
    
    public void GenerateMaze()
    {
        ClearExistingMaze();
        InitializeMaze();
        GenerateMazeLayout();
        BuildMazeRooms();
    }
    
    private void ClearExistingMaze()
    {
        foreach (var obj in roomObjects)
        {
            if (Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }
        roomObjects.Clear();
    }
    
    private void InitializeMaze()
    {
        maze = new Cell[mazeRows, mazeColumns];
        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                maze[r, c] = new Cell(r, c);
            }
        }
    }
    
    private void GenerateMazeLayout()
    {
        // 使用深度优先搜索算法生成迷宫
        Stack<Cell> stack = new Stack<Cell>();
        Cell current = maze[0, 0];
        current.visited = true;
        
        do
        {
            List<Cell> unvisitedNeighbors = GetUnvisitedNeighbors(current);
            
            if (unvisitedNeighbors.Count > 0)
            {
                // 随机选择一个未访问的相邻单元格
                Cell next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                stack.Push(current);
                
                // 移除当前单元格和下一个单元格之间的墙
                RemoveWallsBetween(current, next);
                
                current = next;
                current.visited = true;
            }
            else if (stack.Count > 0)
            {
                // 回溯
                current = stack.Pop();
            }
        } while (stack.Count > 0);
    }
    
    private List<Cell> GetUnvisitedNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        int[][] dirs = new int[][] {
            new int[] {-1, 0}, // 北
            new int[] {0, 1},  // 东
            new int[] {1, 0},  // 南
            new int[] {0, -1}  // 西
        };
        
        for (int i = 0; i < dirs.Length; i++)
        {
            int newRow = cell.row + dirs[i][0];
            int newCol = cell.col + dirs[i][1];
            
            if (newRow >= 0 && newRow < mazeRows && newCol >= 0 && newCol < mazeColumns)
            {
                if (!maze[newRow, newCol].visited)
                {
                    neighbors.Add(maze[newRow, newCol]);
                }
            }
        }
        
        return neighbors;
    }
    
    private void RemoveWallsBetween(Cell current, Cell next)
    {
        int rowDiff = next.row - current.row;
        int colDiff = next.col - current.col;
        
        if (rowDiff == -1) // 下一个单元格在北边
        {
            current.walls[0] = false; // 移除当前单元格的北墙
            next.walls[2] = false;    // 移除下一个单元格的南墙
        }
        else if (rowDiff == 1) // 下一个单元格在南边
        {
            current.walls[2] = false; // 移除当前单元格的南墙
            next.walls[0] = false;    // 移除下一个单元格的北墙
        }
        else if (colDiff == -1) // 下一个单元格在西边
        {
            current.walls[3] = false; // 移除当前单元格的西墙
            next.walls[1] = false;    // 移除下一个单元格的东墙
        }
        else if (colDiff == 1) // 下一个单元格在东边
        {
            current.walls[1] = false; // 移除当前单元格的东墙
            next.walls[3] = false;    // 移除下一个单元格的西墙
        }
    }
    
    private void BuildMazeRooms()
    {
        GameObject mazeParent = new GameObject("Maze");
        roomObjects.Add(mazeParent);
        
        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                // 为每个单元格创建一个房间
                float roomWidth = Random.Range(1f, maxRoomSize);
                float roomLength = Random.Range(1f, maxRoomSize);
                
                // 计算房间位置
                float xPos = c * maxRoomSize;
                float zPos = r * maxRoomSize;
                
                // 创建房间对象
                GameObject room = new GameObject($"Room_{r}_{c}");
                room.transform.parent = mazeParent.transform;
                room.transform.position = new Vector3(xPos, 0, zPos);
                roomObjects.Add(room);
                
                // 创建地板
                CreateFloor(room.transform, roomWidth, roomLength);
                
                // 创建天花板
                CreateCeiling(room.transform, roomWidth, roomLength);
                
                // 创建墙壁
                Cell cell = maze[r, c];
                CreateWalls(room.transform, cell, roomWidth, roomLength);
            }
        }
    }
    
    private void CreateFloor(Transform parent, float width, float length)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.parent = parent;
        
        // 设置地板尺寸和位置
        floor.transform.localScale = new Vector3(width, wallThickness, length);
        floor.transform.localPosition = new Vector3(width/2, -wallThickness/2, length/2);
        
        // 设置材质
        if (floorMaterial != null)
            floor.GetComponent<Renderer>().material = floorMaterial;
        
        roomObjects.Add(floor);
    }
    
    private void CreateCeiling(Transform parent, float width, float length)
    {
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.parent = parent;
        
        // 设置天花板尺寸和位置
        ceiling.transform.localScale = new Vector3(width, wallThickness, length);
        ceiling.transform.localPosition = new Vector3(width/2, roomHeight + wallThickness/2, length/2);
        
        // 设置材质
        if (ceilingMaterial != null)
            ceiling.GetComponent<Renderer>().material = ceilingMaterial;
        
        roomObjects.Add(ceiling);
    }
    
    private void CreateWalls(Transform parent, Cell cell, float width, float length)
    {
        // 北墙 (Z+)
        if (cell.walls[0])
        {
            GameObject northWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            northWall.name = "NorthWall";
            northWall.transform.parent = parent;
            northWall.transform.localScale = new Vector3(width, roomHeight, wallThickness);
            northWall.transform.localPosition = new Vector3(width/2, roomHeight/2, length);
            
            if (wallMaterial != null)
                northWall.GetComponent<Renderer>().material = wallMaterial;
            
            roomObjects.Add(northWall);
        }
        
        // 东墙 (X+)
        if (cell.walls[1])
        {
            GameObject eastWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eastWall.name = "EastWall";
            eastWall.transform.parent = parent;
            eastWall.transform.localScale = new Vector3(wallThickness, roomHeight, length);
            eastWall.transform.localPosition = new Vector3(width, roomHeight/2, length/2);
            
            if (wallMaterial != null)
                eastWall.GetComponent<Renderer>().material = wallMaterial;
            
            roomObjects.Add(eastWall);
        }
        
        // 南墙 (Z-)
        if (cell.walls[2])
        {
            GameObject southWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            southWall.name = "SouthWall";
            southWall.transform.parent = parent;
            southWall.transform.localScale = new Vector3(width, roomHeight, wallThickness);
            southWall.transform.localPosition = new Vector3(width/2, roomHeight/2, 0);
            
            if (wallMaterial != null)
                southWall.GetComponent<Renderer>().material = wallMaterial;
            
            roomObjects.Add(southWall);
        }
        
        // 西墙 (X-)
        if (cell.walls[3])
        {
            GameObject westWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            westWall.name = "WestWall";
            westWall.transform.parent = parent;
            westWall.transform.localScale = new Vector3(wallThickness, roomHeight, length);
            westWall.transform.localPosition = new Vector3(0, roomHeight/2, length/2);
            
            if (wallMaterial != null)
                westWall.GetComponent<Renderer>().material = wallMaterial;
            
            roomObjects.Add(westWall);
        }
    }
}