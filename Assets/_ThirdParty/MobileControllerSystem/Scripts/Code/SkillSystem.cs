// -----------------------------------------------
// Copyright © HeNuo. All rights reserved.
// CreateTime: 2020/8/22   9:36:45
// -----------------------------------------------

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace MobileUISystem
{
    public class SkillSystem : MonoBehaviour
    {
        public MessageBox msg;
        public SkillCanceller skillCanceller;
        public UniversalButton[] skillButtons;
        public SkillSetting[] skillSettings;
        public float[] cooldowns;

        protected virtual void Start()
        {
            cooldowns = new float[3];

            for (int i = 0; i < skillButtons.Length; i++)
            {
                skillButtons[i].SetActiveState(true);
                skillButtons[i].SetText("");
                skillButtons[i].onPointerDown.AddListener(OnSkillButtonPressed);
                skillButtons[i].onDrag.AddListener(OnSkillButtonDragged);
                skillButtons[i].onActivateSkill.AddListener(OnActivateSkill);
                skillButtons[i].onCancelSkill.AddListener(OnCancelSkill);

                skillSettings[i].skillMarker.transform.position = this.transform.position;
                skillSettings[i].skillMarker.SetActive(false);

                cooldowns[i] = 0f;
            }
        }

        protected virtual void Update()
        {

            msg.UpdatePosition(transform.position);

            if (skillCanceller.isAnyFingerDown)
            {
                for (int i = 0; i < skillButtons.Length; i++)
                {
                    skillSettings[i].skillMarker.transform.position = GetSkillMarkerPosition(i);
                }
            }

            this.UpdateCooldown();
        }

        protected virtual void UpdateCooldown()
        {
            for (int i = 0; i < cooldowns.Length; i++)
            {
                if (cooldowns[i] > 0f)
                {
                    cooldowns[i] -= Time.deltaTime;
                    if (cooldowns[i] < 1f)
                    {
                        skillButtons[i].SetText(cooldowns[i].ToString("F1"));
                    }
                    else
                    {
                        skillButtons[i].SetText("" + (int)cooldowns[i]);
                    }


                    if (skillButtons[i].state == UniversalButton.ButtonState.Active)
                    {
                        skillButtons[i].SetActiveState(false);
                    }
                }
                else
                {
                    if (skillButtons[i].state == UniversalButton.ButtonState.Inactive)
                    {
                        skillButtons[i].SetText("");
                        skillButtons[i].SetActiveState(true);
                    }
                }
            }
        }

        /// <summary>
        /// 当技能按钮按下
        /// </summary>
        /// <param name="i"></param>
        protected virtual void OnSkillButtonPressed(int i)
        {
            skillSettings[i].skillMarker.SetActive(true);
            this.UpdateSkillMarkersState(i);
        }
        /// <summary>
        /// 当技能按钮拖动
        /// </summary>
        /// <param name="i"></param>
        protected virtual void OnSkillButtonDragged(int i)
        {
            this.UpdateSkillMarkersState(i);
        }
        /// <summary>
        /// 更新技能指示器状态
        /// </summary>
        /// <param name="i"></param>
        protected virtual void UpdateSkillMarkersState(int i)
        {
            if (skillCanceller.state == UniversalButton.ButtonState.Pressed)
            {
                skillSettings[i].SetMarkerCanCastSkill(false);
            }
            else
            {
                skillSettings[i].SetMarkerCanCastSkill(true);
            }
        }
        /// <summary>
        /// 当技能释放
        /// </summary>
        /// <param name="i"></param>
        protected virtual void OnActivateSkill(int i)
        {
            skillSettings[i].SpawnSkillAt(skillSettings[i].skillMarker.transform.position);
            skillSettings[i].skillMarker.SetActive(false);
            skillSettings[i].skillMarker.transform.position = this.transform.position;
            cooldowns[i] = skillSettings[i].cooldown;
            this.skillButtons[i].directionXZ = Vector3.zero;

            msg.PopText("Activated skill " + i);
        }
        /// <summary>
        /// 当取消技能
        /// </summary>
        /// <param name="i"></param>
        protected virtual void OnCancelSkill(int i)
        {
            skillSettings[i].skillMarker.SetActive(false);
            skillSettings[i].skillMarker.transform.position = this.transform.position;
            this.skillButtons[i].directionXZ = Vector3.zero;

            msg.PopText("Canceled skill " + i);
        }

        protected Vector3 GetSkillMarkerPosition(int i)
        {
            return this.transform.position + skillButtons[i].directionXZ * skillSettings[i].range;
        }
    }


    [Serializable]
    public class SkillSetting
    {
        public int skillId;
        public float range;
        public float cooldown;
        public GameObject skillPrefab;
        public GameObject skillMarker;



        public Material markerActivateSkillTrue;
        public Material markerActivateSkillFalse;
        public float rotationSpeed;
        public float startingSize;
        public float sizeDecaySpeed;

        protected MeshRenderer renderer;

        protected SkillRotatingBox skill;

        public void SetMarkerCanCastSkill(bool can)
        {
            if (renderer == null)
            {
                renderer = skillMarker.GetComponent<MeshRenderer>();
            }

            if (can)
            {
                renderer.material = markerActivateSkillTrue;
            }
            else
            {
                renderer.material = markerActivateSkillFalse;
            }
        }

        public void SpawnSkillAt(Vector3 position)
        {
            skill = this.skillPrefab.GetComponent<SkillRotatingBox>();
            skill.size0 = this.startingSize;
            skill.sizeDecaySpeed = this.sizeDecaySpeed;

            UnityEngine.Object.Instantiate(skillPrefab, position, Quaternion.identity);
        }
    }
}