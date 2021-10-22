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
			Instance      = this;
			Points        = 0;
			_desplayValue = 0;
		}

		private void FixedUpdate()
		{
			int oldV = _desplayValue;
			_desplayValue = (int) (Mathf.Lerp(_desplayValue + 0.5f, Points, Time.fixedUnscaledDeltaTime) + .5f);
			if (oldV != _desplayValue)
				text.text = _desplayValue.ToString();
		}

		/* ech combo will give a additional 15 points.
		 * destroying 3 blocks in a single row will give 150 points (3 * 50)
		 * destroying 4 blocks in a row will give 240 points (4 * 60)
		 * destroying 5 blocks will give 350 points (5 * 70)
		 * 6 blocks 480
		 * 630
		 * 800
		 * ...
		 */
		public static void GiveComboScore(int numberOfBlocks, int combo) =>
			Points += numberOfBlocks * (20 + numberOfBlocks * 10) + combo * 15;

		public static void GiveSingleDestroyScore() =>
			Points += 10;
	}
}