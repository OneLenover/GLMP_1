using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Game : MonoBehaviourPunCallbacks
{
    public static int SCREEN_WIDTH = 60;
    public static int SCREEN_HEIGHT = 80;

    public int nc_x;
    public int nc_y;

    public float speed = 0.1f;

    private float timer = 0;

    public static bool simulationEnabled = false;

    private int initialPopulation;
    private int resources = 10;

    Cell[,] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];

    // Start is called before the first frame update
    void Start()
    {
        PlaceCells(1);

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
    public static bool loadPlace = false;
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

            int patternCenterX = pattern.width / 2;
            int patternCenterY = pattern.height / 2;

            // ¬ычисл€ем смещение, чтобы центр структуры был по центру экрана
            int offsetX = (SCREEN_WIDTH - pattern.width) / 2;
            int offsetY = (SCREEN_HEIGHT - pattern.height) / 2;

            // ¬ыбираем случайные смещени€ по оси x и y
            int randomOffsetX = UnityEngine.Random.Range(0, SCREEN_WIDTH);
            int randomOffsetY = UnityEngine.Random.Range(0, SCREEN_HEIGHT);

            for (int y = 0; y < pattern.height; y++)
            {
                for (int x = 0; x < pattern.width; x++)
                {
                    int newX, newY;
                    // —мещаем координаты клетки на offsetX, offsetY и randomOffsetX, randomOffsetY
                    newX = (x + offsetX + randomOffsetX) % SCREEN_WIDTH;
                    newY = (y + offsetY + randomOffsetY) % SCREEN_HEIGHT;

                    if (pattern.patternString[x + y * pattern.width] == '1')
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

        // Ќаходим крайние клетки структуры
        int minX = SCREEN_WIDTH;
        int maxX = 0;
        int minY = SCREEN_HEIGHT;
        int maxY = 0;

        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                if (grid[x, y].isAlive)
                {
                    if (x < minX)
                        minX = x;
                    if (x > maxX)
                        maxX = x;
                    if (y < minY)
                        minY = y;
                    if (y > maxY)
                        maxY = y;
                }
            }
        }

        // —охран€ем только клетки внутри границ структуры
        pattern.width = maxX - minX + 1;
        pattern.height = maxY - minY + 1;

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
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
                    Cell cell = Instantiate(Resources.Load("Cell", typeof(Cell)), new Vector2(x - nc_x, y - nc_y), Quaternion.identity) as Cell;
                    grid[x, y] = cell;
                    bool alive = RandomAliveCell();
                    grid[x, y].SetAlive(false);
                    if (alive) initialPopulation++;
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
                    else
                    {
                        // –ождение новой клетки - получение ресурса
                        resources++;
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