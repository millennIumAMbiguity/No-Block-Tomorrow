using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace No_Block_Tomorrow.Scripts
{
	public class GameOver : MonoBehaviour
	{
		[SerializeField] private TMP_Text scoreText, highScoreText;

		private void OnEnable()
		{
			scoreText.text = PointSystem.Points.ToString();
			int highScore = PlayerPrefs.GetInt("HighScore", 0);
			if (highScore < PointSystem.Points)
				PlayerPrefs.SetInt("HighScore", highScore = PointSystem.Points);
			highScoreText.text = highScore.ToString();
		}


		public void Restart() => SceneManager.LoadScene(0);
	}
}