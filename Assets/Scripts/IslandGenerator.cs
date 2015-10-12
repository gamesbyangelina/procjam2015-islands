using UnityEngine;
using System.Collections;

public class IslandGenerator : MonoBehaviour {

	//I realise this convention doesn't make any sense, I'm sorry, I'm sorry >_>
	int SAND_TILE = 0;
	int SEA_TILE = 1;
	int GRASS_TILE = 2;

	//The island tiles need a 1 pixel border, so we exaggerate the width and height by a little
	public float brickWidth = 0.66f;
	public float brickHeight = 0.34f;
	public bool simpleGrassTiles = true;

	int[,] level;

	GameObject[,] tiles;
	int[,] tileData;
	public int levelWidth = 40;
	public int levelHeight = 20;
	public float chanceToStartAlive = 0.35f;

	public int generations = 3;
	public int minToSurvive = 3;
	public int birthNumber = 3;

	public int currentSeed;
	public bool randomiseSeed = true;

	// Use this for initialization
	void Start () {
		if(randomiseSeed)
			currentSeed = Random.Range(0, 10000);

		PaintBackground();
		level = GenerateLevelCA();
		AddGrassTiles();
		PaintTiles();
		PlaceScenery();
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.R)){
			Destroy(GameObject.Find("Tiles"));
			Destroy(GameObject.Find("Scenery"));

			currentSeed = Random.Range(0, 10000);

			PaintBackground();
			level = GenerateLevelCA();
			AddGrassTiles();
			PaintTiles();
			PlaceScenery();
		}
	}

	public void PlaceScenery(){
		GameObject scenery = new GameObject("Scenery");
		int piratecount = 0;
		int treasurecount = 0;
		for(int i=0; i<levelWidth; i++){
			for(int j=0; j<levelHeight; j++){
				if(countNeighbours(level, i, j, SEA_TILE) == 0 && piratecount < 4 && Random.Range(0f,1f) < 0.1f){
					GameObject pirate = new GameObject("Pirate");
					pirate.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("island/island_pirate_1"+((char)(97+piratecount))+"1a");
					pirate.GetComponent<SpriteRenderer>().sortingOrder = 2;
					piratecount++;
					pirate.transform.position = new Vector2((i * brickWidth/2) + (j * brickWidth/2), (i * brickHeight/2) - (j * brickHeight/2) + brickHeight/2);
					pirate.transform.parent = scenery.transform;
				}
				else if(countNeighbours(level, i, j, SEA_TILE) == 0 && treasurecount < 6 && Random.Range(0f,1f) < 0.05f){
					GameObject treasure = new GameObject("Treasure");
					treasure.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("island/island_chest_1a1a");
					treasure.GetComponent<SpriteRenderer>().sortingOrder = 1;
					treasure.transform.position = new Vector2((i * brickWidth/2) + (j * brickWidth/2), (i * brickHeight/2) - (j * brickHeight/2));
					treasure.transform.parent = scenery.transform;
					treasurecount++;
				}
			}
		}
	}

	public void AddGrassTiles(){
		//To add grass tiles we're going to take a bit of a lazy approach: any tile completely surrounded by sand is now grassy.
		//Take a copy since we're modifying the level
		int[,] newLevel = new int[levelWidth, levelHeight];
		for(int i=0; i<levelWidth; i++){
			for(int j=0; j<levelHeight; j++){
				if(simpleGrassTiles){
					if(countNeighbours(level, i, j, SAND_TILE) == 8)
						newLevel[i,j] = GRASS_TILE;
					else
						newLevel[i,j] = level[i,j];
				}
				else{
					if(countNeighboursFurther(level, i, j, SAND_TILE) > 18)
						newLevel[i,j] = GRASS_TILE;
					else
						newLevel[i,j] = level[i,j];
				}
			}
		}

		level = newLevel;
	}

	public void PaintBackground(){
		GameObject bgParent = new GameObject("Tiles");
		tiles = new GameObject[levelWidth, levelHeight];
		for(int i=0; i<levelWidth; i++){
			for(int j=0; j<levelHeight; j++){
				GameObject seaTile = new GameObject();
				seaTile.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("island/island_sea_sea_1234");

				//Drawing isometric tiles is a bit funky
				//This is a good resource: http://clintbellanger.net/articles/isometric_math/
				float tx = (i * brickWidth/2) + (j * brickWidth/2);
				float ty = (i * brickHeight/2) - (j * brickHeight/2);
				seaTile.transform.position = new Vector2(tx, ty);
				seaTile.transform.parent = bgParent.transform;

				tiles[i,j] = seaTile;
			}
		}
	}

	public void PaintTiles(){
		// string all = "";
		for(int i=0; i<levelWidth; i++){
			// string line = "";
			for(int j=0; j<levelHeight; j++){
				// line += level[i,j]+".";
				string filename = "";

				//Let's place some sand down for now. We're going to calculate tile adjacencies.
				//We do this in a bit of a funny way. We're going to calculate the tile that should display 'in between' each array marker
				if(level[i,j] == SEA_TILE)
					filename += "1";
				if(i >= level.GetLength(0)-1 || level[i+1,j] == SEA_TILE)
					filename += "2";
				if((i >= level.GetLength(0)-1 || j >= level.GetLength(1)-1) || level[i+1,j+1] == SEA_TILE)
					filename += "3";
				if(j >= level.GetLength(1)-1 || level[i,j+1] == SEA_TILE)
					filename += "4";

				//And then add the last bit of the filename
				if(filename == ""){
					if(level[i,j] == SAND_TILE)
						filename += "1";
					if(i < level.GetLength(0)-1 && level[i+1,j] == SAND_TILE)
						filename += "2";
					if(i < level.GetLength(0)-1 && j < level.GetLength(1)-1 && level[i+1,j+1] == SAND_TILE)
						filename += "3";
					if(j < level.GetLength(1)-1 && level[i,j+1] == SAND_TILE)
						filename += "4";

					if(filename == "")
						filename = "island/island_grass_grass_1234";
					else if(filename == "1234")
						filename = "island/island_sand_sand_1234";
					else
						filename = "island/island_grass_sand_"+filename;

					if(Resources.Load<Sprite>(filename) == null)
						Debug.Log(filename);

					tiles[i,j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(filename);
					
					continue;
				}
				else if(filename == "1234")
					filename = "island/island_sea_sea_1234";
				else
					filename = "island/island_sand_sea_"+filename;

				tiles[i,j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(filename);
			}
			// all += line+"\n";
		}		
		// Debug.Log(all);
	}

	//For more info on using cellular automata, check out: 
	//http://gamedevelopment.tutsplus.com/tutorials/generate-random-cave-levels-using-cellular-automata--gamedev-9664
	public int[,] GenerateLevelCA(){
		
		UnityEngine.Random.seed = currentSeed;
		
		int[,] level = new int[levelWidth+1,levelHeight+1];
		//Randomly initialise the level
		for(int x=0; x<levelWidth+1; x++){
			for(int y=0; y<levelHeight+1; y++){
				if(Random.Range (0f,1f) < chanceToStartAlive){
					level[x,y] = 1;
				}
			}
		}
		
		for(int g=0; g<generations; g++){
			int[,] newlevel = new int[levelWidth+1,levelHeight+1];
			for(int x=0; x<levelWidth+1; x++){
				for(int y=0; y<levelHeight+1; y++){
					int n = countNeighbours(level, x, y, 1);
					if(n < minToSurvive){//n > maxToSurvive || 
						newlevel[x,y] = 0;
					}
					else if(n > birthNumber){
						newlevel[x,y] = 1;
					}
					else{
						newlevel[x,y] = level[x,y];
					}
				}
			}
			level = newlevel;
		}

		for(int x=0; x<levelWidth+1; x++){
			level[x,0] = 1;
			level[x,level.GetLength(1)-1] = 1;
		}
		for(int y=0; y<levelHeight+1; y++){
			level[0,y] = 1;
			level[level.GetLength(0)-1,y] = 1;
		}

		return level;
	}

	public int countNeighbours(int[,] map, int x, int y, int tgt){
		int count = 0;
		for(int i=-1; i<2; i++){
			for(int j=-1; j<2; j++){
				if(i == 0 && j == 0)
					continue;
				int dx = x+i; int dy = y+j;
				if(dx >= 0 && dy >= 0 && dx < map.GetLength(0) && dy < map.GetLength(1)){
					if(map[dx,dy] == tgt){
						count++;
					}
				}
				else{
					count++;
				}
			}
		}
		return count;
	}	

	public int countNeighboursFurther(int[,] map, int x, int y, int tgt){
		int count = 0;
		for(int i=-2; i<3; i++){
			for(int j=-2; j<3; j++){
				if(i == 0 && j == 0)
					continue;
				int dx = x+i; int dy = y+j;
				if(dx >= 0 && dy >= 0 && dx < map.GetLength(0) && dy < map.GetLength(1)){
					if(map[dx,dy] == tgt){
						count++;
					}
				}
				else{
					count++;
				}
			}
		}
		return count;
	}	
	
}
