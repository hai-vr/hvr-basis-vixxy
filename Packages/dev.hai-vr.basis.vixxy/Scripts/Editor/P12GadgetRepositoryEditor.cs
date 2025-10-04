using HVR.Basis.Vixxy.Runtime;
using UnityEditor;
using UnityEngine;

namespace HVR.Basis.Vixxy.Editor
{
    [CustomEditor(typeof(P12GadgetRepository))]
    public class P12GadgetRepositoryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var my = (P12GadgetRepository)target;
            foreach (var gadget in my.GadgetView())
            {
                if (gadget is P12SettableFloatElement floatElement)
                {
                    var slider = EditorGUILayout.Slider(new GUIContent(floatElement.localizedTitle), floatElement.storedValue, floatElement.min, floatElement.max);
                    if (slider != floatElement.storedValue)
                    {
                        floatElement.storedValue = slider;
                    }
                }
            }
        }
    }
}