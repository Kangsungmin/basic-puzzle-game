using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ST_PuzzleDisplay : MonoBehaviour
{
    public World world;

    // this puzzle texture.
    public Texture PuzzleImage;
	public UITexture PreviewTexture;

    // the width and height of the puzzle in tiles.
    public int Height = 3;
    public int Width = 3;

    // 섞는 속도
    public float shuffleSpeed = 0.1f;
    public float moveSpeed = 10.0f;

    // additional scaling value.
    public Vector3 PuzzleScale = new Vector3(1.0f, 1.0f, 1.0f);

    // additional positioning offset.
    public Vector3 PuzzlePosition = new Vector3(0.0f, 0.0f, 0.0f);

    // seperation value between puzzle tiles.
    public float SeperationBetweenTiles = 0.5f;

    // the tile display object.
    public GameObject Tile;

    // the shader used to render the puzzle.
    public Shader PuzzleShader;

    // array of the spawned tiles.
    private GameObject[,] TileDisplayArray;
    private Vector3[,] GridToPosMap;            // Grid 와 Position 고정 매핑 데이터 
    private Vector2Int BlankPosition;           // 현재 비어있는 타일의 위치
    private Vector2Int PrevBlankPosition;       // 이전의 비어있는 타일 위치
    private Vector2Int[,] SuffledTilePosition;  // 섞은 후 타일의 위치
    public int ShuffleComplexity;

    private int[,] Dir = new int[4, 2] { { 1,0 },{ -1,0 }, { 0,-1 },{ 0,1 } };
    
	private List<Vector3>  DisplayPositions = new List<Vector3>();

	// position and scale values.
	private Vector3 Scale;
	private Vector3 Position;

    // has the puzzle been completed?
    public bool End = false;
	public bool Complete = false;

    private void Awake()
    {
        world = (World)FindObjectOfType(typeof(World));
        if(world == null)
        {
            // This is TEST MODE

        }
        string theme = world.GetTheme();
        UnityEngine.Object[] fis = Resources.LoadAll("Theme/" + theme);
        Debug.Log("theme:" + theme + ", Len : " + fis.Length);
        //fis 의 수만큼 랜덤값을 뽑아 이미지를 로드한다.
        int imageNumber = Random.Range(0, fis.Length); // 메타파일 때문에 2로 나누어 이미지파일 총 개수를 센다
        string path = "Theme/" + theme + "/" + theme + imageNumber;
        PuzzleImage = Resources.Load<Texture>(path);
    }

    // Use this for initialization
    void Start () 
	{
		PreviewTexture.mainTexture = PuzzleImage;
		// World에서 난이도 인자를 적용한다.
		Width = world.GetWidth();
        Height = world.GetHeight();
        ShuffleComplexity = world.GetShuffleComplexity();
		// create the games puzzle tiles from the provided image.
		CreatePuzzleTiles();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// move the puzzle to the position set in the inspector.
		this.transform.localPosition = PuzzlePosition;

		// set the scale of the entire puzzle object as set in the inspector.
		this.transform.localScale = PuzzleScale;
	}

    public void ShufflePuzzle()
    {
        //TODO: 하단퍼즐 페이드아웃으로 애니메이션 추가하기
        TileDisplayArray[0, 0].GetComponent<ST_PuzzleTile>().Active = false;
        BlankPosition = new Vector2Int(0, 0);
        PrevBlankPosition = BlankPosition;
        //mix up the puzzle.
        StartCoroutine(JugglePuzzle());
    }

    public bool CanPuzzleMove(ST_PuzzleTile thisTile)
    {
        ST_PuzzleTile MoveTo = CheckIfWeCanMove((int)thisTile.GridLocation.x, (int)thisTile.GridLocation.y, thisTile);

        if (MoveTo == thisTile) return false;
        else return true;
    }

    public Vector3 GetTargetLocation(ST_PuzzleTile thisTile)
	{
		// check if we can move this tile and get the position we can move to.
		ST_PuzzleTile MoveTo = CheckIfWeCanMove((int)thisTile.GridLocation.x, (int)thisTile.GridLocation.y, thisTile);

		if(MoveTo != thisTile)
		{
			// get the target position for this new tile.
			Vector3 TargetPos = MoveTo.TargetPosition;
			Vector2 GridLocation = thisTile.GridLocation;
			thisTile.GridLocation = MoveTo.GridLocation;

			// move the empty tile into this tiles current position.
			MoveTo.LaunchPositionCoroutine(thisTile.TargetPosition, moveSpeed);
			MoveTo.GridLocation = GridLocation;

			// return the new target position.
			return TargetPos;
		}

		// else return the tiles actual position (no movement).
		return thisTile.TargetPosition;
	}

	private ST_PuzzleTile CheckMoveLeft(int Xpos, int Ypos, ST_PuzzleTile thisTile)
	{
		// move left 
		if((Xpos - 1)  >= 0)
		{
			// we can move left, is the space currently being used?
			return GetTileAtThisGridLocation(Xpos - 1, Ypos, thisTile);
		}
		
		return thisTile;
	}
	
	private ST_PuzzleTile CheckMoveRight(int Xpos, int Ypos, ST_PuzzleTile thisTile)
	{
		// move right 
		if((Xpos + 1)  < Width)
		{
			// we can move right, is the space currently being used?
			return GetTileAtThisGridLocation(Xpos + 1, Ypos , thisTile);
		}
		
		return thisTile;
	}
	
	private ST_PuzzleTile CheckMoveDown(int Xpos, int Ypos, ST_PuzzleTile thisTile)
	{
		// move down 
		if((Ypos - 1)  >= 0)
		{
			// we can move down, is the space currently being used?
			return GetTileAtThisGridLocation(Xpos, Ypos  - 1, thisTile);
		}
		
		return thisTile;
	}
	
	private ST_PuzzleTile CheckMoveUp(int Xpos, int Ypos, ST_PuzzleTile thisTile)
	{
		// move up 
		if((Ypos + 1)  < Height)
		{
			// we can move up, is the space currently being used?
			return GetTileAtThisGridLocation(Xpos, Ypos  + 1, thisTile);
		}
		
		return thisTile;
	}
	
	private ST_PuzzleTile CheckIfWeCanMove(int Xpos, int Ypos, ST_PuzzleTile thisTile)
	{
		// check each movement direction
		if(CheckMoveLeft(Xpos, Ypos, thisTile) != thisTile)
		{
			return CheckMoveLeft(Xpos, Ypos, thisTile);
		}
		
		if(CheckMoveRight(Xpos, Ypos, thisTile) != thisTile)
		{
			return CheckMoveRight(Xpos, Ypos, thisTile);
		}
		
		if(CheckMoveDown(Xpos, Ypos, thisTile) != thisTile)
		{
			return CheckMoveDown(Xpos, Ypos, thisTile);
		}
		
		if(CheckMoveUp(Xpos, Ypos, thisTile) != thisTile)
		{
			return CheckMoveUp(Xpos, Ypos, thisTile);
		}

		return thisTile;
	}

    // 모든 타일을 검사하여 해당 x,y grid공간이 비어있는지 검사한 후, 비어있다면 해당 위치의 ST_PuzzleTIle를 리턴. 
	private ST_PuzzleTile GetTileAtThisGridLocation(int x, int y, ST_PuzzleTile thisTile)
	{
		for(int j = Height - 1; j >= 0; j--)
		{
			for(int i = 0; i < Width; i++)
			{
				// check if this tile has the correct grid display location.
				if((TileDisplayArray[i,j].GetComponent<ST_PuzzleTile>().GridLocation.x == x)&&
				   (TileDisplayArray[i,j].GetComponent<ST_PuzzleTile>().GridLocation.y == y))
				{
					if(TileDisplayArray[i,j].GetComponent<ST_PuzzleTile>().Active == false)
					{
						// return this tile active property. 
						return TileDisplayArray[i,j].GetComponent<ST_PuzzleTile>();
					}
				}
			}
		}

		return thisTile;
	}

	private IEnumerator JugglePuzzle()
	{
        /*
         * 좌측하단 퍼즐 페이드아웃
         * 나머지 퍼즐 사라진 후 즉시 섞인상태로 등장
         */
		yield return new WaitForSeconds(2.0f);

        // 퍼즐을 0,0 기준으로부터 랜덤하게 빈공간을 채워넣는다.
        // 0,0 위치와 인접한 퍼즐들의 객체에 접급한다.

        for(int i = 0; i < ShuffleComplexity; i++)
        {
			
            List<Vector2Int> moveList = new List<Vector2Int>();
            //현재 빈공간의 인접한 방향에 대해
            for (int d = 0; d < 4; d++) // 동/서/남/북 검사
            {
                //단, 이전 빈공간위치가 아닌 인접한 퍼즐에 대해
                Vector2Int checkPos = new Vector2Int(BlankPosition.x + Dir[d,0], BlankPosition.y + Dir[d,1]);
                if(checkPos.x >= 0 && checkPos.x < Width && checkPos.y >= 0 && checkPos.y < Height && PrevBlankPosition !=  checkPos)
                {
                    moveList.Add(checkPos);
					Debug.Log("chk: " + checkPos.x + "," + checkPos);
				}
            }
            
            int r = Random.Range(0, moveList.Count);

            Vector2Int next = moveList[r];

            //선택한 퍼즐을 빈 공간으로 위치시킨다.(swap 함수구현) ex) value<1,0>.position = <0,0>
            SwapPuzzle( BlankPosition, next ); // next 위치의 퍼즐과 자리를 바꾼다.

            //전빈공간 위치 갱신
            PrevBlankPosition = BlankPosition;
			
			//빈공간 위치 갱신 
			BlankPosition = next;
		}

        //섞인 위치로 실제 퍼즐을 이동시킨다.
        for(int j = 0; j< Height; j++)
        {
            for(int i = 0; i<Width; i++)
            {
                int start_grid_x = SuffledTilePosition[i, j].x;
                int start_grid_y = SuffledTilePosition[i, j].y;//1,0 -> 0,0
                
				TileDisplayArray[start_grid_x, start_grid_y].GetComponent<ST_PuzzleTile>().GridLocation = new Vector2(i, j); // Tile 의 grid 포지션 갱신
                TileDisplayArray[start_grid_x, start_grid_y].GetComponent<ST_PuzzleTile>().MoveToTarget(GridToPosMap[i, j], shuffleSpeed); // targetTile의 position으로 이동시킴

				/****************************
                 **** 회전 가능한 스테이질 경우****
                 ****************************/
				if (PuzzleZone.GetInstance().m_bRotatable && (start_grid_x != 0 || start_grid_y != 0))
                {
					int r = Random.Range(0, 4); 
					    TileDisplayArray[start_grid_x, start_grid_y].GetComponent<ST_PuzzleTile>().RotateToTarget(90.0000f * r, 10.0f);
				}
				
				yield return new WaitForSeconds(0.1f);
            }
        }

        
		// continually check for the correct answer.
		StartCoroutine(CheckForEnd());

		yield return null;
	}

    public void SwapPuzzle(Vector2Int oriP, Vector2Int nxtP)
    {
        Debug.Log("(" + oriP.x + "," + oriP.y + ") <=> (" + nxtP.x + "," + nxtP.y + ")");
        Vector2Int temp = SuffledTilePosition[nxtP.x, nxtP.y];
        SuffledTilePosition[nxtP.x, nxtP.y] = SuffledTilePosition[oriP.x, oriP.y];
        SuffledTilePosition[oriP.x, oriP.y] = temp;
    }

	public IEnumerator CheckForEnd()
	{
		while(Complete == false)
		{
			// iterate over all the tiles and check if they are in the correct position.
			Complete = true;
			for(int j = Height - 1; j >= 0; j--)
			{
				for(int i = 0; i < Width; i++)
				{
					// check if this tile has the correct grid display location.
					if( (TileDisplayArray[i,j].GetComponent<ST_PuzzleTile>().CorrectLocation == false)
                        || (TileDisplayArray[i, j].GetComponent<ST_PuzzleTile>().IsCorrectRotation() == false) )  
					{
						Complete = false;
						break;
					}
				}
			}
            
			yield return null;
		}
				
		// if we are still complete then all the tiles are correct.
		if(Complete)
		{
			Debug.Log("Puzzle Complete!");
            // 퍼즐 상태 완료 전환
            PuzzleZone.GetInstance().SetState(EGameState.eComplete);

			TileDisplayArray[0, 0].GetComponent<Renderer>().enabled = true;
        }
        // 

        yield return null;
	}

	private Vector2 ConvertIndexToGrid(int index)
	{
		int WidthIndex = index;
		int HeightIndex = 0;

		// take the index value and return the grid array location X,Y.
		for(int i = 0; i < Height; i++)
		{
			if(WidthIndex < Width)
			{
				return new Vector2(WidthIndex, HeightIndex);
			}
			else
			{
				WidthIndex -= Width;
				HeightIndex++;
			}
		}

		return new Vector2(WidthIndex, HeightIndex);
	}

	private void CreatePuzzleTiles()
	{
		// using the width and height variables create an array.
		TileDisplayArray = new GameObject[Width,Height];
        SuffledTilePosition = new Vector2Int[Width, Height];
        GridToPosMap = new Vector3[Width, Height];

        // set the scale and position values for this puzzle.
        Scale = new Vector3(1.0f/Width, 1.0f, 1.0f/Height);
		Tile.transform.localScale = Scale;

		// used to count the number of tiles and assign each tile a correct value.
		int TileValue = 0;

		// spawn the tiles into an array.
		for(int j = Height - 1; j >= 0; j--)
		{
			for(int i = 0; i < Width; i++)
			{
				// calculate the position of this tile all centred around Vector3(0.0f, 0.0f, 0.0f).
				Position = new Vector3(((Scale.x * (i + 0.5f))-(Scale.x * (Width/2.0f))) * (10.0f + SeperationBetweenTiles), 
				                       0.0f, 
				                      ((Scale.z * (j + 0.5f))-(Scale.z * (Height/2.0f))) * (10.0f + SeperationBetweenTiles));

				// set this location on the display grid.
				DisplayPositions.Add(Position);

				// spawn the object into play.
				TileDisplayArray[i,j] = Instantiate(Tile, new Vector3(0.0f, 0.0f, 0.0f) , Quaternion.Euler(90.0f, -180.0f, 0.0f)) as GameObject;
				TileDisplayArray[i,j].gameObject.transform.parent = this.transform;

				// set and increment the display number counter.
				ST_PuzzleTile thisTile = TileDisplayArray[i,j].GetComponent<ST_PuzzleTile>();
				thisTile.ArrayLocation = new Vector2(i,j);
				thisTile.GridLocation = new Vector2(i,j);
				thisTile.LaunchPositionCoroutine(Position, moveSpeed);

                // 퍼즐 원본 위치 저장
                GridToPosMap[i, j] = Position;
                SuffledTilePosition[i, j] = new Vector2Int(i, j);
				TileValue++;

				// create a new material using the defined shader.
				Material thisTileMaterial = new Material(PuzzleShader);

				// apply the puzzle image to it.
				thisTileMaterial.mainTexture = PuzzleImage;
					
				// set the offset and tile values for this material.
				thisTileMaterial.mainTextureOffset = new Vector2(1.0f/Width * i, 1.0f/Height * j);
				thisTileMaterial.mainTextureScale  = new Vector2(1.0f/Width, 1.0f/Height);
					
				// assign the new material to this tile for display.
				TileDisplayArray[i,j].GetComponent<Renderer>().material = thisTileMaterial;
			}
		}

		/*
		// Enable an impossible puzzle for fun!
		// switch the second and third grid location textures.
		Material thisTileMaterial2 = TileDisplayArray[1,3].GetComponent<Renderer>().material;
		Material thisTileMaterial3 = TileDisplayArray[2,3].GetComponent<Renderer>().material;
		TileDisplayArray[1,3].GetComponent<Renderer>().material = thisTileMaterial3;
		TileDisplayArray[2,3].GetComponent<Renderer>().material = thisTileMaterial2;
		*/
	}

    public void SetComplexity(int rhs)
    {
        ShuffleComplexity = rhs;
    }
}
