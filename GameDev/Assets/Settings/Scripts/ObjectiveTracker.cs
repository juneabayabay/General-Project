using UnityEngine;
using UnityEngine.UI;

public class ObjectiveTracker : MonoBehaviour
{
    public static ObjectiveTracker Instance { get; private set; }

    static readonly string[] Titles =
    {
        "Level 1 — Core",
        "Level 2 — Mantle",
        "Level 3 — Earth",
        "Level 4 — Atmosphere",
        "Level 5 — Outer Space"
    };

    static readonly string[] Objectives =
    {
        "Collect 5 objectives to unlock the Mantle.",
        "Objective: Collect objective 2.",
        "Objective: Collect object 3.",
        "Objective: Collect object 4.",
        "Objective: Collect objective 5."
    };

    const int NeedPerStage = 3;
    const int CoreObjectives = 5;

    public static int RequiredCount(int levelNumber)
    {
        return levelNumber == 1 ? CoreObjectives : NeedPerStage;
    }

    public static int CollectedCount(int levelNumber)
    {
        if (Instance == null || levelNumber < 1 || levelNumber > 5)
            return 0;

        return Instance.collected[levelNumber - 1];
    }

    readonly int[] collected = new int[5];
    Text titleText;
    Text objectiveText;
    Text livesText;
    Transform player;
    int shown = -1;
    float noticeUntil;

    void Awake()
    {
        Instance = this;
        player = GameObject.Find("Explorer") != null ? GameObject.Find("Explorer").transform : null;
        BuildHud();
    }

    public static void Collect(int levelNumber, bool isCoin)
    {
        if (Instance == null || isCoin)
            return;

        if (levelNumber >= 1 && levelNumber <= 5)
            Instance.collected[levelNumber - 1] = Mathf.Min(RequiredCount(levelNumber), Instance.collected[levelNumber - 1] + 1);

        Instance.shown = -1;
    }

    public static void ShowNotice(string message)
    {
        if (Instance == null || Instance.objectiveText == null)
            return;

        Instance.objectiveText.text = message;
        Instance.noticeUntil = Time.time + 2.8f;
        Instance.shown = -1;
    }

    public static void SetLives(int lives)
    {
        if (Instance == null || Instance.livesText == null)
            return;

        Instance.livesText.text = "Lives: " + lives;
    }

    void LateUpdate()
    {
        if (player == null || titleText == null || Time.time < noticeUntil)
            return;

        int index = LevelLinker.Instance != null
            ? LevelLinker.Instance.LevelAt(player.position.y)
            : 0;
        index = Mathf.Clamp(index, 0, Titles.Length - 1);

        bool all = AllCollected();
        string line = all
            ? "All objectives collected."
            : Objectives[index] + "  " + collected[index] + "/" + RequiredCount(index + 1);

        if (index == shown && objectiveText.text == line)
            return;

        shown = index;
        titleText.text = all ? "Ascent complete" : Titles[index];
        objectiveText.text = line;
    }

    bool AllCollected()
    {
        for (int i = 0; i < collected.Length; i++)
        {
            if (collected[i] < RequiredCount(i + 1))
                return false;
        }

        return true;
    }

    void BuildHud()
    {
        GameObject canvasObject = new GameObject("Objective Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        titleText = CreateLine(canvasObject.transform, "Level Title", 34, -28f);
        objectiveText = CreateLine(canvasObject.transform, "Level Objective", 26, -78f);
        livesText = CreateLine(canvasObject.transform, "Lives", 28, -128f);
        titleText.text = Titles[0];
        objectiveText.text = Objectives[0] + "  0/" + CoreObjectives;
        livesText.text = "Lives: 3";
        livesText.alignment = TextAnchor.UpperLeft;
        livesText.rectTransform.anchorMin = new Vector2(0f, 1f);
        livesText.rectTransform.anchorMax = new Vector2(0f, 1f);
        livesText.rectTransform.pivot = new Vector2(0f, 1f);
        livesText.rectTransform.anchoredPosition = new Vector2(28f, -28f);
    }

    static Text CreateLine(Transform parent, string name, int size, float y)
    {
        GameObject line = new GameObject(name);
        line.transform.SetParent(parent, false);
        Text text = line.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.alignment = TextAnchor.UpperCenter;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        Outline outline = line.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);
        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(1200f, 80f);
        return text;
    }
}
