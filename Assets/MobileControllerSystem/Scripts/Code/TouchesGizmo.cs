using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileUISystem
{
    /// <summary>
    /// 触摸屏 触点指示器
    /// </summary>
    public class TouchesGizmo : MonoBehaviour
    {
        public RectTransform indicatorPrefabs;
        public List<RectTransform> indies;
        protected RectTransform rect;
        private Canvas canvas;
        private Camera cam;
        protected void Start()
        {
            rect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;
        }

        protected void Update()
        {
            while (indies.Count < Input.touchCount)
            {
                indies.Add(Instantiate(indicatorPrefabs) as RectTransform);
                indies[indies.Count - 1].SetParent(rect, false);
            }
            while (indies.Count > Input.touchCount)
            {
                Destroy(indies[0].gameObject);
                indies.RemoveAt(0);
            }

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    indies[i].anchoredPosition3D = ScreenPointToAnchoredPosition(Input.touches[i].position);
                }
            }
            else
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    indies[i].position = Input.touches[i].position;
                }
            }
        }

        protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            Vector2 localPoint = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPosition, cam, out localPoint))
            {
                return localPoint;
            }
            return Vector2.zero;
        }
    }
}
