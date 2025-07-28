using UnityEngine;

public class GhostInfoZoom : MonoBehaviour {
	enum GhostType {
		Boobert,
		BoobertVoid,
		Axel,
		AxelVoid,
		Wanda,
		WandaVoid,
		Witch,
		WitchVoid,
		Mildew,
		MildewVoid,
		Mustardo,
		MustardoVoid,
		Sulfur,
		SulfurVoid
	}

	[SerializeField] GameObject inventoryPanel;

	[SerializeField] GameObject boobertPanel, boobertVoidPanel, axelPanel, axelVoidPanel, wandaPanel, wandaVoidPanel, witchPanel, witchVoidPanel, mildewPanel, mildewVoidPanel, mustardoPanel, mustardoVoidPanel, sulfurPanel, sulfurVoidPanel;
	GhostType ghostType;

	public void SetGhostType(string ghostName, bool isVoid) {
		switch (ghostName) {
			case "Boobert":
				ghostType = isVoid ? GhostType.BoobertVoid : GhostType.Boobert;
				break;
			case "Axel":
				ghostType = isVoid ? GhostType.AxelVoid : GhostType.Axel;
				break;
			case "Wanda":
				ghostType = isVoid ? GhostType.WandaVoid : GhostType.Wanda;
				break;
			case "Witch":
				ghostType = isVoid ? GhostType.WitchVoid : GhostType.Witch;
				break;
			case "Mildew":
				ghostType = isVoid ? GhostType.MildewVoid : GhostType.Mildew;
				break;
			case "Mustardo":
				ghostType = isVoid ? GhostType.MustardoVoid : GhostType.Mustardo;
				break;
			case "Sulfur":
				ghostType = isVoid ? GhostType.SulfurVoid : GhostType.Sulfur;
				break;
			default:
				Debug.LogWarning($"Unknown ghost name: {ghostName}");
				break;
		}
	}

	public void onClick() {
		switch (ghostType) {
			case GhostType.Boobert:
				boobertPanel.SetActive(true);
				break;
			case GhostType.BoobertVoid:
				boobertVoidPanel.SetActive(true);
				break;
			case GhostType.Axel:
				axelPanel.SetActive(true);
				break;
			case GhostType.AxelVoid:
				axelVoidPanel.SetActive(true);
				break;
			case GhostType.Wanda:
				wandaPanel.SetActive(true);
				break;
			case GhostType.WandaVoid:
				wandaVoidPanel.SetActive(true);
				break;
			case GhostType.Witch:
				witchPanel.SetActive(true);
				break;
			case GhostType.WitchVoid:
				witchVoidPanel.SetActive(true);
				break;
			case GhostType.Mildew:
				mildewPanel.SetActive(true);
				break;
			case GhostType.MildewVoid:
				mildewVoidPanel.SetActive(true);
				break;
			case GhostType.Mustardo:
				mustardoPanel.SetActive(true);
				break;
			case GhostType.MustardoVoid:
				mustardoVoidPanel.SetActive(true);
				break;
			case GhostType.Sulfur:
				mustardoPanel.SetActive(true);
				break;
			case GhostType.SulfurVoid:
				mustardoVoidPanel.SetActive(true);
				break;
		}
	}

	public void onCloseClick() {
		inventoryPanel.SetActive(false);
	}
}