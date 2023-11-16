using Dalamud.Game.ClientState.Objects.SubKinds;

namespace IcWaRotations.Melee
{
	[BetaRotation]
	[RotationDesc(ActionID.HissatsuGyoten)]
	public class SamRotation : SAM_Base
	{

		#region PvPDeclaration
		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Yukikaze { get; } = new BaseAction(ActionID.PvP_Yukikaze);


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Gekko { get; } = new BaseAction(ActionID.PvP_Gekko);


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Kasha { get; } = new BaseAction(ActionID.PvP_Kasha);


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_OgiNamikiri { get; } = new BaseAction(ActionID.PvP_OgiNamikiri);


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Kaeshi { get; } = new BaseAction(ActionID.PvP_Kaeshi);

		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Soten { get; } = new BaseAction(ActionID.PvP_Soten);


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Hyosetsu { get; } = new BaseAction(ActionID.PvP_Hyosetsu, ActionOption.Buff)
		{
			StatusNeed = new StatusID[] { StatusID.PvP_Kaiten },
		};


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Mangetsu { get; } = new BaseAction(ActionID.PvP_Mangetsu)
		{
			StatusNeed = new StatusID[] { StatusID.PvP_Kaiten },
		};


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Oka { get; } = new BaseAction(ActionID.PvP_Oka)
		{
			StatusNeed = new StatusID[] { StatusID.PvP_Kaiten },
		};


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Chiten { get; } = new BaseAction(ActionID.PvP_Chiten, ActionOption.Buff);


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Mineuchi { get; } = new BaseAction(ActionID.PvP_Mineuchi);


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_MeikyoShisui { get; } = new BaseAction(ActionID.PvP_MeikyoShisui, ActionOption.Buff);


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Midare { get; } = new BaseAction(ActionID.PvP_Midare)
		{
			StatusNeed = new StatusID[] { StatusID.PvP_Midare },
		};


		/// <summary>
		/// 
		/// </summary>
		public static IBaseAction PvP_Zantetsuken { get; } = new BaseAction(ActionID.PvP_Zantetsuken)
		{
			FilterForHostiles = tars => tars.Where(t => t is PlayerCharacter),
			ActionCheck = (t, m) => LimitBreakLevel >= 1,
		};
		#endregion

		public override string GameVersion => "6.51";
		public override string RotationName => "Incognito SAM";
		public override string Description => "PvP Rotation for SAM";
		public override CombatType Type => CombatType.PvP;
		public override bool ShowStatus => true;

		protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
			.SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
//			.SetInt(CombatType.PvP, "ZTValue", 30000, "How much HP does the enemy have for LB:Zantetsuken to be done", 1, 100000)
//			.SetInt(CombatType.PvP, "OSValue", 30000, "How much HP does the enemy have for Onslaught to be done", 1, 100000)
			.SetBool(CombatType.PvP, "GuardCancel", false, "Turn on if you want to FORCE RS to use nothing while ENEMIES in guard in PvP");


		protected override bool GeneralGCD(out IAction act)
		{
			act = null;

			#region PvP
			if (Configs.GetBool("GuardCancel") && Player.HasStatus(true, StatusID.PvP_Guard)) return false;

			if (PvP_Chiten.CanUse(out act, CanUseOption.MustUse) && InCombat) return true;
			if (Player.HasStatus(true, StatusID.PvP_Chiten) && PvP_Zantetsuken.IsEnabled)
			{
				if (Configs.GetBool("LBInPvP") && PvP_Zantetsuken.CanUse(out act, CanUseOption.MustUse)) return true;
			}

			if (PvP_Mineuchi.CanUse(out act, CanUseOption.MustUse)) return true;

			if (PvP_MeikyoShisui.CanUse(out act, CanUseOption.MustUse) && InCombat) return true;
			if (Player.HasStatus(true, StatusID.PvP_Midare))
			{
				if (PvP_Midare.CanUse(out act, CanUseOption.MustUse)) return true;
			}


			if (PvP_OgiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;

			if (PvP_Soten.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
			if (PvP_Hyosetsu.CanUse(out act, CanUseOption.MustUse) && Player.HasStatus(true, StatusID.PvP_Kaiten)) return true;
//            if (PvP_Soten.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
			if (PvP_Mangetsu.CanUse(out act, CanUseOption.MustUse) && Player.HasStatus(true, StatusID.PvP_Kaiten)) return true;
//            if (PvP_Soten.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
			if (PvP_Oka.CanUse(out act, CanUseOption.MustUse) && Player.HasStatus(true, StatusID.PvP_Kaiten)) return true;

			if (PvP_Kasha.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Gekko.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Yukikaze.CanUse(out act, CanUseOption.MustUse)) return true;

			return false;
			#endregion
		}


	}
}