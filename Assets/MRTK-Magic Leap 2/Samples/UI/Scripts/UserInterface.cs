// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MagicLeap.MRTK
{
    public class UserInterface : MonoBehaviour
    {
        private const float SIDE_MENU_DEFAULT_WIDTH = 175;
        private const float SIDE_MENU_MAX_WIDTH = 475;

        [Header("Settings")]
        [SerializeField, Tooltip("The default and closest distance for the canvas.")]
        private float _minDistance = 3f;

        [SerializeField, Tooltip("The furthest distance for the canvas.")]
        private float _maxDistance = 4f;

        [SerializeField, Tooltip("The primary workspace, this area will be collapsed in the minimized view.")]
        private GameObject _workspace = null;

        [Header("Interface")]
        [SerializeField, Tooltip("The transform of the side menu.")]
        private RectTransform _sideMenu = null;

        private float _canvasDistance = 3f;
        private GameObject lockButton;

        private bool lockCanvas = false;

        private void Start()
        {
            lockButton = GameObject.Find("Button - Lock");
            ExecuteEvents.Execute<IPointerClickHandler>(lockButton, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }

        private void Update()
        {
            if (!lockCanvas)
            {
                Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * _canvasDistance;
                this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position,position, Time.deltaTime);
                this.gameObject.transform.forward = Camera.main.transform.forward;
            }
        }

        public void ToggleCanvasLock()
        {
            lockCanvas = !lockCanvas;
        }

        /// <summary>
        /// Toggle the canvas distance between the min and max distance.
        /// </summary>
        public void ToggleCanvasDistance()
        {
            _canvasDistance = (_canvasDistance == _minDistance) ? _maxDistance : _minDistance;
        }

        /// <summary>
        /// Toggle the visibility of the workspace.
        /// </summary>
        public void ToggleCanvas()
        {
            ShowCanvas(!_workspace.activeInHierarchy);
        }

        /// <summary>
        /// Set the visibility of the workspace.
        /// </summary>
        /// <param name="visible">The desired visible state of the workspace.</param>
        public void ShowCanvas(bool visible)
        {
            _workspace.SetActive(visible);

            // Adjust the width of the side menu, this allows it to shift left/right.
            _sideMenu.sizeDelta = new Vector2((_workspace.activeInHierarchy) ? SIDE_MENU_DEFAULT_WIDTH : SIDE_MENU_MAX_WIDTH, _sideMenu.sizeDelta.y);
        }

        public void QuitApplication() => Application.Quit();
    }
}
