using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Image))]
    public class HoldButton : Selectable, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private float _upperLimit = 0.25f;
        [SerializeField]
        private float _lowerLimit = 0.05f;

        private float _limitTimercounter = 0;
        private float _limitTimer = 0;

        [SerializeField]
        private UnityEvent _onButtonHeld;

        /// <summary>
        /// Notes if Button is held down
        /// </summary>
        bool _isButtonHeld = false;

        bool isButtonHeld
        {
            get
            {
                return _isButtonHeld;
            }
            set
            {
                _limitTimercounter = _upperLimit;
                _limitTimer = _upperLimit;
                _isButtonHeld = value;
            }
        }

        /// <summary>
        /// Event which is triggered if the button is held down.
        /// </summary>
        public UnityEvent OnButtonHeld
        {
            get
            {
                return _onButtonHeld;
            }
            set
            {
                _onButtonHeld = value;
            }
        }

        /// <summary>
        /// The upper limit the held event can be called
        /// </summary>
        public float upperLimit
        {
            get
            {
                return _upperLimit;
            }
            set
            {
                _upperLimit = value;
            }
        }

        /// <summary>
        /// The lower limit the held event can be called
        /// </summary>
        public float lowerLimit
        {
            get
            {
                return _lowerLimit;
            }
            set
            {
                _lowerLimit = value;
            }
        }

        protected override void Start()
        {
            base.Start();
            _limitTimercounter = _upperLimit;
            _limitTimer = _upperLimit;
        }

        void Update()
        {
            if (interactable && _isButtonHeld)
            {
                _limitTimercounter += Time.deltaTime;
                if (_limitTimer <= _limitTimercounter)
                {
                    _limitTimercounter = 0;
                    _onButtonHeld.Invoke();
                }
                _limitTimer = Mathf.Lerp(_limitTimer, _lowerLimit, Time.deltaTime);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            isButtonHeld = true;
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            isButtonHeld = false;
            base.OnPointerUp(eventData);
        }
    }
}