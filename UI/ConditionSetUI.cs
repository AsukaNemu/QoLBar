using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Dalamud.Interface;
using Dalamud.Logging;

namespace QoLBar
{
    public static class ConditionSetUI
    {
        private static int selectedSet = -1;
        private static bool editingName = false;
        private static bool focusNameInput = false;
        private static readonly List<string> binaryOperators = new()
        {
            "&&",
            " | | ",
            "==",
            " !="
        };

        private static CndSet CurrentSet => selectedSet >= 0 && selectedSet < QoLBar.Config.CndSets.Count ? QoLBar.Config.CndSets[selectedSet] : null;

        public static void Draw()
        {
            var currentSet = CurrentSet;
            var hasSelectedSet = currentSet != null;

            ImGui.PushFont(UiBuilder.IconFont);

            var buttonSize = ImGui.CalcTextSize(FontAwesomeIcon.SignOutAlt.ToIconString()) + ImGui.GetStyle().FramePadding * 2;

            if (ImGui.Button(FontAwesomeIcon.Plus.ToIconString(), buttonSize))
            {
                QoLBar.Config.CndSets.Add(new() { Name = "New Set" });
                QoLBar.Config.Save();
            }

            ImGui.SameLine();

            if (ImGui.Button(FontAwesomeIcon.SignOutAlt.ToIconString(), buttonSize) && hasSelectedSet)
                ;//ImGui.SetClipboardText(EXPORT);
            ImGui.PopFont();
            ImGuiEx.SetItemTooltip("Export condition set to clipboard.");
            ImGui.PushFont(UiBuilder.IconFont);

            ImGui.SameLine();

            if (ImGui.Button(FontAwesomeIcon.SignInAlt.ToIconString(), buttonSize))
            {
                try
                {
                    //var set = IMPORT(ImGui.GetClipboardText());
                    //QoLBar.Config.CndSets.Add(set);
                    //QoLBar.Config.Save();
                }
                catch (Exception e)
                {
                    QoLBar.PrintError($"Failed to import condition set from clipboard!\n{e.Message}");
                }
            }
            ImGui.PopFont();
            ImGuiEx.SetItemTooltip("Import condition set from clipboard.");
            ImGui.PushFont(UiBuilder.IconFont);

            ImGui.SameLine();

            if (ImGui.Button(FontAwesomeIcon.ArrowUp.ToIconString(), buttonSize) && hasSelectedSet)
            {
                QoLBar.Config.CndSets.RemoveAt(selectedSet);

                selectedSet = Math.Max(selectedSet - 1, 0);

                QoLBar.Config.CndSets.Insert(selectedSet, currentSet);
                QoLBar.Config.Save();
            }

            ImGui.SameLine();

            if (ImGui.Button(FontAwesomeIcon.ArrowDown.ToIconString(), buttonSize) && hasSelectedSet)
            {
                QoLBar.Config.CndSets.RemoveAt(selectedSet);

                selectedSet = Math.Min(selectedSet + 1, QoLBar.Config.CndSets.Count);

                QoLBar.Config.CndSets.Insert(selectedSet, currentSet);
                QoLBar.Config.Save();
            }

            ImGui.SameLine();

            ImGui.Button(FontAwesomeIcon.Times.ToIconString(), buttonSize);
            if (hasSelectedSet && ImGui.BeginPopupContextItem(null, ImGuiPopupFlags.MouseButtonLeft))
            {
                if (ImGui.Selectable(FontAwesomeIcon.TrashAlt.ToIconString()))
                {
                    QoLBar.Config.CndSets.RemoveAt(selectedSet);
                    selectedSet = Math.Min(selectedSet, QoLBar.Config.CndSets.Count - 1);
                    currentSet = CurrentSet;
                    hasSelectedSet = currentSet != null;
                    QoLBar.Config.Save();
                }
                ImGui.EndPopup();
            }

            ImGui.SameLine();

            ImGui.BeginGroup();
            ImGui.Dummy(Vector2.Zero);
            ImGui.SameLine();
            ImGui.TextUnformatted(FontAwesomeIcon.Info.ToIconString()); // someone please make it stop
            ImGui.SameLine();
            ImGui.Dummy(Vector2.Zero);
            ImGui.EndGroup();

            ImGui.PopFont();

            ImGuiEx.SetItemTooltip("Double click on a set to edit its name.");

            var listHeight = 250 * ImGuiHelpers.GlobalScale;
            var listWidth = (ImGui.GetWindowContentRegionWidth() - ImGui.GetStyle().ItemSpacing.X) / 2;

            ImGui.SameLine(listWidth);
            ImGui.TextUnformatted("\t\tPresets");

            ImGui.BeginChild("QoLBarConditionSetList", new Vector2(listWidth, listHeight), true);
            DrawConditionSetList();
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.BeginChild("QoLBarConditionSetPresetList", new Vector2(0, listHeight), true);
            ImGui.EndChild();

            if (!hasSelectedSet) return;

            ImGui.BeginChild("QoLBarConditionSetEditor", ImGui.GetContentRegionAvail(), true);
            DrawConditionSetEditor(currentSet);
            ImGui.EndChild();
        }

        public static void DrawConditionSetList()
        {
            for (int i = 0; i < QoLBar.Config.CndSets.Count; i++)
            {
                ImGui.PushID(i);

                var set = QoLBar.Config.CndSets[i];

                if (!editingName || selectedSet != i)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ConditionManager.CheckConditionSet(set) ? 0xFF00FF00u : 0xFF0000FFu);
                    if (ImGui.Selectable(set.Name, selectedSet == i))
                        selectedSet = i;
                    ImGui.PopStyleColor();

                    if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        editingName = true;
                        focusNameInput = true;
                    }
                }
                else
                {
                    ImGui.InputText("##SetName", ref set.Name, 64, ImGuiInputTextFlags.AutoSelectAll);

                    if (focusNameInput)
                    {
                        ImGui.SetKeyboardFocusHere();
                        focusNameInput = false;
                    }

                    if (ImGui.IsItemDeactivated())
                    {
                        editingName = false;
                        QoLBar.Config.Save();
                    }
                }

                ImGui.PopID();
            }
        }

        public static void DrawConditionSetEditor(CndSet set)
        {
            var debugSteps = ConditionManager.GetDebugSteps(set);
            var comboSize = ImGui.CalcTextSize("AND").X;

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, ImGui.GetStyle().ItemSpacing.Y));

            if (set.Conditions.Count == 0)
            {
                ImGui.PushFont(UiBuilder.IconFont);

                if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##Set", new Vector2(comboSize, 0)))
                {
                    set.Conditions.Add(new() { ID = "cf" });
                    QoLBar.Config.Save();
                }

                ImGui.PopFont();
                ImGui.PopStyleVar();

                return;
            }

            for (int i = 0; i < set.Conditions.Count; i++)
            {
                ImGui.PushID(i);

                ImGui.BeginGroup();

                var cndCfg = set.Conditions[i];
                var selectedCondition = ConditionManager.GetCondition(cndCfg.ID);
                var selectedCategory = ConditionManager.GetConditionCategory(selectedCondition);

                ImGui.Columns(3, null, false);

                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(2, ImGui.GetStyle().FramePadding.Y));
                ImGui.Button(FontAwesomeIcon.ArrowsAltV.ToIconString());
                ImGui.PopStyleVar();
                ImGui.PopFont();

                if (ImGui.IsItemActive() && ImGui.IsMouseDragging(ImGuiMouseButton.Left))
                {
                    ImGuiEx.SetupSlider(true, ImGui.GetItemRectSize().Y + ImGui.GetStyle().ItemSpacing.Y, (hitInterval, increment, closing) =>
                    {
                        if (hitInterval)
                            ShiftCondition(set, cndCfg, increment);
                    });
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Right click this button to delete this condition!");

                    if (ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                    {
                        set.Conditions.RemoveAt(i--);
                        QoLBar.Config.Save();
                    }
                }

                ImGui.SameLine();

                if (i != 0)
                {
                    var _ = (int)cndCfg.Operator;
                    ImGui.SetNextItemWidth(comboSize);
                    if (ImGui.BeginCombo("##Operator", binaryOperators[_], ImGuiComboFlags.NoArrowButton))
                    {
                        for (int ind = 0; ind < binaryOperators.Count; ind++)
                        {
                            var op = binaryOperators[ind];
                            if (ImGui.Selectable(op, ind == _))
                                cndCfg.Operator = (ConditionManager.BinaryOperator)ind;
                        }

                        ImGui.EndCombo();
                    }

                    var operatorTooltip = cndCfg.Operator.ToString();
                    if (debugSteps != null && i < debugSteps.Count)
                    {
                        var setSuccess = debugSteps[i];
                        operatorTooltip += $"\nSet (Up to this condition): {(setSuccess ? "True" : "False")}";

                        var setStatusCol = setSuccess ? 0x2000FF00u : 0x200000FFu;
                        ImGui.GetWindowDrawList().AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), setStatusCol, ImGui.GetStyle().FrameRounding);
                    }

                    ImGuiEx.SetItemTooltip(operatorTooltip);
                }
                else
                {
                    ImGui.PushFont(UiBuilder.IconFont);

                    if (ImGui.Button(FontAwesomeIcon.Plus.ToIconString(), new Vector2(comboSize, 0)))
                    {
                        set.Conditions.Add(new() { ID = "cf" });
                        QoLBar.Config.Save();
                    }

                    ImGui.PopFont();
                }

                ImGui.SameLine();

                var prevCursorPos = ImGui.GetCursorPos();
                var __ = false;
                ImGui.PushStyleColor(ImGuiCol.CheckMark, Vector4.Zero);
                if (ImGui.Checkbox("##NOT", ref __))
                {
                    cndCfg.Negate ^= true;
                    QoLBar.Config.Save();
                }
                ImGui.PopStyleColor();

                var notTooltip = "NOT";
                var success = ConditionManager.CheckCondition(cndCfg.ID, cndCfg.Arg, cndCfg.Negate);
                notTooltip += $"\nCondition: {(success ? "True" : "False")}";

                var statusCol = success ? 0x2000FF00u : 0x200000FFu;
                ImGui.GetWindowDrawList().AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), statusCol, ImGui.GetStyle().FrameRounding);

                ImGuiEx.SetItemTooltip(notTooltip);

                ImGui.SameLine();

                if (cndCfg.Negate)
                {
                    var postCursorPos = ImGui.GetCursorPos();
                    ImGui.Dummy(Vector2.Zero); // Why does SetCursorPos behave differently if you do SameLine before it?
                    ImGui.SetCursorPos(prevCursorPos);
                    ImGui.SetWindowFontScale(1.25f);
                    ImGui.TextUnformatted("  ¬");
                    ImGui.SetWindowFontScale(1);
                    ImGui.SetCursorPos(postCursorPos);
                }

                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.BeginCombo("##Category", selectedCategory.CategoryName))
                {
                    foreach (var (category, list) in ConditionManager.ConditionCategories)
                    {
                        if (!ImGui.Selectable(category.CategoryName, category.GetType() == selectedCategory.GetType())) continue;

                        var condition = list[0];
                        cndCfg.ID = condition.ID;
                        cndCfg.Arg = condition is IArgCondition arg ? arg.GetDefaultArg(cndCfg) : 0;
                        QoLBar.Config.Save();

                        selectedCondition = ConditionManager.GetCondition(cndCfg.ID);
                        selectedCategory = ConditionManager.GetConditionCategory(selectedCondition);
                    }

                    ImGui.EndCombo();
                }

                ImGui.NextColumn();

                var conditionList = ConditionManager.ConditionCategories.FirstOrDefault(t => t.category.GetType() == selectedCategory.GetType()).conditions;
                var drawable = selectedCondition as IDrawableCondition;
                var drawExtra = true;
                if (conditionList != null)
                {
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    if (conditionList.Count == 1)
                    {
                        drawExtra = false;
                        try
                        {
                            drawable?.Draw(cndCfg);
                        }
                        catch (Exception e)
                        {
                            PluginLog.Error($"Error while drawing {drawable}!\n{e}");
                        }
                    }
                    else if (ImGui.BeginCombo("##Condition", selectedCondition.ConditionName))
                    {
                        foreach (var condition in conditionList)
                        {
                            if (ImGui.Selectable(condition.ConditionName, condition.GetType() == selectedCondition.GetType()))
                            {
                                cndCfg.ID = condition.ID;
                                cndCfg.Arg = condition is IArgCondition arg ? arg.GetDefaultArg(cndCfg) : 0;
                                QoLBar.Config.Save();

                                selectedCondition = ConditionManager.GetCondition(cndCfg.ID);
                                drawable = selectedCondition as IDrawableCondition;
                            }

                            var d = condition as IDrawableCondition;
                            var tooltip = d?.GetSelectableTooltip(cndCfg);
                            if (!string.IsNullOrEmpty(tooltip))
                                ImGuiEx.SetItemTooltip(tooltip);
                        }

                        ImGui.EndCombo();
                    }

                    var s = drawable?.GetTooltip(cndCfg);
                    if (!string.IsNullOrEmpty(s))
                        ImGuiEx.SetItemTooltip(s);
                }

                ImGui.NextColumn();

                if (drawExtra)
                {
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    try
                    {
                        drawable?.Draw(cndCfg);
                    }
                    catch (Exception e)
                    {
                        PluginLog.Error($"Error while drawing {drawable}!\n{e}");
                    }
                }

                ImGui.NextColumn();

                ImGui.PopID();
            }

            ImGui.PopStyleVar();
        }

        public static void ShiftCondition(CndSet set, CndCfg cndCfg, bool increment)
        {
            var i = set.Conditions.IndexOf(cndCfg);
            if (!increment ? i <= 0 : i >= (set.Conditions.Count - 1)) return;

            var j = (increment ? i + 1 : i - 1);
            var condition = set.Conditions[i];
            set.Conditions.RemoveAt(i);
            set.Conditions.Insert(j, condition);
            QoLBar.Config.Save();
        }
    }
}
