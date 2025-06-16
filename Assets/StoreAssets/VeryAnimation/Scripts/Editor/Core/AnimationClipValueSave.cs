using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace VeryAnimation
{
    internal class AnimationClipValueSave
    {
        private readonly GameObject rootObject;
        //private readonly AnimationClip clip;

        private readonly EditorCurveBinding[] bindings;
        private readonly float?[] floatValues;

        private readonly EditorCurveBinding[] refBindings;
        private readonly UnityEngine.Object[] refValues;

        public AnimationClipValueSave(GameObject gameObject, AnimationClip clip)
        {
            this.rootObject = gameObject;
            //this.clip = clip;

            bindings = AnimationUtility.GetCurveBindings(clip);
            floatValues = new float?[bindings.Length];
            for (int i = 0; i < bindings.Length; i++)
            {
                if (AnimationUtility.GetFloatValue(rootObject, bindings[i], out float floatValue))
                {
                    floatValues[i] = floatValue;
                }
            }

            refBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            refValues = new UnityEngine.Object[refBindings.Length];
            for (int i = 0; i < refBindings.Length; i++)
            {
                if (AnimationUtility.GetObjectReferenceValue(rootObject, refBindings[i], out UnityEngine.Object refValue))
                {
                    refValues[i] = refValue;
                }
            }
        }

        public void ResetValue()
        {
            if (rootObject == null)
                return;

            if (bindings != null)
            {
                for (int i = 0; i < bindings.Length; i++)
                {
                    if (!floatValues[i].HasValue)
                        continue;

                    var t = rootObject.transform.Find(bindings[i].path);
                    if (t == null)
                        continue;

                    Component comp;
                    try
                    {
                        comp = t.GetComponent(bindings[i].type);
                    }
                    catch
                    {
                        continue;
                    }
                    if (comp == null)
                        continue;

                    var so = new SerializedObject(comp);
                    var sp = so.FindProperty(bindings[i].propertyName);
                    if (sp == null)
                        continue;

                    var type = AnimationUtility.GetEditorCurveValueType(rootObject, bindings[i]);
                    if (type == typeof(float))
                    {
                        sp.floatValue = floatValues[i].Value;
                    }
                    else if (type == typeof(int))
                    {
                        sp.intValue = (int)floatValues[i].Value;
                    }
                    else if (type == typeof(bool))
                    {
                        sp.boolValue = floatValues[i].Value != 0f;
                    }
                    else
                    {
                        Assert.IsTrue(false);
                        continue;
                    }

                    so.ApplyModifiedProperties();
                }
            }

            if (refBindings != null)
            {
                for (int i = 0; i < refBindings.Length; i++)
                {
                    var t = rootObject.transform.Find(refBindings[i].path);
                    if (t == null)
                        continue;

                    Component comp;
                    try
                    {
                        comp = t.GetComponent(refBindings[i].type);
                    }
                    catch
                    {
                        continue;
                    }
                    if (comp == null)
                        continue;

                    var so = new SerializedObject(comp);
                    var sp = so.FindProperty(refBindings[i].propertyName);
                    if (sp == null)
                        continue;

                    sp.objectReferenceValue = refValues[i];

                    so.ApplyModifiedProperties();
                }
            }
        }
    }
}
