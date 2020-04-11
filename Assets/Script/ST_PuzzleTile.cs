using UnityEngine;
using System.Collections;
using System;

public class ST_PuzzleTile : MonoBehaviour 
{
	private const float cRotationErrorRange = 1.0f;

	// the target position for this tile.
	public Vector3 TargetPosition;
	// 타일의 회전각
	public Quaternion TargetAngle;

	// is this an active tile?  usually one per game is inactive.
	public bool Active = true;

	// is this tile in the correct location?
	public bool CorrectLocation = false;

	// store this tiles array location.
	public Vector2 ArrayLocation = new Vector2();
	public Vector2 GridLocation = new Vector2();

	public float m_pressedTime = 0.000000f;
	public bool m_bRotated = false;

	void Awake()
	{
		// assign the new target position.
		TargetPosition = this.transform.localPosition;

		// start the movement coroutine to always move the objects to the new target position.
		StartCoroutine(UpdatePosition(10.0f) );
	}

	public  void LaunchPositionCoroutine(Vector3 newPosition, float speed)
	{
		// assign the new target position.
		TargetPosition = newPosition;

		// start the movement coroutine to always move the objects to the new target position.
		StartCoroutine(UpdatePosition(speed));
	}

	public IEnumerator UpdatePosition(float speed)
	{
		// whilst we are not at our target position.
		while(TargetPosition != this.transform.localPosition)
		{
			// lerp towards our target.
			this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, TargetPosition, speed * Time.deltaTime);
			yield return null;
		}

		// after each move check if we are now in the correct location.
		if(ArrayLocation == GridLocation){CorrectLocation = true;}else{CorrectLocation = false;}

		// if we are not an active tile then hide our renderer and collider.
		if(Active == false)
		{
			this.GetComponent<Renderer>().enabled = false;
			this.GetComponent<Collider>().enabled = false;
		}

		yield return null;
	}

    public IEnumerator UpdateRotation(float speed)
    {
		// whilst we are not at our target rotation.
		while (  TargetAngle.y * 10000 != this.transform.localRotation.y * 10000 )
		{
			transform.localRotation = Quaternion.Lerp(transform.localRotation, TargetAngle, Time.fixedDeltaTime * speed);
			yield return null;
		}

		yield return null;
	}

	public void ExecuteAdditionalMove(float shuffleSpeed)
	{
		// get the puzzle display and return the new target location from this tile. 
		LaunchPositionCoroutine(this.transform.parent.GetComponent<ST_PuzzleDisplay>().GetTargetLocation(this.GetComponent<ST_PuzzleTile>()), shuffleSpeed);
	}

    public void MoveToTarget(Vector3 targetPosition, float shuffleSpeed)
    {
        LaunchPositionCoroutine(targetPosition, shuffleSpeed);
    }

    public void RotateToTarget(float targetRotation, float rotateSpeed)
    {
		TargetAngle = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + targetRotation, transform.localEulerAngles.z);
		StartCoroutine(UpdateRotation(rotateSpeed));
    }

    private void OnMouseDrag()
    {
		if ( (PuzzleZone.GetInstance().GetState() == EGameState.eOnGame) && (PuzzleZone.GetInstance().m_bRotatable) )
        {
            if(m_bRotated == false)
            {
				m_pressedTime += 10 * Time.deltaTime;
				if (m_pressedTime > 4)
				{
					// lerp towards our target.
					PuzzleZone.GetInstance().AudioPlayPuzzleSpin();
					float adjustedAngle = (float) Math.Round(transform.localEulerAngles.y);
					//.Log(adjustedAngle);
					TargetAngle = Quaternion.Euler(transform.localEulerAngles.x, adjustedAngle + 90.000f, transform.localEulerAngles.z);
					StartCoroutine(UpdateRotation(8.0f));
					m_bRotated = true;
				}
			}

		}

	}

    void OnMouseUp()//OnMouseDown()
	{
		//Debug.Log(m_pressedTime);
        if( (PuzzleZone.GetInstance().GetState() == EGameState.eOnGame))
        {
            if(m_bRotated == true)
            {
				m_bRotated = false;
			}
            else
            {
				if (this.transform.parent.GetComponent<ST_PuzzleDisplay>().CanPuzzleMove(this.GetComponent<ST_PuzzleTile>()))
				{
					PuzzleZone.GetInstance().AudioPlayPuzzleMove();
					PuzzleZone.GetInstance().IncreaseMoveCount(); // 이동 횟수 증가
																  // get the puzzle display and return the new target location from this tile. 
					LaunchPositionCoroutine(this.transform.parent.GetComponent<ST_PuzzleDisplay>().GetTargetLocation(this.GetComponent<ST_PuzzleTile>()), 10.0f);
				}
			}
			m_pressedTime = 0;
		}
		
	}

    public bool IsCorrectRotation()
    {
		float abs = Mathf.Abs(transform.eulerAngles.y - 180);
		return abs < cRotationErrorRange ? true : false;    //오차 범위 이내라면 true
    }
}
