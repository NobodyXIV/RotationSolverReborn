﻿namespace RotationSolver.Basic.Actions;

/// <summary>
/// Setting from the developer.
/// </summary>
public class ActionSetting()
{
    public ActionID[]? Ninjutsu { get; set; } = null;
    public Func<uint?>? MPOverride { get; set; } = null;
    public bool IsMeleeRange { get; set; } = false;
    public bool StatusFromSelf { get; set; } = true;
    public StatusID[]? TargetStatusProvide { get; set; } = null;
    public StatusID[]? TargetStatusNeed { get; set; } = null;
    public Func<BattleChara, bool> CanTarget { get; set; } = t => true;
    public ActionID[]? ComboIdsNot { get; set; }

    public ActionID[]? ComboIds { get; set; }
    /// <summary>
    /// Status that this action provides.
    /// </summary>
    public StatusID[]? StatusProvide { get; set; } = null;

    /// <summary>
    /// Status that this action needs.
    /// </summary>
    public StatusID[]? StatusNeed { get; set; } = null;

    public Func<bool>? RotationCheck { get; set; } = null;
    internal Func<bool>? ActionCheck { get; set; } = null;

    internal Func<ActionConfig>? CreateConfig { get; set; } = null;

    public bool IsFriendly { get; set; }

    private TargetType _type = TargetType.Big;
    public TargetType TargetType 
    { 
        get => IBaseAction.TargetOverride ?? _type; 
        set => _type = value; 
    }

    public EnemyPositional EnemyPositional { get; set; } = EnemyPositional.None;

    public bool EndSpecial { get; set; }

}