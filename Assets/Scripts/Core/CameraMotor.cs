using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class CameraMotor : MonoBehaviour
    {
        public Transform target;

        private void LateUpdate()
        {
            transform.position = target.position;
        }
    }
}