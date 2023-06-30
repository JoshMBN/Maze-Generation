using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MazeConstructor))]           

public class GameController : MonoBehaviour
{
    private AIController aIController;
    public GameObject playerPrefab;
    public GameObject monsterPrefab;
    public Vector3 playerPosition;
    private MazeConstructor constructor;
    [SerializeField] private int rows;
    [SerializeField] private int cols;
    private Material pathSphereMat;

    


    void Awake()
    {
        constructor = GetComponent<MazeConstructor>();
        aIController = GetComponent<AIController>();
    }
    
    void Start()
    {
        constructor.GenerateNewMaze(rows, cols, OnTreasureTrigger);
        aIController.Graph = constructor.graph;
        aIController.Player = CreatePlayer();
        aIController.Monster = CreateMonster();
        aIController.HallWidth = constructor.hallWidth;
        playerPosition = aIController.Player.transform.position;
        aIController.StartAI();
    }

    

    private GameObject CreatePlayer()
    {
        Vector3 playerStartPosition = new Vector3(constructor.hallWidth, 1, constructor.hallWidth);
        GameObject player = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
        player.tag = "Generated";

        return player;
    }

    private GameObject CreateMonster()
    {
        Vector3 monsterPosition = new Vector3(constructor.goalCol * constructor.hallWidth, 0f, constructor.goalRow * constructor.hallWidth);
        GameObject monster = Instantiate(monsterPrefab, monsterPosition, Quaternion.identity);
        monster.tag = "Generated";
        
        return monster;
    }

    private void OnTreasureTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("You Won!");
        aIController.StopAI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShowPathToTreasure();
        }
    }

    private void ShowPathToTreasure()
    {
        // Destroy any previous path spheres
        DestroyPathSpheres();

        // Get the player's current position
        Vector3 playerPosition = aIController.Player.transform.position;

        // Find the node corresponding to the player's position
        int startX = Mathf.RoundToInt(playerPosition.x / constructor.hallWidth);
        int startY = Mathf.RoundToInt(playerPosition.z / constructor.hallWidth);
        Node startNode = constructor.graph[startX, startY];

        // Find the node corresponding to the treasure's position
        Node treasureNode = constructor.graph[constructor.goalRow, constructor.goalCol];

        // Find the path from the player's position to the treasure using AIController's FindPath method
        List<Node> path = aIController.FindPath(startY, startX, constructor.goalRow, constructor.goalCol);

        // Place spheres along the path
        foreach (Node node in path)
        {
            Vector3 position = new Vector3(node.y * constructor.hallWidth, 0f, node.x * constructor.hallWidth);
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
            sphere.name = "PathSphere";
            sphere.tag = "Generated";
            // Set the material for the path spheres
            sphere.GetComponent<MeshRenderer>().sharedMaterial = pathSphereMat;
            // Adjust the scale of the spheres if needed
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Debug.Log(position);
        }
    }

    private void DestroyPathSpheres()
    {
        GameObject[] pathSpheres = GameObject.FindGameObjectsWithTag("Generated");

        foreach (GameObject sphere in pathSpheres)
        {
            if (sphere.name == "PathSphere")
            {
                Destroy(sphere);
            }
        }
    }
}