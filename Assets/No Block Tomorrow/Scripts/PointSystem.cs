using TMPro;
using UnityEngine;

namespace No_Block_Tomorrow.Scripts
{
	public class PointSystem : MonoBehaviour
	{
		public static PointSystem Instance;

		private static int desplayValue;

		public TMP_Text text;

		public static int Points { get; set; }

		private void Awake()
		{
			Instance     = this;
			Points       = 0;
			desplayValue = 0;
		}

		private void FixedUpdate()
		{
			int oldV = desplayValue;
			desplayValue = (int) (Mathf.Lerp(desplayValue + 0.5f, Points, Time.fixedUnscaledDeltaTime) + .5f);
			if (oldV != desplayValue)
				text.text = desplayValue.ToString();
		}
	}
}