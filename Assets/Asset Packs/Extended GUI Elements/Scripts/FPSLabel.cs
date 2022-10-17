using System;

namespace UnityEngine.UI
{
    public class FPSLabel : Text
    {
        [SerializeField]
        protected float _updateInterval = 0.5F;
        [SerializeField]
        protected string _formatString = "{0:0} FPS";

        float _fps;
        float _accum = 0;
        int _frames = 0;
        float _timeleft;

        /// <summary>
        /// The upadte interval of how often the label should update.
        /// </summary>
        public float updateInterval
        {
            get
            {
                return _updateInterval;
            }
            set
            {
                _updateInterval = value;
            }
        }

        /// <summary>
        /// Helper string to determine the format of the label.
        /// </summary>
        public string formatString
        {
            get
            {
                return _formatString;
            }
            set
            {
                _formatString = value;
            }
        }

        /// <summary>
        /// The current FPS
        /// </summary>
        public float fps
        {
            get
            {
                return _fps;
            }
            set
            {
                _fps = value;
            }
        }

        /// <summary>
        /// Updates the label with the fps.
        /// </summary>
        protected virtual void Update()
        {
            _timeleft -= Time.deltaTime;
            _accum += Time.timeScale / Time.deltaTime;
            ++_frames;

            if (_timeleft <= 0.0)
            {
                fps = _accum / _frames;
                text = String.Format(formatString, fps);

                _timeleft = updateInterval;
                _accum = 0.0F;
                _frames = 0;
            }
        }

        /// <summary>
        /// Instantly updates the fps counter.
        /// </summary>
        public void UpdateFPS()
        {
            _timeleft = 0;
            Update();
        }
    }
}