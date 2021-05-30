using System;
using UnityEngine;

namespace SLibrary
{
    public class OffsetFollower : MonoBehaviour
    {
        public Vector3 Offset;
        private Transform _target;

        public Transform Target
        {
            set
            {
                _target = value;
                Update();
            }
        }

        private void Update()
        {
            if (ReferenceEquals(_target, null))
            {
                return;
            }

            transform.position = _target.position + Offset;
        }
    }
}