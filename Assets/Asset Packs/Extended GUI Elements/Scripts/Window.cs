using System;
using UnityEngine.Serialization;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

namespace UnityEngine.UI
{
    /// <summary>
    /// Creates a UI Window which act similar to a system OS window. The window is as standard closed upon creating it and to show it you should call OpenWindow(bool).
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class Window : UIBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        bool _dragable;
        [SerializeField]
        bool _resizable;
        [SerializeField]
        bool _windowInteractable = true;

        [SerializeField]
        Image _targetGraphics;

        [SerializeField]
        float _borderSize = 5;
        [SerializeField]
        float _dragAreaHeight = 25;

        [SerializeField]
        Vector2 _minSize = new Vector2(50, 50);
        [SerializeField]
        Vector2 _maxSize = new Vector2(1000, 1000);

        [SerializeField]
        GameObject[] _windowComponents;

        [SerializeField]
        TransitionStates _transitionState;
        [SerializeField]
        WindowColorBlock _colors;
        [SerializeField]
        WindowSpriteState _sprites;

        [SerializeField]
        UnityEvent _onWindowOpened;
        [SerializeField]
        UnityEvent _onWindowClosed;
        [SerializeField]
        UnityEvent _onWindowFocused;
        [SerializeField]
        UnityEvent _onWindowUnfocused;
        [SerializeField]
        UnityEvent _onWindowDragged;
        [SerializeField]
        UnityEvent _onWindowResized;
        [SerializeField]
        UnityEvent _onWindowEnabled;
        [SerializeField]
        UnityEvent _onWindowDisabled;

        Vector2 _targetsize;

        bool _isDraging = false;
        bool _isResizing = false;
        Direction _resizeDirection;

        static Window _currentFocusedWindow;

        /// <summary>
        /// Gets the window which is currently in focus
        /// </summary>
        public static Window currentFocusedWindow
        {
            get
            {
                return _currentFocusedWindow;
            }
            protected set
            {
                if (_currentFocusedWindow != null)
                {
                    _currentFocusedWindow.UnFocus();
                }
                _currentFocusedWindow = value;
            }
        }

        /// <summary>
        /// Is this window currently focused?
        /// </summary>
        public bool isWindowFocused
        {
            get
            {
                return (_currentFocusedWindow == this);
            }
        }

        /// <summary>
        /// The minimum size the window can have,
        /// </summary>
        public Vector2 minSize
        {
            get
            {
                return _minSize;
            }

            set
            {
                _minSize = value;
            }
        }

        /// <summary>
        /// The maximum size the window can have.
        /// </summary>
        public Vector2 maxSize
        {
            get
            {
                return _maxSize;
            }

            set
            {
                _maxSize = value;
            }
        }

        /// <summary>
        /// Event for when the window closes/disabled.
        /// </summary>
        public UnityEvent onWindowClosed
        {
            get
            {
                return _onWindowClosed;
            }
            set
            {
                _onWindowClosed = value;
            }
        }

        /// <summary>
        /// Event for when the window opens/enabled.
        /// </summary>
        public UnityEvent onWindowOpened
        {
            get
            {
                return _onWindowOpened;
            }
            set
            {
                _onWindowOpened = value;
            }
        }

        /// <summary>
        /// Decides if the window can be dragged/moved on the header.
        /// </summary>
        public bool dragable
        {
            get
            {
                return _dragable;
            }

            set
            {
                _dragable = value;
            }
        }

        /// <summary>
        /// Decides if the window can be resized on the borders.
        /// </summary>
        public bool resizable
        {
            get
            {
                return _resizable;
            }

            set
            {
                _resizable = value;
            }
        }

        /// <summary>
        /// Event for when the window gets focus.
        /// </summary>
        public UnityEvent onWindowFocused
        {
            get
            {
                return _onWindowFocused;
            }

            set
            {
                _onWindowFocused = value;
            }
        }

        /// <summary>
        /// Event for when the window loses focus.
        /// </summary>
        public UnityEvent onWindowUnfocused
        {
            get
            {
                return _onWindowUnfocused;
            }

            set
            {
                _onWindowUnfocused = value;
            }
        }

        /// <summary>
        /// Event for when the window is Moved.
        /// </summary>
        public UnityEvent onWindowDragged
        {
            get
            {
                return _onWindowDragged;
            }

            set
            {
                _onWindowDragged = value;
            }
        }

        /// <summary>
        /// Event for when the window is resized.
        /// </summary>
        public UnityEvent onWindowResized
        {
            get
            {
                return _onWindowResized;
            }

            set
            {
                _onWindowResized = value;
            }
        }

        /// <summary>
        /// Event for when the window is marked as enabled.
        /// </summary>
        public UnityEvent onWindowEnabled
        {
            get
            {
                return _onWindowEnabled;
            }

            set
            {
                _onWindowEnabled = value;
            }
        }

        /// <summary>
        /// Event for when the window is marked as disabled.
        /// </summary>
        public UnityEvent onWindowDisabled
        {
            get
            {
                return _onWindowDisabled;
            }

            set
            {
                _onWindowDisabled = value;
            }
        }

        /// <summary>
        /// The size of the borders if this window.
        /// </summary>
        public float borderSize
        {
            get
            {
                return _borderSize;
            }

            set
            {
                _borderSize = value;
            }
        }

        /// <summary>
        /// Can the window and components of the window be interacted with, moving and resizing is still possible when this is false.
        /// </summary>
        public bool windowInteractable
        {
            get
            {
                return _windowInteractable;
            }
            set
            {
                if (currentFocusedWindow == this)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
                Selectable[] selectables = this.transform.GetComponentsInChildren<Selectable>(true);
                foreach (Selectable sel in selectables)
                {
                    if (sel.gameObject == _windowComponents.Any())
                        continue;

                    sel.interactable = value;
                }
                if (value && value != _windowInteractable)
                {
                    onWindowEnabled.Invoke();
                }
                else if (!value && value != _windowInteractable)
                {
                    onWindowDisabled.Invoke();
                }
                _windowInteractable = value;
                StateTransition();
            }
        }

        /// <summary>
        /// the color posibilities of the window depending of the state of the window
        /// </summary>
        public WindowColorBlock colors
        {
            get
            {
                return _colors;
            }

            set
            {
                _colors = value;
            }
        }

        /// <summary>
        /// the sprite posibilities of the window, depending of the state of the window
        /// </summary>
        public WindowSpriteState sprites
        {
            get
            {
                return _sprites;
            }

            set
            {
                _sprites = value;
            }
        }

        /// <summary>
        /// the graphics of the window.
        /// </summary>
        public Image targetGraphics
        {
            get
            {
                return _targetGraphics;
            }

            set
            {
                _targetGraphics = value;
            }
        }

        /// <summary>
        /// how the game should trasit from one state to another
        /// </summary>
        public TransitionStates transitionState
        {
            get
            {
                return _transitionState;
            }

            set
            {
                _transitionState = value;
            }
        }

        /// <summary>
        /// Is this window in focus?
        /// </summary>
        public bool isFocused
        {
            get
            {
                return (this == currentFocusedWindow);
            }
        }

        /// <summary>
        /// Upon resizing, the size we want to reach
        /// </summary>
        protected Vector2 Targetsize
        {
            get
            {
                return _targetsize;
            }
            set
            {
                _targetsize = value;
            }
        }

        /// <summary>
        /// Is the window being dragged
        /// </summary>
        protected bool isDraging
        {
            get
            {
                return _isDraging;
            }

            set
            {
                _isDraging = value;
            }
        }

        /// <summary>
        /// Is the window being resized
        /// </summary>
        protected bool isResizing
        {
            get
            {
                return _isResizing;
            }
            set
            {
                _isResizing = value;
            }
        }

        /// <summary>
        /// The direction the window is being resized.
        /// </summary>
        public Direction ResizeDirection
        {
            get
            {
                return _resizeDirection;
            }
            set
            {
                _resizeDirection = value;
            }
        }


        /// <summary>
        /// Setups
        /// </summary>
        protected override void Awake()
        {
            if (targetGraphics == null)
            {
                targetGraphics = this.GetComponent<Image>();
            }
            windowInteractable = _windowInteractable;
            StateTransition(true);
        }

        /// <summary>
        /// Handles clicks outside the window.
        /// </summary>
        protected virtual void Update()
        {
            if (EventSystem.current.currentSelectedGameObject != null && currentFocusedWindow != this && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(this.transform))
            {
                SetFocus();
            }
            if (isWindowFocused)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(3) || Input.GetMouseButtonDown(4) || Input.GetMouseButtonDown(5) || Input.GetMouseButtonDown(6))
                {
                    if (this.GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        if (!RectTransformUtility.RectangleContainsScreenPoint(this.GetComponent<RectTransform>(), Input.mousePosition))
                        {
                            UnFocus();
                        }
                    }
                    else
                    {
                        if (!RectTransformUtility.RectangleContainsScreenPoint(this.GetComponent<RectTransform>(), Input.mousePosition, this.GetComponentInParent<Canvas>().worldCamera))
                        {
                            UnFocus();
                        }
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            /*if (!_drawGizmos)
                return;

            Vector3 pos = new Vector3();
            Vector3 size = new Vector3();
            Rect _headerArea = new Rect(0, 0, this.GetComponent<RectTransform>().sizeDelta.x, _dragAreaHeight);


            if (GetComponentInParent<Canvas>() != null && GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace && GetComponentInParent<Canvas>().worldCamera != null)
            {
                size = this.GetComponent<RectTransform>().sizeDelta;
                pos = GetComponentInParent<Canvas>().worldCamera.ScreenToWorldPoint(
                    new Vector3(this.transform.position.x - (size.x * this.GetComponent<RectTransform>().pivot.x), this.transform.position.y - (size.y * this.GetComponent<RectTransform>().pivot.y)));
                size = new Vector2(size.x / GetWorldScale(transform).x, size.y / GetWorldScale(transform).y);

                Gizmos.DrawCube(pos + new Vector3(_headerArea.x + (_headerArea.width / 2.0f), _headerArea.y + (_headerArea.height / 2.0f)), new Vector3(_headerArea.width, _headerArea.height));
            }
            else
            {
                size = this.GetComponent<RectTransform>().sizeDelta;
                pos = new Vector3(this.transform.position.x - (size.x * this.GetComponent<RectTransform>().pivot.x), this.transform.position.y - (size.y * this.GetComponent<RectTransform>().pivot.y));
                size = new Vector2(size.x / GetWorldScale(transform).x, size.y / GetWorldScale(transform).y);

                //Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                Gizmos.color = new Color(1f, 1f, 1f, 0.5f);

                Gizmos.DrawCube(pos + new Vector3(_headerArea.x + (_headerArea.width / 2.0f), _headerArea.y + (_headerArea.height / 2.0f)), new Vector3(_headerArea.width, _headerArea.height));

                //Header
                Gizmos.DrawLine(pos + new Vector3(_headerArea.x, _headerArea.y), pos + new Vector3(_headerArea.x, _headerArea.y + _headerArea.height));
                Gizmos.DrawLine(pos + new Vector3(_headerArea.x + _headerArea.width, _headerArea.y), pos + new Vector3(_headerArea.x + _headerArea.width, _headerArea.y + _headerArea.height));
                Gizmos.DrawLine(pos + new Vector3(_headerArea.x, _headerArea.y), pos + new Vector3(_headerArea.x + _headerArea.width, _headerArea.y));
                Gizmos.DrawLine(pos + new Vector3(_headerArea.x, _headerArea.y + _headerArea.height), pos + new Vector3(_headerArea.x + _headerArea.width, _headerArea.y + _headerArea.height));

                Gizmos.color = Color.green;

                //Top border
                Gizmos.DrawLine(pos + new Vector3(0, size.y), pos + new Vector3(size.x, size.y));
                Gizmos.DrawLine(pos + new Vector3(0, size.y - borderSize), pos + new Vector3(size.x, size.y - borderSize));
                Gizmos.DrawLine(pos + new Vector3(0, size.y - borderSize), pos + new Vector3(0, size.y));
                Gizmos.DrawLine(pos + new Vector3(size.x, size.y - borderSize), pos + new Vector3(size.x, size.y));

                //bottom border
                Gizmos.DrawLine(pos + new Vector3(0, 0), pos + new Vector3(size.x, 0));
                Gizmos.DrawLine(pos + new Vector3(0, borderSize), pos + new Vector3(size.x, borderSize));
                Gizmos.DrawLine(pos + new Vector3(0, borderSize), pos + new Vector3(0, 0));
                Gizmos.DrawLine(pos + new Vector3(size.x, borderSize), pos + new Vector3(size.x, 0));

                //left border
                Gizmos.DrawLine(pos + new Vector3(0, 0), pos + new Vector3(0, size.y));
                Gizmos.DrawLine(pos + new Vector3(borderSize, 0), pos + new Vector3(borderSize, size.y));
                Gizmos.DrawLine(pos + new Vector3(borderSize, 0), pos + new Vector3(0, 0));
                Gizmos.DrawLine(pos + new Vector3(borderSize, size.y), pos + new Vector3(0, size.y));

                //rigth border
                Gizmos.DrawLine(pos + new Vector3(size.x, 0), pos + new Vector3(size.x, size.y));
                Gizmos.DrawLine(pos + new Vector3(size.x - borderSize, 0), pos + new Vector3(size.x - borderSize, size.y));
                Gizmos.DrawLine(pos + new Vector3(size.x - borderSize, 0), pos + new Vector3(size.x, 0));
                Gizmos.DrawLine(pos + new Vector3(size.x - borderSize, size.y), pos + new Vector3(size.x, size.y));
            }*/
        }

        /// <summary>
        /// Invokes event when enabled/opened
        /// </summary>
        protected override void OnEnable()
        {
            _onWindowOpened.Invoke();
        }

        /// <summary>
        /// Invokes event when disabled/closes
        /// </summary>
        protected override void OnDisable()
        {
            _onWindowClosed.Invoke();
        }

        /// <summary>
        /// Opens the window by setting it to active.
        /// </summary>
        public virtual void OpenWindow()
        {
            OpenWindow(true);
        }

        /// <summary>
        /// Opens the window by setting it to active and set it to either being in focus or unfocused.
        /// </summary>
        /// <param name="focusOnOpen">Decide if the window should be in focus when opened.</param>
        public virtual void OpenWindow(bool focusOnOpen)
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
                if (focusOnOpen)
                {
                    currentFocusedWindow = this;
                    this.GetComponent<RectTransform>().SetAsLastSibling();
                }
                StateTransition(true);
            }
        }

        /// <summary>
        /// Closes the window and removes its focus
        /// </summary>
        public virtual void CloseWindow()
        {
            if (this.gameObject.activeSelf)
            {
                if (_currentFocusedWindow == this)
                {
                    _currentFocusedWindow = null;
                    UnFocus();
                }
                this.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Moves the window to a target position depending on anchors and pivot. The method does NOT limit the position to window screen space only!
        /// </summary>
        /// <param name="pos">The target position of the window</param>
        public virtual void MoveWindow(Vector2 pos)
        {
            MoveWindow(new Vector3(pos.x, pos.y, 0));
        }

        /// <summary>
        /// Moves the window to a target position depending on anchors and pivot. The method does NOT limit the position to window screen space only!
        /// </summary>
        /// <param name="pos">The target position of the window</param>
        public virtual void MoveWindow(Vector3 pos)
        {
            if (this.gameObject.activeSelf)
            {
                this.transform.position = pos;
                _onWindowDragged.Invoke();
            }
        }

        /// <summary>
        /// Resizes the window/recttransform to a target size in a certain direction. The method takes into account the min, and max size.
        /// </summary>
        /// <param name="size">The target size of the window</param>
        /// <param name="sizedirection">The direction(s) the window should be scaled. None scales it depending on the anchors and pivot. (use logical OR (|) on the direction enum to parse multiple directions).</param>
        public virtual void ResizeWindow(Vector2 size, Direction sizedirection)
        {
            if (this.gameObject.activeSelf)
            {
                Vector2 sizeDelta = new Vector2(Mathf.Clamp(size.x, _minSize.x, _maxSize.x), Mathf.Clamp(size.y, _minSize.y, _maxSize.y)) - this.GetComponent<RectTransform>().rect.size;
                Vector3 worldScale = GetWorldScale(this.transform);

                if (GetComponentInParent<Canvas>() != null && GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace && GetComponentInParent<Canvas>().worldCamera != null)
                {
                    if (sizedirection == Direction.None)
                    {
                        this.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta + sizeDelta;
                        return;
                    }
                    if ((sizedirection & Direction.Left) == Direction.Left)
                    {
                        this.transform.position = this.transform.position + new Vector3(-sizeDelta.x * (1 - this.GetComponent<RectTransform>().pivot.x) / worldScale.x, 0);
                        this.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta + new Vector2(sizeDelta.x, 0);
                    }
                    if ((sizedirection & Direction.Right) == Direction.Right)
                    {
                        this.transform.position = this.transform.position + new Vector3(sizeDelta.x * (this.GetComponent<RectTransform>().pivot.x) / worldScale.x, 0);
                        this.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta + new Vector2(sizeDelta.x, 0);
                    }
                    if ((sizedirection & Direction.Up) == Direction.Up)
                    {
                        this.transform.position = this.transform.position + new Vector3(0, sizeDelta.y * (this.GetComponent<RectTransform>().pivot.y) / worldScale.y, 0);
                        this.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta + new Vector2(0, sizeDelta.y);
                    }
                    if ((sizedirection & Direction.Down) == Direction.Down)
                    {
                        this.transform.position = this.transform.position + new Vector3(0, -sizeDelta.y * (1 - this.GetComponent<RectTransform>().pivot.y) / worldScale.y, 0);
                        this.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta + new Vector2(0, sizeDelta.y);
                    }
                }
                else
                {
                    if (sizedirection == Direction.None)
                    {
                        this.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta + sizeDelta;
                        return;
                    }

                    if ((sizedirection & Direction.Left) == Direction.Left)
                    {
                        this.transform.position = this.transform.position + new Vector3(-sizeDelta.x * (1 - this.GetComponent<RectTransform>().pivot.x) / worldScale.x, 0);
                        this.GetComponent<RectTransform>().sizeDelta = (this.GetComponent<RectTransform>().sizeDelta + new Vector2(sizeDelta.x, 0));
                    }
                    if ((sizedirection & Direction.Right) == Direction.Right)
                    {
                        this.transform.position = this.transform.position + new Vector3(sizeDelta.x * (this.GetComponent<RectTransform>().pivot.x) / worldScale.x, 0);
                        this.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta + new Vector2(sizeDelta.x, 0);
                    }
                    if ((sizedirection & Direction.Up) == Direction.Up)
                    {
                        this.transform.position = this.transform.position + new Vector3(0, sizeDelta.y * (this.GetComponent<RectTransform>().pivot.y) / worldScale.y, 0);
                        this.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta + new Vector2(0, sizeDelta.y);
                    }
                    if ((sizedirection & Direction.Down) == Direction.Down)
                    {
                        this.transform.position = this.transform.position + new Vector3(0, -sizeDelta.y * (1 - this.GetComponent<RectTransform>().pivot.y) / worldScale.y, 0);
                        this.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta + new Vector2(0, sizeDelta.y);
                    }
                }
                _onWindowResized.Invoke();
            }
        }

        /// <summary>
        /// Sets this window as the topmost component of the parent, forcing it on top of any other component of the parent.
        /// </summary>
        public virtual void SetFocus()
        {
            if (this.gameObject.activeSelf)
            {
                if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.transform.IsChildOf(this.transform))
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }

                if (_currentFocusedWindow != this)
                {
                    currentFocusedWindow = this;

                    StateTransition();

                    _onWindowFocused.Invoke();
                }
                this.GetComponent<RectTransform>().SetAsLastSibling();
            }
        }

        /// <summary>
        /// Removes the focus from the current window.
        /// </summary>
        public virtual void UnFocus()
        {
            if (this.gameObject.activeSelf)
            {
                if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(this.transform))
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }

                if (_currentFocusedWindow == this)
                {
                    //Change only the field as otherwise this unfocus would causes stackoverflow, futhermore there is no need to call this method twice when called from script,
                    //just remove focus and let the overtaking focused window do its job.
                    _currentFocusedWindow = null;

                    StateTransition();

                    _onWindowUnfocused.Invoke();
                }
            }
        }

        /// <summary>
        /// Makes sure the window gets focused when clicked.
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (_currentFocusedWindow != this)
            {
                SetFocus();
            }

            Vector2 localCursor;

            RectTransform rect = GetComponent<RectTransform>();

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;

            localCursor = new Vector2(localCursor.x + (rect.pivot.x * rect.sizeDelta.x), localCursor.y + (rect.pivot.y * rect.sizeDelta.y));
            Targetsize = rect.rect.size;

            if (resizable)
            {
                if (localCursor.x <= borderSize)
                {
                    ResizeDirection |= Direction.Left;
                    isResizing = true;
                }
                if (localCursor.y <= borderSize)
                {
                    ResizeDirection |= Direction.Down;
                    isResizing = true;
                }
                if (localCursor.y >= rect.sizeDelta.y - borderSize)
                {
                    ResizeDirection |= Direction.Up;
                    isResizing = true;
                }
                if (localCursor.x >= rect.sizeDelta.x - borderSize)
                {
                    ResizeDirection |= Direction.Right;
                    isResizing = true;
                }
            }

            if (!isResizing && dragable && localCursor.y >= rect.sizeDelta.y - _dragAreaHeight)
            {
                isDraging = true;
            }
        }

        /// <summary>
        /// Handles dragging - that is resizing and moving the window.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 _mousepos = eventData.position - eventData.delta;
            Vector3 worldScale = GetWorldScale(this.transform);
            if (dragable && isDraging)
            {
                if (GetComponentInParent<Canvas>() != null && GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace && GetComponentInParent<Canvas>().worldCamera != null)
                {
                    Vector3 wposb = new Vector3();
                    Vector3 wposa = new Vector3();
                    if (GetComponentInParent<Canvas>().worldCamera.orthographic)
                    {
                        wposb = GetComponentInParent<Canvas>().worldCamera.ScreenToWorldPoint(new Vector3(_mousepos.x, _mousepos.y, 0));
                        wposa = GetComponentInParent<Canvas>().worldCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 0));
                    }
                    else
                    {
                        wposb = GetComponentInParent<Canvas>().worldCamera.ScreenToWorldPoint(new Vector3(_mousepos.x, _mousepos.y,
                            (GetComponentInParent<Canvas>().transform.position.z - GetComponentInParent<Canvas>().worldCamera.transform.position.z)));
                        wposa = GetComponentInParent<Canvas>().worldCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y,
                            (GetComponentInParent<Canvas>().transform.position.z - GetComponentInParent<Canvas>().worldCamera.transform.position.z)));
                    }
                    MoveWindow(transform.position + new Vector3(wposa.x - wposb.x, wposa.y - wposb.y, 0));
                }
                else
                {
                    MoveWindow(transform.position + new Vector3(eventData.position.x - _mousepos.x, eventData.position.y - _mousepos.y, 0));
                }
                _mousepos = eventData.position;
            }
            else if (isResizing && resizable)
            {
                if (resizable)
                {
                    if (GetComponentInParent<Canvas>() != null && GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace && GetComponentInParent<Canvas>().worldCamera != null)
                    {
                        Vector3 wposb = new Vector3();
                        Vector3 wposa = new Vector3();

                        Vector2 currentPointerPosition = eventData.position;

                        if (GetComponentInParent<Canvas>().worldCamera.orthographic)
                        {
                            wposb = GetComponentInParent<Canvas>().worldCamera.ScreenToWorldPoint(_mousepos);
                            wposa = GetComponentInParent<Canvas>().worldCamera.ScreenToWorldPoint(currentPointerPosition);
                        }
                        else
                        {
                            wposb = GetComponentInParent<Canvas>().worldCamera.ScreenToWorldPoint(new Vector3(_mousepos.x, _mousepos.y, (GetComponentInParent<Canvas>().transform.position.z - GetComponentInParent<Canvas>().worldCamera.transform.position.z)));
                            wposa = GetComponentInParent<Canvas>().worldCamera.ScreenToWorldPoint(new Vector3(currentPointerPosition.x, currentPointerPosition.y, (GetComponentInParent<Canvas>().transform.position.z - GetComponentInParent<Canvas>().worldCamera.transform.position.z)));
                        }

                        wposb = new Vector3(wposb.x * worldScale.x,
                            wposb.y * worldScale.y, wposb.z);
                        wposa = new Vector3(wposa.x * worldScale.x,
                            wposa.y * worldScale.y, wposa.z);

                        Vector2 mouseDelta = (wposa - wposb);

                        if ((ResizeDirection & Direction.Left) == Direction.Left)
                        {
                            mouseDelta = new Vector2(-mouseDelta.x, mouseDelta.y);
                        }
                        if ((ResizeDirection & Direction.Down) == Direction.Down)
                        {
                            mouseDelta = new Vector2(mouseDelta.x, -mouseDelta.y);
                        }
                        ResizeWindow(Targetsize + mouseDelta, ResizeDirection);
                        Targetsize += mouseDelta;
                        _mousepos = currentPointerPosition;
                    }
                    else
                    {
                        Vector2 currentPointerPosition = eventData.position;
                        Vector2 mouseDelta = (currentPointerPosition - _mousepos);

                        if ((ResizeDirection & Direction.Left) == Direction.Left)
                        {
                            mouseDelta = new Vector2(mouseDelta.x * -1, mouseDelta.y);
                        }
                        if ((ResizeDirection & Direction.Down) == Direction.Down)
                        {
                            mouseDelta = new Vector2(mouseDelta.x, mouseDelta.y * -1);
                        }
                        mouseDelta = new Vector2(mouseDelta.x * worldScale.x, mouseDelta.y * worldScale.y);
                        ResizeWindow(Targetsize + mouseDelta, ResizeDirection);
                        Targetsize += mouseDelta;
                        _mousepos = currentPointerPosition;
                    }
                }
            }
        }

        /// <summary>
        /// Clean up after dragging
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            isDraging = false;
            isResizing = false;
            ResizeDirection = Direction.None;
        }

        /// <summary>
        /// Force the window to update arcording to its current state
        /// </summary>
        public virtual void StateTransition()
        {
            StateTransition(false, false);
        }

        /// <summary>
        /// Force the window to update arcording to its current state
        /// </summary>
        /// <param name="instant">Should the state change be instant</param>
        public virtual void StateTransition(bool instant)
        {
            StateTransition(instant, false);
        }

        /// <summary>
        /// Force the window to update arcording to its current state
        /// </summary>
        /// <param name="instant">Should the state change be instant</param>
        /// <param name="ignoreState">Should the change ignore the state and display the window as active and in focus</param>
        public virtual void StateTransition(bool instant, bool ignoreState)
        {
            if (!_windowInteractable && !ignoreState)
            {
                switch (_transitionState)
                {
                    case TransitionStates.Colors:
                        _targetGraphics.CrossFadeColor(_colors.disabledColor, instant ? 0f : _colors.fadeDuration, true, true);
                        break;
                    case TransitionStates.Sprites:
                        _targetGraphics.overrideSprite = _sprites.disabledSprite;
                        break;
                    case TransitionStates.Both:
                        _targetGraphics.CrossFadeColor(_colors.disabledColor, instant ? 0f : _colors.fadeDuration, true, true);
                        _targetGraphics.overrideSprite = _sprites.disabledSprite;
                        break;
                    default:
                        _targetGraphics.CrossFadeColor(_colors.disabledColor, instant ? 0f : _colors.fadeDuration, true, true);
                        break;
                }
            }
            else if (ignoreState || isFocused)
            {
                switch (_transitionState)
                {
                    case TransitionStates.Colors:
                        _targetGraphics.CrossFadeColor(_colors.focusedColor, instant ? 0f : _colors.fadeDuration, true, true);
                        break;
                    case TransitionStates.Sprites:
                        _targetGraphics.overrideSprite = _sprites.focusedSprite;
                        break;
                    case TransitionStates.Both:
                        _targetGraphics.CrossFadeColor(_colors.focusedColor, instant ? 0f : _colors.fadeDuration, true, true);
                        _targetGraphics.overrideSprite = _sprites.focusedSprite;
                        break;
                    default:
                        _targetGraphics.CrossFadeColor(_colors.focusedColor, instant ? 0f : _colors.fadeDuration, true, true);
                        break;
                }
            }
            else
            {
                switch (_transitionState)
                {
                    case TransitionStates.Colors:
                        _targetGraphics.CrossFadeColor(_colors.unFocusedColor, instant ? 0f : _colors.fadeDuration, true, true);
                        break;
                    case TransitionStates.Sprites:
                        _targetGraphics.overrideSprite = _sprites.unFocusedSprite;
                        break;
                    case TransitionStates.Both:
                        _targetGraphics.CrossFadeColor(_colors.unFocusedColor, instant ? 0f : _colors.fadeDuration, true, true);
                        _targetGraphics.overrideSprite = _sprites.unFocusedSprite;
                        break;
                    default:
                        _targetGraphics.CrossFadeColor(_colors.unFocusedColor, instant ? 0f : _colors.fadeDuration, true, true);
                        break;
                }
            }
        }

        private Vector3 GetWorldScale(Transform trans)
        {
            Vector3 retvec = new Vector3(1.0f / trans.localScale.x, 1.0f / trans.localScale.y, 1.0f / trans.localScale.z);

            if (trans.parent != null)
            {
                retvec.Scale(GetWorldScale(trans.parent));
                return retvec;
            }
            else
            {
                return retvec;
            }
        }
    }

    [Serializable]
    public struct WindowColorBlock
    {
        [FormerlySerializedAs("focusedColor")]
        [SerializeField]
        private Color m_FocusedColor;

        [FormerlySerializedAs("unFocusedColor")]
        [SerializeField]
        private Color m_UnFocusedColor;

        [FormerlySerializedAs("disabledColor")]
        [SerializeField]
        private Color m_DisabledColor;

        [Range(1, 5)]
        [SerializeField]
        private float m_ColorMultiplier;

        [FormerlySerializedAs("fadeDuration")]
        [SerializeField]
        private float m_FadeDuration;

        public Color focusedColor { get { return m_FocusedColor; } set { m_FocusedColor = value; } }
        public Color unFocusedColor { get { return m_UnFocusedColor; } set { m_UnFocusedColor = value; } }
        public Color disabledColor { get { return m_DisabledColor; } set { m_DisabledColor = value; } }
        public float colorMultiplier { get { return m_ColorMultiplier; } set { m_ColorMultiplier = value; } }
        public float fadeDuration { get { return m_FadeDuration; } set { m_FadeDuration = value; } }

        public static WindowColorBlock defaultColorBlock
        {
            get
            {
                var c = new WindowColorBlock
                {
                    m_FocusedColor = new Color32(255, 255, 255, 255),
                    m_UnFocusedColor = new Color32(200, 200, 200, 255),
                    m_DisabledColor = new Color32(200, 200, 200, 128),
                    colorMultiplier = 1.0f,
                    fadeDuration = 0.1f
                };
                return c;
            }
        }
    }

    [Serializable]
    public struct WindowSpriteState
    {
        [FormerlySerializedAs("focusedSprite")]
        [SerializeField]
        private Sprite m_FocusedSprite;

        [FormerlySerializedAs("unFocusedSprite")]
        [SerializeField]
        private Sprite m_UnFocusedSprite;

        [FormerlySerializedAs("disabledSprite")]
        [SerializeField]
        private Sprite m_DisabledSprite;

        public Sprite focusedSprite { get { return m_FocusedSprite; } set { m_FocusedSprite = value; } }
        public Sprite unFocusedSprite { get { return m_UnFocusedSprite; } set { m_UnFocusedSprite = value; } }
        public Sprite disabledSprite { get { return m_DisabledSprite; } set { m_DisabledSprite = value; } }
    }

    [Serializable]
    public enum TransitionStates
    {
        Colors,
        Sprites,
        Both,
    }

    [Flags]
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
    }
}