// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {

    public class ScrollBarCollider :
        MonoBehaviour,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler {

        [SerializeField] ScrollBarController scrollBarController;
        [SerializeField] RectTransform scrollBarButtonContainer;

        // Lazy and other variables
        ButtonController sliderButton => scrollBarController.sliderButton;
        bool isHorizontal => scrollBarController.isHorizontal;
        int currentPosition => scrollBarController.currentPosition;

        Camera _canvasCamera;
        Camera canvasCamera => _canvasCamera ??= GameObject.Find("PixelPerfectCamera").GetComponent<Camera>() as Camera;

        float? nf => null;
        int? nn => null;

        float scale;
        int limit;
        int length;

        bool isDragging;

        Vector2 pointerPosition = new Vector2();

        public void OnBeginDrag(PointerEventData eventData) {
            if (sliderButton.ownTransform.isMovingSmoothly) return;

            isDragging = true;

            scale = scrollBarController.currentScale;
            limit = scrollBarController.limit;
            length = scrollBarController.length;

            MoveSlider(delta: isHorizontal ? eventData.delta.x : eventData.delta.y);
        }

        public void OnDrag(PointerEventData eventData) {
            if (isDragging && CanMove(eventData)) {
                MoveSlider(delta: isHorizontal ? eventData.delta.x : eventData.delta.y);
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (isDragging) {
                MoveSlider(delta: isHorizontal ? eventData.delta.x : eventData.delta.y);
                isDragging = false;
            }
        }

        bool CanMove(PointerEventData eventData) {
            GetPointerPosition(eventData);

            var value = pointerPosition[isHorizontal ? 0 : 1] * (isHorizontal ? 1 : -1);

            return value >= 0 && value <= length;
        }

        void MoveSlider(float delta) {
            delta /= 2;
            delta *= isHorizontal ? 1 : -1;

            if (currentPosition == limit && delta > 0 || currentPosition == 0 && delta < 0) return;

            var change = sliderButton.ownTransform.SetFloatPositionByDelta(
                dx: isHorizontal ? delta : nf,
                dy: isHorizontal ? nf : -delta
            );

            var adjustment = currentPosition >= limit ? limit : currentPosition <= 0 ? 0 : nn;

            sliderButton.ownTransform.SetPosition(
                x: isHorizontal ? adjustment : nn,
                y: isHorizontal ? nn : -adjustment
            );

            scrollBarController.SetContentPosition();
        }

        void GetPointerPosition(PointerEventData eventData) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect: scrollBarButtonContainer,
                screenPoint: eventData.position,
                cam: canvasCamera,
                localPoint: out pointerPosition
            );
        }
    }

}