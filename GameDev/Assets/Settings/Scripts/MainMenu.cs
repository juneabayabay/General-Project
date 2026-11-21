using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static bool GameStarting;

    GameObject mainGroup;
    GameObject settingsGroup;
    GameObject characterGroup;
    Image[] characterFrames;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetPlaySession()
    {
        GameStarting = false;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OpenMenuFirst()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioListener.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (scene.name == "SampleScene" && !GameStarting)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if (scene.name != "MainMenu" || FindAnyObjectByType<MainMenu>() != null)
            return;

        new GameObject("Main Menu").AddComponent<MainMenu>();
    }

    void Awake()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject events = new GameObject("EventSystem");
            events.AddComponent<EventSystem>();
            events.AddComponent<InputSystemUIInputModule>();
        }

        Sprite background = Resources.Load<Sprite>("MainMenu/menu_bg");
        Sprite slab = SpriteFrom(DrawSlab(240, 52));
        Sprite panel = SpriteFrom(DrawSlab(280, 190));
        Sprite groove = SpriteFrom(DrawGroove(80, 14));
        Sprite fill = SpriteFrom(DrawFill(80, 14));
        Sprite handle = SpriteFrom(DrawHandle(16, 16));

        Canvas canvas = CreateCanvas();
        Image wash = CreateImage(canvas.transform, "Cave Wash", null);
        wash.color = new Color(0.08f, 0.04f, 0.03f, 1f);
        Stretch(wash.rectTransform);

        RectTransform stage = new GameObject("Cave", typeof(RectTransform)).GetComponent<RectTransform>();
        stage.SetParent(canvas.transform, false);
        Stretch(stage);
        if (background != null)
        {
            Image art = CreateImage(stage, "Art", background);
            Stretch(art.rectTransform);
            art.preserveAspect = false;
        }

        mainGroup = new GameObject("Buttons", typeof(RectTransform));
        mainGroup.transform.SetParent(stage, false);
        RectTransform buttons = mainGroup.GetComponent<RectTransform>();
        Stretch(buttons);

        Image plaque = CreateImage(buttons, "Title Plate", slab);
        plaque.rectTransform.anchorMin = plaque.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        plaque.rectTransform.sizeDelta = new Vector2(1100f, 220f);
        plaque.rectTransform.anchoredPosition = new Vector2(0f, 300f);
        plaque.raycastTarget = false;

        CreateLabel(buttons, "JUMP BEYOND", new Vector2(0f, 332f), 78);
        CreateLabel(buttons, "EARTH", new Vector2(0f, 248f), 78);
        CreateChoice(buttons, "PLAY", slab, new Vector2(0f, 90f), Play, 980f, 130f);
        CreateChoice(buttons, "CHARACTER", slab, new Vector2(0f, -60f), ShowCharacters, 980f, 130f);
        CreateChoice(buttons, "SETTINGS", slab, new Vector2(0f, -210f), ShowSettings, 980f, 130f);
        CreateChoice(buttons, "QUIT", slab, new Vector2(0f, -360f), Quit, 460f, 96f);

        settingsGroup = new GameObject("Settings", typeof(RectTransform));
        settingsGroup.transform.SetParent(stage, false);
        RectTransform settings = settingsGroup.GetComponent<RectTransform>();
        settings.anchorMin = settings.anchorMax = new Vector2(0.5f, 0.5f);
        settings.sizeDelta = new Vector2(620f, 760f);
        Image board = CreateImage(settings, "Board", panel);
        Stretch(board.rectTransform);

        CreateLabel(settings, "SETTINGS", new Vector2(0f, 280f), 48);
        CreateLabel(settings, "MUSIC", new Vector2(-180f, 120f), 32);
        CreateLabel(settings, "SOUND", new Vector2(-180f, 10f), 32);
        CreateVolumeSlider(settings, "MusicVolume", new Vector2(40f, 120f), groove, fill, handle, true);
        CreateVolumeSlider(settings, "SoundVolume", new Vector2(40f, 10f), groove, fill, handle, false);
        CreateChoice(settings, "BACK", slab, new Vector2(0f, -250f), ShowMain, 320f, 90f);
        settingsGroup.SetActive(false);

        BuildCharacters(stage, slab);
    }

    void Play()
    {
        GameStarting = true;
        SceneManager.LoadScene("SampleScene");
    }

    void ShowSettings()
    {
        mainGroup.SetActive(false);
        settingsGroup.SetActive(true);
    }

    void ShowMain()
    {
        settingsGroup.SetActive(false);
        characterGroup.SetActive(false);
        mainGroup.SetActive(true);
    }

    void ShowCharacters()
    {
        mainGroup.SetActive(false);
        settingsGroup.SetActive(false);
        characterGroup.SetActive(true);
        RefreshCharacters();
    }

    void PickCharacter(int index)
    {
        PlayerPrefs.SetInt("Climber", index);
        RefreshCharacters();
    }

    void RefreshCharacters()
    {
        int selected = Climber.Selected;
        for (int i = 0; i < characterFrames.Length; i++)
            characterFrames[i].color = i == selected ? new Color(1f, 0.82f, 0.35f, 1f) : new Color(0.35f, 0.28f, 0.22f, 1f);
    }

    void BuildCharacters(Transform stage, Sprite slab)
    {
        characterGroup = new GameObject("Characters", typeof(RectTransform));
        characterGroup.transform.SetParent(stage, false);
        RectTransform root = characterGroup.GetComponent<RectTransform>();
        root.anchorMin = root.anchorMax = new Vector2(0.5f, 0.5f);
        root.sizeDelta = new Vector2(640f, 820f);
        Image board = CreateImage(root, "Board", slab);
        Stretch(board.rectTransform);
        board.color = new Color(0.22f, 0.16f, 0.12f, 0.94f);
        board.raycastTarget = false;

        CreateLabel(root, "CHARACTER", new Vector2(0f, 340f), 46);
        Sprite portrait = Resources.Load<Sprite>("Idle/idle_00");
        characterFrames = new Image[Climber.Names.Length];
        for (int i = 0; i < Climber.Names.Length; i++)
        {
            int index = i;
            float x = -225f + i * 150f;
            GameObject choice = new GameObject(Climber.Names[i], typeof(RectTransform), typeof(Image), typeof(Button));
            choice.transform.SetParent(root, false);
            RectTransform rect = choice.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(130f, 220f);
            rect.anchoredPosition = new Vector2(x, 40f);
            Image frame = choice.GetComponent<Image>();
            frame.sprite = slab;
            frame.type = Image.Type.Simple;
            characterFrames[i] = frame;
            Button button = choice.GetComponent<Button>();
            button.targetGraphic = frame;
            button.onClick.AddListener(() => PickCharacter(index));

            if (portrait != null)
            {
                Image face = CreateImage(choice.transform, "Portrait", portrait);
                face.rectTransform.anchorMin = face.rectTransform.anchorMax = new Vector2(0.5f, 0.62f);
                face.rectTransform.sizeDelta = new Vector2(90f, 120f);
                face.preserveAspect = true;
                face.color = Climber.Colors[i];
                face.raycastTarget = false;
            }

            CreateLabel(choice.transform, Climber.Names[i], new Vector2(0f, -78f), 22);
        }

        CreateChoice(root, "BACK", slab, new Vector2(0f, -320f), ShowMain, 300f, 84f);
        characterGroup.SetActive(false);
    }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    static Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Menu Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static void CreateChoice(Transform parent, string label, Sprite slab, Vector2 position, UnityEngine.Events.UnityAction click, float width, float height)
    {
        GameObject buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = position;

        Image image = buttonObject.GetComponent<Image>();
        image.sprite = Stamp(slab, label);
        image.type = Image.Type.Simple;
        image.preserveAspect = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.86f, 0.45f, 1f);
        colors.pressedColor = new Color(0.75f, 0.55f, 0.22f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.fadeDuration = 0.05f;
        button.colors = colors;
        button.onClick.AddListener(click);
    }

    static void CreateLabel(Transform parent, string text, Vector2 position, float height)
    {
        Sprite sprite = SpriteFrom(DrawTextPlate(text, 3));
        Image image = CreateImage(parent, text, sprite);
        image.rectTransform.anchorMin = image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        image.rectTransform.sizeDelta = new Vector2(sprite.rect.width * (height / sprite.rect.height), height);
        image.rectTransform.anchoredPosition = position;
        image.raycastTarget = false;
    }

    static void CreateVolumeSlider(Transform parent, string prefKey, Vector2 position, Sprite groove, Sprite fillSprite, Sprite handleSprite, bool music)
    {
        GameObject root = new GameObject(prefKey, typeof(RectTransform));
        root.transform.SetParent(parent, false);
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(460f, 42f);
        rect.anchoredPosition = position;

        Image background = CreateImage(root.transform, "Background", groove);
        background.rectTransform.anchorMin = new Vector2(0f, 0.28f);
        background.rectTransform.anchorMax = new Vector2(1f, 0.72f);
        background.rectTransform.offsetMin = Vector2.zero;
        background.rectTransform.offsetMax = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(root.transform, false);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.28f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.72f);
        fillAreaRect.offsetMin = new Vector2(6f, 0f);
        fillAreaRect.offsetMax = new Vector2(-6f, 0f);

        Image fill = CreateImage(fillArea.transform, "Fill", fillSprite);
        fill.rectTransform.anchorMin = Vector2.zero;
        fill.rectTransform.anchorMax = new Vector2(0f, 1f);
        fill.rectTransform.offsetMin = Vector2.zero;
        fill.rectTransform.offsetMax = Vector2.zero;
        fill.raycastTarget = false;

        GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(root.transform, false);
        RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(8f, 0f);
        handleAreaRect.offsetMax = new Vector2(-8f, 0f);

        Image handle = CreateImage(handleArea.transform, "Handle", handleSprite);
        handle.rectTransform.sizeDelta = new Vector2(34f, 34f);

        Slider slider = root.AddComponent<Slider>();
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = PlayerPrefs.GetFloat(prefKey, 1f);
        slider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat(prefKey, value);
            if (music)
                AudioListener.volume = value;
        });
    }

    static Image CreateImage(Transform parent, string name, Sprite sprite)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        return image;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    static Sprite Stamp(Sprite slab, string label)
    {
        Texture2D source = slab.texture;
        Texture2D copy = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        copy.filterMode = FilterMode.Point;
        copy.SetPixels32(source.GetPixels32());
            BlitText(copy, label, 3, new Color32(255, 208, 84, 255));
        copy.Apply();
        return Sprite.Create(copy, new Rect(0f, 0f, copy.width, copy.height), new Vector2(0.5f, 0.5f), 100f);
    }

    static Sprite SpriteFrom(Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }

    static Texture2D DrawBackground(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int top = height - 1 - y;
                int edge = 16 + ((x * 5 + top * 3) % 7);
                bool frame = x < edge || x >= width - edge || top < Edge(x) || top >= height - Edge(x + 4);
                Color32 color = frame ? Stone(x, top) : Grass(x, top);
                pixels[y * width + x] = color;
            }
        }

        DrawBush(pixels, width, height, 70, 18);
        DrawBush(pixels, width, height, 150, 14);
        DrawBush(pixels, width, height, 330, 16);
        DrawBush(pixels, width, height, 410, 12);
        texture.SetPixels32(pixels);
        return texture;
    }

    static int Edge(int along)
    {
        int clump = (along / 14) % 4;
        if (clump == 0)
            return 28;
        if (clump == 2)
            return 22;
        return 16;
    }

    static Color32 Grass(int x, int y)
    {
        int noise = (x * 13 + y * 29) % 17;
        if (noise == 0)
            return new Color32(78, 156, 68, 255);
        if (noise == 3)
            return new Color32(176, 226, 138, 255);
        if (x % 9 == 0 && y % 6 < 3)
            return new Color32(92, 170, 74, 255);
        return new Color32(126, 206, 104, 255);
    }

    static Color32 Stone(int x, int y)
    {
        bool mortar = x % 18 == 0 || y % 14 == 0;
        if (mortar)
            return new Color32(86, 98, 90, 255);
        int shade = (x / 18 + y / 14) % 3;
        if (shade == 0)
            return new Color32(198, 206, 196, 255);
        if (shade == 1)
            return new Color32(168, 178, 168, 255);
        return new Color32(214, 220, 210, 255);
    }

    static void DrawBush(Color32[] pixels, int width, int height, int x, int radius)
    {
        int ground = 24;
        for (int py = ground - radius; py <= ground; py++)
        {
            for (int px = x - radius; px <= x + radius; px++)
            {
                int dx = px - x;
                int dy = py - (ground - radius / 2);
                if (dx * dx + dy * dy > radius * radius)
                    continue;
                if (px < 0 || py < 0 || px >= width || py >= height)
                    continue;
                pixels[py * width + px] = (px + py) % 3 == 0
                    ? new Color32(48, 112, 52, 255)
                    : new Color32(70, 146, 62, 255);
            }
        }
    }

    static Texture2D DrawSlab(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[width * height];
        Color32 clear = new Color32(0, 0, 0, 0);
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = clear;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!InsideSlab(x, y, width, height, 8))
                    continue;

                Color32 color = new Color32(58, 46, 40, 255);
                if (y < 7)
                    color = new Color32(96, 70, 36, 255);
                else if (y > height - 9)
                    color = new Color32(28, 20, 16, 255);
                if (!InsideSlab(x, y, width, height, 6))
                    color = new Color32(232, 170, 58, 255);
                pixels[(height - 1 - y) * width + x] = color;
            }
        }

        texture.SetPixels32(pixels);
        return texture;
    }

    static bool InsideSlab(int x, int y, int width, int height, int radius)
    {
        int left = x < radius ? radius - x : 0;
        int right = x >= width - radius ? x - (width - 1 - radius) : 0;
        int top = y < radius ? radius - y : 0;
        int bottom = y >= height - radius ? y - (height - 1 - radius) : 0;
        int dx = Mathf.Max(left, right);
        int dy = Mathf.Max(top, bottom);
        return dx * dx + dy * dy <= radius * radius;
    }

    static Texture2D DrawGroove(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool edge = x < 2 || y < 2 || x >= width - 2 || y >= height - 2;
                pixels[y * width + x] = edge
                    ? new Color32(42, 58, 74, 255)
                    : new Color32(92, 108, 120, 255);
            }
        }

        texture.SetPixels32(pixels);
        return texture;
    }

    static Texture2D DrawFill(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color32(118, 168, 132, 255);
        texture.SetPixels32(pixels);
        return texture;
    }

    static Texture2D DrawHandle(int width, int height)
    {
        return DrawSlab(width, height);
    }

    static Texture2D DrawTextPlate(string text, int scale)
    {
        int width = text.Length * 6 * scale;
        int height = 7 * scale;
        Texture2D texture = new Texture2D(Mathf.Max(1, width), Mathf.Max(1, height), TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[texture.width * texture.height];
        Color32 clear = new Color32(0, 0, 0, 0);
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = clear;
        texture.SetPixels32(pixels);
        BlitText(texture, text, scale, new Color32(255, 208, 84, 255));
        return texture;
    }

    static void BlitText(Texture2D texture, string text, int scale, Color32 color)
    {
        int textWidth = text.Length * 6 * scale - scale;
        int textHeight = 7 * scale;
        int originX = (texture.width - textWidth) / 2;
        int originY = (texture.height - textHeight) / 2;
        int cursor = 0;
        foreach (char character in text)
        {
            if (character == ' ')
            {
                cursor += 6 * scale;
                continue;
            }

            string glyph;
            if (!Glyphs.TryGetValue(character, out glyph))
            {
                cursor += 6 * scale;
                continue;
            }

            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (glyph[row * 5 + col] != '1')
                        continue;
                    for (int py = 0; py < scale; py++)
                    {
                        for (int px = 0; px < scale; px++)
                        {
                            int x = originX + cursor + col * scale + px;
                            int y = originY + (6 - row) * scale + py;
                            if (x >= 0 && y >= 0 && x < texture.width && y < texture.height)
                                texture.SetPixel(x, y, color);
                        }
                    }
                }
            }

            cursor += 6 * scale;
        }
    }

    static readonly Dictionary<char, string> Glyphs = new Dictionary<char, string>
    {
        { 'A', "01110100011000111111100011000110001" },
        { 'B', "11110100011000111110100011000111110" },
        { 'C', "01111100001000010000100001000001111" },
        { 'D', "11110100011000110001100011000111110" },
        { 'E', "11111100001000011110100001000011111" },
        { 'G', "01111100001000010111100011000101110" },
        { 'H', "10001100011000111111100011000110001" },
        { 'I', "11111001000010000100001000010001111" },
        { 'J', "001110001000010000100100011000101110" },
        { 'K', "10001100101010011000101001001010001" },
        { 'L', "10000100001000010000100001000011111" },
        { 'M', "10001110111010110001100011000110001" },
        { 'N', "10001110011010110011100011000110001" },
        { 'O', "01110100011000110001100011000101110" },
        { 'P', "11110100011000111110100001000010000" },
        { 'Q', "01110100011000110001101011001001101" },
        { 'R', "11110100011000111110101001001010001" },
        { 'S', "01111100001000001110000010000111110" },
        { 'T', "11111001000010000100001000010000100" },
        { 'U', "10001100011000110001100011000101110" },
        { 'V', "10001100011000110001010100010000100" },
        { 'Y', "10001100010101000100001000010000100" }
    };
}

public static class Climber
{
    public static readonly string[] Names = { "SCOUT", "EMBER", "RIDGE", "NOVA" };

    public static readonly Color[] Colors =
    {
        Color.white,
        new Color(1f, 0.62f, 0.38f, 1f),
        new Color(0.55f, 0.95f, 0.62f, 1f),
        new Color(0.55f, 0.78f, 1f, 1f)
    };

    public static int Selected
    {
        get { return Mathf.Clamp(PlayerPrefs.GetInt("Climber", 0), 0, Names.Length - 1); }
    }

    public static Color Current
    {
        get { return Colors[Selected]; }
    }
}
