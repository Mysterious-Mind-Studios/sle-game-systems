
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Jobs;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using Cinemachine;

using Unity.Mathematics;
using Unity.Jobs;

namespace SLE.Systems.Experimental.Camera
{
    using SLE.Systems.Experimental.Camera.Jobs;
    using SLE.Systems.Experimental.Camera.Data;

    public class CameraSystem : SystemBase
    {
        public static float screenThreshold { get; set; } = 10;

        private static void ThrowMissingCinemachineComponent()
        {
            throw new MissingReferenceException($"The camera controller requires a cinemachine camera that has a {nameof(CinemachineFramingTransposer)}");
        }

        public CameraSystem()
        {
            activeCameraControllers = new HashSet<CameraController>(GameObject.FindObjectsOfType<CameraController>());

            int length = activeCameraControllers.Count;

            cacheCameraControllers        = new CameraController[length];
            cacheControllerData           = new ControllerData[length];
            cameraControllerTransformList = new TransformAccessArray(length);

            activeCameraControllers.CopyTo(cacheCameraControllers);

            int i;
            for (i = 0; i < length; i++)
            {
                CameraController camController = cacheCameraControllers[i];

                camController._id = i;
                camController._transposer = camController._virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

                if (!camController._transposer)
                    ThrowMissingCinemachineComponent();

                camController._minZoom = math.max(0, camController._minZoom);
                camController._maxZoom = math.min(1000, camController._maxZoom);

                cacheControllerData[i] = new ControllerData(in camController);
                cameraControllerTransformList.Add(camController.transform);
            }

            CameraController.onControllerDataChange += OnCameraControllerDataChange;
            CameraController.OnComponentEnable      += OnCameraControllerEnable;
            CameraController.OnComponentDisable     += OnCameraControllerDisable;

            currentKeyboard = Keyboard.current;
            currentMouse    = Mouse.current;

            locked = activeCameraControllers.Count == 0;
        }

        private void OnCameraControllerEnable(CameraController camController)
        {
            if(activeCameraControllers.Add(camController))
            {
                int length = activeCameraControllers.Count;

                Array.Resize(ref cacheCameraControllers, length);
                Array.Resize(ref cacheControllerData, length);

                cameraControllerTransformList.capacity = length;
                cameraControllerTransformList.SetTransforms(null);

                activeCameraControllers.CopyTo(cacheCameraControllers);

                int i;
                for (i = 0; i < length; i++)
                {
                    CameraController cc = cacheCameraControllers[i];

                    cc._id = i;
                    cc._transposer = cc._virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

                    if (!cc._transposer)
                        ThrowMissingCinemachineComponent();

                    cc._minZoom = math.max(0, cc._minZoom);
                    cc._maxZoom = math.min(1000, cc._maxZoom);

                    cacheControllerData[i] = new ControllerData(in cc);
                    cameraControllerTransformList.Add(cc.transform);
                }

                locked = false;
            }
        }
        private void OnCameraControllerDisable(CameraController camController)
        {
            if (activeCameraControllers.Remove(camController))
            {
                int length = activeCameraControllers.Count;

                Array.Resize(ref cacheCameraControllers, length);
                Array.Resize(ref cacheControllerData, length);

                cameraControllerTransformList.capacity = length;
                cameraControllerTransformList.SetTransforms(null);

                activeCameraControllers.CopyTo(cacheCameraControllers);

                int i;
                for (i = 0; i < length; i++)
                {
                    CameraController cc = cacheCameraControllers[i];

                    cc._id = i;
                    cc._transposer = cc._virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

                    if (!cc._transposer)
                        ThrowMissingCinemachineComponent();

                    cc._minZoom = math.max(0, cc._minZoom);
                    cc._maxZoom = math.min(1000, cc._maxZoom);

                    cacheControllerData[i] = new ControllerData(in cc);
                    cameraControllerTransformList.Add(cc.transform);
                }

                locked = length == 0;
            }
        }
        private void OnCameraControllerDataChange(CameraController camController)
        {
            int index = camController._id;

            cacheControllerData[index] = new ControllerData(in camController);
        }


        bool locked;

        HashSet<CameraController> activeCameraControllers;

        // --- Cache Data --- //

        Keyboard currentKeyboard;
        Mouse    currentMouse;

        CameraController[]   cacheCameraControllers;
        ControllerData[]     cacheControllerData;
        TransformAccessArray cameraControllerTransformList;

        Vector3 cacheMousePositionInput;
        Vector3 cacheMouseScrollDelta;

        // --- Cache Data --- //

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (cameraControllerTransformList.isCreated)
                    cameraControllerTransformList.Dispose();
            }

            activeCameraControllers = null;
            cacheCameraControllers  = null;

            currentKeyboard = null;
            currentMouse    = null;
        }

        public override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        {
            if (locked)
                return handle;

            float x = 0;
            float z = 0;

#if ENABLE_INPUT_SYSTEM

            cacheMousePositionInput = currentMouse.position.ReadValue();
            cacheMouseScrollDelta   = currentMouse.scroll.ReadValue();

            if (currentKeyboard[Key.A].isPressed                   ||
                currentKeyboard[Key.LeftArrow].wasPressedThisFrame ||
                cacheMousePositionInput.x <= screenThreshold)
                x = -1;

            if (currentKeyboard[Key.D].isPressed                    ||
                currentKeyboard[Key.RightArrow].wasPressedThisFrame ||
                cacheMousePositionInput.x >= Screen.width - screenThreshold)
                x = 1;

            if (currentKeyboard[Key.W].isPressed                 ||
                currentKeyboard[Key.UpArrow].wasPressedThisFrame ||
                cacheMousePositionInput.y >= Screen.height - screenThreshold)
                z = 1;

            if (currentKeyboard[Key.S].isPressed                   ||
                currentKeyboard[Key.DownArrow].wasPressedThisFrame ||
                cacheMousePositionInput.y <= screenThreshold)
                z = -1;
#else
            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");

            Vector3 mousePosition = Input.GetMousePosition();

            if (mousePosition.x <= screenThreshold)
                x = -1;

            if (mousePosition.x >= Screen.width - screenThreshold)
                x = 1;

            if (mousePosition.y >= Screen.height - screenThreshold)
                z = 1;

            if (mousePosition.y <= screenThreshold)
                z = -1;
#endif

            unsafe
            {
                fixed (ControllerData* controllerDataPtr = &cacheControllerData[0])
                {
                    UpdateCameraControllerTransformJob job = new UpdateCameraControllerTransformJob
                    {
                        controllerDataPtr = controllerDataPtr,
                        axisInput = new Vector3(x, 0, z),
                        deltaTime = deltaTime,
                    };

                    return job.Schedule(cameraControllerTransformList);
                }
            }
        }

        public override void OnUpdate(float time, float deltaTime)
        {
            if (locked) return;

            int i;
            int length = cacheCameraControllers.Length;

            float cameraDistance;

            for (i = 0; i < length; i++)
            {
                CameraController cc = cacheCameraControllers[i];

                cameraDistance = cc._transposer.m_CameraDistance + -cacheMouseScrollDelta.y * deltaTime * cc.scrollSensivity;

                cc._transposer.m_CameraDistance = math.round(math.clamp(cameraDistance, cc._minZoom, cc._maxZoom)); 
            }
        }

        public override void OnStop()
        {
            CameraController.onControllerDataChange -= OnCameraControllerDataChange;
            CameraController.OnComponentEnable      -= OnCameraControllerEnable;
            CameraController.OnComponentDisable     -= OnCameraControllerDisable;
        }
    }
}
