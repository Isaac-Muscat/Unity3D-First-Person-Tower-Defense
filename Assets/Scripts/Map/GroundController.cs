using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public int numEnemySpawns, minDistanceBetweenSpawns, minDistanceFromPlayerSpawn;
    public float percentObstacles;
    public float obstacleHeight;
    public Vector2 tileDims; // Number of tiles over x and z
    public float tileSize; // Width of each square tile
    // Prefabs
    public GameObject obstaclePrefab, buildingPrefab, enemySpawnPrefab, playerSpawnPrefab, enemyPathPrefab;
    private List<List<Vector2Int>> spawnerPath;
    
    /*
        PlayerSpawn: player spawn point to be guarded
        Obstacle: obstacle for player blocks path generation
        EnemyPath: path the enemy walks on
        Building: tile to build towers
        Empty: space for player to walk on
    */
    public enum tileType {Obstacle, EnemyPath, Building, Empty, EnemySpawn, PlayerSpawn};
    private tileType[,] grid;
    private Vector2 playerSpawnIndex;
    private List<Vector2> enemySpawnIndexes;


   private void Awake() {
       grid = new tileType[(int)tileDims.x, (int)tileDims.y];

       playerSpawnIndex = new Vector2( Random.Range(1, tileDims.x-1), Random.Range(1, tileDims.y-1));

       enemySpawnIndexes = new List<Vector2>();
       spawnerPath = new List<List<Vector2Int>>();
       // Divide by 10 in order to account for the weird behaviour of scale property
        transform.localScale = new Vector3(tileDims.x*tileSize/10, 1, tileDims.y*tileSize/10);
        InitializeGrid(tileType.Empty, 1);
        GeneratePlayerSpawnPoint();
        CreateMap();
        GenerateEnemySpawnPoints();
        CreateEnemyPath();
        DisplayGrid();
   }
    void Start()
    {
        

    }

    void CreateEnemyPath(){
        foreach(Vector2 enemySpawnIndex in enemySpawnIndexes){
            Tile[,] tileGrid = new Tile[(int)tileDims.x, (int)tileDims.y];
            List<Tile> openSet = new List<Tile>();
            List<Tile> closedSet = new List<Tile>();
            for (int i = 0; i < tileDims.x; i++)
            {
                for (int j = 0; j < tileDims.y; j++)
                {
                    tileGrid[i, j] = new Tile(i, j);
                }
            }

            openSet.Add(tileGrid[(int)enemySpawnIndex.x, (int)enemySpawnIndex.y]);
            Tile current = tileGrid[(int)enemySpawnIndex.x, (int)enemySpawnIndex.y];
            current.g = 0;
            current.f = current.getDistance(playerSpawnIndex);

            int count = 0;
            while(openSet.Count>0 && count < 1000){
                count++; if(count>=1000) Debug.Log("Caught INF LOOP at a*");
                openSet.Sort(Tile.CompareTileCost);
                current = openSet[0];
                openSet.RemoveAt(0);
                if(current.x == (int)playerSpawnIndex.x && current.y == (int)playerSpawnIndex.y){
                    List<Vector2Int> path = new List<Vector2Int>();
                    path.Add(new Vector2Int(current.x, current.y));
                    Tile parent = current.parent;
                    int c = 0;
                    while(parent.parent!=null && c<1000){
                        c++; if(c>=1000) Debug.Log("Caught INF LOOP at a* parenting");
                        path.Add(new Vector2Int(parent.x, parent.y));
                        grid[parent.x, parent.y] = tileType.EnemyPath;
                        parent = parent.parent;
                    }
                    path.Add(new Vector2Int(parent.x, parent.y));
                    spawnerPath.Add(path);
                    break;
                }

                // Get neighbors
                int x = current.x; int y = current.y; 
                List<Tile> neighbours = new List<Tile>();
                if(x>0 && grid[x-1, y] != tileType.Obstacle){
                    neighbours.Add(tileGrid[x-1, y]);
                }
                if(x<tileDims.x-1 && grid[x+1, y] != tileType.Obstacle){
                    neighbours.Add(tileGrid[x+1, y]);
                }
                if(y<tileDims.y-1 && grid[x, y+1] != tileType.Obstacle){
                    neighbours.Add(tileGrid[x, y+1]);
                }
                if(y>0 && grid[x, y-1] != tileType.Obstacle){
                    neighbours.Add(tileGrid[x, y-1]);
                }

                // assign scores
                foreach (Tile n in neighbours)
                {
                    float newg = current.g + 1;
                    if(newg < n.g){
                        n.g = newg;
                        n.f = n.getDistance(playerSpawnIndex) + n.g;
                        n.parent = current;
                        if(!openSet.Contains(n))
                            openSet.Add(n);
                    }
                }
            }
        }
    }

    // Creates a random map with obstacles and buildpoints
    void CreateMap(){
        List<int[]> tiles = new List<int[]>();
        for (int i = 0; i < tileDims.x; i++)
        {
            for (int j = 0; j < tileDims.y; j++)
            {
                if(grid[i, j] != tileType.PlayerSpawn && grid[i, j] != tileType.EnemySpawn)
                    tiles.Add(new int[2]{i, j});
            }
        }

        int numObstacles  = (int)(tiles.Count*percentObstacles);

        for (int i = 0; i < numObstacles; i++)
        {
            int randomIdx = Random.Range(0, tiles.Count-1);
            int[] index = tiles[randomIdx];
            tiles.RemoveAt(randomIdx);
            grid[index[0], index[1]] = tileType.Obstacle;
        }

        // Flood Fill
        Queue<int[]> unvisitedTiles = new Queue<int[]>();
        List<int[]> accessableTiles = new List<int[]>();
        unvisitedTiles.Enqueue(new int[2]{(int)playerSpawnIndex.x, (int)playerSpawnIndex.y});
        bool[,] visited = new bool[(int)tileDims.x, (int)tileDims.y];
        int count = 0;
        while(unvisitedTiles.Count>0 && count < 10000){
            count++; if(count>=10000) Debug.Log("Caught INF LOOP at flood fill");
            int[] current = unvisitedTiles.Dequeue();
            accessableTiles.Add(current);
            int x = current[0]; int y = current[1];
            visited[x, y] = true;
            if(x>0 && grid[x-1, y] != tileType.Obstacle && !visited[x-1, y]){
                unvisitedTiles.Enqueue(new int[2]{x-1, y});
                visited[x-1, y] = true;
            }

            if(x<tileDims.x-1 && grid[x+1, y] != tileType.Obstacle && !visited[x+1, y]){
                unvisitedTiles.Enqueue(new int[2]{x+1, y});
                visited[x+1, y] = true;
            }

            if(y<tileDims.y-1 && grid[x, y+1] != tileType.Obstacle && !visited[x, y+1]){
                unvisitedTiles.Enqueue(new int[2]{x, y+1});
                visited[x, y+1] = true;
            }
            
            if(y>0 && grid[x, y-1] != tileType.Obstacle && !visited[x, y-1]) {
                unvisitedTiles.Enqueue(new int[2]{x, y-1});
                visited[x, y-1] = true;
            }
        }

        InitializeGrid(tileType.Obstacle, 0.8f);
        foreach (int[] index in accessableTiles)
        {
            grid[index[0], index[1]] = tileType.Empty;
        }

        GeneratePlayerSpawnPoint();

    }

    bool SpawnPointsSpaced(){
        if(enemySpawnIndexes.Count>1){
            for (int i = 0; i < enemySpawnIndexes.Count; i++)
            {
                for (int j = 0; j < enemySpawnIndexes.Count; j++)
                {
                    if(j==i) continue;
                    if(Mathf.Abs(enemySpawnIndexes[i].x-enemySpawnIndexes[j].x)+Mathf.Abs(enemySpawnIndexes[i].y-enemySpawnIndexes[j].y) <minDistanceBetweenSpawns) return false;
                }
            }
        }
        return true;
    }

    // Generates the enemy spawn points
    void GenerateEnemySpawnPoints(){
        for(int i = 0; i<numEnemySpawns;i++){
            int x = Random.Range(0, (int)tileDims.x-1);
            int y = Random.Range(0, (int)tileDims.y-1);
            int count = 0;
            while(count<10000 && !(Mathf.Abs(x-playerSpawnIndex.x)+Mathf.Abs(y-playerSpawnIndex.y) >minDistanceFromPlayerSpawn && SpawnPointsSpaced()&& grid[x, y] == tileType.Empty)){
                count++;if(count >= 10000) {Debug.Log("Caught INF LOOP at generate enemy spawns");break;}
                if(count%1000==0) {
                    if(minDistanceBetweenSpawns>=3){
                        minDistanceBetweenSpawns--;
                    } else {
                        minDistanceFromPlayerSpawn-=1;
                    }
                }
                x = Random.Range(0, (int)tileDims.x-1);
                y = Random.Range(0, (int)tileDims.y-1);
            }
            enemySpawnIndexes.Add(new Vector2(x, y));
            grid[(int)enemySpawnIndexes[i].x,(int)enemySpawnIndexes[i].y] = tileType.EnemySpawn;
        }
    }

    void GeneratePlayerSpawnPoint(){
        grid[(int)playerSpawnIndex.x,(int)playerSpawnIndex.y] = tileType.PlayerSpawn;
    }

    void InitializeGrid(tileType type, float percent){
        for (int i = 0; i < tileDims.x; i++)
        {
            for (int j = 0; j < tileDims.y; j++)
            {
                if(Random.Range(0, 1)/100<=percent)
                    grid[i, j] = type;
            }
        }
    }

    void DisplayGrid(){
        for (int i = 0; i < tileDims.x; i++)
        {
            for (int j = 0; j < tileDims.y; j++)
            {
                GameObject prefab;
                Vector3 position = new Vector3(i*tileSize-tileSize*tileDims.x/2+tileSize/2, transform.position.y+obstacleHeight/2, j*tileSize-tileSize*tileDims.y/2+tileSize/2);
                switch (grid[i, j])
                {
                    case tileType.Obstacle:
                        prefab = Instantiate(obstaclePrefab, position,Quaternion.identity);
                        prefab.transform.localScale = new Vector3(tileSize, obstacleHeight, tileSize);
                        prefab.transform.position = position;
                        prefab.transform.parent = transform.Find("Obstacles");
                        break;
                    case tileType.EnemySpawn:
                        position.y = transform.position.y+enemySpawnPrefab.GetComponent<Renderer>().bounds.size.y/2;
                        prefab = Instantiate(enemySpawnPrefab, position,Quaternion.identity);
                        prefab.transform.localScale = new Vector3(tileSize, 1, tileSize);
                        prefab.transform.position = position;
                        prefab.transform.parent = transform.Find("SpawnPoints");
                        foreach (List<Vector2Int> path in spawnerPath)
                        {
                            if(path[path.Count-1].x == i && path[path.Count-1].y == j){
                                prefab.GetComponent<SpawnerController>().path = path;
                            }
                        }
                        break;
                    case tileType.PlayerSpawn:
                        position.y = transform.position.y+obstacleHeight;
                        prefab = Instantiate(playerSpawnPrefab, position,Quaternion.identity);
                        prefab.transform.localScale = new Vector3(tileSize, obstacleHeight*2, tileSize);
                        prefab.transform.position = position;
                        prefab.transform.parent = transform.Find("SpawnPoints");
                        break;
                    case tileType.EnemyPath:
                        position.y = transform.position.y+tileSize/20;
                        prefab = Instantiate(enemyPathPrefab, position,Quaternion.identity);
                        prefab.transform.localScale = new Vector3(tileSize, tileSize/10, tileSize);
                        prefab.transform.position = position;
                        prefab.transform.parent = transform.Find("EnemyPath");
                        break;
                    case tileType.Building:
                        position.y = transform.position.y+tileSize/20;
                        prefab = Instantiate(buildingPrefab, position,Quaternion.identity);
                        prefab.transform.localScale = new Vector3(tileSize, tileSize/10, tileSize);
                        prefab.transform.position = position;
                        prefab.transform.parent = transform;
                        break;

                    default: break;
                }
            }
        }

        GameObject bottom = Instantiate(obstaclePrefab, new Vector3(0, transform.position.y+3*obstacleHeight/2, -(tileSize*tileDims.y+tileSize)/2), Quaternion.identity);
        bottom.transform.localScale = new Vector3(tileSize*tileDims.x, obstacleHeight*3, tileSize);
        bottom.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);

        GameObject top = Instantiate(obstaclePrefab, new Vector3(0, transform.position.y+3*obstacleHeight/2, +(tileSize*tileDims.y+tileSize)/2), Quaternion.identity);
        top.transform.localScale = new Vector3(tileSize*tileDims.x, obstacleHeight*3, tileSize);
        top.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);

        GameObject right = Instantiate(obstaclePrefab, new Vector3((tileSize*tileDims.x+tileSize)/2, transform.position.y+3*obstacleHeight/2, 0), Quaternion.identity);
        right.transform.localScale = new Vector3(tileSize, obstacleHeight*3, tileSize*tileDims.y);
        right.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);

        GameObject left = Instantiate(obstaclePrefab, new Vector3(-(tileSize*tileDims.x+tileSize)/2, transform.position.y+3*obstacleHeight/2, 0), Quaternion.identity);
        left.transform.localScale = new Vector3(tileSize, obstacleHeight*3, tileSize*tileDims.y);
        left.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
    }

    void OnDrawGizmos() {
        // Draws the x part of tiles for debugging
        float yOff = 0.05f;
        Vector2 halfDims = new Vector2(tileDims.x*tileSize/2, tileDims.y*tileSize/2);
        Gizmos.color = Color.black;
        for(int i = 0;i<=tileDims.x;i++){
            Gizmos.DrawLine(
                new Vector3(i*tileSize-halfDims.x, transform.position.y+yOff, halfDims.y), 
                new Vector3(i*tileSize-halfDims.x, transform.position.y+yOff, -halfDims.y));
        }

        // Draws the y part of tiles for debugging
        for(int i = 0;i<=tileDims.y;i++){
            Gizmos.DrawLine(
                new Vector3(halfDims.x, transform.position.y+yOff, i*tileSize-halfDims.y), 
                new Vector3(-halfDims.x, transform.position.y+yOff, i*tileSize-halfDims.y));
        }
    }
}
