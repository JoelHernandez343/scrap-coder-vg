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

        [SerializeField] int visor;
        // [SerializeField] int content;

        [SerializeField] NodeTransform contentT;

        // State variables
        public int currentPosition => isHorizontal
            ? sliderButton.ownTransform.x
            : -sliderButton.ownTransform.y;

        // Lazy variables
        int content => isHorizontal ? contentT.width : contentT.height;

        float? _previousScale;
        float previousScale {
            get => _previousScale ??= currentScale;
            set => _previousScale = value;
        }

        public float currentScale => content / (float)length;

        int sliderLength => (int)System.Math.Round(visor / currentScale);

        public int length => isHorizontal
            ? ownTransform.width - firstButton.ownTransform.width - finalButton.ownTransform.width + 2
            : ownTransform.height - firstButton.ownTransform.height - finalButton.ownTransform.height + 2;

        public int limit => length - sliderLength;

        float? nf => null;
        int? nn => null;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        int futurePosition => sliderButton.ownTransform.futurePosition[isHorizontal ? 0 : 1] * (isHorizontal ? 1 : -1);
        int contentFuturePosition => contentT.futurePosition[isHorizontal ? 0 : 1] * (isHorizontal ? 1 : -1);

        // Methods
        void Start() {
            if (sliderLength > length) {
                gameObject.SetActive(false);
                return;
            }

            RefreshSlider();

            firstButton.AddListener(() => MoveSliderBy(-sliderLength / 3));
            finalButton.AddListener(() => MoveSliderBy(sliderLength / 3));
        }

        public void RefreshSlider() {

            var newPosition = (int)(currentPosition * previousScale / currentScale);

            sliderButton.ownTransform.SetPosition(
                x: isHorizontal ? newPosition : nn,
                y: isHorizontal ? nn : -newPosition
            );

            sliderButton.ownTransform.ExpandByNewDimensions(
                newWidth: isHorizontal ? sliderLength : nn,
                newHeight: isHorizontal ? nn : sliderLength
            );

            previousScale = currentScale;
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
                // Debug.Log(newDelta);
                MoveContent(changed, smooth: true);
            }
        }

        int Round(float number)
            => number >= 0
                ? (int)System.Math.Floor(number)
                : (int)System.Math.Ceiling(number);


        public void MoveContent(int delta, bool smooth = false) {
            if (delta == 0) return;

            if (!smooth) {
                var newPosition = Round(currentPosition * currentScale);

                contentT.SetPosition(
                    x: isHorizontal ? -newPosition : nn,
                    y: isHorizontal ? nn : newPosition
                );
            } else {
                var realDelta = delta * currentScale;

                contentT.SetFloatPositionByDelta(
                    dx: isHorizontal ? -realDelta : nf,
                    dy: isHorizontal ? nf : realDelta,
                    smooth: smooth
                );
            }

        }

    }
}