using UnityEngine;
using UnityEngine.UI;

public class UIButtonManager : MonoBehaviour {

	[Header("Inventory")]
	[SerializeField] GameObject inventoryPanel;
	[SerializeField] GameObject inventoryCloseButton;
	[SerializeField] GameObject inventoryOpenButton;

	[Header("Shop")]
	[SerializeField] GameObject shopPanel;
	[SerializeField] GameObject shopCloseButton;
	[SerializeField] GameObject shopOpenButton;

	[Header("Profile")]
	[SerializeField] GameObject profilePanel;
	[SerializeField] GameObject profileCloseButton;
	[SerializeField] GameObject profileOpenButton;

	[Header("Trap Selection")]
	[SerializeField] GameObject trapSelecttPanel;
	[SerializeField] GameObject trapSelectToggle;
	[SerializeField] GameObject[] trapList;
	[SerializeField] Image trapIconImage;

	public void openInventory() {
		inventoryPanel.SetActive(true);
	}

	public void closeInventory() {
		inventoryPanel.SetActive(false);
	}

	public void openProfile() {
		profilePanel.SetActive(true);
	}
	public void closeProfile() { 
		profilePanel.SetActive(false);
	}

	public void openShop() {
		shopPanel.SetActive(true);
	}

	public void closeShop() { 
		shopPanel.SetActive(false);
	}

	public void toggleTrapSelect() {
		trapSelectToggle.SetActive(false);
		trapSelecttPanel.SetActive(true);
	}

	public void trapSelected(Button selectedTrap) {
		trapSelectToggle.SetActive(true);
		trapSelecttPanel.SetActive(false);

		Image selectedImage = selectedTrap.GetComponent<Image>();
		if (selectedImage != null && trapIconImage != null) {
			trapIconImage.sprite = selectedImage.sprite;
		}
	}
}

