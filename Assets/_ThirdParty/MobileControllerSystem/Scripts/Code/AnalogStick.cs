using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine;

namespace MobileUISystem
{
    /// <summary>
    /// 摇杆
    /// </summary>
    public class AnalogStick : UniversalButton
    {
        public RectTransform dpadInner;
        public RectTransform dpadOuter;
        public RectTransform directionalPointer;
        public RectTransform dpadCosmetic;

        public float innerRadius;
        public float pointerRadius;
        protected float tmpFloat;
        protected Vector3 tmpVec;

        protected override void Awake()
        {
            isAimable = true;
            base.Awake();
            innerRadius = dpadInner.rect.width / 2f;
            pointerRadius = pointer.rect.width / 2f;
            directionalPointer.gameObject.SetActive(false);

            if (isActive) state = ButtonState.Active;
            else state = ButtonState.Inactive;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (state == ButtonState.Active)
            {

                if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    isFingerDown = true;
                    fingerId = eventData.pointerId;

                    initialFingerPosition = ScreenPointToLocalAnchoredPosition(eventData.position);
                    fingerPosition = initialFingerPosition;
                    //tmpFloat = (fingerPosition - dpadInner.anchoredPosition3D).magnitude;

                    //if (tmpFloat < innerRadius)
                    //{
                    //    aimer.anchoredPosition3D = fingerPosition;
                    //}
                    //else
                    //{
                    //    tmpVec = dpadInner.anchoredPosition3D - fingerPosition;
                    //    tmpVec = Vector3.ClampMagnitude(tmpVec, aimerRadius);
                    //    tmpVec = fingerPosition + tmpVec;

                    //    aimer.anchoredPosition3D = new Vector3(tmpVec.x, tmpVec.y < aimerRadius ? aimerRadius : tmpVec.y, tmpVec.z);
                    //}
                }
                else
                {
                    isFingerDown = true;
                    fingerId = eventData.pointerId;
                    initialFingerPosition = eventData.position;
                    fingerPosition = initialFingerPosition;

                    tmpFloat = (fingerPosition - dpadInner.position).magnitude;

                    if (tmpFloat < innerRadius)
                    {
                        aimer.position = new Vector3(fingerPosition.x, fingerPosition.y < aimerRadius ? aimerRadius : fingerPosition.y, fingerPosition.z);
                    }
                    else
                    {
                        tmpVec = dpadInner.position - fingerPosition;
                        tmpVec = Vector3.ClampMagnitude(tmpVec, aimerRadius);
                        tmpVec = fingerPosition + tmpVec;

                        aimer.position = new Vector3(tmpVec.x, tmpVec.y < aimerRadius ? aimerRadius : tmpVec.y, tmpVec.z);
                    }
                }



                aimer.gameObject.SetActive(true);
                pointer.gameObject.SetActive(true);
                dpadCosmetic.gameObject.SetActive(false);

                UpdateAiming(eventData);

                state = ButtonState.Pressed;
                if (onPointerDown != null)
                {
                    onPointerDown.Invoke(btnIndex);
                }
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            directionalPointer.gameObject.SetActive(false);
            dpadCosmetic.gameObject.SetActive(true);
        }

        protected override void UpdateAiming(PointerEventData eventData)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                fingerPosition = eventData.position;
                Vector3 arp = ScreenPointToLocalAnchoredPosition(eventData.position);
                rawDir = arp - aimer.anchoredPosition3D;
                rawDir = Vector3.ClampMagnitude(rawDir, aimerRadius);
                pointer.anchoredPosition3D = aimer.anchoredPosition3D + Vector3.ClampMagnitude(rawDir, aimerRadius - pointerRadius);
            }
            else
            {
                fingerPosition.x = eventData.position.x;
                fingerPosition.y = eventData.position.y;
                rawDir = fingerPosition - aimer.position;
                rawDir = Vector3.ClampMagnitude(rawDir, aimerRadius);
                pointer.position = aimer.position + Vector3.ClampMagnitude(rawDir, aimerRadius - pointerRadius);
            }

            this.UpdateDirection();

            if (direction.magnitude > 0.01f)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    directionalPointer.anchoredPosition3D = aimer.anchoredPosition3D + direction.normalized * aimerRadius;
                }
                else
                {
                    directionalPointer.position = aimer.position + direction.normalized * aimerRadius;
                }
                directionalPointer.up = direction;
                directionalPointer.gameObject.SetActive(true);
            }
            else
            {
                directionalPointer.gameObject.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AnalogStick))]
    public class AnalogStickInspector : Editor
    {
        protected SerializedProperty isAimable;
        protected SerializedProperty aimer;
        protected SerializedProperty pointer;
        protected SerializedProperty skillCanceller;
        protected SerializedProperty isActive;
        protected SerializedProperty isFingerDown;

        protected SerializedProperty fingerId;
        protected SerializedProperty isManualAimOverride;
        protected SerializedProperty direction;
        protected SerializedProperty state;
        protected SerializedProperty text;

        protected SerializedProperty onPointerDown;
        protected SerializedProperty onBeginDrag;
        protected SerializedProperty onDrag;
        protected SerializedProperty onPointerUp;
        protected SerializedProperty onEndDrag;

        protected SerializedProperty dpadInner;
        protected SerializedProperty directionalPointer;
        protected SerializedProperty dpadCosmetic;

        protected virtual void OnEnable()
        {
            isAimable = serializedObject.FindProperty("isAimable");
            aimer = serializedObject.FindProperty("aimer");
            pointer = serializedObject.FindProperty("pointer");
            skillCanceller = serializedObject.FindProperty("skillCanceller");
            isActive = serializedObject.FindProperty("isActive");
            isFingerDown = serializedObject.FindProperty("isFingerDown");
            fingerId = serializedObject.FindProperty("fingerId");
            isManualAimOverride = serializedObject.FindProperty("isManualAimOverride");
            direction = serializedObject.FindProperty("direction");
            state = serializedObject.FindProperty("state");
            text = serializedObject.FindProperty("text");
            dpadInner = serializedObject.FindProperty("dpadInner");
            directionalPointer = serializedObject.FindProperty("directionalPointer");
            dpadCosmetic = serializedObject.FindProperty("dpadCosmetic");


            onPointerDown = serializedObject.FindProperty("onPointerDown");
            onBeginDrag = serializedObject.FindProperty("onBeginDrag");
            onDrag = serializedObject.FindProperty("onDrag");
            onPointerUp = serializedObject.FindProperty("onPointerUp");
            onEndDrag = serializedObject.FindProperty("onEndDrag");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var script = target as AnalogStick;
            EditorGUILayout.TextArea("-----[ Config ]---------------", GUIStyle.none);
            EditorGUILayout.PropertyField(isAimable);
            EditorGUILayout.PropertyField(aimer);
            EditorGUILayout.PropertyField(pointer);
            EditorGUILayout.PropertyField(directionalPointer);
            EditorGUILayout.PropertyField(dpadInner);
            EditorGUILayout.PropertyField(dpadCosmetic);

            EditorGUILayout.TextArea("-----[ Parameters ]---------------", GUIStyle.none);
            EditorGUILayout.PropertyField(isActive);
            EditorGUILayout.PropertyField(isFingerDown);
            EditorGUILayout.PropertyField(fingerId);
            EditorGUILayout.PropertyField(direction);

            EditorGUILayout.PropertyField(state);

            EditorGUILayout.PropertyField(onPointerDown);
            EditorGUILayout.PropertyField(onBeginDrag);
            EditorGUILayout.PropertyField(onDrag);
            EditorGUILayout.PropertyField(onPointerUp);
            EditorGUILayout.PropertyField(onEndDrag);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}