using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookUIFlip : MonoBehaviour {
	[System.Serializable]
	public class SpriteAnimation {
		public string name;
		public Sprite[] frames;
	}

	public Image bookImage; // UI Image component to display sprites
	public float frameDuration = 0.2f;
	public List<SpriteAnimation> animations;

	private Coroutine currentRoutine;
	private Dictionary<string, Sprite[]> animationDict;

	[SerializeField] GameObject encyclopediaPanel;
	[SerializeField] GameObject openBookButtonPanel;

	[SerializeField] GameObject ghostInfo;

	[Header("Fields")]
	[SerializeField] GameObject ghostName;
	[SerializeField] GameObject image;
	[SerializeField] GameObject desc;
	[SerializeField] GameObject encountered;
	[SerializeField] GameObject caught;

	[SerializeField] EnemyData[] enemyData;
	[SerializeField] EnemyData[] voidEnemyData;

	bool viewingVoid = false;

	int page = 0;

	void Awake() {
		// Build dictionary for quick lookup
		animationDict = new Dictionary<string, Sprite[]>();
		foreach (var anim in animations) {
			if (!animationDict.ContainsKey(anim.name))
				animationDict.Add(anim.name, anim.frames);
		}
	}

	/// <summary>
	/// Call this to play an animation by name.
	/// </summary>
	/// <param name="name">Animation name</param>
	/// <param name="reverse">Play backwards</param>
	public void PlayAnimation(string name, bool reverse = false) {
		Debug.Log($"Trying to play animation: '{name}'");

		if (!animationDict.TryGetValue(name, out Sprite[] frames)) {
			Debug.LogWarning($"Animation '{name}' not found.");
			return;
		}
		Debug.Log($"Found animation '{name}' with {frames.Length} frames.");

		if (currentRoutine != null)
			StopCoroutine(currentRoutine);

		currentRoutine = StartCoroutine(PlayFrames(frames, reverse));
	}

	private void displayGhostInfo(EnemyData data) {
		ghostName.GetComponentInChildren<TMP_Text>().text = data.name;
		image.GetComponentInChildren<Image>().sprite = data.image;
		desc.GetComponentInChildren<TMP_Text>().text = data.desc;
		//encountered.GetComponentInChildren<TMP_Text>().text = PlayerPrefs.Load(data.encountered);
		//caught.GetComponentInChildren<TMP_Text>().text = PlayerPrefs.Load(data.caught);
	}

	public void toggleVoid() {
		if (voidEnemyData == null || page >= voidEnemyData.Length || voidEnemyData[page] == null) {
			Debug.LogWarning($"Void version missing for page {page}");
			return;
		}
		viewingVoid = true;
		displayGhostInfo(voidEnemyData[page]);
	}

	public void toggleNormal() {
		if (enemyData == null || page >= enemyData.Length || enemyData[page] == null) {
			Debug.LogWarning($"Normal version missing for page {page}");
			return;
		}

		viewingVoid = false;
		displayGhostInfo(enemyData[page]);
	}

	private IEnumerator PlayFrames(Sprite[] frames, bool reverse) {
		if (frames == null || frames.Length == 0)
			yield break;

		int start = reverse ? frames.Length - 1 : 0;
		int end = reverse ? -1 : frames.Length;
		int step = reverse ? -1 : 1;

		for (int i = start; i != end; i += step) {
			bookImage.sprite = frames[i];
			float rotation = i * 30;
			//ghostInfo.LeanRotateY(rotation, i);
			yield return new WaitForSeconds(frameDuration);
		}
		ghostInfo.LeanRotateY(0, 0.1f);
	}

	// Helper method for buttons that only pass the animation name (plays forward)
	public void PlayAnimationByName(string name) {
		PlayAnimation(name, false);
		if (name == "Next") {
			if (page >= enemyData.Length - 1) {
				page = 0;
			}
			else {
				page++;
			}
		}
		if (name == "Prev") {
			if (page <= 0) {
				page = enemyData.Length - 1;
			}
			else {
				page--;
			}
		}
		if (viewingVoid) {
			displayGhostInfo(voidEnemyData[page]);
		}
		else {
			displayGhostInfo(enemyData[page]);
		}
	}

	public void openBook() {
		displayGhostInfo(enemyData[page]);
		openBookButtonPanel.SetActive(false);
		encyclopediaPanel.SetActive(true);
	}

	public void closeBook() {
		openBookButtonPanel.SetActive(true);
		encyclopediaPanel.SetActive(false);
	}
}