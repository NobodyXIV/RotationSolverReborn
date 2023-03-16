﻿using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic;
using RotationSolver.Basic.Helpers;
using System;


namespace RotationSolver.Actions.BaseAction;

public partial class BaseAction
{
    public bool ElapsedAfterGCD(uint gcdCount = 0, uint abilityCount = 0)
    {
        if (!IsCoolingDown) return false;
        var elapsed = RecastTimeElapsedOneCharge;
        return CooldownHelper.ElapsedAfterGCD(elapsed, gcdCount, abilityCount);
    }

    public bool ElapsedAfter(float time)
    {
        if (!IsCoolingDown) return false;
        var elapsed = RecastTimeElapsedOneCharge;
        return CooldownHelper.ElapsedAfter(elapsed, time);
    }

    public bool WillHaveOneChargeGCD(uint gcdCount = 0, uint abilityCount = 0)
    {
        if (HaveOneCharge) return true;
        var recast = RecastTimeRemainOneCharge;
        return CooldownHelper.RecastAfterGCD(recast, gcdCount, abilityCount);
    }

    public bool WillHaveOneCharge(float remain) => WillHaveOneCharge(remain, true);

    private bool WillHaveOneCharge(float remain, bool addWeaponRemain)
    {
        if (HaveOneCharge) return true;
        var recast = RecastTimeRemainOneCharge;
        return CooldownHelper.RecastAfter(recast, remain, addWeaponRemain);
    }


    private unsafe RecastDetail* CoolDownDetail => ActionManager.Instance()->GetRecastGroupDetail(CoolDownGroup - 1);
    /// <summary>
    /// 复唱时间
    /// </summary>
    private unsafe float RecastTime => CoolDownDetail->Total;

    /// <summary>
    /// 复唱经过时间
    /// </summary>
    public unsafe float RecastTimeElapsed => CoolDownDetail->Elapsed;

    /// <summary>
    /// 是否正在冷却中
    /// </summary>
    public unsafe bool IsCoolingDown => CoolDownDetail->IsActive != 0;

    /// <summary>
    /// 复唱剩余时间
    /// </summary>
    public float RecastTimeRemain => RecastTime - RecastTimeElapsed;

    /// <summary>
    /// 技能的最大层数
    /// </summary>
    public unsafe ushort MaxCharges => Math.Max(ActionManager.GetMaxCharges(AdjustedID, Service.Player.Level), (ushort)1);
    /// <summary>
    /// 是否起码有一层技能
    /// </summary>
    public bool HaveOneCharge => IsCoolingDown ? RecastTimeElapsed >= RecastTimeOneCharge : true;
    /// <summary>
    /// 当前技能层数
    /// </summary>
    public ushort CurrentCharges => IsCoolingDown ? (ushort)(RecastTimeElapsed / RecastTimeOneCharge) : MaxCharges;

    public float RecastTimeOneCharge => ActionManager.GetAdjustedRecastTime(ActionType.Spell, AdjustedID) / 1000f;

    /// <summary>
    /// 下一层转好的时间
    /// </summary>
    private float RecastTimeRemainOneCharge => RecastTimeRemain % RecastTimeOneCharge;
    private float RecastTimeElapsedOneCharge => RecastTimeElapsed % RecastTimeOneCharge;

}
