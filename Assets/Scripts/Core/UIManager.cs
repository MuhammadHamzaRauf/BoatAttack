using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GangsterMafia.Core;

namespace GangsterMafia.Core
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("UI Settings")]
        [SerializeField] private float _defaultTransitionDuration = 0.3f;
        [SerializeField] private bool _useAnimations = true;
        
        [Header("UI Panels")]
        [SerializeField] private List<UIPanel> _uiPanels = new List<UIPanel>();
        
        private Dictionary<string, UIPanel> _panelDictionary = new Dictionary<string, UIPanel>();
        private UIPanel _currentActivePanel;
        private Stack<UIPanel> _panelHistory = new Stack<UIPanel>();
        
        // Events
        public System.Action<string> OnPanelOpened;
        public System.Action<string> OnPanelClosed;
        public System.Action<string> OnPanelTransitionStarted;
        public System.Action<string> OnPanelTransitionCompleted;
        
        protected override void OnSingletonAwake()
        {
            InitializePanels();
        }
        
        private void InitializePanels()
        {
            _panelDictionary.Clear();
            
            foreach (var panel in _uiPanels)
            {
                if (panel != null)
                {
                    _panelDictionary[panel.PanelName] = panel;
                    panel.Initialize(this);
                    
                    // Hide all panels initially
                    if (panel.gameObject.activeInHierarchy)
                    {
                        panel.gameObject.SetActive(false);
                    }
                }
            }
        }
        
        // Show panel by name
        public void ShowPanel(string panelName, bool addToHistory = true)
        {
            if (!_panelDictionary.ContainsKey(panelName))
            {
                Debug.LogWarning($"Panel '{panelName}' not found in UIManager");
                return;
            }
            
            UIPanel targetPanel = _panelDictionary[panelName];
            
            if (_currentActivePanel != null && _currentActivePanel != targetPanel)
            {
                if (addToHistory)
                {
                    _panelHistory.Push(_currentActivePanel);
                }
                HidePanel(_currentActivePanel.PanelName, false);
            }
            
            _currentActivePanel = targetPanel;
            targetPanel.Show();
            
            OnPanelOpened?.Invoke(panelName);
            EventManager.Instance.TriggerEvent(Events.UI_PANEL_OPENED, panelName);
        }
        
        // Hide panel by name
        public void HidePanel(string panelName, bool removeFromHistory = true)
        {
            if (!_panelDictionary.ContainsKey(panelName))
            {
                Debug.LogWarning($"Panel '{panelName}' not found in UIManager");
                return;
            }
            
            UIPanel targetPanel = _panelDictionary[panelName];
            targetPanel.Hide();
            
            if (removeFromHistory && _panelHistory.Count > 0)
            {
                _currentActivePanel = _panelHistory.Pop();
                if (_currentActivePanel != null)
                {
                    _currentActivePanel.Show();
                }
            }
            
            OnPanelClosed?.Invoke(panelName);
            EventManager.Instance.TriggerEvent(Events.UI_PANEL_CLOSED, panelName);
        }
        
        // Show panel with transition
        public void ShowPanelWithTransition(string panelName, float duration = -1)
        {
            if (duration < 0) duration = _defaultTransitionDuration;
            
            StartCoroutine(ShowPanelCoroutine(panelName, duration));
        }
        
        private IEnumerator ShowPanelCoroutine(string panelName, float duration)
        {
            OnPanelTransitionStarted?.Invoke(panelName);
            
            ShowPanel(panelName);
            
            if (_useAnimations && duration > 0)
            {
                yield return new WaitForSeconds(duration);
            }
            
            OnPanelTransitionCompleted?.Invoke(panelName);
        }
        
        // Hide panel with transition
        public void HidePanelWithTransition(string panelName, float duration = -1)
        {
            if (duration < 0) duration = _defaultTransitionDuration;
            
            StartCoroutine(HidePanelCoroutine(panelName, duration));
        }
        
        private IEnumerator HidePanelCoroutine(string panelName, float duration)
        {
            OnPanelTransitionStarted?.Invoke(panelName);
            
            if (_useAnimations && duration > 0)
            {
                yield return new WaitForSeconds(duration);
            }
            
            HidePanel(panelName);
            
            OnPanelTransitionCompleted?.Invoke(panelName);
        }
        
        // Go back to previous panel
        public void GoBack()
        {
            if (_panelHistory.Count > 0)
            {
                string currentPanelName = _currentActivePanel != null ? _currentActivePanel.PanelName : "";
                HidePanel(currentPanelName, true);
            }
        }
        
        // Check if panel is visible
        public bool IsPanelVisible(string panelName)
        {
            if (!_panelDictionary.ContainsKey(panelName))
                return false;
                
            return _panelDictionary[panelName].IsVisible;
        }
        
        // Get current active panel
        public UIPanel GetCurrentActivePanel()
        {
            return _currentActivePanel;
        }
        
        // Get panel by name
        public UIPanel GetPanel(string panelName)
        {
            if (_panelDictionary.ContainsKey(panelName))
                return _panelDictionary[panelName];
                
            return null;
        }
        
        // Hide all panels
        public void HideAllPanels()
        {
            foreach (var panel in _uiPanels)
            {
                if (panel != null && panel.gameObject.activeInHierarchy)
                {
                    panel.Hide();
                }
            }
            
            _currentActivePanel = null;
            _panelHistory.Clear();
        }
        
        // Show loading screen
        public void ShowLoadingScreen()
        {
            ShowPanel("LoadingPanel");
        }
        
        // Hide loading screen
        public void HideLoadingScreen()
        {
            HidePanel("LoadingPanel");
        }
        
        // Set panel visibility
        public void SetPanelVisibility(string panelName, bool visible)
        {
            if (visible)
            {
                ShowPanel(panelName);
            }
            else
            {
                HidePanel(panelName);
            }
        }
        
        // Toggle panel visibility
        public void TogglePanel(string panelName)
        {
            if (IsPanelVisible(panelName))
            {
                HidePanel(panelName);
            }
            else
            {
                ShowPanel(panelName);
            }
        }
        
        // Clear panel history
        public void ClearPanelHistory()
        {
            _panelHistory.Clear();
        }
        
        // Get panel count
        public int GetPanelCount()
        {
            return _uiPanels.Count;
        }
        
        // Get all panel names
        public string[] GetAllPanelNames()
        {
            string[] names = new string[_uiPanels.Count];
            for (int i = 0; i < _uiPanels.Count; i++)
            {
                names[i] = _uiPanels[i] != null ? _uiPanels[i].PanelName : "";
            }
            return names;
        }
    }
    
    // Base class for UI panels
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("Panel Settings")]
        [SerializeField] protected string _panelName = "DefaultPanel";
        [SerializeField] protected bool _startVisible = false;
        [SerializeField] protected bool _useAnimations = true;
        [SerializeField] protected float _animationDuration = 0.3f;
        
        protected UIManager _uiManager;
        protected CanvasGroup _canvasGroup;
        protected RectTransform _rectTransform;
        protected bool _isVisible = false;
        
        // Properties
        public string PanelName => _panelName;
        public bool IsVisible => _isVisible;
        public bool UseAnimations => _useAnimations;
        public float AnimationDuration => _animationDuration;
        
        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        protected virtual void Start()
        {
            if (_startVisible)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
        
        // Initialize panel with UIManager reference
        public virtual void Initialize(UIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        // Show panel
        public virtual void Show()
        {
            if (_isVisible) return;
            
            gameObject.SetActive(true);
            _isVisible = true;
            
            if (_useAnimations)
            {
                StartCoroutine(ShowAnimation());
            }
            else
            {
                SetVisibleState(true);
            }
            
            OnPanelShown();
        }
        
        // Hide panel
        public virtual void Hide()
        {
            if (!_isVisible) return;
            
            if (_useAnimations)
            {
                StartCoroutine(HideAnimation());
            }
            else
            {
                SetVisibleState(false);
            }
            
            OnPanelHidden();
        }
        
        // Show animation
        protected virtual IEnumerator ShowAnimation()
        {
            SetVisibleState(false);
            
            float elapsedTime = 0f;
            while (elapsedTime < _animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / _animationDuration;
                
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = progress;
                }
                
                if (_rectTransform != null)
                {
                    _rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                }
                
                yield return null;
            }
            
            SetVisibleState(true);
        }
        
        // Hide animation
        protected virtual IEnumerator HideAnimation()
        {
            float elapsedTime = 0f;
            while (elapsedTime < _animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / _animationDuration;
                
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = 1f - progress;
                }
                
                if (_rectTransform != null)
                {
                    _rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress);
                }
                
                yield return null;
            }
            
            SetVisibleState(false);
            gameObject.SetActive(false);
        }
        
        // Set visible state
        protected virtual void SetVisibleState(bool visible)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? 1f : 0f;
                _canvasGroup.interactable = visible;
                _canvasGroup.blocksRaycasts = visible;
            }
            
            if (_rectTransform != null)
            {
                _rectTransform.localScale = visible ? Vector3.one : Vector3.zero;
            }
        }
        
        // Override these methods in derived classes
        protected virtual void OnPanelShown() { }
        protected virtual void OnPanelHidden() { }
        
        // Public methods for external access
        public virtual void Refresh() { }
        public virtual void Reset() { }
    }
}
