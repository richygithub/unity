using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


class Sampler
{
    public string name;
    //public 
    int count;
    float frameRate;
    float length;
    float dt;

    Vector3[] pos = null;
    Quaternion[] quat =null;

    public Sampler( string name,float frameRate,float length)
    {
        this.name = name;
        count = Mathf.RoundToInt(frameRate * length)+1;
        this.frameRate= frameRate;
        this.length = length;
        this.dt = 1 / frameRate;
        pos = new Vector3[count];
        quat = new Quaternion[count];
           

    }

    void calcCurve(AnimationCurve curve)
    {
        int count = Mathf.RoundToInt(frameRate * length) + 1;
        float dt = 1 / frameRate;

        float[] v = new float[count];

        for (int idx = 0; idx < count; idx++)
        {
            v[idx] = curve.Evaluate(dt * idx);
        }

        return;


    }

    public void addCurve(string name, AnimationCurve curve)
    {
        
        bool propPos = name.IndexOf("m_LocalPosition")>= 0;
        char axis = name[name.Length - 1];
        if( propPos )
        {
            switch (axis)
            {
                case 'x':
                    for (int idx = 0; idx < count; idx++)
                    {
                        pos[idx].x = curve.Evaluate(dt * idx);
                    }
                    break;
                case 'y':
                    for (int idx = 0; idx < count; idx++)
                    {
                        pos[idx].y = curve.Evaluate(dt * idx);
                    }
                    break;
                case 'z':
                    for (int idx = 0; idx < count; idx++)
                    {
                        pos[idx].z = curve.Evaluate(dt * idx);
                    }
                    break;
                default:
                    Debug.LogError("unregnoize property:"+name);
                    break;
            }
            return;
        }
        bool propRotation = name.IndexOf("m_LocalRotation")>= 0;
        if(propRotation)
        {
            switch (axis)
            {
                case 'x':
                    for (int idx = 0; idx < count; idx++)
                    {
                        quat[idx].x = curve.Evaluate(dt * idx);
                    }
                    break;
                case 'y':
                    for (int idx = 0; idx < count; idx++)
                    {
                        quat[idx].y = curve.Evaluate(dt * idx);
                    }
                    break;
                case 'z':
                    for (int idx = 0; idx < count; idx++)
                    {
                        quat[idx].z = curve.Evaluate(dt * idx);
                    }
                    break;
                case 'w':
                    for (int idx = 0; idx < count; idx++)
                    {
                        quat[idx].w = curve.Evaluate(dt * idx);
                    }
                    break;
                default:
                    Debug.LogError("unregnoize property:"+name);
                    break;
            }
 
            return;
        }
    }

}

[CustomEditor(typeof(AnimInfo))]
[CanEditMultipleObjects]
public class AnimEditor : Editor
{

    AnimInfo anim;
    Vector2 pos = Vector2.zero;//new Vector2(100,100); 
    Vector2 pos0 = Vector2.zero;

    Dictionary<string, Sampler> animMap= new Dictionary<string, Sampler>();

    void calcCurve(AnimationCurve curve,float frameRate,float length)
    {
        int count = Mathf.RoundToInt(frameRate * length)+1;
        float dt = 1 / frameRate;

        float[] v = new float[count];

        for(int idx = 0;  idx < count;idx++)
        {
            v[idx] = curve.Evaluate(dt*idx);
        }

        return;


    }

    public override void OnInspectorGUI()
    {
        anim = (AnimInfo)target;
        base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(anim);
        //EditorGUILayout.LabelField("abcd");
        if(anim.clip != null)
        {
            EditorGUILayout.LabelField("frameRate:"+anim.clip.frameRate+" ,length:"+anim.clip.length);
            pos0 = EditorGUILayout.BeginScrollView(pos0,GUILayout.Height(200));
 
            foreach( var binding in AnimationUtility.GetCurveBindings(anim.clip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(anim.clip, binding);
                string s = string.Format("{0}({1}):{2}, KeyLen:{3}",binding.path,binding.GetHashCode(), binding.propertyName,  curve.keys.Length);
                if (!animMap.ContainsKey(binding.path))
                {
                    animMap.Add(binding.path, new Sampler(binding.path,anim.clip.frameRate,anim.clip.length));
                }
                animMap[binding.path].addCurve(binding.propertyName, curve);

                /*
                if(binding.propertyName == "m_LocalPosition.x")
                {
                    calcCurve(curve, anim.clip.frameRate, anim.clip.length);
                }
                */

                //EditorGUILayout.LabelField( binding.propertyName + ", Keys: " + curve.keys.Length);
                EditorGUILayout.LabelField(s);


            }
            EditorGUILayout.EndScrollView();

         
        }

        EditorGUILayout.Separator();

        if(anim.obj != null)
        {
            var smr = anim.obj.GetComponentInChildren<SkinnedMeshRenderer>();
            var bones = smr.bones;
            var root = smr.rootBone;

            pos = EditorGUILayout.BeginScrollView(pos);
            for(int idx = 0; idx < bones.Length; idx++)
            {
                string s = string.Format("{0}->({7}).pos[{1},{2},{3}], localPos[{4},{5},{6}]",bones[idx].name,
                    bones[idx].position.x,bones[idx].position.y,bones[idx].position.z,
                    bones[idx].localPosition.x,bones[idx].localPosition.y,bones[idx].localPosition.z,bones[idx].name.GetHashCode());

                EditorGUILayout.LabelField(s);
            }

            EditorGUILayout.EndScrollView();

        }
    }
}