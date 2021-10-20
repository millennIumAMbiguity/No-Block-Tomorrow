using No_Block_Tomorrow.Scripts.Cells;
using UnityEngine;
using UnityEngine.InputSystem;

namespace No_Block_Tomorrow.Scripts
{
	public class Input : MonoBehaviour
	{
		public static            bool                 EnableInput = true;
		[SerializeField] private InputActionReference press;
		[SerializeField] private InputActionReference pos;


		private void Awake() => EnableInput = true;

		private void OnEnable()
		{
			press.action.Enable();
			press.action.performed += Press;
			pos.action.Enable();
		}

		private void OnDisable()
		{
			press.action.Disable();
			press.action.performed -= Press;
			pos.action.Disable();
		}

		private void Press(InputAction.CallbackContext value)
		{
			if (!EnableInput) return;
			CellSpawner.Instance.combo = 0;
			#if ENABLE_INPUT_SYSTEM
			Vector3 mousePosition = pos.action.ReadValue<Vector2>();
			#else
			Vector3 mousePosition = Input.mousePosition;
			#endif
			RaycastHit2D hit = Physics2D.Raycast(
				Camera.main.ScreenToWorldPoint(mousePosition), Camera.main.transform.forward);
			if (hit.collider is null) return;
			if (hit.collider.TryGetComponent(out Cell cell)) {
				cell.Kill(0, true);
				PointSystem.Points += 10;
			} else if (hit.collider.TryGetComponent(out Button button)) button.Invoke();
		}
	}
}