using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static int SCREEN_WIDTH = 60;
    public static int SCREEN_HEIGHT = 80;

    public int nc_x = 30;
    public int nc_y = 40;

    public float speed = 0.1f;

    private float timer = 0;

    public static bool simulationEnabled = false;

    Cell[,] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];

    // Start is called before the first frame update
    void Start()
    {
        PlaceCells(2);
    }

    // Update is called once per frame
    void Update()
    {
        if (simulationEnabled)
        {
            if (timer >= speed)
            {
                timer = 0f;
                CountNeighbors();

                PopulationControl();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }

        UserInput();
    }

    public void ClearField()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                grid[x, y].SetAlive(false);
            }
        }
    }

    public static bool load = false;
    private void LoadPattern()
    {
        string path = Path.Combine(Application.persistentDataPath, "patterns/test.xml");

        if (!File.Exists(path))
        {
            return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));

        using (StreamReader reader = new StreamReader(path))
        {
            Pattern pattern = (Pattern)serializer.Deserialize(reader.BaseStream);

            bool isAlive;
            int offsetX = UnityEngine.Random.Range(0, SCREEN_WIDTH);
            int offsetY = UnityEngine.Random.Range(0, SCREEN_HEIGHT);

            for (int y = 0; y < 80; y++)
            {
                for (int x = 0; x < 60; x++)
                {
                    int newX = (x + offsetX) % SCREEN_WIDTH;
                    int newY = (y + offsetY) % SCREEN_HEIGHT;

                    if (pattern.patternString[x + y * SCREEN_WIDTH] == '1')
                    {
                        isAlive = true;
                    }
                    else
                    {
                        isAlive = false;
                    }

                    grid[newX, newY].SetAlive(isAlive);
                }
            }
        }
    }

    private void SavePattern()
    {
        string path = Path.Combine(Application.persistentDataPath, "patterns/test.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(path));

        Pattern pattern = new Pattern();

        string patternString = null;

        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                if (grid[x, y].isAlive == false)
                {
                    patternString += "0";
                }
                else
                {
                    patternString += "1";
                }
            }
        }

        pattern.patternString = patternString;

        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));

        using (StreamWriter writer = new StreamWriter(path))
        {
            serializer.Serialize(writer.BaseStream, pattern);
        }

        Debug.Log(pattern.patternString);
    }

    public static bool DrawEnable = true;
    public static bool save = false;
    void UserInput()
    {
        if (Input.touchCount == 1)
        {
            if (GridMovement.isTouchEnabled == false)
            {
                if (Input.GetMouseButton(0) && DrawEnable)
                {
                    Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    int x = Mathf.RoundToInt(mousePoint.x) + nc_x;
                    int y = Mathf.RoundToInt(mousePoint.y) + nc_y;

                    if (x >= 0 && y >= 0 && x < SCREEN_WIDTH && y < SCREEN_HEIGHT)
                    {
                        grid[x, y].SetAlive(true);
                    }
                }

                else if (Input.GetMouseButton(0) && !DrawEnable)
                {
                    Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    int x = Mathf.RoundToInt(mousePoint.x) + nc_x;
                    int y = Mathf.RoundToInt(mousePoint.y) + nc_y;

                    if (x >= 0 && y >= 0 && x < SCREEN_WIDTH && y < SCREEN_HEIGHT)
                    {
                        grid[x, y].SetAlive(false);
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.P))
            {
                simulationEnabled = false;
            }

            if (Input.GetKeyUp(KeyCode.B))
            {
                simulationEnabled = true;
            }

            if (save)
            {
                SavePattern();
                save = false;
            }

            if (load)
            {
                LoadPattern();
                load = false;
            }
        }
    }

    void PlaceCells(int type)
    {
        if (type == 1)
        {
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                for (int x = 0; x < SCREEN_WIDTH; x++)
                {
                    Cell cell = Instantiate(Resources.Load("PreFabs/Cell", typeof(Cell)), new Vector2(x - nc_x, y - nc_y), Quaternion.identity) as Cell;
                    grid[x, y] = cell;
                    grid[x, y].SetAlive(false);
                }
            }
        }

        else if (type == 2)
        {
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                for (int x = 0; x < SCREEN_WIDTH; x++)
                {
                    Cell cell = Instantiate(Resources.Load("PreFabs/Cell", typeof(Cell)), new Vector2(x - nc_x, y - nc_y), Quaternion.identity) as Cell;
                    grid[x, y] = cell;
                    grid[x, y].SetAlive(RandomAliveCell());
                }
            }
        }

    }

    void CountNeighbors()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                int numNeighbors = 0;

                // check north neighbor
                if (grid[x, (y + 1) % SCREEN_HEIGHT].isAlive)
                {
                    numNeighbors++;
                }

                // check east neighbor
                if (grid[(x + 1) % SCREEN_WIDTH, y].isAlive)
                {
                    numNeighbors++;
                }

                // check south neighbor
                if (grid[x, (y - 1 + SCREEN_HEIGHT) % SCREEN_HEIGHT].isAlive)
                {
                    numNeighbors++;
                }

                // check west neighbor
                if (grid[(x - 1 + SCREEN_WIDTH) % SCREEN_WIDTH, y].isAlive)
                {
                    numNeighbors++;
                }

                // check northeast neighbor
                if (grid[(x + 1) % SCREEN_WIDTH, (y + 1) % SCREEN_HEIGHT].isAlive)
                {
                    numNeighbors++;
                }

                // check northwest neighbor
                if (grid[(x - 1 + SCREEN_WIDTH) % SCREEN_WIDTH, (y + 1) % SCREEN_HEIGHT].isAlive)
                {
                    numNeighbors++;
                }

                // check southeast neighbor
                if (grid[(x + 1) % SCREEN_WIDTH, (y - 1 + SCREEN_HEIGHT) % SCREEN_HEIGHT].isAlive)
                {
                    numNeighbors++;
                }

                // check southwest neighbor
                if (grid[(x - 1 + SCREEN_WIDTH) % SCREEN_WIDTH, (y - 1 + SCREEN_HEIGHT) % SCREEN_HEIGHT].isAlive)
                {
                    numNeighbors++;
                }

                grid[x, y].numNeighbors = numNeighbors;
            }
        }
    }

    void PopulationControl()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                if (grid[x, y].isAlive)
                {
                    if (grid[x, y].numNeighbors != 2 && grid[x, y].numNeighbors != 3)
                    {
                        grid[x, y].SetAlive(false);
                    }
                }

                else
                {
                    if (grid[x, y].numNeighbors == 3)
                    {
                        grid[x, y].SetAlive(true);
                    }
                }
            }
        }
    }

    bool RandomAliveCell()
    {
        int rand = UnityEngine.Random.Range(0, 100);

        if (rand > 75)
        {
            return true;
        }

        return false;
    }
}