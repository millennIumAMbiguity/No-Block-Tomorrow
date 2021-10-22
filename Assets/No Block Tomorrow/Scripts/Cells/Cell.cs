using System.Collections;
using UnityEngine;

namespace No_Block_Tomorrow.Scripts.Cells
{
	public class Cell : MonoBehaviour
	{
		public int x, y;
		public int type;

		public bool destroyed;

		public bool destroyedX, destroyedY;

		private Vector3 _targetPos;

		private Transform _trans;
		private float     _velocity;

		private void Awake() => _trans = transform;

		private void FixedUpdate()
		{
			_velocity += Time.fixedDeltaTime;
			Vector3 newPos = _trans.localPosition;
			newPos = new Vector3(newPos.x, newPos.y - _velocity, newPos.z);
			if (newPos.y <= _targetPos.y) {
				enabled              = false;
				_trans.localPosition = _targetPos;
				return;
			}

			_trans.localPosition = newPos;
		}

		private void OnEnable() => CellManager.Instance.FallingCells++;

		private void OnDisable() => CellManager.Instance.FallingCells--;

		public void Jump()
		{
			if (destroyed) return;
			_targetPos           = new Vector3(x - CellManager.Instance.columns / 2f, y + CellManager.BaseYPos);
			_trans.localPosition = new Vector3(_targetPos.x, _trans.localPosition.y);
			if (enabled) return;
			_velocity = 0;
			enabled   = true;
		}


		private void Kill(bool isMouse = false, float gravityDelay = .2f)
		{
			CellManager.Instance.PlayBlockBrakeAudio();
			destroyed                        = true;
			CellManager.Instance.Cells[x, y] = null;
			if (y + 1 < CellManager.Instance.rows) { }

			CellManager.Instance.Gravity(
				isMouse ? 0 : gravityDelay, CellManager.Instance.GetCell(x, y + 1));
			Vector3 newPos = _trans.localPosition;
			//trans.localPosition = new Vector3(newPos.x, newPos.y, 1);
			Destroy(gameObject, 1);
			gameObject.SetActive(false);
		}

		public void Kill(float delay, bool isMouse = false)
		{
			if (destroyed) return;
			destroyed = true;
			StartCoroutine(KillDelay(delay, isMouse));

			IEnumerator KillDelay(float delay, bool isMouse = false)
			{
				yield return new WaitForSeconds(delay);
				Kill(isMouse);
			}
		}

		public void Kill(float delay, float gravityDelay)
		{
			if (destroyed) return;
			destroyed = true;
			StartCoroutine(KillDelay(delay, gravityDelay));

			IEnumerator KillDelay(float delay, float gravityDelay)
			{
				yield return new WaitForSeconds(delay);
				Kill(false, gravityDelay);
			}
		}
	}
}