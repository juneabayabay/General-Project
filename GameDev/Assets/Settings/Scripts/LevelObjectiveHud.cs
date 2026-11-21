using UnityEngine;
using UnityEngine.UI;

public class LevelObjectiveHud : MonoBehaviour
{
    public Transform player;
    public Text titleText;
    public Text objectiveText;
    public float layerHeight = 27.52f;

    public string[] titles =
    {
        "Level 1 — Core",
        "Level 2 — Mantle",
        "Level 3 — Earth",
        "Level 4 — Atmosphere",
        "Level 5 — Outer Space"
    };

    public string[] objectives =
    {
        "Objective: Collect objective 1.",
        "Objective: Collect objective 2.",
        "Objective: Collect object 3.",
        "Objective: Collect object 4.",
        "Objective: Collect objective 5."
    };

    int current = -1;

    void LateUpdate()
    {
        if (player == null || titles.Length == 0)
            return;

        int index = LevelLinker.Instance != null
            ? LevelLinker.Instance.LevelAt(player.position.y)
            : Mathf.FloorToInt((player.position.y + layerHeight * 0.5f) / layerHeight);
        index = Mathf.Clamp(index, 0, titles.Length - 1);
        if (index == current)
            return;

        current = index;
        if (titleText != null)
            titleText.text = titles[index];
        if (objectiveText != null)
            objectiveText.text = objectives[index];
    }
}
