using Dalamud.Game.ClientState.Objects.SubKinds;

namespace IcWaRotations.Healer
{
	[BetaRotation]
	[RotationDesc(ActionID.PrimalRend)]
	public class SgeRotation : SGE_Base
	{
		
    #region PvPDeclaration

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Dosis3 { get; } = new BaseAction(ActionID.PvP_Dosis);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Phlegma3 { get; } = new BaseAction(ActionID.PvP_Phlegma3);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Pneuma { get; } = new BaseAction(ActionID.PvP_Pneuma);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Eukrasia { get; } = new BaseAction(ActionID.PvP_Eukrasia,ActionOption.Buff);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Icarus { get; } = new BaseAction(ActionID.PvP_Icarus);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Toxikon { get; } = new BaseAction(ActionID.PvP_Toxikon);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Kardia { get; } = new BaseAction(ActionID.PvP_Kardia);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_EukrasianDosis3 { get; } = new BaseAction(ActionID.PvP_EukrasianDosis2,ActionOption.Buff);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Toxikon2 { get; } = new BaseAction(ActionID.PvP_Toxikon2);


    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Mesotes { get; } = new BaseAction(ActionID.PvP_Mesotes)
    {
        FilterForHostiles = tars => tars.Where(t => t is PlayerCharacter),
        ActionCheck = (t, m) => LimitBreakLevel >= 1,
    };


    #endregion

		public override string GameVersion => "6.51";
		public override string RotationName => "Incognito SGE";
		public override string Description => "PvP Rotation for SGE";
		public override CombatType Type => CombatType.PvP;
		public override bool ShowStatus => true;
		

		protected override bool GeneralGCD(out IAction act)
		{
			act = null;

			#region PvP
			if (Configs.GetBool("GuardCancel") && Player.HasStatus(true, StatusID.PvP_Guard)) return false;


//			if (PvP_Onslaught.CanUse(out act, CanUseOption.MustUse) && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("OSValue")) return true;
			if (PvP_Phlegma3.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Pneuma.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Eukrasia.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Toxikon.CanUse(out act, CanUseOption.MustUse)) return true;

//			if (PvP_EukrasianDosis3.CanUse(out act, CanUseOption.MustUse)) return true;
			if (Player.HasStatus(true, StatusID.PvP_Addersting) && PvP_Toxikon2.CanUse(out act, CanUseOption.MustUse)) return true;

			if (PvP_Dosis3.CanUse(out act, CanUseOption.MustUse)) return true;

			return false;
			#endregion
		}


	}
}
