using UnityEngine;
using UnityEngine.Events;

namespace No_Block_Tomorrow.Scripts
{
	public class Button : MonoBehaviour
	{
		[SerializeField] private UnityEvent onClick;

		public void Invoke() => onClick.Invoke();
	}
}