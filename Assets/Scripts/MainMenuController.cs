using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenuController : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private List<LevelInfo> levels = new List<LevelInfo>();
    
    private UIDocument _uiDocument;
    private VisualElement _root;
    private VisualElement _carouselContainer;
    private VisualElement _paginationContainer;
    private ScrollView _scrollView;
    private Button _selectLevelButton;
    private Button _playButton;
    private int _selectedLevelIndex = -1;

    private void Awake()
    {
        Debug.Log("MainMenuController Awake");
        
        _uiDocument = GetComponent<UIDocument>();
        _root = _uiDocument.rootVisualElement;
        
        // Query elements
        _selectLevelButton = _root.Q<Button>("SelectLevelButton");
        if (_selectLevelButton == null) Debug.LogError("SELECT LEVEL BUTTON NOT FOUND!");
        
        _playButton = _root.Q<Button>("PlayButton");
        _carouselContainer = _root.Q<VisualElement>("LevelCarousel");
        if (_carouselContainer == null) Debug.LogError("LEVEL CAROUSEL NOT FOUND!");
        
        _paginationContainer = _root.Q<VisualElement>("PaginationDots");
        _scrollView = _root.Q<ScrollView>("LevelScrollView");
        if (_scrollView == null) Debug.LogError("SCROLL VIEW NOT FOUND!");
        
        // Register events
        _selectLevelButton.clicked += ShowCarousel;
        Debug.Log("ShowCarousel event registered");
        
        _playButton.clicked += PlaySelectedLevel;
        
        // Initialize level cards
        CreateLevelCards();
    }

    private void ShowCarousel()
    {
        Debug.Log("ShowCarousel triggered");
        
        // Hide the select level button
        if (_selectLevelButton != null)
        {
            Debug.Log("Hiding SelectLevelButton");
            _selectLevelButton.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("SelectLevelButton is null!");
        }
        
        // Find and show the carousel container
        VisualElement carouselParent = _carouselContainer?.parent?.parent as VisualElement;
        if (carouselParent != null)
        {
            Debug.Log("Showing LevelCarouselContainer");
            carouselParent.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.LogError("LevelCarouselContainer not found in hierarchy!");
        }
    }

    private void CreateLevelCards()
    {
        Debug.Log("Creating level cards...");
        
        if (_carouselContainer == null)
        {
            Debug.LogError("Cannot create cards: LevelCarousel is null!");
            return;
        }

        _carouselContainer.Clear();
        _paginationContainer.Clear();

        for (int i = 0; i < levels.Count; i++)
        {
            var card = CreateLevelCard(levels[i]);
            _carouselContainer.Add(card);
    
            if (i == levels.Count - 1)
                card.AddToClassList("no-margin"); // 移除最后一张卡片的右边距
        }

        CreatePaginationDots();
        TogglePlayButton();
        
        Debug.Log("Level cards created");
    }

    private VisualElement CreateLevelCard(LevelInfo level)
    {
        Debug.Log($"Building card for {level.Name}");
        
        var card = new VisualElement { name = "LevelCard" };
        card.AddToClassList("level-card");
        
        // Add image
        var img = new Image { name = "LevelImage" };
        Texture2D texture = Resources.Load<Texture2D>(level.ThumbnailPath);
        if (texture == null)
        {
            Debug.LogError($"Missing thumbnail: {level.ThumbnailPath}");
            img.image = Texture2D.whiteTexture; // Fallback
        }
        else
        {
            img.image = texture;
        }
        card.Add(img);
        
        // Add label
        var label = new Label(level.Name);
        label.name = "LevelLabel";
        label.AddToClassList("level-card-label");
        card.Add(label);
        
        // Add click handler
        card.RegisterCallback<PointerDownEvent>(e => 
        {
            Debug.Log($"Level selected: {level.Name}");
            SelectLevel(levels.IndexOf(level));
        });
        
        return card;
    }

    private void CreatePaginationDots()
    {
        Debug.Log("Creating pagination dots");
        
        for (int i = 0; i < levels.Count; i++)
        {
            var dot = new VisualElement { name = "PaginationDot" };
            dot.AddToClassList("pagination-dot");
            
            dot.RegisterCallback<ClickEvent>(e => 
            {
                Debug.Log($"Pagination dot {i} clicked");
                SelectLevel(i);
            });
            
            _paginationContainer.Add(dot);
        }
    }

    private void SelectLevel(int index)
    {
        if (index < 0 || index >= levels.Count) 
        {
            Debug.LogError("Invalid level index");
            return;
        }

        Debug.Log($"Selecting level: {levels[index].Name}");
        _selectedLevelIndex = index;
        UpdateCarouselPosition();
        UpdatePagination();
        TogglePlayButton();
    }

    private void UpdateCarouselPosition()
    {
        Debug.Log("Updating carousel position");
        
        const float cardWidth = 240f; // Card width + gap
        float offset = _selectedLevelIndex * cardWidth;

        if (_scrollView != null)
        {
            // Calculate viewport width
            float viewportWidth = _scrollView.layout.width - 40; // Subtract scrollbar
            float totalWidth = _carouselContainer.layout.width;
            
            // Calculate scroll position
            float scrollPosition = offset - (viewportWidth / 2 - cardWidth / 2);
            scrollPosition = Mathf.Clamp(scrollPosition, 0, totalWidth - viewportWidth);
            
            // Apply scroll position
            Debug.Log($"Setting scroll position to {-scrollPosition}px");
            _scrollView.contentContainer.style.left = new StyleLength(-scrollPosition);
        }
        else
        {
            Debug.LogError("ScrollView not initialized!");
        }
    }

    private void UpdatePagination()
    {
        Debug.Log("Updating pagination dots");
        
        var dots = _paginationContainer.Children().ToList();
        foreach (var dot in dots)
        {
            dot.RemoveFromClassList("active");
        }
        
        if (_selectedLevelIndex >= 0 && _selectedLevelIndex < dots.Count)
        {
            dots[_selectedLevelIndex].AddToClassList("active");
        }
        else
        {
            Debug.LogWarning("Selected level index out of range");
        }
    }

    private void TogglePlayButton()
    {
        Debug.Log("Toggling play button");
        _playButton.style.display = _selectedLevelIndex != -1 ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void PlaySelectedLevel()
    {
        if (_selectedLevelIndex == -1) 
        {
            Debug.LogError("No level selected!");
            return;
        }
        
        Debug.Log($"Loading level: {levels[_selectedLevelIndex].SceneName}");
        SceneManager.LoadScene(levels[_selectedLevelIndex].SceneName);
    }

    private void OnDisable()
    {
        Debug.Log("MainMenuController disabling");
        _selectLevelButton.clicked -= ShowCarousel;
        _playButton.clicked -= PlaySelectedLevel;
    }

    [System.Serializable]
    public struct LevelInfo
    {
        public string Name;
        public string ThumbnailPath;
        public string SceneName;
    }
}