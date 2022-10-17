using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class ToggleSelectable : Selectable
    {
        [SerializeField]
        private bool _isToggled = false;
        [SerializeField]
        private ColorBlock _toggledColors = ColorBlock.defaultColorBlock;
        [SerializeField]
        private SpriteState _toggledSpriteState;
        [SerializeField]
        private AnimationTriggers _toggledAnimationTriggers = new AnimationTriggers();

        public bool isToggled
        {
            get
            {
                return _isToggled;
            }
            set
            {
                _isToggled = value;
                DoStateTransition(currentSelectionState, false);
            }
        }

        public ColorBlock toggledColors
        {
            get
            {
                return _toggledColors;
            }
            set
            {
                _toggledColors = value;
            }
        }

        public SpriteState toggledSpriteState
        {
            get
            {
                return _toggledSpriteState;
            }
            set
            {
                _toggledSpriteState = value;
            }
        }

        public AnimationTriggers toggledAnimationTriggers
        {
            get
            {
                return _toggledAnimationTriggers;
            }
            set
            {
                _toggledAnimationTriggers = value;
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (isToggled)
            {
                Color tintColor;
                Sprite transitionSprite;
                string triggerName;

                switch (state)
                {
                    case SelectionState.Normal:
                        tintColor = _toggledColors.normalColor;
                        transitionSprite = null;
                        triggerName = _toggledAnimationTriggers.normalTrigger;
                        break;
                    case SelectionState.Highlighted:
                        tintColor = _toggledColors.highlightedColor;
                        transitionSprite = _toggledSpriteState.highlightedSprite;
                        triggerName = _toggledAnimationTriggers.highlightedTrigger;
                        break;
                    case SelectionState.Pressed:
                        tintColor = _toggledColors.pressedColor;
                        transitionSprite = _toggledSpriteState.pressedSprite;
                        triggerName = _toggledAnimationTriggers.pressedTrigger;
                        break;
                    case SelectionState.Disabled:
                        tintColor = _toggledColors.disabledColor;
                        transitionSprite = toggledSpriteState.disabledSprite;
                        triggerName = _toggledAnimationTriggers.disabledTrigger;
                        break;
                    default:
                        tintColor = Color.black;
                        transitionSprite = null;
                        triggerName = string.Empty;
                        break;
                }

                if (gameObject.activeInHierarchy)
                {
                    switch (transition)
                    {
                        case Transition.ColorTint:
                            StartColorTween(tintColor * _toggledColors.colorMultiplier, instant);
                            break;
                        case Transition.SpriteSwap:
                            DoSpriteSwap(transitionSprite);
                            break;
                        case Transition.Animation:
                            TriggerAnimation(triggerName);
                            break;
                    }
                }
            }
            else
            {
                base.DoStateTransition(state, instant);
            }
        }

        void StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
                return;

            targetGraphic.CrossFadeColor(targetColor, instant ? 0f : _toggledColors.fadeDuration, true, true);
        }

        void DoSpriteSwap(Sprite newSprite)
        {
            if (image == null)
                return;

            image.overrideSprite = newSprite;
        }

        void TriggerAnimation(string triggername)
        {
            if (animator == null || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
                return;

            animator.ResetTrigger(toggledAnimationTriggers.normalTrigger);
            animator.ResetTrigger(toggledAnimationTriggers.pressedTrigger);
            animator.ResetTrigger(toggledAnimationTriggers.highlightedTrigger);
            animator.ResetTrigger(toggledAnimationTriggers.disabledTrigger);
            animator.SetTrigger(triggername);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            isToggled = !isToggled;
            base.OnPointerUp(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
        }
    }
}