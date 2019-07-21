using AniLipSync.VRM;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;
using VRM;

public class ControllerInputManager : MonoBehaviour
{
    public VRMBlendShapeProxy blendShapeProxy;
    public Blinker blinker;
    public AnimMorphTarget animMorphTarget;

    private SteamVR_ActionSet[] actionSets;

    // VRMの表情
    private Dictionary<BlendShapeKey, float> shapeValueDictionary = new Dictionary<BlendShapeKey, float> {
            {new BlendShapeKey(BlendShapePreset.Neutral), 0.0f }, // NEUTRAL(標準)
            {new BlendShapeKey(BlendShapePreset.Blink), 0.0f }, // BLINK(目を閉じる、まばたき)
            {new BlendShapeKey(BlendShapePreset.Joy), 0.0f }, // JOY(喜び)
            {new BlendShapeKey(BlendShapePreset.Angry), 0.0f }, // ANGRY(怒り)
            {new BlendShapeKey(BlendShapePreset.Sorrow), 0.0f }, // SORROW(悲しみ)
            {new BlendShapeKey(BlendShapePreset.Fun), 0.0f }, // FUN(楽しみ)
            {new BlendShapeKey(BlendShapePreset.Blink_L), 0.0f }, // BLINK_L(左目閉じる)
            {new BlendShapeKey(BlendShapePreset.Blink_R), 0.0f }, // BLINK_R(右目閉じる)
            {new BlendShapeKey("><"), 0.0f }, // ><(アリシアちゃん専用表情)
        };
    private List<BlendShapeKey> keyList;

    private int GetKeyIndex(BlendShapePreset preset) { return keyList.FindIndex(d => d.Name == preset.ToString().ToUpper()); }
    private int GetKeyIndex(string preset) { return keyList.FindIndex(d => d.Name == preset.ToUpper()); }
    private BlendShapeKey GetKey(BlendShapePreset preset) { return keyList[GetKeyIndex(preset)]; }
    private BlendShapeKey GetKey(string preset) { return keyList[GetKeyIndex(preset)]; }

    // Use this for initialization
    void Start()
    {
        keyList = new List<BlendShapeKey>(shapeValueDictionary.Keys);
        actionSets = SteamVR_Input.actionSets;
        if (actionSets == null) { actionSets = SteamVR_Input_References.instance.actionSetObjects; }
    }

    // Update is called once per frame
    void Update()
    {
        if (blendShapeProxy == null) return;

        var sources = SteamVR_Input_Source.GetUpdateSources();
        foreach (var source in sources)
        {
            if (source == SteamVR_Input_Sources.Any) continue;
            foreach (var actionSet in actionSets)
            {
                foreach (var action in actionSet.allActions) //ボタン
                {
                    if (action is SteamVR_Action_Boolean)
                    {
                        var actionBoolean = (SteamVR_Action_Boolean)action;
                        if (actionBoolean.GetStateDown(source))
                        {
                            var name = actionBoolean.GetShortName();
                            if (name == "InteractUI") // トリガー半引き
                            {

                            }
                            else if (name == "GrabPinch") // トリガー全引き
                            {

                            }
                            else if (name == "Teleport") // タッチパッド押し
                            {

                            }
                            else if (name == "GrabGrip") // グリップボタン押し
                            {

                            }
                        }
                    }
                    else if (action is SteamVR_Action_Single) //Axis
                    {
                        var actionSingle = (SteamVR_Action_Single)action;
                        if (actionSingle.GetChanged(source))
                        {
                            var name = actionSingle.GetShortName();
                            var axis = actionSingle.GetAxis(source);
                            if (name == "Squeeze") // トリガー
                            {

                            }
                        }
                    }
                    else if (action is SteamVR_Action_Vector2) // Padやスティック
                    {
                        var actionVector2 = (SteamVR_Action_Vector2)action;
                        if (actionVector2.GetChanged(source))
                        {
                            var name = actionVector2.GetShortName();
                            var axis = actionVector2.GetAxis(source);
                            //
                            // タッチパッドの座標(両手とも)
                            //        (0, 1)
                            // (-1, 0)(0, 0)(1, 0)
                            //        (0,-1)
                            //
                            // タッチパッドを離した時は(0,0)が飛んでくる
                            //
                            if (name == "TouchPad")
                            {
                                var isLeft = source == SteamVR_Input_Sources.LeftHand;

                                //全ての表情を一旦無効にする
                                foreach (var key in keyList)
                                {
                                    shapeValueDictionary[key] = 0.0f;
                                }

                                if (Mathf.Approximately(axis.x, 0.0f) && Mathf.Approximately(axis.y, 0.0f)) //中心(離した時)
                                {
                                    //まばたきとリップシンクを復活させる
                                    blinker.enabled = true;
                                    animMorphTarget.curveAmplifier = 100f;
                                    shapeValueDictionary[GetKey(BlendShapePreset.Neutral)] = 1.0f;
                                }
                                else
                                {
                                    //表情とぶつからないようにまばたきを止めてリップシンクを弱くする
                                    blinker.enabled = false;
                                    animMorphTarget.curveAmplifier = 0.1f;
                                    if (axis.x < 0) // 左
                                    {
                                        if (axis.y > 0) // 左上
                                        {
                                            shapeValueDictionary[GetKey(isLeft ? BlendShapePreset.Blink_L : BlendShapePreset.Blink)] = 1.0f;
                                        }
                                        else if (axis.y < 0) // 左下
                                        {
                                            shapeValueDictionary[GetKey(isLeft ? BlendShapePreset.Joy : BlendShapePreset.Angry)] = 1.0f;
                                        }
                                    }
                                    else if (axis.x > 0) // 右
                                    {
                                        if (axis.y > 0) // 右上
                                        {
                                            shapeValueDictionary[isLeft ? GetKey("><") : GetKey(BlendShapePreset.Blink_R)] = 1.0f;
                                        }
                                        else if (axis.y < 0) // 右下
                                        {
                                            shapeValueDictionary[GetKey(isLeft ? BlendShapePreset.Sorrow : BlendShapePreset.Fun)] = 1.0f;
                                        }
                                    }

                                }
                                //表情を適用する
                                blendShapeProxy.SetValues(shapeValueDictionary.ToList());
                            }
                        }
                    }
                }
            }
        }
    }
}