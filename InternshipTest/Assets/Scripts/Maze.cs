using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public bool visited = false;
        public GameObject north;
        public GameObject south;
        public GameObject east;
        public GameObject west;

    }

    public GameObject wall;
    public float wallLenght = 1.0f;
    public int xSize = 5;
    public int ySize = 5;
    private Vector3 initialPos;
    private GameObject wallHolder;
    private Cell[] cells;
    private int currentCell = 0;
    private int totalCells;
    private int visitedCells = 0;
    private bool startedBulding = false;
    private int currentNeightbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateWalls();
    }

    void CreateWalls()
    {
        wallHolder = new GameObject();
        wallHolder.name = "Maze";

        initialPos = new Vector3((-xSize / 2) + wallLenght / 2, 0, (-ySize / 2) + wallLenght / 2);
        Vector3 myPos = initialPos;
        GameObject tempWall;

        //for X Axis
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j <= xSize; j++)
            {
                myPos = new Vector3 (initialPos.x + (j * wallLenght)- wallLenght / 2,0.0f, initialPos.z + (i * wallLenght) - wallLenght / 2);
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
                
            }
        }

        //for Y Axis
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPos = new Vector3 (initialPos.x + (j * wallLenght), 0, initialPos.z + (i * wallLenght) - wallLenght);
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0, 90, 0)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;

            }
        }
        CreateCells();
    }

    void CreateCells()
    {
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;
        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Cell[xSize * ySize];
        int westEastProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        //Get All Children
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }

        //assign walls to cells
        for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++)
        {
            cells[cellprocess] = new Cell();
            cells[cellprocess].west = allWalls[westEastProcess];
            cells[cellprocess].south = allWalls[childProcess + (xSize + 1) * ySize];

            if (termCount == xSize)
            {
                westEastProcess += 2;
                termCount = 0;
            }

            else
                westEastProcess++;

            termCount++;
            childProcess++;

            cells[cellprocess].east = allWalls[westEastProcess];
            cells[cellprocess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];
        }
        CreateMaze();
    }

    void CreateMaze()
    {
        while(visitedCells  < totalCells)
        {
            if (startedBulding)
            {
                FindNeighbour();
                if(cells[currentNeightbour].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeightbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeightbour;

                    if(lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBulding = true;
            }

            
        }

        Debug.Log("finished");



        FindNeighbour();
    }

    void BreakWall()
    {
        switch (wallToBreak)
        {
            case 1 : Destroy(cells[currentCell].north); break;
            case 2: Destroy(cells[currentCell].west); break;
            case 3: Destroy(cells[currentCell].east); break;
            case 4: Destroy(cells[currentCell].south); break;
        }
    }

    void FindNeighbour()
    {
        int lenght = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;

        //check parameters
        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;

        //east
        if (currentCell + 1 < totalCells && (currentCell + 1) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbours[lenght] = currentCell + 1;
                connectingWall[lenght] = 3;
                lenght++;
            }
        }

        //west
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[lenght] = currentCell - 1;
                connectingWall[lenght] = 2;
                lenght++;
            }
        }

        //north
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[lenght] = currentCell + xSize;
                connectingWall[lenght] = 1;
                lenght++;
            }
        }

        //south
        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbours[lenght] = currentCell - xSize;
                connectingWall[lenght] = 4;
                lenght++;
            }
        }

       if(lenght != 0)
        {
            int theChosenCell = Random.Range(0, lenght);
            currentNeightbour = neighbours[theChosenCell];
            wallToBreak = connectingWall[theChosenCell];
        }
        else
        {
            if (backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
