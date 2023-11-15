using Dalamud.Game.ClientState.Objects.SubKinds;

namespace IcWaRotations.Tank
{
	[BetaRotation]
	[RotationDesc(ActionID.Plunge)]
	public class DrkRotation : DRK_Base
	{
		
    #region PvPDeclaration

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_HardSlash { get; } = new BaseAction(ActionID.PvP_HardSlash);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_SyphonStrike { get; } = new BaseAction(ActionID.PvP_SyphonStrike);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_SoulEater { get; } = new BaseAction(ActionID.PvP_SoulEater);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Quietus { get; } = new BaseAction(ActionID.PvP_Quietus,ActionOption.Buff);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Shadowbringer { get; } = new BaseAction(ActionID.PvP_Shadowbringer);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Plunge { get; } = new BaseAction(ActionID.PvP_Plunge);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_BlackestNight { get; } = new BaseAction(ActionID.PvP_BlackestNight,ActionOption.Buff);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Bloodspiller { get; } = new BaseAction(ActionID.PvP_Bloodspiller)
    {
        StatusNeed = new StatusID[] { StatusID.PvP_Blackblood },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_SaltedEarth { get; } = new BaseAction(ActionID.PvP_SaltedEarth, ActionOption.Buff);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_SaltAndDarkness { get; } = new BaseAction(ActionID.PvP_SaltAndDarkness,ActionOption.Buff);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Eventide { get; } = new BaseAction(ActionID.PvP_Eventide,ActionOption.Buff)
    {
        FilterForHostiles = tars => tars.Where(t => t is PlayerCharacter),
        ActionCheck = (t, m) => LimitBreakLevel >= 1,
    };


    #endregion

		public override string GameVersion => "6.51";
		public override string RotationName => "Incognito DRK";
		public override string Description => "PvP Rotation for DRK , Manual:Shadowbringer";
		public override CombatType Type => CombatType.PvP;
		public override bool ShowStatus => true;

		protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
			.SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
			.SetInt(CombatType.PvP, "EVValue", 30000, "How much HP does the enemy have for LB:Eventide to be done", 1, 100000)
			.SetInt(CombatType.PvP, "SBValue", 30000, "Shadowbringer:PlayerHP", 1, 100000)
			.SetBool(CombatType.PvP, "GuardCancel", false, "Turn on if you want to FORCE RS to use nothing while ENEMIES in guard in PvP");


		protected override bool GeneralGCD(out IAction act)
		{
			act = null;

			#region PvP
			if (Configs.GetBool("GuardCancel") && Player.HasStatus(true, StatusID.PvP_Guard)) return false;
   
			if (Configs.GetBool("LBInPvP") && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("EVValue") && PvP_Eventide.IsEnabled)
			{
				if (PvP_Eventide.CanUse(out act, CanUseOption.MustUse)) return true;
			}
   
			if (PvP_Quietus.CanUse(out act, CanUseOption.MustUse)) return true;
   
			
            if (PvP_Bloodspiller.CanUse(out act, CanUseOption.MustUse)) return true;
            if (PvP_SaltedEarth.CanUse(out act, CanUseOption.MustUse) && InCombat) return true;
            if (Player.HasStatus(true, StatusID.PvP_SaltedEarthDMG) && PvP_SaltAndDarkness.CanUse(out act, CanUseOption.MustUse)) return true;
   
   
			if (PvP_SoulEater.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_SyphonStrike.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_HardSlash.CanUse(out act, CanUseOption.MustUse)) return true;

			return base.GeneralGCD(out act);
			#endregion
		}

		protected override bool AttackAbility(out IAction act)
		{
			if (PvP_Plunge.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_BlackestNight.CanUse(out act, CanUseOption.MustUse) && InCombat) return true;
			if (!PvP_Shadowbringer.IsCoolingDown)
			{
				act = PvP_Shadowbringer;
				return true;
			}
			return base.AttackAbility(out act);
		}


	}
}
