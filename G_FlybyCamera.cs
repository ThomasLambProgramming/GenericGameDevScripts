using UnityEngine;
using UnityEngine.InputSystem;

namespace GenericHelperScripts
{
    //Only use this class for debugging purposes
    public class G_FlybyCamera : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private float m_cameraSpeed = 5f;
        [SerializeField] private float m_rotateXSpeed = 5f;
        [SerializeField] private float m_rotateYSpeed = 5f;
        [SerializeField] private float m_risingSpeed = 5f;
        [SerializeField] private float m_boostMultiplier = 3f;
        [SerializeField] private bool m_usesOldInput = false;

        private float m_currentMoveSpeed = 0;
        private bool m_holdingShift = false;
        private Vector2 m_mouseInput;
        private Vector2 m_keyboardInput;

        void Start()
        {
            if (m_camera == null)
            {
                //If a camera isn't already set and we are attached to a camera make sure it is the one that is rendered
                //we are setting the camera to be the last camera rendered. 
                m_camera = GetComponent<Camera>();

                if (m_camera == null)
                    m_camera = Camera.main;
            }

            if (m_camera == null)
                return;

            //in urp depth gets automatically changed to priority. quite annoying that it isnt mentioned.
            m_camera.depth = 50;
            //if none of this works than there is no camera.

            //below was an attempt to add if urp do this other option to get it to work but yeah, not needed since depth gets changed.
            //if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().Name.Contains("UniversalRenderPipeline"))
            //{
            //    UniversalAdditionalCameraData cameraRenderer = m_camera.GetComponent<UniversalAdditionalCameraData>();
            //    if (cameraRenderer == null || cameraRenderer.renderType != CameraRenderType.Overlay)
            //        return;
            //    //remove it and make sure that the camera is on the end and is last to be rendered
            //    if (cameraRenderer.cameraStack.Contains(m_camera))
            //        cameraRenderer.cameraStack.Remove(m_camera);
            //    cameraRenderer.cameraStack.Add(m_camera);
            //}

            if (!m_usesOldInput)
            {
                //todo: setup generic input system for the repo and then to expand for fishing wizard
            }

            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (m_camera == null)
                return;

            if (m_usesOldInput)
            {
                m_mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                m_keyboardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (Input.GetKey(KeyCode.Space))
                    m_camera.transform.position += new Vector3(0, 1, 0) * (Time.deltaTime * m_risingSpeed);

                if (Input.GetKey(KeyCode.LeftControl))
                    m_camera.transform.position += new Vector3(0, -1, 0) * (Time.deltaTime * m_risingSpeed);

                if (Input.GetMouseButtonDown(0))
                    Cursor.lockState = CursorLockMode.Locked;

                if (Input.GetKeyDown(KeyCode.LeftShift))
                    m_holdingShift = true;
                if (Input.GetKeyUp(KeyCode.LeftShift))
                    m_holdingShift = false;
            }

            m_currentMoveSpeed = m_cameraSpeed;
            if (m_holdingShift)
                m_currentMoveSpeed = m_cameraSpeed * m_boostMultiplier;

            m_camera.transform.position += m_camera.transform.forward * (m_keyboardInput.y * m_currentMoveSpeed * Time.deltaTime);
            m_camera.transform.position += m_camera.transform.right * (m_keyboardInput.x * m_currentMoveSpeed * Time.deltaTime);
            m_camera.transform.rotation *= Quaternion.Euler(-m_mouseInput.y * m_rotateYSpeed * Time.deltaTime, m_mouseInput.x * m_rotateXSpeed * Time.deltaTime, 0);

            //Remove z rotation as it will cause wrong rotations, this camera does have gimbal lock but it will serve just fine for flyby camera needs.
            Vector3 cameraRot = m_camera.transform.rotation.eulerAngles;
            cameraRot.z = 0;
            m_camera.transform.rotation = Quaternion.Euler(cameraRot);
        }

        private void MouseInputDown(InputAction.CallbackContext a_context)
        {
            m_mouseInput = a_context.ReadValue<Vector2>();
        }

        private void KeyboardInputDown(InputAction.CallbackContext a_context)
        {
            m_keyboardInput = a_context.ReadValue<Vector2>();
        }

        private void MouseInputUp(InputAction.CallbackContext a_context)
        {
            m_mouseInput = Vector2.zero;
        }

        private void KeyboardInputUp(InputAction.CallbackContext a_context)
        {
            m_keyboardInput = Vector2.zero;
        }
    }
}