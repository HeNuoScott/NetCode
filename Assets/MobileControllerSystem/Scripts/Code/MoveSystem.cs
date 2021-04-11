// -----------------------------------------------
// Copyright © HeNuo. All rights reserved.
// CreateTime: 2020/8/23   7:45:30
// -----------------------------------------------

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
namespace MobileUISystem
{
    public class MoveSystem : MonoBehaviour
    {
        public UniversalButton inputMove;
        public bool lerpStopping = false;
        public Transform dirMarker;
        public float moveSpeed;

        public MessageBox msg;

        protected Vector3 cachedInput;


        protected virtual void Update()
        {
            if (inputMove.isFingerDown)
            {
                cachedInput = inputMove.directionXZ;
                if (cachedInput != Vector3.zero) transform.forward = cachedInput;
            }
            else
            {
                if (lerpStopping)
                {
                    cachedInput = Vector3.Lerp(cachedInput, Vector3.zero, moveSpeed * Time.deltaTime);
                }
                else
                {
                    cachedInput = Vector3.zero;
                }
            }

            transform.Translate(cachedInput * moveSpeed * Time.deltaTime, Space.World);
            dirMarker.position = transform.position + cachedInput;
            msg.UpdatePosition(transform.position);
        }
    }
}