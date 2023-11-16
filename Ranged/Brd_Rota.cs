using Dalamud.Game.ClientState.Objects.SubKinds;

namespace IcWaRotations.Ranged
{
	[BetaRotation]
	[RotationDesc(ActionID.Wildfire)]
	public class BrdRotation : BRD_Base
	{
		
        public override string GameVersion => "6.51";
		public override string RotationName => "Incognito BRD";
		public override string Description => "PvP Rotation for BRD";
		public override CombatType Type => CombatType.PvP;
		public override bool ShowStatus => true;


//		protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
//			.SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
//			.SetInt(CombatType.PvP, "MDValue", 25000, "How much HP does the enemy have for LB:Meteodrive to be done", 1, 100000)
//			.SetBool(CombatType.PvP, "ThunderclapPvP", true, "Use Thunderclap")
//			.SetInt(CombatType.PvP, "SCValue", 30000, "How much HP does the enemy have for Thunderclap to be done", 1, 100000)
//			.SetBool(CombatType.PvP, "GuardCancel", false, "Turn on if you want to FORCE RS to use nothing while ENEMIES in guard in PvP");



		protected override bool GeneralGCD(out IAction act)
		{
			act = null;

			#region PvP
			if (Configs.GetBool("GuardCancel") && Player.HasStatus(true, StatusID.PvP_Guard)) return false;

			if (PvP_EmpyrealArrow.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
			if (PvP_SilentNocturne.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_BlastArrow.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_ApexArrow.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
			if (PvP_PowerfulShot.CanUse(out act, CanUseOption.IgnoreCastCheck)) return true;


			return false;
			#endregion
		}
	}
}