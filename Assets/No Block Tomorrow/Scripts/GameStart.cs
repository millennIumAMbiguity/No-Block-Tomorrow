using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace No_Block_Tomorrow.Scripts
{
	public class GameStart : MonoBehaviour
	{
		private static           bool       _haveStarted;
		[SerializeField] private GameObject musicPrefab;
		[SerializeField] private float      ratio = 1;

		private void Awake()
		{
			if (!_haveStarted) {
				_haveStarted = true;
				DontDestroyOnLoad(Instantiate(musicPrefab));
			}

			SceneManager.LoadScene(Camera.main.aspect > ratio ? 1 : 2);
		}
	}
}