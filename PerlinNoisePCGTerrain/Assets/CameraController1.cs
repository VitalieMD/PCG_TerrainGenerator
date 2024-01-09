using UnityEngine;
using Cinemachine;


    public class CameraController1 : MonoBehaviour
    {
        public AxisState xAxis, yAxis;
        [SerializeField] private Transform _camPosition;

        private void Start()
        {
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
        }

        private void FixedUpdate()
        {
            xAxis.Update(Time.deltaTime);
            yAxis.Update(Time.deltaTime);
        }

        private void LateUpdate()
        {
            var localEulerAngles = _camPosition.localEulerAngles;
            localEulerAngles =
                new Vector3(yAxis.Value, localEulerAngles.y, localEulerAngles.z);
            _camPosition.localEulerAngles = localEulerAngles;

            var eulerAngles = transform.eulerAngles;
            eulerAngles = new Vector3(eulerAngles.x, xAxis.Value, eulerAngles.z);
            transform.eulerAngles = eulerAngles;
        }
    }
