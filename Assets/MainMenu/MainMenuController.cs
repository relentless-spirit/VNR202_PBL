using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Dựng UI trong Editor (ExecuteAlways) và khi chạy game, để bạn thấy menu ngay trong Hierarchy/Scene
/// mà không cần bấm Play — tránh tạo trùng nhờ tìm MainMenuCanvas sẵn có.
/// </summary>
[ExecuteAlways]
public class MainMenuController : MonoBehaviour
{
    const string CanvasObjectName = "MainMenuCanvas";

    static Sprite _whiteUiSprite;

    /// <summary>
    /// Image tạo bằng script không có sprite mặc định → Unity không vẽ gì (kể cả nút).
    /// </summary>
    static Sprite WhiteUiSprite()
    {
        if (_whiteUiSprite != null)
            return _whiteUiSprite;
        var tex = Texture2D.whiteTexture;
        _whiteUiSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        return _whiteUiSprite;
    }

    static void PrepareUiImage(Image img)
    {
        img.sprite = WhiteUiSprite();
        img.type = Image.Type.Simple;
    }

    static readonly Color GoldAntique = new Color(0.78f, 0.62f, 0.28f, 1f);
    [SerializeField] Color backdropBase = new Color(0.11f, 0.08f, 0.06f, 1f);
    [SerializeField] Color parchmentWash = new Color(0.72f, 0.58f, 0.38f, 0.09f);
    [SerializeField] Color vignetteTop = new Color(0.42f, 0.28f, 0.14f, 0.28f);
    [SerializeField] Color vignetteBottom = new Color(0.05f, 0.02f, 0.02f, 0.62f);
    [SerializeField] Color centerGlow = new Color(0.75f, 0.55f, 0.28f, 0.07f);
    [SerializeField] Color accentStripe = new Color(0.72f, 0.55f, 0.22f, 0.92f);
    [SerializeField] Color accentStripeInner = new Color(0.2f, 0.12f, 0.08f, 0.85f);
    [SerializeField] Color cardTint = new Color(0.22f, 0.17f, 0.12f, 0.78f);
    [SerializeField] Color cardBorder = new Color(0.55f, 0.4f, 0.22f, 0.55f);
    [SerializeField] Color primaryButton = new Color(0.48f, 0.14f, 0.14f, 1f);
    [SerializeField] Color primaryButtonHover = new Color(0.62f, 0.2f, 0.18f, 1f);
    [SerializeField] Color secondaryButton = new Color(0.94f, 0.88f, 0.78f, 0.32f);
    [SerializeField] Color secondaryButtonHover = new Color(0.96f, 0.9f, 0.8f, 0.45f);

    [SerializeField] [TextArea(6, 24)]
    string helpStoryText =
        "Trò chơi đưa bạn đi qua các giai đoạn lịch sử Việt Nam qua từng bản đồ — từ những năm kháng chiến đến thời kỳ đổi mới. "
        + "Bạn sẽ nhập vai nhân vật, hoàn thành nhiệm vụ và khám phá sự kiện được gợi nhớ qua bối cảnh, đối thoại và môi trường.\n\n"
        + "Mục tiêu là vừa chơi vừa nắm được dòng thời gian và ý nghĩa của từng mốc sử, phù hợp với nội dung môn học / đồ án của nhóm.";

    [SerializeField] [TextArea(6, 24)]
    string helpHowToText =
        "• Di chuyển: dùng phím hoặc điều khiển theo thiết lập trong game (WASD / mũi tên tùy bản build).\n"
        + "• Tương tác: đến gần điểm gợi ý / NPC / vật thể và dùng phím tương tác khi có nhắc trên màn hình.\n"
        + "• Nhiệm vụ: mở nhật ký / mục tiêu (nếu có trong scene) để biết việc cần làm tiếp theo.\n"
        + "• Lưu: tuỳ cấu hình project — thoát qua menu hoặc checkpoint tự động.\n\n"
        + "Từ menu chính: Chơi — bắt đầu (cần thêm scene gameplay vào Build Settings sau MainMenu); Thoát — đóng game.";

    GameObject _helpOverlayRoot;
    GameObject _helpStoryScroll;
    GameObject _helpHowScroll;
    Text _helpStoryBody;
    Text _helpHowBody;
    Image _helpTabStoryBg;
    Image _helpTabHowBg;

    void OnEnable()
    {
        var existing = GameObject.Find(CanvasObjectName);
        if (existing != null && MainMenuCanvasLooksValid(existing))
        {
            // Canvas đã lưu trong scene: BuildCanvas không chạy → các field _help* vẫn null, onClick có thể mất.
            EnsureHelpReferences();
            WireHelpUiInteractions();
            return;
        }

        if (existing != null)
            DestroyImmediate(existing);

        EnsureEventSystem();
        BuildCanvas();
        WireHelpUiInteractions();
    }

    /// <summary>Gán lại tham chiếu từ hierarchy (khi không gọi BuildCanvas).</summary>
    void EnsureHelpReferences()
    {
        var canvas = GameObject.Find(CanvasObjectName);
        if (canvas == null)
            return;

        var root = canvas.transform.Find("HelpOverlayRoot")?.gameObject;
        _helpOverlayRoot = root;
        if (root == null)
            return;

        _helpStoryScroll = root.transform.Find("HelpPanel/Body/StoryScroll")?.gameObject;
        _helpHowScroll = root.transform.Find("HelpPanel/Body/HowScroll")?.gameObject;
        _helpStoryBody = _helpStoryScroll?.transform.Find("Viewport/Content")?.GetComponent<Text>();
        _helpHowBody = _helpHowScroll?.transform.Find("Viewport/Content")?.GetComponent<Text>();
        _helpTabStoryBg = root.transform.Find("HelpPanel/TabBar/TabStory")?.GetComponent<Image>();
        _helpTabHowBg = root.transform.Find("HelpPanel/TabBar/TabHowTo")?.GetComponent<Image>();
    }

    void WireHelpUiInteractions()
    {
        var canvas = GameObject.Find(CanvasObjectName);
        if (canvas == null)
            return;

        EnsureHelpReferences();

        var fab = canvas.transform.Find("HelpHintButton")?.GetComponent<Button>();
        if (fab != null)
        {
            fab.onClick.RemoveAllListeners();
            fab.onClick.AddListener(OpenHelpPanel);
        }

        if (_helpOverlayRoot == null)
            return;

        var dimBtn = _helpOverlayRoot.transform.Find("Dim")?.GetComponent<Button>();
        if (dimBtn != null)
        {
            dimBtn.onClick.RemoveAllListeners();
            dimBtn.onClick.AddListener(CloseHelpPanel);
        }

        var closeBtn = _helpOverlayRoot.transform.Find("HelpPanel/CloseBtn")?.GetComponent<Button>();
        if (closeBtn != null)
        {
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(CloseHelpPanel);
        }

        var tabStory = _helpOverlayRoot.transform.Find("HelpPanel/TabBar/TabStory")?.GetComponent<Button>();
        if (tabStory != null)
        {
            tabStory.onClick.RemoveAllListeners();
            tabStory.onClick.AddListener(() => SetHelpTab(0));
        }

        var tabHow = _helpOverlayRoot.transform.Find("HelpPanel/TabBar/TabHowTo")?.GetComponent<Button>();
        if (tabHow != null)
        {
            tabHow.onClick.RemoveAllListeners();
            tabHow.onClick.AddListener(() => SetHelpTab(1));
        }
    }

    static bool MainMenuCanvasLooksValid(GameObject canvasRoot)
    {
        if (canvasRoot.transform.Find("HelpHintButton") == null)
            return false;
        if (canvasRoot.transform.Find("HelpOverlayRoot") == null)
            return false;

        var buttons = canvasRoot.GetComponentsInChildren<Button>(true);
        if (buttons == null || buttons.Length < 2)
            return false;
        var firstBtnImg = buttons[0].GetComponent<Image>();
        return firstBtnImg != null && firstBtnImg.sprite != null;
    }

    static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
            return;

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    static Font DefaultUiFont()
    {
        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
               ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    void BuildCanvas()
    {
        var canvasGo = new GameObject(CanvasObjectName);
        var canvas = canvasGo.AddComponent<Canvas>();
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = mainCam;
            canvas.planeDistance = 5f;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        canvas.sortingOrder = 100;
        canvas.pixelPerfect = false;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        var root = new GameObject("Backdrop");
        root.transform.SetParent(canvasGo.transform, false);
        StretchFull(root.AddComponent<RectTransform>());
        var rootImg = root.AddComponent<Image>();
        PrepareUiImage(rootImg);
        rootImg.color = backdropBase;
        rootImg.raycastTarget = false;

        AddVignetteLayer(root.transform, "ParchmentWash", parchmentWash, Vector2.zero, Vector2.one);

        AddVignetteLayer(root.transform, "VignetteTop", vignetteTop,
            new Vector2(0f, 0.42f), Vector2.one);
        AddVignetteLayer(root.transform, "VignetteBottom", vignetteBottom,
            Vector2.zero, new Vector2(1f, 0.4f));

        var glow = new GameObject("CenterGlow");
        glow.transform.SetParent(root.transform, false);
        var glowRt = glow.AddComponent<RectTransform>();
        glowRt.anchorMin = new Vector2(0.5f, 0.5f);
        glowRt.anchorMax = new Vector2(0.5f, 0.5f);
        glowRt.pivot = new Vector2(0.5f, 0.5f);
        glowRt.anchoredPosition = new Vector2(0f, 40f);
        glowRt.sizeDelta = new Vector2(1100f, 620f);
        var glowImg = glow.AddComponent<Image>();
        PrepareUiImage(glowImg);
        glowImg.color = centerGlow;
        glowImg.raycastTarget = false;

        var stripeOuter = new GameObject("AccentStripeOuter");
        stripeOuter.transform.SetParent(root.transform, false);
        var stripeOuterRt = stripeOuter.AddComponent<RectTransform>();
        stripeOuterRt.anchorMin = new Vector2(0f, 1f);
        stripeOuterRt.anchorMax = new Vector2(1f, 1f);
        stripeOuterRt.pivot = new Vector2(0.5f, 1f);
        stripeOuterRt.anchoredPosition = Vector2.zero;
        stripeOuterRt.sizeDelta = new Vector2(0f, 6f);
        var stripeOutImg = stripeOuter.AddComponent<Image>();
        PrepareUiImage(stripeOutImg);
        stripeOutImg.color = accentStripe;
        stripeOutImg.raycastTarget = false;

        var stripeInner = new GameObject("AccentStripeInner");
        stripeInner.transform.SetParent(stripeOuter.transform, false);
        var stripeInnerRt = stripeInner.AddComponent<RectTransform>();
        stripeInnerRt.anchorMin = new Vector2(0f, 0.5f);
        stripeInnerRt.anchorMax = new Vector2(1f, 0.5f);
        stripeInnerRt.pivot = new Vector2(0.5f, 0.5f);
        stripeInnerRt.anchoredPosition = Vector2.zero;
        stripeInnerRt.sizeDelta = new Vector2(0f, 1f);
        var stripeInImg = stripeInner.AddComponent<Image>();
        PrepareUiImage(stripeInImg);
        stripeInImg.color = accentStripeInner;
        stripeInImg.raycastTarget = false;

        var thinLine = new GameObject("AccentLineBottom");
        thinLine.transform.SetParent(root.transform, false);
        var tlRt = thinLine.AddComponent<RectTransform>();
        tlRt.anchorMin = new Vector2(0f, 0f);
        tlRt.anchorMax = new Vector2(1f, 0f);
        tlRt.pivot = new Vector2(0.5f, 0f);
        tlRt.anchoredPosition = new Vector2(0f, 0f);
        tlRt.sizeDelta = new Vector2(0f, 1f);
        var thinImg = thinLine.AddComponent<Image>();
        PrepareUiImage(thinImg);
        thinImg.color = new Color(GoldAntique.r, GoldAntique.g, GoldAntique.b, 0.35f);
        thinImg.raycastTarget = false;

        CreateTitleBlock(root.transform);

        var menuCard = new GameObject("MenuCard");
        menuCard.transform.SetParent(root.transform, false);
        var cardRt = menuCard.AddComponent<RectTransform>();
        cardRt.anchorMin = new Vector2(0.5f, 0.5f);
        cardRt.anchorMax = new Vector2(0.5f, 0.5f);
        cardRt.pivot = new Vector2(0.5f, 0.5f);
        cardRt.anchoredPosition = new Vector2(0f, -72f);
        cardRt.sizeDelta = new Vector2(520f, 288f);

        var cardBg = menuCard.AddComponent<Image>();
        PrepareUiImage(cardBg);
        cardBg.color = cardTint;
        cardBg.raycastTarget = false;

        var cardOutline = menuCard.AddComponent<Outline>();
        cardOutline.effectColor = cardBorder;
        cardOutline.effectDistance = new Vector2(1.5f, 1.5f);

        var cardTopBar = new GameObject("CardTopAccent");
        cardTopBar.transform.SetParent(menuCard.transform, false);
        var barRt = cardTopBar.AddComponent<RectTransform>();
        barRt.anchorMin = new Vector2(0f, 1f);
        barRt.anchorMax = new Vector2(1f, 1f);
        barRt.pivot = new Vector2(0.5f, 1f);
        barRt.offsetMin = new Vector2(16f, -3f);
        barRt.offsetMax = new Vector2(-16f, 0f);
        var topBarImg = cardTopBar.AddComponent<Image>();
        PrepareUiImage(topBarImg);
        topBarImg.color = new Color(GoldAntique.r, GoldAntique.g, GoldAntique.b, 0.75f);
        topBarImg.raycastTarget = false;

        AddCornerBrackets(menuCard.transform, new Color(GoldAntique.r, GoldAntique.g, GoldAntique.b, 0.65f));

        var stack = new GameObject("ButtonStack");
        stack.transform.SetParent(menuCard.transform, false);
        var stackRect = stack.AddComponent<RectTransform>();
        stackRect.anchorMin = new Vector2(0.5f, 0.5f);
        stackRect.anchorMax = new Vector2(0.5f, 0.5f);
        stackRect.pivot = new Vector2(0.5f, 0.5f);
        stackRect.anchoredPosition = new Vector2(0f, -8f);
        stackRect.sizeDelta = new Vector2(400f, 240f);

        var vlg = stack.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 18f;
        vlg.padding = new RectOffset(28, 28, 28, 28);
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;

        CreateMenuButton(stack.transform, "Chơi", OnPlayClicked, primary: true);
        CreateMenuButton(stack.transform, "Thoát", OnQuitClicked, primary: false);

        CreateFooter(root.transform);

        CreateHelpHintButton(canvasGo.transform);
        CreateHelpOverlay(canvasGo.transform);

        if (Camera.main != null)
            Camera.main.backgroundColor = new Color(0.13f, 0.1f, 0.08f, 1f);

        Canvas.ForceUpdateCanvases();
        RefreshHelpScrollContentSizes();
    }

    void Start()
    {
        EnsureHelpReferences();
        WireHelpUiInteractions();
        RefreshHelpScrollContentSizes();
    }

    void CreateHelpHintButton(Transform canvas)
    {
        var go = new GameObject("HelpHintButton");
        go.transform.SetParent(canvas, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(1f, 0f);
        rt.anchoredPosition = new Vector2(-28f, 28f);
        rt.sizeDelta = new Vector2(56f, 56f);

        var img = go.AddComponent<Image>();
        PrepareUiImage(img);
        img.color = new Color(0.72f, 0.52f, 0.22f, 0.96f);
        img.raycastTarget = true;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.transition = Selectable.Transition.ColorTint;
        var cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1f, 0.95f, 0.88f, 1f);
        cb.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        cb.fadeDuration = 0.1f;
        btn.colors = cb;
        btn.onClick.AddListener(OpenHelpPanel);

        var icon = new GameObject("Icon");
        icon.transform.SetParent(go.transform, false);
        var ir = icon.AddComponent<RectTransform>();
        StretchFull(ir);
        var t = icon.AddComponent<Text>();
        t.font = DefaultUiFont();
        t.fontSize = 38;
        t.fontStyle = FontStyle.Bold;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = new Color(0.18f, 0.1f, 0.05f, 1f);
        t.text = "!";
        t.raycastTarget = false;
    }

    void CreateHelpOverlay(Transform canvas)
    {
        var overlay = new GameObject("HelpOverlayRoot");
        overlay.transform.SetParent(canvas, false);
        _helpOverlayRoot = overlay;
        var ort = overlay.AddComponent<RectTransform>();
        StretchFull(ort);
        overlay.SetActive(false);

        // Lớp nền đen mờ phía sau (chỉ hiển thị; click đóng vẫn do Dim phía trên).
        var backdropDim = new GameObject("BackdropDim");
        backdropDim.transform.SetParent(overlay.transform, false);
        var bdRt = backdropDim.AddComponent<RectTransform>();
        StretchFull(bdRt);
        var bdImg = backdropDim.AddComponent<Image>();
        PrepareUiImage(bdImg);
        bdImg.color = new Color(0.03f, 0.02f, 0.02f, 0.98f);
        bdImg.raycastTarget = false;

        var dim = new GameObject("Dim");
        dim.transform.SetParent(overlay.transform, false);
        var drt = dim.AddComponent<RectTransform>();
        StretchFull(drt);
        var dimImg = dim.AddComponent<Image>();
        PrepareUiImage(dimImg);
        dimImg.color = new Color(0f, 0f, 0f, 0.28f);
        dimImg.raycastTarget = true;
        var dimBtn = dim.AddComponent<Button>();
        dimBtn.targetGraphic = dimImg;
        dimBtn.transition = Selectable.Transition.None;
        dimBtn.onClick.AddListener(CloseHelpPanel);

        var panel = new GameObject("HelpPanel");
        panel.transform.SetParent(overlay.transform, false);
        var prt = panel.AddComponent<RectTransform>();
        prt.anchorMin = new Vector2(0.5f, 0.5f);
        prt.anchorMax = new Vector2(0.5f, 0.5f);
        prt.pivot = new Vector2(0.5f, 0.5f);
        prt.sizeDelta = new Vector2(780f, 520f);

        var pBg = panel.AddComponent<Image>();
        PrepareUiImage(pBg);
        pBg.color = new Color(0.18f, 0.14f, 0.1f, 0.97f);
        pBg.raycastTarget = true;

        var pOut = panel.AddComponent<Outline>();
        pOut.effectColor = cardBorder;
        pOut.effectDistance = new Vector2(1.5f, 1.5f);

        var header = new GameObject("Header");
        header.transform.SetParent(panel.transform, false);
        var hrt = header.AddComponent<RectTransform>();
        hrt.anchorMin = new Vector2(0f, 1f);
        hrt.anchorMax = new Vector2(1f, 1f);
        hrt.pivot = new Vector2(0.5f, 1f);
        hrt.anchoredPosition = new Vector2(0f, -12f);
        hrt.sizeDelta = new Vector2(-40f, 44f);

        var title = new GameObject("Title");
        title.transform.SetParent(header.transform, false);
        var titRt = title.AddComponent<RectTransform>();
        titRt.anchorMin = new Vector2(0f, 0f);
        titRt.anchorMax = new Vector2(1f, 1f);
        titRt.offsetMin = Vector2.zero;
        titRt.offsetMax = new Vector2(-48f, 0f);
        var tit = title.AddComponent<Text>();
        tit.font = DefaultUiFont();
        tit.fontSize = 26;
        tit.fontStyle = FontStyle.Bold;
        tit.alignment = TextAnchor.MiddleLeft;
        tit.color = new Color(0.95f, 0.88f, 0.75f, 1f);
        tit.text = "Hướng dẫn";
        tit.raycastTarget = false;

        var closeGo = new GameObject("CloseBtn");
        closeGo.transform.SetParent(header.transform, false);
        var crt = closeGo.AddComponent<RectTransform>();
        crt.anchorMin = new Vector2(1f, 0.5f);
        crt.anchorMax = new Vector2(1f, 0.5f);
        crt.pivot = new Vector2(1f, 0.5f);
        crt.anchoredPosition = new Vector2(0f, 0f);
        crt.sizeDelta = new Vector2(40f, 40f);
        var cImg = closeGo.AddComponent<Image>();
        PrepareUiImage(cImg);
        cImg.color = new Color(0.4f, 0.15f, 0.12f, 0.9f);
        var cBtn = closeGo.AddComponent<Button>();
        cBtn.targetGraphic = cImg;
        cBtn.transition = Selectable.Transition.ColorTint;
        var cc = cBtn.colors;
        cc.highlightedColor = new Color(0.55f, 0.22f, 0.18f, 1f);
        cBtn.colors = cc;
        cBtn.onClick.AddListener(CloseHelpPanel);
        var cx = new GameObject("X");
        cx.transform.SetParent(closeGo.transform, false);
        var cxr = cx.AddComponent<RectTransform>();
        StretchFull(cxr);
        var ctx = cx.AddComponent<Text>();
        ctx.font = DefaultUiFont();
        ctx.fontSize = 28;
        ctx.alignment = TextAnchor.MiddleCenter;
        ctx.color = new Color(0.95f, 0.9f, 0.85f, 1f);
        ctx.text = "×";
        ctx.raycastTarget = false;

        var tabBar = new GameObject("TabBar");
        tabBar.transform.SetParent(panel.transform, false);
        var tbrt = tabBar.AddComponent<RectTransform>();
        tbrt.anchorMin = new Vector2(0f, 1f);
        tbrt.anchorMax = new Vector2(1f, 1f);
        tbrt.pivot = new Vector2(0.5f, 1f);
        tbrt.anchoredPosition = new Vector2(0f, -56f);
        tbrt.sizeDelta = new Vector2(-40f, 44f);

        var tabH = tabBar.AddComponent<HorizontalLayoutGroup>();
        tabH.spacing = 10f;
        tabH.padding = new RectOffset(0, 0, 0, 0);
        tabH.childAlignment = TextAnchor.MiddleLeft;
        tabH.childControlWidth = false;
        tabH.childControlHeight = true;
        tabH.childForceExpandWidth = false;

        void AddTabButton(string label, bool isStory, UnityEngine.Events.UnityAction onClick)
        {
            var tgo = new GameObject(isStory ? "TabStory" : "TabHowTo");
            tgo.transform.SetParent(tabBar.transform, false);
            var le = tgo.AddComponent<LayoutElement>();
            le.preferredWidth = 200f;
            le.preferredHeight = 40f;
            var tImg = tgo.AddComponent<Image>();
            PrepareUiImage(tImg);
            var sel = new Color(0.5f, 0.36f, 0.18f, 1f);
            var unsel = new Color(0.28f, 0.22f, 0.16f, 0.95f);
            tImg.color = isStory ? sel : unsel;
            if (isStory)
                _helpTabStoryBg = tImg;
            else
                _helpTabHowBg = tImg;
            var tBtn = tgo.AddComponent<Button>();
            tBtn.targetGraphic = tImg;
            tBtn.transition = Selectable.Transition.None;
            tBtn.onClick.AddListener(onClick);
            var tt = new GameObject("Label");
            tt.transform.SetParent(tgo.transform, false);
            var ttr = tt.AddComponent<RectTransform>();
            StretchFull(ttr);
            var tx = tt.AddComponent<Text>();
            tx.font = DefaultUiFont();
            tx.fontSize = 20;
            tx.fontStyle = FontStyle.Bold;
            tx.alignment = TextAnchor.MiddleCenter;
            tx.color = new Color(0.94f, 0.88f, 0.78f, 1f);
            tx.text = label;
            tx.raycastTarget = false;
        }

        AddTabButton("Cốt chuyện", true, () => SetHelpTab(0));
        AddTabButton("Cách chơi", false, () => SetHelpTab(1));

        var body = new GameObject("Body");
        body.transform.SetParent(panel.transform, false);
        var bodyRt = body.AddComponent<RectTransform>();
        bodyRt.anchorMin = new Vector2(0f, 0f);
        bodyRt.anchorMax = new Vector2(1f, 1f);
        bodyRt.pivot = new Vector2(0.5f, 0.5f);
        bodyRt.offsetMin = new Vector2(20f, 20f);
        bodyRt.offsetMax = new Vector2(-20f, -108f);

        _helpStoryScroll = CreateHelpScrollArea(body.transform, "StoryScroll", helpStoryText, out _helpStoryBody);
        _helpHowScroll = CreateHelpScrollArea(body.transform, "HowScroll", helpHowToText, out _helpHowBody);
        SetHelpTab(0);
    }

    GameObject CreateHelpScrollArea(Transform parent, string name, string body, out Text textOut)
    {
        var scrollGo = new GameObject(name);
        scrollGo.transform.SetParent(parent, false);
        var srt = scrollGo.AddComponent<RectTransform>();
        StretchFull(srt);

        var sr = scrollGo.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Clamped;
        sr.scrollSensitivity = 24f;

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollGo.transform, false);
        var vpRt = viewport.AddComponent<RectTransform>();
        StretchFull(vpRt);
        var vpImg = viewport.AddComponent<Image>();
        PrepareUiImage(vpImg);
        vpImg.color = new Color(0.06f, 0.05f, 0.04f, 0.45f);
        vpImg.raycastTarget = true;
        viewport.AddComponent<RectMask2D>();

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var crt = content.AddComponent<RectTransform>();
        crt.anchorMin = new Vector2(0f, 1f);
        crt.anchorMax = new Vector2(1f, 1f);
        crt.pivot = new Vector2(0.5f, 1f);
        crt.anchoredPosition = Vector2.zero;
        crt.offsetMin = new Vector2(14f, 0f);
        crt.offsetMax = new Vector2(-14f, 0f);
        crt.sizeDelta = new Vector2(0f, 400f);

        var txt = content.AddComponent<Text>();
        txt.font = DefaultUiFont();
        txt.fontSize = 19;
        txt.color = new Color(0.9f, 0.85f, 0.76f, 1f);
        txt.alignment = TextAnchor.UpperLeft;
        txt.horizontalOverflow = HorizontalWrapMode.Wrap;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        txt.supportRichText = false;
        txt.text = body;
        txt.raycastTarget = false;

        sr.content = crt;
        sr.viewport = vpRt;

        textOut = txt;
        return scrollGo;
    }

    void RefreshHelpScrollContentSizes()
    {
        if (_helpStoryBody == null || _helpHowBody == null)
            return;

        const float wrap = 628f;
        ApplyTextPreferredHeight(_helpStoryBody, wrap);
        ApplyTextPreferredHeight(_helpHowBody, wrap);
        Canvas.ForceUpdateCanvases();
    }

    static void ApplyTextPreferredHeight(Text txt, float wrapWidth)
    {
        var rt = txt.rectTransform;
        var settings = txt.GetGenerationSettings(new Vector2(wrapWidth, 0f));
        float h = txt.cachedTextGeneratorForLayout.GetPreferredHeight(txt.text, settings);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, Mathf.Max(h + 28f, 120f));
    }

    void OpenHelpPanel()
    {
        EnsureHelpReferences();
        if (_helpStoryBody != null)
            _helpStoryBody.text = helpStoryText;
        if (_helpHowBody != null)
            _helpHowBody.text = helpHowToText;

        if (_helpOverlayRoot != null)
            _helpOverlayRoot.SetActive(true);

        RefreshHelpScrollContentSizes();
        SetHelpTab(0);
    }

    void CloseHelpPanel()
    {
        if (_helpOverlayRoot != null)
            _helpOverlayRoot.SetActive(false);
    }

    void SetHelpTab(int index)
    {
        if (_helpStoryScroll != null)
            _helpStoryScroll.SetActive(index == 0);
        if (_helpHowScroll != null)
            _helpHowScroll.SetActive(index == 1);

        void ResetScroll(GameObject scroll)
        {
            if (scroll == null) return;
            var sr = scroll.GetComponent<ScrollRect>();
            if (sr != null)
                sr.verticalNormalizedPosition = 1f;
        }

        ResetScroll(_helpStoryScroll);
        ResetScroll(_helpHowScroll);

        var sel = new Color(0.5f, 0.36f, 0.18f, 1f);
        var unsel = new Color(0.28f, 0.22f, 0.16f, 0.95f);
        if (_helpTabStoryBg != null)
            _helpTabStoryBg.color = index == 0 ? sel : unsel;
        if (_helpTabHowBg != null)
            _helpTabHowBg.color = index == 1 ? sel : unsel;
    }

    static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void AddVignetteLayer(Transform parent, string name, Color color, Vector2 anchorMin,
        Vector2 anchorMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var vi = go.AddComponent<Image>();
        PrepareUiImage(vi);
        vi.color = color;
        vi.raycastTarget = false;
    }

    void AddOrnamentalDivider(Transform parent, Color gold)
    {
        var row = new GameObject("OrnamentDivider");
        row.transform.SetParent(parent, false);
        var rt = row.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.06f);
        rt.anchorMax = new Vector2(0.5f, 0.06f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(440f, 14f);

        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.spacing = 12f;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        var lineTint = new Color(gold.r, gold.g, gold.b, 0.5f);

        var left = new GameObject("LineL");
        left.transform.SetParent(row.transform, false);
        var ll = left.AddComponent<LayoutElement>();
        ll.preferredWidth = 130f;
        ll.preferredHeight = 2f;
        // LayoutElement đã kéo theo RectTransform — không AddComponent<RectTransform> lần hai.
        var lrt = (RectTransform)left.transform;
        lrt.sizeDelta = new Vector2(130f, 2f);
        var li = left.AddComponent<Image>();
        PrepareUiImage(li);
        li.color = lineTint;
        li.raycastTarget = false;

        var gem = new GameObject("Gem");
        gem.transform.SetParent(row.transform, false);
        var gle = gem.AddComponent<LayoutElement>();
        gle.preferredWidth = gle.preferredHeight = 10f;
        ((RectTransform)gem.transform).sizeDelta = new Vector2(10f, 10f);
        var gi = gem.AddComponent<Image>();
        PrepareUiImage(gi);
        gi.color = new Color(gold.r, gold.g, gold.b, 0.72f);
        gi.raycastTarget = false;
        gem.transform.localEulerAngles = new Vector3(0f, 0f, 45f);

        var right = new GameObject("LineR");
        right.transform.SetParent(row.transform, false);
        var lr = right.AddComponent<LayoutElement>();
        lr.preferredWidth = 130f;
        lr.preferredHeight = 2f;
        var rrt = (RectTransform)right.transform;
        rrt.sizeDelta = new Vector2(130f, 2f);
        var ri = right.AddComponent<Image>();
        PrepareUiImage(ri);
        ri.color = lineTint;
        ri.raycastTarget = false;
    }

    void AddCornerBrackets(Transform card, Color gold)
    {
        const float inset = 11f;
        const float arm = 36f;
        const float t = 2f;

        void CornerTL()
        {
            var g = new GameObject("BracketTL");
            g.transform.SetParent(card, false);
            var grt = g.AddComponent<RectTransform>();
            grt.anchorMin = grt.anchorMax = new Vector2(0f, 1f);
            grt.pivot = new Vector2(0f, 1f);
            grt.anchoredPosition = new Vector2(inset, -inset);
            grt.sizeDelta = new Vector2(arm, arm);

            var h = new GameObject("H");
            h.transform.SetParent(g.transform, false);
            var hr = h.AddComponent<RectTransform>();
            hr.anchorMin = hr.anchorMax = new Vector2(0f, 1f);
            hr.pivot = new Vector2(0f, 1f);
            hr.anchoredPosition = Vector2.zero;
            hr.sizeDelta = new Vector2(arm - t, t);
            var hi = h.AddComponent<Image>();
            PrepareUiImage(hi);
            hi.color = gold;
            hi.raycastTarget = false;

            var v = new GameObject("V");
            v.transform.SetParent(g.transform, false);
            var vr = v.AddComponent<RectTransform>();
            vr.anchorMin = vr.anchorMax = new Vector2(0f, 1f);
            vr.pivot = new Vector2(0f, 1f);
            vr.anchoredPosition = new Vector2(0f, -t);
            vr.sizeDelta = new Vector2(t, arm - t);
            var vi = v.AddComponent<Image>();
            PrepareUiImage(vi);
            vi.color = gold;
            vi.raycastTarget = false;
        }

        void CornerTR()
        {
            var g = new GameObject("BracketTR");
            g.transform.SetParent(card, false);
            var grt = g.AddComponent<RectTransform>();
            grt.anchorMin = grt.anchorMax = new Vector2(1f, 1f);
            grt.pivot = new Vector2(1f, 1f);
            grt.anchoredPosition = new Vector2(-inset, -inset);
            grt.sizeDelta = new Vector2(arm, arm);

            var h = new GameObject("H");
            h.transform.SetParent(g.transform, false);
            var hr = h.AddComponent<RectTransform>();
            hr.anchorMin = hr.anchorMax = new Vector2(1f, 1f);
            hr.pivot = new Vector2(1f, 1f);
            hr.anchoredPosition = Vector2.zero;
            hr.sizeDelta = new Vector2(arm - t, t);
            var hiTr = h.AddComponent<Image>();
            PrepareUiImage(hiTr);
            hiTr.color = gold;
            hiTr.raycastTarget = false;

            var v = new GameObject("V");
            v.transform.SetParent(g.transform, false);
            var vr = v.AddComponent<RectTransform>();
            vr.anchorMin = vr.anchorMax = new Vector2(1f, 1f);
            vr.pivot = new Vector2(1f, 1f);
            vr.anchoredPosition = new Vector2(-(arm - t), -t);
            vr.sizeDelta = new Vector2(t, arm - t);
            var viTr = v.AddComponent<Image>();
            PrepareUiImage(viTr);
            viTr.color = gold;
            viTr.raycastTarget = false;
        }

        void CornerBL()
        {
            var g = new GameObject("BracketBL");
            g.transform.SetParent(card, false);
            var grt = g.AddComponent<RectTransform>();
            grt.anchorMin = grt.anchorMax = new Vector2(0f, 0f);
            grt.pivot = new Vector2(0f, 0f);
            grt.anchoredPosition = new Vector2(inset, inset);
            grt.sizeDelta = new Vector2(arm, arm);

            var v = new GameObject("V");
            v.transform.SetParent(g.transform, false);
            var vr = v.AddComponent<RectTransform>();
            vr.anchorMin = vr.anchorMax = new Vector2(0f, 0f);
            vr.pivot = new Vector2(0f, 0f);
            vr.anchoredPosition = Vector2.zero;
            vr.sizeDelta = new Vector2(t, arm - t);
            var viBl = v.AddComponent<Image>();
            PrepareUiImage(viBl);
            viBl.color = gold;
            viBl.raycastTarget = false;

            var h = new GameObject("H");
            h.transform.SetParent(g.transform, false);
            var hr = h.AddComponent<RectTransform>();
            hr.anchorMin = hr.anchorMax = new Vector2(0f, 0f);
            hr.pivot = new Vector2(0f, 0f);
            hr.anchoredPosition = new Vector2(t, 0f);
            hr.sizeDelta = new Vector2(arm - t, t);
            var hiBl = h.AddComponent<Image>();
            PrepareUiImage(hiBl);
            hiBl.color = gold;
            hiBl.raycastTarget = false;
        }

        void CornerBR()
        {
            var g = new GameObject("BracketBR");
            g.transform.SetParent(card, false);
            var grt = g.AddComponent<RectTransform>();
            grt.anchorMin = grt.anchorMax = new Vector2(1f, 0f);
            grt.pivot = new Vector2(1f, 0f);
            grt.anchoredPosition = new Vector2(-inset, inset);
            grt.sizeDelta = new Vector2(arm, arm);

            var h = new GameObject("H");
            h.transform.SetParent(g.transform, false);
            var hr = h.AddComponent<RectTransform>();
            hr.anchorMin = hr.anchorMax = new Vector2(0f, 0f);
            hr.pivot = new Vector2(0f, 0f);
            hr.anchoredPosition = Vector2.zero;
            hr.sizeDelta = new Vector2(arm - t, t);
            var hiBr = h.AddComponent<Image>();
            PrepareUiImage(hiBr);
            hiBr.color = gold;
            hiBr.raycastTarget = false;

            var v = new GameObject("V");
            v.transform.SetParent(g.transform, false);
            var vr = v.AddComponent<RectTransform>();
            vr.anchorMin = vr.anchorMax = new Vector2(1f, 0f);
            vr.pivot = new Vector2(1f, 0f);
            vr.anchoredPosition = Vector2.zero;
            vr.sizeDelta = new Vector2(t, arm - t);
            var viBr = v.AddComponent<Image>();
            PrepareUiImage(viBr);
            viBr.color = gold;
            viBr.raycastTarget = false;
        }

        CornerTL();
        CornerTR();
        CornerBL();
        CornerBR();
    }

    void CreateTitleBlock(Transform parent)
    {
        var block = new GameObject("TitleBlock");
        block.transform.SetParent(parent, false);
        var blockRect = block.AddComponent<RectTransform>();
        blockRect.anchorMin = new Vector2(0.5f, 0.78f);
        blockRect.anchorMax = new Vector2(0.5f, 0.78f);
        blockRect.pivot = new Vector2(0.5f, 0.5f);
        blockRect.anchoredPosition = new Vector2(0f, 12f);
        blockRect.sizeDelta = new Vector2(1000f, 220f);

        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(block.transform, false);
        var titleRect = titleGo.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.58f);
        titleRect.anchorMax = new Vector2(0.5f, 0.58f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(980f, 128f);
        var titleText = titleGo.AddComponent<Text>();
        titleText.font = DefaultUiFont();
        titleText.fontSize = 64;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.supportRichText = true;
        titleText.color = Color.white;
        titleText.text =
            "<color=#d4af37>VNR202</color><color=#f0e6d2> · PBL</color>\n<size=24><color=#a09078><i>Dòng chảy thời gian</i></color></size>";
        titleText.raycastTarget = false;

        var shadow = titleGo.AddComponent<Shadow>();
        shadow.effectColor = new Color(0.02f, 0.01f, 0.01f, 0.65f);
        shadow.effectDistance = new Vector2(3f, -3f);

        var outline = titleGo.AddComponent<Outline>();
        outline.effectColor = new Color(0.15f, 0.08f, 0.04f, 0.35f);
        outline.effectDistance = new Vector2(1f, -1f);

        var subGo = new GameObject("Subtitle");
        subGo.transform.SetParent(block.transform, false);
        var subRect = subGo.AddComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 0.22f);
        subRect.anchorMax = new Vector2(0.5f, 0.22f);
        subRect.pivot = new Vector2(0.5f, 0.5f);
        subRect.anchoredPosition = Vector2.zero;
        subRect.sizeDelta = new Vector2(900f, 44f);
        var subText = subGo.AddComponent<Text>();
        subText.font = DefaultUiFont();
        subText.fontSize = 22;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.color = new Color(0.82f, 0.74f, 0.6f, 1f);
        subText.text = "Ghi lại hành trình — mở những trang sử đã qua";
        subText.raycastTarget = false;

        AddOrnamentalDivider(block.transform, GoldAntique);
    }

    void CreateFooter(Transform parent)
    {
        var footer = new GameObject("Footer");
        footer.transform.SetParent(parent, false);
        var rt = footer.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.04f);
        rt.anchorMax = new Vector2(0.5f, 0.04f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(800f, 36f);
        var t = footer.AddComponent<Text>();
        t.font = DefaultUiFont();
        t.fontSize = 15;
        t.alignment = TextAnchor.MiddleCenter;
        t.supportRichText = true;
        t.color = new Color(0.55f, 0.48f, 0.38f, 0.9f);
        t.text = "<i>Phục vụ học tập & tìm hiểu lịch sử Việt Nam</i>  ·  Menu chính";
        t.raycastTarget = false;
    }

    void CreateMenuButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick, bool primary)
    {
        var go = new GameObject("Btn_" + label);
        go.transform.SetParent(parent, false);

        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = primary ? 64f : 56f;
        le.minHeight = primary ? 58f : 50f;

        var img = go.AddComponent<Image>();
        PrepareUiImage(img);
        img.color = Color.white;
        img.raycastTarget = true;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.transition = Selectable.Transition.ColorTint;
        var colors = btn.colors;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.12f;
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);

        if (primary)
        {
            colors.normalColor = primaryButton;
            colors.highlightedColor = primaryButtonHover;
            colors.pressedColor = new Color(0.32f, 0.08f, 0.08f, 1f);
        }
        else
        {
            colors.normalColor = secondaryButton;
            colors.highlightedColor = secondaryButtonHover;
            colors.pressedColor = new Color(0.85f, 0.78f, 0.68f, 0.22f);
        }

        btn.colors = colors;
        btn.onClick.AddListener(onClick);
        btn.navigation = new Navigation { mode = Navigation.Mode.None };

        if (primary)
        {
            var accentBar = new GameObject("LeftAccent");
            accentBar.transform.SetParent(go.transform, false);
            accentBar.transform.SetAsFirstSibling();
            var barRt = accentBar.AddComponent<RectTransform>();
            barRt.anchorMin = new Vector2(0f, 0.15f);
            barRt.anchorMax = new Vector2(0f, 0.85f);
            barRt.pivot = new Vector2(0f, 0.5f);
            barRt.anchoredPosition = new Vector2(12f, 0f);
            barRt.sizeDelta = new Vector2(4f, 0f);
            var abImg = accentBar.AddComponent<Image>();
            PrepareUiImage(abImg);
            abImg.color = new Color(0.92f, 0.78f, 0.45f, 0.55f);
            abImg.raycastTarget = false;
        }
        else
        {
            var btnOutline = go.AddComponent<Outline>();
            btnOutline.effectColor = new Color(0.62f, 0.5f, 0.35f, 0.5f);
            btnOutline.effectDistance = new Vector2(1f, 1f);
        }

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var tr = textGo.AddComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = new Vector2(primary ? 28f : 16f, 0f);
        tr.offsetMax = Vector2.zero;
        var text = textGo.AddComponent<Text>();
        text.font = DefaultUiFont();
        text.fontSize = primary ? 30 : 26;
        text.fontStyle = primary ? FontStyle.Bold : FontStyle.Normal;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = primary ? new Color(0.98f, 0.94f, 0.9f, 1f) : new Color(0.9f, 0.84f, 0.74f, 1f);
        text.text = label;
        text.raycastTarget = false;

        var hoverFx = go.AddComponent<MainMenuButtonHover>();
        hoverFx.hoverMultiplier = primary ? 1.04f : 1.022f;
    }

    void OnPlayClicked()
    {
        if (SceneManager.sceneCountInBuildSettings > 1)
            SceneManager.LoadScene(1);
        else
            Debug.LogWarning(
                "Chưa có scene chơi trong Build Settings. Thêm scene (sau MainMenu) để nút Chơi chuyển màn.");
    }

    void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

/// <summary>
/// Phóng to nhẹ nút khi hover (chỉ trong lúc chạy / khi có EventSystem).
/// </summary>
public class MainMenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public float hoverMultiplier = 1.04f;
    Vector3 _baseScale = Vector3.one;

    void OnEnable()
    {
        _baseScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Application.isPlaying)
            return;
        transform.localScale = _baseScale * hoverMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Application.isPlaying)
            return;
        transform.localScale = _baseScale;
    }
}
