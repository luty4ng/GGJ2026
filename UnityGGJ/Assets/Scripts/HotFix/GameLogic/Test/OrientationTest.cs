using System;
using UnityEngine;

namespace GameLogic
{
    public class OrientationTest : MonoBehaviour
    {
        public Transform trans;
        public float mouseSensitivity = 100f;
        private void OnDrawGizmos()
        {
            var forward = (trans.position - transform.position).normalized;
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            var up = Vector3.Cross(forward, right).normalized;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, forward * 10);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, right * 10);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, up * 10);
            
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawRay(transform.position, trans.TransformVector(m_worldSpaceRotationAxis) * 10);   
        }

        private void Update()
        {
            float xAxis = 0;
            float yAxis = 0;
            float k_inputThreshold = 0.01f;
            xAxis = Input.GetAxis("Mouse X") * 5;
            yAxis = Input.GetAxis("Mouse Y") * 5;    


            bool hasSignificantInput = false;
            Vector3 rotationDelta = Vector3.zero;
            if (Mathf.Abs(xAxis) > k_inputThreshold)
            {
                rotationDelta.y = xAxis;
                hasSignificantInput = true;
            }
            if (Mathf.Abs(yAxis) > k_inputThreshold)
            {
                rotationDelta.x = -yAxis;
                hasSignificantInput = true;
            }

            if (hasSignificantInput)
            {
                Quaternion rotationIncrement = Quaternion.Euler(rotationDelta);
                transform.rotation = transform.rotation * rotationIncrement;
            }
        }

        private Vector3 m_worldSpaceRotationAxis;

        private Vector3 CalculateWorldSpaceRotationAxis(Vector2 mouseInput)
        {
            Vector3 forward = transform.forward;
            Vector3 flatForward = new Vector3(forward.x, 0, forward.z).normalized;
            Vector3 horizontalAxis = Vector3.up;
            Vector3 verticalAxis = Vector3.Cross(horizontalAxis, flatForward).normalized;
            Vector3 rotationAxis = (verticalAxis * mouseInput.x + horizontalAxis * -mouseInput.y).normalized;
    
            return rotationAxis;
        }
    }
}