using UnityEngine;

public class UiManager : MonoBehaviour {
	public static UiManager Singleton { get; private set; }

	[Header("UI items")]
	[SerializeField] private GameObject HotbarUI;
	[SerializeField] private GameObject TrapSelectionUI;

	public enum UIItems {
		Hotbar,
		TrapSelection
	}

	void Start() {
		if (Singleton == null) {
			Singleton = this;
		}
	}

	public void TurnOff(int item) {
		TurnOff((UIItems)item);
	}

	public void TurnOn(int item) {
		TurnOn((UIItems)item);
	}

	public void FlipActive(int item) {
		FlipActive((UIItems)item);
	}

	public void TurnOff(UIItems item) {
		switch (item) {
			case UIItems.Hotbar:
				HotbarUI.SetActive(false);
				break;
			case UIItems.TrapSelection:
				TrapSelectionUI.SetActive(false);
				break;
		}
	}

	public void TurnOn(UIItems item) {
		switch (item) {
			case UIItems.Hotbar:
				HotbarUI.SetActive(true);
				break;
			case UIItems.TrapSelection:
				TrapSelectionUI.SetActive(true);
				break;
		}
	}

	public void FlipActive(UIItems item) {
		switch (item) {
			case UIItems.Hotbar:
				HotbarUI.SetActive(!HotbarUI.activeSelf);
				break;
			case UIItems.TrapSelection:
				TrapSelectionUI.SetActive(!TrapSelectionUI.activeSelf);
				break;
		}
	}

	public void TurnAllOff() {
		HotbarUI?.SetActive(false);
		TrapSelectionUI?.SetActive(false);
	}

	public void TurnAllOn() {
		HotbarUI?.SetActive(true);
		TrapSelectionUI?.SetActive(true);
	}

	public void FlipAllActive() {
		if (HotbarUI != null)
			HotbarUI.SetActive(!HotbarUI.activeSelf);
		if (TrapSelectionUI != null)
			TrapSelectionUI.SetActive(!TrapSelectionUI.activeSelf);
	}
}
