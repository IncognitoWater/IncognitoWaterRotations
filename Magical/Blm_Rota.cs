using Dalamud.Game.ClientState.Objects.SubKinds;

namespace IcWaRotations.Magical
{
	[BetaRotation]
	[RotationDesc(ActionID.Manaward)]
	public class BlmRotation : BLM_Base
	{

		public override string GameVersion => "6.51";
		public override string RotationName => "Incognito BLM";
		public override string Description => "PvP Rotation for BLM";
		public override CombatType Type => CombatType.PvP;
		public override bool ShowStatus => true;

		protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
			.SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
			.SetInt(CombatType.PvP, "SRValue", 30000, "How much HP does the enemy have for LB:SoulResonance to be done", 1, 100000)
			.SetInt(CombatType.PvP, "AMValue", 25000, "How much HP does the enemy have for AetherialManipulation to be done", 1, 100000)
			.SetBool(CombatType.PvP, "GuardCancel", false, "Turn on if you want to FORCE RS to use nothing while ENEMIES in guard in PvP");


		protected override bool GeneralGCD(out IAction act)
		{
			act = null;

			#region PvP
			if (Configs.GetBool("GuardCancel") && Player.HasStatus(true, StatusID.PvP_Guard)) return false;

			if (Configs.GetBool("LBInPvP") && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("SRValue") && PvP_SoulResonance.IsEnabled )
			{
				if (PvP_SoulResonance.CanUse(out act, CanUseOption.MustUse)) return true;

				if (PvP_Superflare.CanUse(out act, CanUseOption.MustUse)) return true;
                if (PvP_Foul.CanUse(out act, CanUseOption.MustUse)) return true;
                if (PvP_AetherialManipulation.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_Burst.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_Paradox.CanUse(out act, CanUseOption.MustUse)) return true;

//    			if (PvP_Freeze.CanUse(out act, CanUseOption.MustUse) && Player.HasStatus(true, StatusID.PvP_UmbralIce3)) return true;
//	    		if (PvP_Blizzard4.CanUse(out act, CanUseOption.MustUse) && Player.HasStatus(true, StatusID.PvP_UmbralIce2)) return true;
			    if (PvP_Blizzard.CanUse(out act, CanUseOption.MustUse)) return true;
			}

			if (PvP_AetherialManipulation.CanUse(out act, CanUseOption.MustUse) && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("AMValue")) return true;
			if (PvP_Nightwing.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Burst.CanUse(out act, CanUseOption.MustUse) && InCombat) return true;
			if (PvP_Paradox.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Superflare.CanUse(out act, CanUseOption.MustUse) && HostileTarget && HostileTarget.DistanceToPlayer() < 9 && InCombat) return true;

//			if (PvP_Freeze.CanUse(out act, CanUseOption.MustUse) && Player.HasStatus(true, StatusID.PvP_UmbralIce3)) return true;
//			if (PvP_Blizzard4.CanUse(out act, CanUseOption.MustUse) && Player.HasStatus(true, StatusID.PvP_UmbralIce2)) return true;
			if (PvP_Blizzard.CanUse(out act, CanUseOption.MustUse)) return true;

			return false;
			#endregion
		}


	}
}
