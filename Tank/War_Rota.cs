using Dalamud.Game.ClientState.Objects.SubKinds;

namespace IcWaRotations.Tank
{
	[BetaRotation]
	[RotationDesc(ActionID.PrimalRend)]
	public class WarRotation : WAR_Base
	{

		public override string GameVersion => "6.51";
		public override string RotationName => "Incognito WAR";
		public override string Description => "PvP Rotation for WAR";
		public override CombatType Type => CombatType.PvP;
		public override bool ShowStatus => true;

		protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
			.SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
			.SetInt(CombatType.PvP, "PSValue", 30000, "How much HP does the enemy have for LB:PrimalScream to be done", 1, 100000)
			.SetInt(CombatType.PvP, "OSValue", 30000, "How much HP does the enemy have for Onslaught to be done", 1, 100000)
			.SetBool(CombatType.PvP, "GuardCancel", false, "Turn on if you want to FORCE RS to use nothing while ENEMIES in guard in PvP");


		protected override bool GeneralGCD(out IAction act)
		{
			act = null;

			#region PvP

			if (Configs.GetBool("LBInPvP") && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("PSValue") && PvP_PrimalScream.IsEnabled)
			{
				if (PvP_PrimalScream.CanUse(out act, CanUseOption.MustUse)) return true;

				if (PvP_PrimalRend.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_FellCleave.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_Bloodwhetting.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_ChaoticCyclone.CanUse(out act, CanUseOption.MustUse)) return true;
			}

			if (PvP_Onslaught.CanUse(out act, CanUseOption.MustUse) && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("OSValue")) return true;
			if (PvP_Orogeny.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Blota.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Bloodwhetting.CanUse(out act, CanUseOption.MustUse) && InCombat) return true;
			if (PvP_FellCleave.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_ChaoticCyclone.CanUse(out act, CanUseOption.MustUse)) return true;


			
			if (PvP_PrimalRend.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_StormsPath.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Maim.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_HeavySwing.CanUse(out act, CanUseOption.MustUse)) return true;

			return false;
			#endregion
		}


	}
}
