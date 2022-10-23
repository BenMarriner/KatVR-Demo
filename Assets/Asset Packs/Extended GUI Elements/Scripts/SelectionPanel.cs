using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class StringChangeEvent : UnityEvent<string, int> { }
    [RequireComponent(typeof(Image))]
    //[RequireComponent(typeof(Selectable))]
    public class SelectionPanel : Selectable, ISelectHandler, IDeselectHandler, IPointerEnterHandler
    {
        [SerializeField]
        Text optionText;
        [SerializeField]
        Text selectionText;
        [SerializeField]
        HoldButton leftButton;
        [SerializeField]
        HoldButton rightButton;

        [SerializeField]
        string[] _selectionOptions;
        [SerializeField]
        bool _useNumbersOnly = false;
        [SerializeField]
        int _minValue = 1;
        [SerializeField]
        int _maxValue = 10;

        [SerializeField]
        bool _allowOverflow = true;

        [SerializeField]
        StringChangeEvent _onTextChanged;

        [SerializeField]
        float _changeTimerMin = 0.05f;
        [SerializeField]
        float _changeTimerMax = 0.25f;

        float _currentChangeTime;
        float _changeTimer = 0;

        bool _isSelected = false;

        [SerializeField]
        int _index = 0;

        /// <summary>
        /// Should the items only be numbers?
        /// </summary>
        public bool useNumbersOnly
        {
            get
            {
                return _useNumbersOnly;
            }
            set
            {
                _useNumbersOnly = value;
                Index = _index;
            }
        }

        /// <summary>
        /// The minimum value if useNumbersonly is true.
        /// </summary>
        public int minValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                if (minValue >= value)
                {
                    _minValue = _maxValue - 1;
                }
                else
                {
                    _minValue = value;
                }
                Index = _index;
            }
        }

        /// <summary>
        /// The maximum value if useNumbersonly is true.
        /// </summary>
        public int maxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                if (minValue >= value)
                {
                    _maxValue = _minValue + 1;
                }
                else
                {
                    _maxValue = value;
                }
                Index = _index;
            }
        }

        /// <summary>
        /// Checks if the index goes over the highest or lowest possible value, if true thevalue will be 0 if it's larger than the highest index, and higest value if it goes under 0.
        /// </summary>
        public bool allowOverflow
        {
            get
            {
                return _allowOverflow;
            }
            set
            {
                _allowOverflow = value;
            }
        }

        bool isSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _changeTimer = _changeTimerMax;
                _currentChangeTime = _changeTimerMax;
                _isSelected = value;
            }
        }

        /// <summary>
        /// The minimum time (in seconds) for how often the index can be changed
        /// </summary>
        public float changeTimerMin
        {
            get
            {
                return _changeTimerMin;
            }
            set
            {
                _changeTimerMin = value;
            }
        }

        /// <summary>
        /// The maximum time (in seconds) for how often the index can be changed
        /// </summary>
        public float changeTimerMax
        {
            get
            {
                return _changeTimerMax;
            }
            set
            {
                _changeTimerMax = value;
            }
        }

        /// <summary>
        /// The event which is called when the index is changed.
        /// </summary>
        public StringChangeEvent onTextChanged
        {
            get
            {
                return _onTextChanged;
            }
            set
            {
                _onTextChanged = value;
            }
        }

        /// <summary>
        /// The selection option displayed if usenumbers is false.
        /// </summary>
        public string[] selectionOptions
        {
            get
            {
                return _selectionOptions;
            }
            set
            {
                _selectionOptions = value;
                Index = _index;
            }
        }

        /// <summary>
        /// Gets or sets the Index of the of the component.
        /// </summary>
        public int Index
        {
            get
            {
                if (_selectionOptions.Length <= 0 && !_useNumbersOnly)
                {
                    return -1;
                }
                return _index;
            }
            set
            {
                if (_selectionOptions.Length <= 0 && !_useNumbersOnly)
                {
                    if (!allowOverflow)
                    {
                        rightButton.interactable = false;
                        leftButton.interactable = false;
                    }
                    else
                    {
                        rightButton.interactable = true;
                        leftButton.interactable = true;
                    }
                    return;
                }
                if (_useNumbersOnly)
                {
                    if (value + _minValue > _maxValue)
                    {
                        if (_allowOverflow)
                        {
                            _index = 0;
                        }
                        else
                        {
                            leftButton.interactable = true;
                            rightButton.interactable = false;
                            _index = (_maxValue - _minValue) - _minValue;
                        }
                    }
                    else if (value + _minValue < _minValue)
                    {
                        if (_allowOverflow)
                        {
                            _index = (_maxValue - _minValue);
                        }
                        else
                        {
                            rightButton.interactable = true;
                            leftButton.interactable = false;
                            _index = 0;
                        }
                    }
                    else
                    {
                        _index = value;
                        leftButton.interactable = true;
                        rightButton.interactable = true;
                    }
                    selectionText.text = (_index + _minValue).ToString();
                }
                else
                {
                    if (value >= _selectionOptions.Length)
                    {
                        if (_allowOverflow)
                        {
                            _index = 0;
                        }
                        else
                        {
                            leftButton.interactable = true;
                            rightButton.interactable = false;
                            _index = _selectionOptions.Length - 1;
                        }
                    }
                    else if (value < 0)
                    {
                        if (_allowOverflow)
                        {
                            _index = _selectionOptions.Length - 1;
                        }
                        else
                        {
                            rightButton.interactable = true;
                            leftButton.interactable = false;
                            _index = 0;
                        }
                    }
                    else
                    {
                        _index = value;
                        leftButton.interactable = true;
                        rightButton.interactable = true;
                    }
                    selectionText.text = _selectionOptions[_index];
                }
                _onTextChanged.Invoke(selectionText.text, _index);
            }
        }

        // Use this for initialization
        protected override void Awake()
        {
            base.Awake();
            _currentChangeTime = _changeTimerMax;
            if (selectionText == null)
            {
                Debug.LogError("the value \"selectionText\" have not been assigned...");
                return;
            }
            if (leftButton == null)
            {
                Debug.LogError("the value \"leftImage\" have not been assigned...");
                return;
            }
            if (rightButton == null)
            {
                Debug.LogError("the value \"rightImage\" have not been assigned...");
                return;
            }
            if (minValue >= maxValue)
            {
                Debug.LogWarning("Max Value is larger or equal to the Min Value... Max Value have been set to Min Value + 1...");
                _maxValue = _minValue + 1;
            }

            leftButton.OnButtonHeld.AddListener(DecrementIndex);
            leftButton.OnButtonHeld.AddListener(this.GetComponent<Selectable>().Select);

            rightButton.OnButtonHeld.AddListener(IncrementIndex);
            rightButton.OnButtonHeld.AddListener(this.GetComponent<Selectable>().Select);


            leftButton.upperLimit = changeTimerMax;
            leftButton.lowerLimit = changeTimerMin;
            rightButton.upperLimit = changeTimerMax;
            rightButton.lowerLimit = changeTimerMin;

            Index = _index;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (isSelected)
            {
                _changeTimer += Time.deltaTime;
                if (Input.GetAxis("Horizontal") <= -0.1f || Input.GetAxis("Mouse ScrollWheel") <= -0.1)
                {
                    if (_changeTimer >= _currentChangeTime)
                    {
                        _changeTimer = 0;
                        Index--;
                    }
                    _currentChangeTime = Mathf.Lerp(_currentChangeTime, _changeTimerMin, Time.deltaTime);
                }
                else if (Input.GetAxis("Horizontal") >= 0.1f || Input.GetAxis("Mouse ScrollWheel") >= 0.1)
                {
                    if (_changeTimer >= _currentChangeTime)
                    {
                        _changeTimer = 0;
                        Index++;
                    }
                    _currentChangeTime = Mathf.Lerp(_currentChangeTime, _changeTimerMin, Time.deltaTime);
                }
                else
                {
                    _changeTimer = _maxValue;
                    _currentChangeTime = _changeTimerMax;
                }
            }
        }

        /// <summary>
        /// gets the value of the current index as a string.
        /// </summary>
        /// <returns>The string of the current index value.</returns>
        public string GetIndexedValueAsString()
        {
            return selectionOptions[Index];
        }

        /// <summary>
        /// gets the value of the current index as an integer.
        /// </summary>
        /// <returns>The interger of the current index value, or the index if usenumbers is false.</returns>
        public int GetIndexedValueAsInteger()
        {
            if (_useNumbersOnly)
            {
                return Convert.ToInt32(Index + _minValue);
            }
            else
            {
                return Index;
            }
        }

        /// <summary>
        /// Gets the current index value as a bool value.
        /// </summary>
        /// <returns>a bool value of the index</returns>
        public bool GetIndexedValueAsBoolean()
        {
            return Convert.ToBoolean(Index);
        }

        void IncrementIndex()
        {
            Index++;
        }

        void DecrementIndex()
        {
            Index--;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            isSelected = true;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            isSelected = false;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            Select();
        }
    }
}