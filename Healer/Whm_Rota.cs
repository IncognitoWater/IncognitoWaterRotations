using Dalamud.Game.ClientState.Objects.SubKinds;

namespace IcWaRotations.Healer
{
	[BetaRotation]
	[RotationDesc(ActionID.ChainStratagem)]
	public class WhmRotation : WHM_Base
	{

		public override string GameVersion => "6.51";
		public override string RotationName => "Incognito WHM";
		public override string Description => "PvP Rotation for WHM";
		public override CombatType Type => CombatType.PvP;
		public override bool ShowStatus => true;

		protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
			.SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
			.SetInt(CombatType.PvP, "APValue", 60000, "How much HP does the enemy have for LB:AfflatusPurgation to be done",1,100000)
			.SetBool(CombatType.PvP, "GuardCancel", false, "Turn on if you want to FORCE RS to use nothing while ENEMIES in guard in PvP")
			.SetInt(CombatType.PvP, "Cure2Value", 60000, "How much HP does the enemy have for Cure2 to be done",1,100000)
			.SetInt(CombatType.PvP, "MiracleOfNatureValue", 40000, "How much HP does the enemy have for MiracleOfNature to be done",1,100000)
			.SetInt(CombatType.PvP, "SeraphStrikeValue", 25000, "How much HP does the enemy have for > SeraphStrike to be done",1,100000);

		protected override bool GeneralGCD(out IAction act)
		{
			act = null;

			#region PvP
			if (Configs.GetBool("GuardCancel") && Player.HasStatus(true, StatusID.PvP_Guard)) return false;

			if (Configs.GetBool("LBInPvP") && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("APValue") && PvP_AfflatusPurgation.CanUse(out act, CanUseOption.MustUse)) return true;

			if (Player.CurrentHp < Configs.GetInt("Cure2Value") && PvP_Cure2.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_AfflatusMisery.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Aquaveil.CanUse(out act, CanUseOption.MustUse)) return true;

			if (HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("MiracleOfNatureValue") && PvP_MiracleOfNature.CanUse(out act, CanUseOption.MustUse)) return true;
			if (HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("SeraphStrikeValue") && PvP_SeraphStrike.CanUse(out act, CanUseOption.MustUse)) return true;

			if (Player.HasStatus(true, StatusID.PvP_Cure3Ready))
			{
				if (PvP_Cure3.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
			}


			if (PvP_Glare3.CanUse(out act, CanUseOption.MustUse)) return true;
			
			return false;
			#endregion
		}


	}
}