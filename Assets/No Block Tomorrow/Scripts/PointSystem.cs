using TMPro;
using UnityEngine;

namespace No_Block_Tomorrow.Scripts
{
	public class PointSystem : MonoBehaviour
	{
		public static PointSystem Instance;

		private static int _desplayValue;

		public TMP_Text text;

		public static int Points { get; set; }

		private void Awake()
		{
			Instance     = this;
			Points       = 0;
			_desplayValue = 0;
		}

		private void FixedUpdate()
		{
			int oldV = _desplayValue;
			_desplayValue = (int) (Mathf.Lerp(_desplayValue + 0.5f, Points, Time.fixedUnscaledDeltaTime) + .5f);
			if (oldV != _desplayValue)
				text.text = _desplayValue.ToString();
		}
	}
}