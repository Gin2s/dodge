using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public static class SceneSetup
{
    [MenuItem("Dodge/Setup Scene")]
    public static void SetupScene()
    {
        foreach (var n in new[] { "Player", "Spawner", "Canvas", "GameManager", "Lanes", "Background" })
        {
            var e = GameObject.Find(n);
            if (e != null) Object.DestroyImmediate(e);
        }

        EnsureTag("Obstacle");
        Directory.CreateDirectory("Assets/Sprites");
        Directory.CreateDirectory("Assets/Prefabs");

        var playerSprite  = CreateAndSaveSprite("Assets/Sprites/Player.png",  CreateHeartTexture(128, new Color(1f, 0.25f, 0.45f), new Color(0.7f, 0f, 0.2f), 5));
        var obstacleSprite = CreateAndSaveSprite("Assets/Sprites/Obstacle.png", CreateSpikeBallTexture(128, Color.white, new Color(0.6f, 0.6f, 0.6f), 5, 8));

        // カメラ
        Camera.main.backgroundColor = new Color(0.92f, 0.97f, 1f);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;


        // レーン線
        var lanesObj = new GameObject("Lanes");
        Color[] laneColors = {
            new Color(1f, 0.5f, 0.7f, 0.6f),
            new Color(0.5f, 0.75f, 1f, 0.6f),
            new Color(1f, 0.8f, 0.3f, 0.6f),
            new Color(0.5f, 0.9f, 0.6f, 0.6f),
        };
        var pixelTex = new Texture2D(1, 1);
        pixelTex.SetPixel(0, 0, Color.white);
        pixelTex.Apply();
        var pixelSprite = Sprite.Create(pixelTex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

        float[] dividers = { -3f, -1f, 1f, 3f };
        for (int i = 0; i < dividers.Length; i++)
        {
            var line = new GameObject("Line");
            line.transform.SetParent(lanesObj.transform);
            line.transform.position = new Vector3(dividers[i], 0, 0);
            line.transform.localScale = new Vector3(0.04f, 100f, 1f);
            var sr = line.AddComponent<SpriteRenderer>();
            sr.sprite = pixelSprite;
            sr.color = laneColors[i];
        }

        // Player
        var player = new GameObject("Player");
        player.transform.position = new Vector3(0, -4, 0);
        player.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        var playerSr = player.AddComponent<SpriteRenderer>();
        playerSr.sprite = playerSprite;
        playerSr.sortingOrder = 1;
        player.AddComponent<BoxCollider2D>();
        player.AddComponent<PlayerController>();

        // Obstacle Prefab
        var obstacleObj = new GameObject("Obstacle");
        var obstacleSr = obstacleObj.AddComponent<SpriteRenderer>();
        obstacleSr.sprite = obstacleSprite;
        obstacleSr.sortingOrder = 1;
        obstacleObj.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        obstacleObj.tag = "Obstacle";
        obstacleObj.AddComponent<BoxCollider2D>();
        obstacleObj.AddComponent<Obstacle>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(obstacleObj, "Assets/Prefabs/Obstacle.prefab");
        Object.DestroyImmediate(obstacleObj);

        // Spawner
        var spawnerObj = new GameObject("Spawner");
        var spawner = spawnerObj.AddComponent<ObstacleSpawner>();
        var so = new SerializedObject(spawner);
        so.FindProperty("obstaclePrefab").objectReferenceValue = prefab;
        so.ApplyModifiedProperties();

        // Canvas
        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        var scoreText = CreateText("ScoreText", canvasObj.transform, "0.0s",
            new Vector2(10, -10), new Vector2(250, 50), TextAnchor.UpperLeft, 36, new Color(0.1f, 0.1f, 0.3f));
        SetAnchor(scoreText, new Vector2(0, 1), new Vector2(0, 1));

        var bestText = CreateText("BestText", canvasObj.transform, "Best: 0.0s",
            new Vector2(10, -60), new Vector2(250, 36), TextAnchor.UpperLeft, 24, new Color(0.4f, 0.4f, 0.65f));
        SetAnchor(bestText, new Vector2(0, 1), new Vector2(0, 1));

        // GameOver Panel
        var panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(canvasObj.transform, false);
        var panelRect = panel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(700, 420);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(1f, 1f, 1f, 0.93f);

        CreateText("GameOverText", panel.transform, "GAME OVER",
            new Vector2(0, 100), new Vector2(680, 110), TextAnchor.MiddleCenter, 80, new Color(1f, 0.3f, 0.45f));
        CreateText("HintText", panel.transform, "Press Enter to retry",
            new Vector2(0, -60), new Vector2(680, 50), TextAnchor.MiddleCenter, 36, new Color(0.4f, 0.4f, 0.7f));

        panel.SetActive(false);

        // Title Panel
        var titlePanel = new GameObject("TitlePanel");
        titlePanel.transform.SetParent(canvasObj.transform, false);
        var titleRect = titlePanel.AddComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(700, 500);
        var titleImg = titlePanel.AddComponent<Image>();
        titleImg.color = new Color(1f, 1f, 1f, 0.93f);

        CreateText("TitleText", titlePanel.transform, "DODGE",
            new Vector2(0, 120), new Vector2(680, 130), TextAnchor.MiddleCenter, 96, new Color(1f, 0.3f, 0.45f));
        CreateText("SubText", titlePanel.transform, "ハートを守れ！",
            new Vector2(0, 20), new Vector2(680, 60), TextAnchor.MiddleCenter, 40, new Color(0.3f, 0.3f, 0.6f));
        CreateText("HintText", titlePanel.transform, "Press Enter to start",
            new Vector2(0, -100), new Vector2(680, 50), TextAnchor.MiddleCenter, 36, new Color(0.5f, 0.5f, 0.75f));

        // GameManager
        var gmObj = new GameObject("GameManager");
        var gm = gmObj.AddComponent<GameManager>();
        var soGm = new SerializedObject(gm);
        soGm.FindProperty("spawner").objectReferenceValue = spawner;
        soGm.FindProperty("player").objectReferenceValue = player.GetComponent<PlayerController>();
        soGm.FindProperty("scoreText").objectReferenceValue = scoreText.GetComponent<Text>();
        soGm.FindProperty("bestText").objectReferenceValue = bestText.GetComponent<Text>();
        soGm.FindProperty("gameOverPanel").objectReferenceValue = panel;
        soGm.FindProperty("titlePanel").objectReferenceValue = titlePanel;
        soGm.ApplyModifiedProperties();

        Debug.Log("セットアップ完了！");
    }

    static Sprite CreateAndSaveSprite(string path, Texture2D tex)
    {
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 100;
        importer.filterMode = FilterMode.Bilinear;
        AssetDatabase.ImportAsset(path);
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static Texture2D CreateHeartTexture(int size, Color fill, Color outline, int outlineWidth)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];
        float pad = outlineWidth + 1;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                // ハートの数式: (x²+y²-1)³ - x²y³ <= 0
                // [-1.2, 1.2] にマッピング（少し上にオフセット）
                float nx = (x - size / 2f) / (size / 2f - pad) * 1.2f;
                float ny = (y - size / 2f) / (size / 2f - pad) * 1.2f + 0.15f;
                float nx2 = (x - size / 2f) / (size / 2f - pad + outlineWidth) * 1.2f;
                float ny2 = (y - size / 2f) / (size / 2f - pad + outlineWidth) * 1.2f + 0.15f;

                bool fill_ = HeartEq(nx, ny);
                bool outline_ = !fill_ && HeartEq(nx2, ny2);
                pixels[y * size + x] = fill_ ? fill : outline_ ? outline : Color.clear;
            }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    static bool HeartEq(float x, float y)
    {
        float a = x * x + y * y - 1f;
        return a * a * a - x * x * y * y * y <= 0f;
    }

    static Texture2D CreateSpikeBallTexture(int size, Color fill, Color outline, int outlineWidth, int spikes)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];
        float cx = (size - 1) / 2f, cy = (size - 1) / 2f;
        float outerR = size / 2f - 2f;
        float innerR = outerR * 0.62f;

        // スパイク多角形の頂点（内外交互）
        var verts = new Vector2[spikes * 2];
        for (int i = 0; i < spikes * 2; i++)
        {
            float angle = Mathf.PI * 2f * i / (spikes * 2) - Mathf.PI / 2f;
            float r = (i % 2 == 0) ? outerR : innerR;
            verts[i] = new Vector2(cx + r * Mathf.Cos(angle), cy + r * Mathf.Sin(angle));
        }

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                bool inShape = IsInPolygon(x, y, verts);
                bool inOutline = !inShape && IsNearPolygon(x, y, verts, outlineWidth);
                pixels[y * size + x] = inShape ? fill : inOutline ? outline : Color.clear;
            }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    static bool IsInPolygon(float px, float py, Vector2[] poly)
    {
        bool inside = false;
        int j = poly.Length - 1;
        for (int i = 0; i < poly.Length; j = i++)
        {
            if ((poly[i].y > py) != (poly[j].y > py) &&
                px < (poly[j].x - poly[i].x) * (py - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
                inside = !inside;
        }
        return inside;
    }

    static bool IsNearPolygon(float px, float py, Vector2[] poly, float dist)
    {
        // 各辺との距離をチェック
        for (int i = 0; i < poly.Length; i++)
        {
            var a = poly[i];
            var b = poly[(i + 1) % poly.Length];
            if (DistToSegment(px, py, a, b) <= dist) return true;
        }
        return false;
    }

    static float DistToSegment(float px, float py, Vector2 a, Vector2 b)
    {
        float dx = b.x - a.x, dy = b.y - a.y;
        float t = Mathf.Clamp01(((px - a.x) * dx + (py - a.y) * dy) / (dx * dx + dy * dy));
        float qx = a.x + t * dx, qy = a.y + t * dy;
        return Mathf.Sqrt((px - qx) * (px - qx) + (py - qy) * (py - qy));
    }

    static GameObject CreateText(string name, Transform parent, string content, Vector2 pos, Vector2 size, TextAnchor anchor, int fontSize, Color color)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        var text = obj.AddComponent<Text>();
        text.text = content;
        text.alignment = anchor;
        text.fontSize = fontSize;
        text.color = color;
        text.font = Font.CreateDynamicFontFromOSFont("Arial", fontSize);
        return obj;
    }

    static void SetAnchor(GameObject obj, Vector2 min, Vector2 max)
    {
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.pivot = min;
    }

    static void EnsureTag(string tag)
    {
        var tm = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var tags = tm.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.arraySize++;
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tm.ApplyModifiedProperties();
    }
}
