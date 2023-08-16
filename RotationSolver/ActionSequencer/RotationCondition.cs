﻿using ECommons.GameHelpers;
using RotationSolver.Localization;
using RotationSolver.UI;

namespace RotationSolver.ActionSequencer;

internal class RotationCondition : ICondition
{
    public ComboConditionType ComboConditionType;
    PropertyInfo _prop;
    public string PropertyName { get; set; } = string.Empty;

    MethodInfo _method;
    public string MethodName { get; set; } = string.Empty;

    BaseAction _action;

    public ActionID ID { get; set; } = ActionID.None;

    public int Condition;

    public int Param1;
    public int Param2;
    public float Param3;

    private void UpdateInfo(ICustomRotation rotation)
    {
        ConditionHelper.CheckBaseAction(rotation, ID, ref _action);
        ConditionHelper.CheckMemberInfo(rotation, PropertyName, ref _prop);
        ConditionHelper.CheckMemberInfo(rotation, MethodName, ref _method);
    }

    public bool IsTrue(ICustomRotation rotation)
    {
        if (!Player.Available) return false;
        UpdateInfo(rotation);

        switch (ComboConditionType)
        {
            case ComboConditionType.Bool:
                if (_prop == null) return false;
                if (_prop.GetValue(rotation) is bool b)
                {
                    return Condition > 0 ? !b : b;
                }
                return false;

            case ComboConditionType.Byte:
                if (_prop == null) return false;
                if (_prop.GetValue(rotation) is byte by)
                {
                    switch (Condition)
                    {
                        case 0:
                            return by > Param1;
                        case 1:
                            return by < Param1;
                        case 2:
                            return by == Param1;
                    }
                }
                return false;

            case ComboConditionType.Float:
                if (_prop == null) return false;
                if (_prop.GetValue(rotation) is float fl)
                {
                    switch (Condition)
                    {
                        case 0:
                            return fl > Param3;
                        case 1:
                            return fl < Param3;
                        case 2:
                            return fl == Param3;
                    }
                }
                return false;

            case ComboConditionType.Last:
                try
                {
                    if (_method?.Invoke(rotation, new object[] { Param1 > 0, new IAction[] { _action } }) is bool boo)
                    {
                        return Condition > 0 ? boo : !boo;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
        }

        return false;
    }

    string searchTxt = string.Empty;
    public void Draw(ICustomRotation rotation)
    {
        UpdateInfo(rotation);

        ImGuiHelper.DrawCondition(IsTrue(rotation));
        ImGui.SameLine();

        ConditionHelper.DrawByteEnum($"##Category{GetHashCode()}", ref ComboConditionType, EnumTranslations.ToName);

        switch (ComboConditionType)
        {
            case ComboConditionType.Bool:
                ImGui.SameLine();
                ImGuiHelper.SearchItemsReflection($"##Comparation{GetHashCode()}", _prop?.GetMemberName(), ref searchTxt, rotation.AllBools, i =>
                {
                    _prop = i;
                    PropertyName = i.Name;
                });

                ImGui.SameLine();
                ImGui.SetNextItemWidth(80);
                ImGui.Combo($"##IsOrNot{GetHashCode()}", ref Condition, new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                }, 2);
                break;

            case ComboConditionType.Byte:
                ImGui.SameLine();
                ImGuiHelper.SearchItemsReflection($"##ByteChoice{GetHashCode()}", _prop?.GetMemberName(), ref searchTxt, rotation.AllBytes, i =>
                {
                    _prop = i;
                    PropertyName = i.Name;
                });


                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                ImGui.Combo($"##Comparation{GetHashCode()}", ref Condition, new string[] { ">", "<", "=" }, 3);

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ImGui.DragInt($"##Value{GetHashCode()}", ref Param1);

                break;
            case ComboConditionType.Float:
                ImGui.SameLine();
                ImGuiHelper.SearchItemsReflection($"##FloatChoice{GetHashCode()}", _prop?.GetMemberName(), ref searchTxt, rotation.AllFloats, i =>
                {
                    _prop = i;
                    PropertyName = i.Name;
                });


                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                ImGui.Combo($"##Comparation{GetHashCode()}", ref Condition, new string[] { ">", "<", "=" }, 3);

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ImGui.DragFloat($"##Value{GetHashCode()}", ref Param3);

                break;

            //case ComboConditionType.Last:
            //    ImGui.SameLine();
            //    ImGuiHelper.SearchItemsReflection($"##Time{GetHashCode()}", _method?.GetMemberName(), ref searchTxt, rotation.AllLast, i =>
            //    {
            //        _method = i;
            //        MethodName = i.Name;
            //    });

            //    ImGui.SameLine();
            //    ImGui.SetNextItemWidth(80);
            //    ImGui.Combo($"##IsNot{GetHashCode()}", ref Condition, new string[]
            //    {
            //        LocalizationManager.RightLang.ActionSequencer_Is,
            //        LocalizationManager.RightLang.ActionSequencer_Isnot,
            //    }, 2);

            //    ImGui.SameLine();
            //    var name = _action?.Name ?? string.Empty;
            //    ImGuiHelper.SearchCombo($"##ActionChoice{GetHashCode()}", name, ref searchTxt, rotation.AllBaseActions, i =>
            //    {
            //        _action = (BaseAction)i;
            //        ID = (ActionID)_action.ID;
            //    });

            //    ImGui.SameLine();
            //    ImGui.SetNextItemWidth(50);
            //    ImGui.Combo($"##Adjust{GetHashCode()}", ref Param1, new string[] { "Original", "Adjusted" }, 2);

            //    break;
        }
    }
}

public enum ComboConditionType : byte
{
    Bool,
    Byte,
    Float,
    Last,
}
