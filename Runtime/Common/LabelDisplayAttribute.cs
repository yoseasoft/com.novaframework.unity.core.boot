/// -------------------------------------------------------------------------------
/// CoreEngine Framework
///
/// Copyright (C) 2025, Hurley, Independent Studio.
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CoreEngine
{
    /// <summary>
    /// 字段名称标签的属性定义
    /// </summary>
    public sealed class FieldLabelNameAttribute : HeaderAttribute
    {
        public FieldLabelNameAttribute(string header) : base(header) { }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 字段名称标签的绘制对象类
    /// </summary>
    [CustomPropertyDrawer(typeof(FieldLabelNameAttribute))]
    public sealed class FieldLabelNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FieldLabelNameAttribute attr = attribute as FieldLabelNameAttribute;
            label.text = attr.header;

            System.Type targetType = property.serializedObject.targetObject.GetType();
            FieldInfo fieldInfo = targetType.GetField(property.name);
            TooltipAttribute tooltipAttr = fieldInfo.GetCustomAttribute(typeof(TooltipAttribute), false) as TooltipAttribute;
            if (null != tooltipAttr && false == string.IsNullOrEmpty(tooltipAttr.tooltip))
            {
                label.tooltip = tooltipAttr.tooltip;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }
    }
#endif

    /// <summary>
    /// 枚举名称标签的属性定义
    /// </summary>
    public sealed class EnumLabelNameAttribute : HeaderAttribute
    {
        public EnumLabelNameAttribute(string header) : base(header) { }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 枚举名称标签的绘制对象类
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumLabelNameAttribute))]
    public sealed class EnumLabelNameDrawer : PropertyDrawer
    {
        private readonly IList<string> _displayNames = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumLabelNameAttribute attr = attribute as EnumLabelNameAttribute;
            System.Type targetType = property.serializedObject.targetObject.GetType();
            FieldInfo fieldInfo = targetType.GetField(property.name);
            System.Type enumType = fieldInfo.FieldType;
            foreach (string enumName in property.enumNames)
            {
                FieldInfo enumFieldInfo = enumType.GetField(enumName);
                System.Attribute enumAttr = enumFieldInfo.GetCustomAttribute(typeof(HeaderAttribute), false);
                if (null == enumAttr)
                {
                    _displayNames.Add(enumName);
                }
                else
                {
                    _displayNames.Add((enumAttr as HeaderAttribute)?.header);
                }
            }

            EditorGUI.BeginChangeCheck();
            int value = EditorGUI.Popup(position, attr.header, property.enumValueIndex, _displayNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = value;
            }
        }
    }
#endif
}
