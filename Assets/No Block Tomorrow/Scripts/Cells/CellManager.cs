using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace No_Block_Tomorrow.Scripts.Cells
{
	public class CellManager : MonoBehaviour
	{
		public const  float       BaseYPos = 0.5f;
		public static CellManager Instance;

		[SerializeField] private GameObject  prefab;
		[SerializeField] private Sprite[]    types;
		[SerializeField] private AudioClip   blockBrake;
		[SerializeField] private AudioSource audio;
		[SerializeField] private GameObject  gameOverObj;

		[HideInInspector] public int combo;
		public                   int columns = 9;
		public                   int rows    = 21;

		private int  _fallingCells;
		private bool _isWaiting;

		public Cell[,] Cells;


		public int FallingCells {
			get => _fallingCells;
			set {
				_fallingCells = value;
				if (value == 0 && !_isWaiting)
					StartCoroutine(WaitForIdle());
			}
		}

		private void Awake()
		{
			Instance = this;
			Cells    = new Cell[columns, rows];
			for (int x = 0; x < columns; x++)
			for (int y = 0; y < rows; y++) {
				//check if at risk att making a 3 pair; if so, set respective value
				int disallowedY = -1, disallowedX = -1;
				if (y > 1 && Cells[x, y - 1].type == Cells[x, y - 2].type)
					disallowedY = Cells[x, y - 1].type;
				if (x > 1 && Cells[x - 1, y].type == Cells[x - 2, y].type)
					disallowedX = Cells[x - 1, y].type;

				var newCell = Instantiate(prefab, transform).GetComponent<Cell>();
				newCell.transform.localPosition = new Vector3(x - columns / 2f, y + BaseYPos);
				newCell.x                       = x;
				newCell.y                       = y;
				newCell.type                    = Random.Range(0, types.Length);
				//shift value until allowed
				while (newCell.type == disallowedY || newCell.type == disallowedX)
					newCell.type = (newCell.type + 1) % types.Length;
				newCell.GetComponent<SpriteRenderer>().sprite = types[newCell.type];
				Cells[x, y]                                   = newCell;
			}
		}

		public void PlayBlockBrakeAudio()
		{
			audio.pitch += .025f;
			audio.PlayOneShot(blockBrake);
		}

		private IEnumerator WaitForIdle()
		{
			_isWaiting = true;
			int wait = 0;
			while (wait++ < 10) {
				yield return new WaitForFixedUpdate();
				if (FallingCells > 0)
					wait = 0;
			}

			CheckForCombo();
			_isWaiting = false;
		}


		private void CheckForCombo()
		{
			var cellsToKill    = new List<Cell>();
			var cellsKillDelay = new List<float>();

			audio.pitch = 1;
			combo       = 0;

			for (int y = rows - 1; y >= 0; y--)
			for (int x = 0; x < columns; x++)
				Combo(x, y);

			for (int i = 0; i < cellsToKill.Count; i++)
				cellsToKill[i].Kill(cellsKillDelay[i], combo * .2f + .2f);

			if (combo == 0)
				Input.EnableInput = true;
			else StartCoroutine(WaitAndLookForGravity(combo * .2f + .3f));

			IEnumerator WaitAndLookForGravity(float delay)
			{
				yield return new WaitForSeconds(delay);
				if (!_isWaiting && FallingCells == 0) 
					Input.EnableInput = true;
			}
			
			bool Combo(int x, int y)
			{
				if (GetCell(x, y) is null) return false;

				int type = GetCell(x, y).type;

				int typeInXCount = 1;
				var cellsX       = new List<Cell>(0);
				for (int localX = x + 1; localX < columns; localX++) {
					Cell target = GetCell(localX, y);
					if (!ValidateX(target)) break;
					typeInXCount++;
					cellsX.Add(target);
				}

				for (int localX = x - 1; localX >= 0; localX--) {
					Cell target = GetCell(localX, y);
					if (!ValidateX(target)) break;
					typeInXCount++;
					cellsX.Add(target);
				}

				int typeInYCount = 1;
				var cellsY       = new List<Cell>(0);
				for (int localY = y + 1; localY < columns; localY++) {
					Cell target = GetCell(x, localY);
					if (!ValidateY(target)) break;
					typeInYCount++;
					cellsY.Add(target);
				}

				for (int localY = y - 1; localY >= 0; localY--) {
					Cell target = GetCell(x, localY);
					if (!ValidateY(target)) break;
					typeInYCount++;
					cellsY.Add(target);
				}

				if (typeInXCount > 2) {
					cellsToKill.AddRange(cellsX);
					foreach (Cell cell in cellsX) {
						cell.destroyedX = true;
						cellsKillDelay.Add(.2f * combo + .1f * Mathf.Abs(cell.x - x));
						//cell.Kill(.2f * combo + .1f * Mathf.Abs(cell.x - x));
					}

					PointSystem.GiveComboScore(typeInXCount, combo);
					combo++;
				}

				if (typeInYCount > 2) {
					cellsToKill.AddRange(cellsY);
					foreach (Cell cell in cellsY) {
						cell.destroyedY = true;
						cellsKillDelay.Add(.2f * combo + .1f * Mathf.Abs(cell.y - y));
						//cell.Kill(.2f * combo + .1f * Mathf.Abs(cell.y - y));
					}

					PointSystem.GiveComboScore(typeInYCount, combo);
					combo++;
				}

				if (typeInXCount < 3 && typeInYCount < 3) return false;
				cellsToKill.Add(GetCell(x, y));
				cellsKillDelay.Add(.2f * (combo - 1));
				//GetCell(x, y).Kill(.2f * (combo - 1));
				return true;

				bool ValidateX(Cell target) => target is { } && target.type == type && !target.destroyedX;
				bool ValidateY(Cell target) => target is { } && target.type == type && !target.destroyedY;
			}
		}

		public void GiveUp()
		{
			audio.pitch = 1;
			int count = 0;
			for (int y = rows - 1; y >= 0; y--)
			for (int x = 0; x < columns; x++) {
				Cell c = Cells[x, y];
				if (c is null) continue;
				c.Kill(++count * .1f);
				PointSystem.Points += 10;
			}

			StartCoroutine(EndGameDelay(++count * .1f));

			IEnumerator EndGameDelay(float delay)
			{
				yield return new WaitForSeconds(delay);
				Input.EnableInput = true;
				gameOverObj.SetActive(true);
			}
		}

		public Cell GetCell(int x, int y)
		{
			if (y < 0 || y >= rows || x < 0 || x >= columns) return null;
			Cell c                          = null;
			while (c is null && y < rows) c = Cells[x, y++];

			return c;
		}

		private void GravityFast(Cell cell)
		{
			Gravity(.2f, GetCell(cell.x, cell.y + 1));
			if (cell.destroyed) {
				Cells[cell.x, cell.y] = null;
				return;
			}

			int localY = cell.y;
			while (localY-- > 0)
				if (Cells[cell.x, localY] is { }) {
					localY++;
					break;
				}

			if (localY < 0) localY = 0;

			Cells[cell.x, cell.y] = null;
			Cells[cell.x, localY] = cell;
			cell.y                = localY;
			cell.Jump();
		}
		private void Gravity(Cell cell)
		{
			if (cell is null || cell.y < 1) return;
			GravityFast(cell);
		}
		
		public void Gravity(float delay, Cell cell)
		{
			if (cell is null || cell.y < 1) return;
			if (Input.EnableInput && (!cell.destroyed || GetCell(cell.x, cell.y + 1) is { })) Input.EnableInput = false;
			StartCoroutine(GravityEnumerator(delay, cell));

			IEnumerator GravityEnumerator(float delayInSeconds, Cell targetCell)
			{
				yield return new WaitForSeconds(delayInSeconds);
				GravityFast(targetCell);
			}
		}
	}
}