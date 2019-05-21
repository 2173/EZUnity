/* Author:          ezhex1991@outlook.com
 * CreateTime:      2019-04-22 19:09:42
 * Organization:    #ORGANIZATION#
 * Description:     
 */
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZUnity
{
    public class EZColorBlenderWindow : EditorWindow
    {
        public bool useHDR;

        public Color srcColor = Color.white;
        public Color dstColor = Color.grey;

        public Dictionary<string, Color> colors = new Dictionary<string, Color>();
        public Dictionary<string, string> colorStrings = new Dictionary<string, string>();

        public SerializedObject serializedObject { get; private set; }
        public VisualTreeAsset uiTemplate;
        public StyleSheet styleSheet;
        public Dictionary<string, ColorField> colorFields = new Dictionary<string, ColorField>();
        public Dictionary<string, TextField> colorStringFields = new Dictionary<string, TextField>();

        [MenuItem("EZUnity/Experimental/Color Blender", false)]
        private static void CreateWindow()
        {
            GetWindow<EZColorBlenderWindow>("EZ Color Blender").Show();
        }

        private void OnEnable()
        {
            CalculateAll();
            serializedObject = new SerializedObject(this);
            InitElements();
        }
        private void OnDisable()
        {
            serializedObject.Dispose();
        }

        private void InitElements()
        {
            VisualElement visualTree = uiTemplate.CloneTree();
            visualTree.styleSheets.Add(styleSheet);

            ObjectField scriptTitleField = visualTree.Q<ObjectField>(name: "ScriptTitleField");
            scriptTitleField.value = MonoScript.FromScriptableObject(this);
            scriptTitleField.label = "Script";

            VisualElement srcInput = visualTree.Q(name: "SrcColor");
            ColorField sourceColorField = srcInput.Q<ColorField>(name: "ColorField");
            sourceColorField.BindProperty(serializedObject.FindProperty("srcColor"));
            sourceColorField.RegisterValueChangedCallback((e) =>
            {
                colors["SrcColor"] = e.newValue;
                CalculateAll();
                Refresh();
            });

            VisualElement dstInput = visualTree.Q(name: "DstColor");
            ColorField destinationColorField = dstInput.Q<ColorField>(name: "ColorField");
            destinationColorField.BindProperty(serializedObject.FindProperty("dstColor"));
            destinationColorField.RegisterValueChangedCallback((e) =>
            {
                colors["DstColor"] = e.newValue;
                CalculateAll();
                Refresh();
            });

            visualTree.Query(classes: new string[] { "color-info" })
                .ForEach((element) =>
                {
                    string colorName = element.parent.name;
                    element.Q<Label>(name = "ColorName").text = colorName;
                    colorFields[colorName] = element.Q<ColorField>(name = "ColorField");
                    colorStringFields[colorName] = element.Q<TextField>(name = "ColorString");
                    colorFields[colorName].value = colors[colorName];
                    colorStringFields[colorName].value = colorStrings[colorName];
                });

            rootVisualElement.Clear();
            rootVisualElement.Add(visualTree);
        }
        private void Refresh()
        {
            foreach (var field in colorFields)
            {
                string colorName = field.Key;
                colorFields[colorName].value = colors[colorName];
                colorStringFields[colorName].value = colorStrings[colorName];
            }
        }

        public void CalculateAll()
        {
            SetColor("SrcColor", srcColor);
            SetColor("DstColor", dstColor);
            SetColor("Add", Add(srcColor, dstColor));
            SetColor("Minus", Minus(srcColor, dstColor));
            SetColor("Multiply", Multiply(srcColor, dstColor));
            SetColor("Divide", Divide(srcColor, dstColor));
            SetColor("Opacity", Opacity(srcColor, dstColor));
            SetColor("Darken", Darken(srcColor, dstColor));
            SetColor("Lighten", Lighten(srcColor, dstColor));
            SetColor("Screen", Screen(srcColor, dstColor));
            SetColor("ColorBurn", ColorBurn(srcColor, dstColor));
            SetColor("ColorDodge", ColorDodge(srcColor, dstColor));
            SetColor("LinearBurn", LinearBurn(srcColor, dstColor));
            SetColor("LinearDodge", LinearDodge(srcColor, dstColor));
            SetColor("Overlay", Overlay(srcColor, dstColor));
            SetColor("HardLight", HardLight(srcColor, dstColor));
            SetColor("SoftLight", SoftLight(srcColor, dstColor));
            SetColor("VividLight", VividLight(srcColor, dstColor));
            SetColor("LinearLight", LinearLight(srcColor, dstColor));
            SetColor("PinLight", PinLight(srcColor, dstColor));
            SetColor("HardMix", HardMix(srcColor, dstColor));
            SetColor("Difference", Difference(srcColor, dstColor));
            SetColor("Exclusion", Exclusion(srcColor, dstColor));
            Vector3 srcHSV;
            Vector3 dstHSV;
            Color.RGBToHSV(srcColor, out srcHSV.x, out srcHSV.y, out srcHSV.z);
            Color.RGBToHSV(dstColor, out dstHSV.x, out dstHSV.y, out dstHSV.z);
            SetColor("HSVBlend_H", HSVBlend_H(srcHSV, dstHSV));
            SetColor("HSVBlend_S", HSVBlend_S(srcHSV, dstHSV));
            SetColor("HSVBlend_V", HSVBlend_V(srcHSV, dstHSV));
            SetColor("HSVBlend_HS", HSVBlend_HS(srcHSV, dstHSV));
        }
        public void SetColor(string name, Color color)
        {
            colors[name] = color;
            colorStrings[name] = ColorUtility.ToHtmlStringRGBA(color);
        }

        public static Color CalcPerComponent(Color src, Color dst, Func<float, float, float> func)
        {
            return new Color
            (
                func(src.r, dst.r),
                func(src.g, dst.g),
                func(src.b, dst.b),
                func(src.a, dst.a)
            );
        }

        public static Color Inverse(Color src)
        {
            return new Color(1 - src.r, 1 - src.g, 1 - src.b, 1 - src.a);
        }

        public static Color Add(Color src, Color dst)
        {
            return src + dst;
        }
        public static Color Minus(Color src, Color dst)
        {
            return src - dst;
        }
        public static Color Multiply(Color src, Color dst)
        {
            return src * dst;
        }
        public static Color Divide(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) => a / b);
        }
        public static Color Min(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) => Mathf.Min(a, b));
        }
        public static Color Max(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) => Mathf.Max(a, b));
        }

        // 不透明度
        public static Color Opacity(Color src, Color dst)
        {
            return src.a * src + (1 - src.a) * dst;
        }
        // 变暗
        public static Color Darken(Color src, Color dst)
        {
            return Min(src, dst);
        }
        // 变亮
        public static Color Lighten(Color src, Color dst)
        {
            return Max(src, dst);
        }
        // 滤色
        public static Color Screen(Color src, Color dst)
        {
            return Inverse(Inverse(src) * Inverse(dst));
        }
        // 颜色加深
        public static Color ColorBurn(Color src, Color dst)
        {
            return src - Divide(Inverse(src) * Inverse(dst), dst);
        }
        // 颜色减淡
        public static Color ColorDodge(Color src, Color dst)
        {
            return src + Divide(src * dst, Inverse(dst));
        }
        // 线性加深
        public static Color LinearBurn(Color src, Color dst)
        {
            return src + dst - Color.white;
        }
        // 线性减淡
        public static Color LinearDodge(Color src, Color dst)
        {
            return src + dst;
        }
        // 叠加
        public static Color Overlay(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) =>
            {
                if (a <= 0.5f) return a * b;
                else return 1 - (1 - a) * (1 - b) * 2;
            });
        }
        // 强光
        public static Color HardLight(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) =>
            {
                if (b <= 0.5f) return a * b * 2;
                else return 1 - (1 - a) * (1 - b) * 2;
            });
        }
        // 柔光
        public static Color SoftLight(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) =>
            {
                if (b <= 0.5f) return a * b * 2 + a * a * (1 - 2 * b);
                else return a * (1 - b) * 2 + Mathf.Sqrt(a * (2 * b - 1));
            });
        }
        // 亮光
        public static Color VividLight(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) =>
            {
                if (b <= 0.5f) return a - (1 - a) * (1 - 2 * b) / (2 * b);
                else return a + a * (2 * b - 1) / (2 * (1 - b));
            });
        }
        // 线性光
        public static Color LinearLight(Color src, Color dst)
        {
            return src - 2 * dst - Color.white;
        }
        // 点光
        public static Color PinLight(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) =>
            {
                if (b <= 0.5f) return Mathf.Min(a, 2 * b);
                else return Mathf.Min(a, 2 * b - 1);
            });
        }
        // 实色混合
        public static Color HardMix(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) =>
            {
                return a < 1 - b ? 0 : 1;
            });
        }
        // 差值
        public static Color Difference(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) =>
            {
                return Mathf.Abs(a - b);
            });
        }
        // 排除
        public static Color Exclusion(Color src, Color dst)
        {
            return CalcPerComponent(src, dst, (a, b) =>
            {
                return a + b - a * b * 2;
            });
        }

        public static Color HSVBlend_H(Vector3 srcHSV, Vector3 dstHSV)
        {
            return Color.HSVToRGB(dstHSV.x, srcHSV.y, srcHSV.z);
        }
        public static Color HSVBlend_S(Vector3 srcHSV, Vector3 dstHSV)
        {
            return Color.HSVToRGB(srcHSV.x, dstHSV.y, srcHSV.z);
        }
        public static Color HSVBlend_V(Vector3 srcHSV, Vector3 dstHSV)
        {
            return Color.HSVToRGB(srcHSV.x, srcHSV.y, dstHSV.z);
        }
        public static Color HSVBlend_HS(Vector3 srcHSV, Vector3 dstHSV)
        {
            return Color.HSVToRGB(dstHSV.x, dstHSV.y, srcHSV.z);
        }
    }
}
