using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace WatermillTweaks
{
    // REFERENCE: https://github.com/erdelf/GodsOfRimworld/blob/master/Source/Ankh/ModControl.cs
    // REFERENCE: https://github.com/erdelf/PrisonerRansom/
    public static class ListingStandardHelper
    {
        private static float gap = 12f;
        private static float lineGap = 3f;

        public static float Gap { get => gap; set => gap = value; }
        public static float LineGap { get => lineGap; set => lineGap = value; }

        public static void AddHorizontalLine(this Listing_Standard listing_Standard, float? gap = null)
        {
            listing_Standard.Gap(gap ?? lineGap);
            listing_Standard.GapLine(gap ?? lineGap);
        }

        public static void AddLabelLine(this Listing_Standard listing_Standard, string label, float? height = null)
        {
            listing_Standard.Gap(Gap);
            Rect lineRect = listing_Standard.GetRect(height);

            // TODO: tooltips
            //Widgets.DrawHighlightIfMouseover(lineRect);
            //TooltipHandler.TipRegion(lineRect, "TODO: TIP GOES HERE");

            Widgets.Label(lineRect, label);
        }

        public static Rect GetRect(this Listing_Standard listing_Standard, float? height = null)
        {
            return listing_Standard.GetRect(height ?? Text.LineHeight);
        }

        public static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, float leftPartPct = 0.5f, float? height = null)
        {
            Rect lineRect = listing_Standard.GetRect(height);
            leftHalf = lineRect.LeftPart(leftPartPct).Rounded();
            return lineRect;
        }

        public static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, out Rect rightHalf, float leftPartPct = 0.5f, float? height = null)
        {
            Rect lineRect = listing_Standard.LineRectSpilter(out leftHalf, leftPartPct, height);
            rightHalf = lineRect.RightPart(1f - leftPartPct).Rounded();
            return lineRect;
        }

        // TODO: these could be simplified or better formalized...

        public static void AddLabeledRadioList(this Listing_Standard listing_Standard, string header, string[] labels, ref string val, float? headerHeight = null)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.AddLabelLine(header, headerHeight);
            listing_Standard.AddRadioList<string>(GenerateLabeledRadioValues(labels), ref val);
        }

        public static void AddLabeledRadioList<T>(this Listing_Standard listing_Standard, string header, Dictionary<string, T> dict, ref T val, float? headerHeight = null)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.AddLabelLine(header, headerHeight);
            listing_Standard.AddRadioList<T>(GenerateLabeledRadioValues<T>(dict), ref val);
        }

        private static void AddRadioList<T>(this Listing_Standard listing_Standard, List<LabeledRadioValue<T>> items, ref T val, float? height = null)
        {
            foreach (LabeledRadioValue<T> item in items)
            {
                listing_Standard.Gap(Gap);
                Rect lineRect = listing_Standard.GetRect(height);
                if (Widgets.RadioButtonLabeled(lineRect, item.Label, EqualityComparer<T>.Default.Equals(item.Value, val)))
                    val = item.Value;
            }
        }

        private static List<LabeledRadioValue<string>> GenerateLabeledRadioValues(string[] labels)
        {
            List<LabeledRadioValue<string>> list = new List<LabeledRadioValue<string>>();
            foreach (string label in labels)
            {
                list.Add(new LabeledRadioValue<string>(label, label));
            }
            return list;
        }

        // (label, value) => (key, value)
        private static List<LabeledRadioValue<T>> GenerateLabeledRadioValues<T>(Dictionary<string, T> dict)
        {
            List<LabeledRadioValue<T>> list = new List<LabeledRadioValue<T>>();
            foreach (KeyValuePair<string, T> entry in dict)
            {
                list.Add(new LabeledRadioValue<T>(entry.Key, entry.Value));
            }
            return list;
        }

        public class LabeledRadioValue<T>
        {
            private string label;
            private T val;

            public LabeledRadioValue(string label, T val)
            {
                Label = label;
                Value = val;
            }

            public string Label
            {
                get { return label; }
                set { label = value; }
            }

            public T Value
            {
                get { return val; }
                set { val = value; }
            }
        }

        public static void AddLabeledTextField(this Listing_Standard listing_Standard, string label, ref string settingsValue, float leftPartPct = 0.5f)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.LineRectSpilter(out Rect leftHalf, out Rect rightHalf, leftPartPct);

            // TODO: tooltips
            //Widgets.DrawHighlightIfMouseover(lineRect);
            //TooltipHandler.TipRegion(lineRect, "TODO: TIP GOES HERE");

            Widgets.Label(leftHalf, label);

            string buffer = settingsValue.ToString();
            settingsValue = Widgets.TextField(rightHalf, buffer);
        }

        public static void AddLabeledNumericalTextField<T>(this Listing_Standard listing_Standard, string label, ref T settingsValue, float leftPartPct = 0.5f, float minValue = 1f, float maxValue = 100000f) where T : struct
        {
            listing_Standard.Gap(Gap);
            listing_Standard.LineRectSpilter(out Rect leftHalf, out Rect rightHalf, leftPartPct);

            // TODO: tooltips
            //Widgets.DrawHighlightIfMouseover(lineRect);
            //TooltipHandler.TipRegion(lineRect, "TODO: TIP GOES HERE");

            Widgets.Label(leftHalf, label);

            string buffer = settingsValue.ToString();
            Widgets.TextFieldNumeric<T>(rightHalf, ref settingsValue, ref buffer, minValue, maxValue);
        }

        public static void AddLabeledCheckbox(this Listing_Standard listing_Standard, string label, ref bool settingsValue)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.CheckboxLabeled(label, ref settingsValue);
        }

        public static void AddLabeledSlider(this Listing_Standard listing_Standard, string label, ref float value, float leftValue, float rightValue)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.LineRectSpilter(out Rect leftHalf, out Rect rightHalf);

            Widgets.Label(leftHalf, label);

            float bufferVal = value;
            // NOTE: this BottomPart will probably need some reworking if the height of rect is greater than a line
            value = Widgets.HorizontalSlider(rightHalf.BottomPart(0.70f), bufferVal, leftValue, rightValue);
        }

        public static void SliderLabeled(this Listing_Standard ls, string label, ref int val, string format, float min = 0f, float max = 100f, string tooltip = null)
        {
            float fVal = val;
            ls.SliderLabeled(label, ref fVal, format, min, max);
            val = (int)fVal;
        }
        public static void SliderLabeled(this Listing_Standard ls, string label, ref float val, string format, float min = 0f, float max = 1f, string tooltip = null)
        {
            Rect rect = ls.GetRect(Text.LineHeight);
            Rect rect2 = rect.LeftPart(.70f).Rounded();
            Rect rect3 = rect.RightPart(.30f).Rounded().LeftPart(.67f).Rounded();
            Rect rect4 = rect.RightPart(.10f).Rounded();

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, label);

            float result = Widgets.HorizontalSlider(rect3, val, min, max, true);
            val = result;
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect4, String.Format(format, val));
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }

            Text.Anchor = anchor;
            ls.Gap(ls.verticalSpacing);
        }
    }
}