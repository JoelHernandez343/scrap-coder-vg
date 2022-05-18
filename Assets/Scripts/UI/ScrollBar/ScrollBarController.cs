// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {

    public class ScrollBarController : MonoBehaviour {

        // Editor variables
        [SerializeField] public bool isHorizontal;

        [SerializeField] ButtonController firstButton;
        [SerializeField] ButtonController finalButton;

        [SerializeField] public ButtonController sliderButton;

        [SerializeField] public int visor;

        [SerializeField] public NodeTransform contentT;

        // State variables
        public float currentScale;

        int sliderLength;

        // Lazy and other variables variables
        public int currentPosition => isHorizontal
            ? sliderButton.ownTransform.x
            : -sliderButton.ownTransform.y;

        int content => isHorizontal ? contentT.width : contentT.height;

        float? _previousScale;
        float previousScale {
            get => _previousScale ??= currentScale;
            set => _previousScale = value;
        }

        int? _previousSliderLength;
        int previousSliderLength {
            get => _previousSliderLength ??= sliderLength;
            set => _previousSliderLength = value;
        }

        public int length => isHorizontal
            ? ownTransform.width - firstButton.ownTransform.width - finalButton.ownTransform.width - (sliderOffset * 2)
            : ownTransform.height - firstButton.ownTransform.height - finalButton.ownTransform.height - (sliderOffset * 2);

        public int limit => length - sliderLength;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        int futurePosition => sliderButton.ownTransform.futurePosition[isHorizontal ? 0 : 1] * (isHorizontal ? 1 : -1);
        int contentFuturePosition => contentT.futurePosition[isHorizontal ? 0 : 1] * (isHorizontal ? 1 : -1);

        int minSliderLength => isHorizontal
            ? sliderButton.ownTransform.minWidth
            : sliderButton.ownTransform.minHeight;

        const int sliderOffset = 2;
        float? nf => null;
        int? nn => null;

        // Methods
        void Start() {
            Initialize();
        }

        void Initialize() {
            RefreshSlider();

            firstButton.AddListener(() => MoveSliderBy(-sliderLength / 13));
            finalButton.AddListener(() => MoveSliderBy(sliderLength / 13));
        }

        void RefreshDimensions() {
            currentScale = content / (float)length;
            sliderLength = (int)System.Math.Round(visor / currentScale);
        }

        public void RefreshSlider() {
            RefreshDimensions();

            if (sliderLength >= length) {
                sliderButton.ownTransform.SetPosition(
                    x: isHorizontal ? 0 : nn,
                    y: isHorizontal ? nn : -0
                );

                SetContentPosition();

                gameObject.SetActive(false);
                return;
            }

            if (sliderLength < minSliderLength) {
                currentScale = visor / (float)minSliderLength;
                sliderLength = minSliderLength;
            }

            var newPosition = currentPosition +
                (currentPosition == 0 ? 0 : (previousSliderLength - sliderLength));

            newPosition = newPosition < 0
                ? 0
                : newPosition > limit
                ? limit
                : newPosition;


            sliderButton.ownTransform.SetPosition(
                x: isHorizontal ? newPosition : nn,
                y: isHorizontal ? nn : -newPosition
            );

            sliderButton.ownTransform.ExpandByNewDimensions(
                newWidth: isHorizontal ? sliderLength : nn,
                newHeight: isHorizontal ? nn : sliderLength
            );

            previousSliderLength = sliderLength;

            SetContentPosition();

            gameObject.SetActive(true);
        }

        void MoveSliderBy(int delta) {

            if (futurePosition >= limit && delta > 0) return;
            if (futurePosition <= 0 && delta < 0) return;

            var newDelta = delta;

            if (futurePosition + delta > limit && delta > 0) {
                newDelta = limit - futurePosition;
            } else if (futurePosition + delta < 0 && delta < 0) {
                newDelta = -futurePosition;
            }

            sliderButton.ownTransform.SetPositionByDelta(
                dx: isHorizontal ? newDelta : nn,
                dy: isHorizontal ? nn : -newDelta,
                smooth: true
            );

            if ((futurePosition <= limit && currentPosition < limit) || (futurePosition >= 0 && currentPosition > 0)) {
                var changed = newDelta;

                SetContentPosition();
            }
        }

        int Round(float number)
            => number >= 0
                ? (int)System.Math.Floor(number)
                : (int)System.Math.Ceiling(number);


        public void SetContentPosition() {
            var newPosition = futurePosition == limit
                ? content - visor
                : Round(futurePosition * currentScale);

            contentT.SetPosition(
                x: isHorizontal ? -newPosition : nn,
                y: isHorizontal ? nn : newPosition,
                smooth: true
            );
        }

    }
}