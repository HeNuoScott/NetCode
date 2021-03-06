using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

namespace MobileUISystem
{

    [System.Serializable] public class UnityEventInt : UnityEvent<int> { }

    /// <summary>
    /// 虚拟按钮
    /// </summary>
    public class UniversalButton : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        #region :: Config ::
        public bool isAimable = false;// 是否可瞄准(不可瞄准为瞬间释放)
        public RectTransform btn;
        public RectTransform aimer;
        public RectTransform pointer;
        public RectTransform skillCanceller;
        protected SkillCanceller cachedSkillCanceller;
        public bool hasText;
        public Text text;
        public Image img;
        #endregion

        #region :: Parameters ::
        public ButtonState state;
        public bool isActive;
        public float btnRadius;
        public float aimerRadius;
        public bool isManualAimOverride = false;
        public bool isFingerDown = false;
        public bool isPointerUpOutOfBound = false;
        public Vector3 initialFingerPosition;
        public int fingerId = -99;
        public Vector3 fingerPosition;
        public Vector3 direction;// 方向 
        public Vector3 directionXZ;//方向 xz
        public Vector3 rawDir;
        public float cancellerRadius;
        [SerializeField]
        public float Horizontal
        {
            get { return direction.x; }
        }
        [SerializeField]
        public float Vertical
        {
            get { return direction.y; }
        }
        #endregion

        #region Cosmetics
        protected Vector3 refScale;
        protected Vector3 onPressedScale;
        public Color colorActive;
        public Color colorInactive;
        public Color colorPressed;
        #endregion

        #region Events
        public int btnIndex;
        public UnityEventInt onPointerDown;
        public UnityEventInt onBeginDrag;
        public UnityEventInt onDrag;
        public UnityEventInt onPointerUp;
        public UnityEventInt onEndDrag;
        public UnityEventInt onActivateSkill;
        public UnityEventInt onCancelSkill;
        #endregion

        public enum ButtonState
        {
            /// <summary>
            /// 激活
            /// </summary>
            Active,
            /// <summary>
            /// 不激活
            /// </summary>
            Inactive,
            /// <summary>
            /// 按下
            /// </summary>
            Pressed
        }

        protected bool canActivateSkill = false;

        protected Canvas canvas;
        protected Camera cam;

        protected virtual void Awake()
        {
            if (isAimable)
            {
                aimer.gameObject.SetActive(false);
                pointer.gameObject.SetActive(false);
            }
            btn = GetComponent<RectTransform>();

            this.UpdateBound();

            refScale = btn.localScale;
            onPressedScale = refScale * 0.75f;

            img = GetComponent<Image>();
            colorActive = img.color;

            this.UpdateButtonState();

            canvas = GetComponentInParent<Canvas>();
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (state == ButtonState.Active)
            {
                isFingerDown = true;
                fingerId = eventData.pointerId;
                initialFingerPosition = eventData.position;
                fingerPosition = initialFingerPosition;
                isPointerUpOutOfBound = false;

                if (isAimable)
                {
                    aimer.gameObject.SetActive(true);
                    pointer.gameObject.SetActive(true);
                    pointer.localPosition = aimer.localPosition;                    
                }

                if (skillCanceller != null)
                {
                    this.UpdateSkillCancellerState();
                }

                state = ButtonState.Pressed;
                this.UpdateColor();

                if (onPointerDown != null)
                {
                    onPointerDown.Invoke(btnIndex);
                }
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (isAimable && state == ButtonState.Pressed)
            {
                isManualAimOverride = true;

                this.UpdateAiming(eventData);

                if (onBeginDrag != null)
                {
                    onBeginDrag.Invoke(btnIndex);
                }
            }

        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (isAimable && eventData.pointerId == fingerId && state == ButtonState.Pressed)
            {
                UpdateAiming(eventData);

                if (skillCanceller != null)
                {
                    this.UpdateSkillCancellerState();
                }

                if (onDrag != null)
                {
                    onDrag.Invoke(btnIndex);
                }
            }

        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (state == ButtonState.Pressed)
            {
                isFingerDown = false;
                fingerId = -99;

                if (Vector3.Distance(initialFingerPosition, fingerPosition) > btnRadius)
                {
                    isPointerUpOutOfBound = true;
                }

                if (isAimable)
                {
                    aimer.gameObject.SetActive(false);
                    pointer.gameObject.SetActive(false);

                    if (skillCanceller != null)
                    {
                        this.UpdateSkillCancellerState();
                    }
                }

                state = ButtonState.Active;
                this.UpdateButtonState();

                if (canActivateSkill && onActivateSkill != null)
                {
                    onActivateSkill.Invoke(btnIndex);
                }
                else if (onCancelSkill != null)
                {
                    onCancelSkill.Invoke(btnIndex);
                }

                if (onPointerUp != null)
                {
                    onPointerUp.Invoke(btnIndex);
                }

                this.UpdateColor();
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (isAimable)
            {
                isManualAimOverride = false;

                if (onEndDrag != null)
                {
                    onEndDrag.Invoke(btnIndex);
                }
            }
        }

        protected virtual void UpdateAiming(PointerEventData eventData)
        {
            
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                fingerPosition = eventData.position;
                Vector3 arp = ScreenPointToLocalAnchoredPosition(eventData.position);
                rawDir = arp - aimer.anchoredPosition3D;
                rawDir = Vector3.ClampMagnitude(rawDir, aimerRadius);
                pointer.anchoredPosition3D = aimer.anchoredPosition3D + rawDir;
            }
            else
            {
                fingerPosition.x = eventData.position.x;
                fingerPosition.y = eventData.position.y;
                rawDir = fingerPosition - aimer.position;
                rawDir = Vector3.ClampMagnitude(rawDir, aimerRadius);
                pointer.position = aimer.position + rawDir;
            }
            
            this.UpdateDirection();
        }

        protected virtual void UpdateDirection()
        {
            direction = rawDir / aimerRadius;
            directionXZ.x = direction.x;
            directionXZ.y = 0f;
            directionXZ.z = direction.y;
        }

        public virtual void SetActiveState(bool active)
        {
            isActive = active;
            this.UpdateButtonState();
        }

        protected virtual void UpdateButtonState()
        {
            if (isActive)
            {
                state = ButtonState.Active;
            }
            else
            {
                state = ButtonState.Inactive;

            }
            this.UpdateColor();
        }

        protected virtual void UpdateColor()
        {
            switch (state)
            {
                case ButtonState.Active:
                    img.color = colorActive;
                    break;
                case ButtonState.Inactive:
                    img.color = colorInactive;
                    break;
                case ButtonState.Pressed:
                    img.color = colorPressed;
                    break;
            }
        }

        protected void UpdateSkillCancellerState()
        {
            if (cachedSkillCanceller == null)
            {
                cachedSkillCanceller = skillCanceller.GetComponent<SkillCanceller>();
            }

            if (IsFingerOverSkillCancellerButton())
            {
                canActivateSkill = false;
                cachedSkillCanceller.state = ButtonState.Pressed;
            }
            else
            {
                canActivateSkill = true;
                cachedSkillCanceller.state = ButtonState.Active;
            }
        }

        protected bool IsFingerOverSkillCancellerButton()
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                return RectTransformUtility.RectangleContainsScreenPoint(skillCanceller, fingerPosition, cam);
            }
            else
            {
                return Vector3.Distance(fingerPosition, skillCanceller.position) < cancellerRadius;
            }
        }

        public virtual void SetText(string t)
        {
            if (text != null)
            {
                text.text = t;
            }
        }

        /// <summary>
        /// 检查边界
        /// </summary>
        public virtual void UpdateBound()
        {
            btnRadius = btn.rect.width / 2f;
            if (isAimable)
            {
                aimerRadius = aimer.rect.width / 2f;
            }
            if (skillCanceller != null)
            {
                cancellerRadius = skillCanceller.rect.width / 2f;
            }
        }

        protected Vector2 ScreenPointToLocalAnchoredPosition(Vector2 screenPosition)
        {
            Vector2 localPoint = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(btn, screenPosition, cam, out localPoint))
            {
                Vector2 pivotOffset = btn.pivot * btn.sizeDelta;
                return localPoint - (aimer.anchorMax * btn.sizeDelta) + pivotOffset;
            }
            return Vector2.zero;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UniversalButton)), CanEditMultipleObjects]
    public class UniversalBtnInspector : Editor
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

        protected bool showRawButtonEvents = false;
        protected SerializedProperty onPointerDown;
        protected SerializedProperty onBeginDrag;
        protected SerializedProperty onDrag;
        protected SerializedProperty onPointerUp;
        protected SerializedProperty onEndDrag;

        protected bool showGameLogicEvents = true;
        protected SerializedProperty btnIndex;
        protected SerializedProperty onActivateSkill;
        protected SerializedProperty onCancelSkill;

        protected SerializedProperty colorActive;
        protected SerializedProperty colorInactive;
        protected SerializedProperty colorPressed;
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

            btnIndex = serializedObject.FindProperty("btnIndex");
            onActivateSkill = serializedObject.FindProperty("onActivateSkill");
            onCancelSkill = serializedObject.FindProperty("onCancelSkill");

            onPointerDown = serializedObject.FindProperty("onPointerDown");
            onBeginDrag = serializedObject.FindProperty("onBeginDrag");
            onDrag = serializedObject.FindProperty("onDrag");
            onPointerUp = serializedObject.FindProperty("onPointerUp");
            onEndDrag = serializedObject.FindProperty("onEndDrag");

            colorActive = serializedObject.FindProperty("colorActive");
            colorInactive = serializedObject.FindProperty("colorInactive");
            colorPressed = serializedObject.FindProperty("colorPressed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            /*
            var script = target as UniversalButton;
            EditorGUILayout.TextArea("-----[ Config ]---------------", GUIStyle.none);
            script.scaler = EditorGUILayout.ObjectField("scaler", script.scaler, typeof(CanvasScaler), true) as CanvasScaler;
            script.isAimable = EditorGUILayout.Toggle("isAimable", script.isAimable);
            if (script.isAimable)
            {
                script.aimer = EditorGUILayout.ObjectField("aimer", script.aimer, typeof(RectTransform), true) as RectTransform;
                script.pointer = EditorGUILayout.ObjectField("pointer", script.pointer, typeof(RectTransform), true) as RectTransform;
                script.skillCanceller = EditorGUILayout.ObjectField("skillCanceller", script.skillCanceller, typeof(RectTransform), true) as RectTransform;

            }
            EditorGUILayout.TextArea("-----[ Parameters ]---------------", GUIStyle.none);
            script.isActive = EditorGUILayout.Toggle("isActive", script.isActive);
            script.isFingerDown = EditorGUILayout.Toggle("isFingerDown", script.isFingerDown);
            script.fingerId = EditorGUILayout.IntField("fingerId", script.fingerId);

            if (script.isAimable)
            {
                script.isManualAimOverride = EditorGUILayout.Toggle("isManualAimOverride", script.isManualAimOverride);
                script.direction = EditorGUILayout.Vector3Field("direction", script.direction);

            }

            script.state = (UniversalButton.ButtonState)EditorGUILayout.EnumPopup("state", script.state);
            */

            /*
            EditorGUILayout.TextArea("-----[ Config ]---------------", GUIStyle.none);
            scaler.objectReferenceValue = EditorGUILayout.ObjectField("scaler", scaler.objectReferenceValue, typeof(CanvasScaler), true) as CanvasScaler;
            isAimable.boolValue = EditorGUILayout.Toggle("isAimable", isAimable.boolValue);
            if (isAimable.boolValue)
            {
                aimer.objectReferenceValue = EditorGUILayout.ObjectField("aimer", aimer.objectReferenceValue, typeof(RectTransform), true) as RectTransform;
                pointer.objectReferenceValue = EditorGUILayout.ObjectField("pointer", pointer.objectReferenceValue, typeof(RectTransform), true) as RectTransform;
                skillCanceller.objectReferenceValue = EditorGUILayout.ObjectField("skillCanceller", skillCanceller.objectReferenceValue, typeof(RectTransform), true) as RectTransform;
            }




            EditorGUILayout.TextArea("-----[ Parameters ]---------------", GUIStyle.none);
            isActive.boolValue = EditorGUILayout.Toggle("isActive", isActive.boolValue);
            isFingerDown.boolValue = EditorGUILayout.Toggle("isFingerDown", isFingerDown.boolValue);
            fingerId.intValue = EditorGUILayout.IntField("fingerId", fingerId.intValue);

            if (isAimable.boolValue)
            {
                isManualAimOverride.boolValue = EditorGUILayout.Toggle("isManualAimOverride", isManualAimOverride.boolValue);
                direction.vector3Value = EditorGUILayout.Vector3Field("direction", direction.vector3Value);

            }
            */
            //state.enumValueIndex = (int)(UniversalButton.ButtonState)EditorGUILayout.EnumPopup("state", state.);

            EditorGUILayout.TextArea("-----[ Config ]---------------", GUIStyle.none);
            EditorGUILayout.PropertyField(isAimable);
            if (isAimable.boolValue)
            {
                EditorGUILayout.PropertyField(aimer);
                EditorGUILayout.PropertyField(pointer);
                EditorGUILayout.PropertyField(skillCanceller);
            }
            EditorGUILayout.PropertyField(text);
            EditorGUILayout.PropertyField(btnIndex);

            EditorGUILayout.TextArea("-----[ Parameters ]---------------", GUIStyle.none);
            EditorGUILayout.PropertyField(isActive);
            EditorGUILayout.PropertyField(isFingerDown);
            EditorGUILayout.PropertyField(fingerId);

            if (isAimable.boolValue)
            {
                EditorGUILayout.PropertyField(isManualAimOverride);
                EditorGUILayout.PropertyField(direction);
            }
            EditorGUILayout.PropertyField(state);

            //EditorGUILayout.PropertyField(colorActive);
            EditorGUILayout.PropertyField(colorInactive);
            EditorGUILayout.PropertyField(colorPressed);

            EditorGUILayout.TextArea("-----[ Events ]---------------", GUIStyle.none);
            showGameLogicEvents = EditorGUILayout.Toggle("showGameLogicEvents", showGameLogicEvents);
            showRawButtonEvents = EditorGUILayout.Toggle("showRawButtonEvents", showRawButtonEvents);

            if (showGameLogicEvents)
            {
                EditorGUILayout.PropertyField(onActivateSkill);
                EditorGUILayout.PropertyField(onCancelSkill);
            }


            if (showRawButtonEvents)
            {
                EditorGUILayout.PropertyField(onPointerDown);
                EditorGUILayout.PropertyField(onBeginDrag);
                EditorGUILayout.PropertyField(onDrag);
                EditorGUILayout.PropertyField(onPointerUp);
                EditorGUILayout.PropertyField(onEndDrag);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}