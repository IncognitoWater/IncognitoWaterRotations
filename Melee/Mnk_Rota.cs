namespace IcWaRotations.Melee
{
	[BetaRotation]
	[RotationDesc(ActionID.Brotherhood)]
	public class MnkRotation : MNK_Base
	{

		public override string GameVersion => "6.51";
		public override string RotationName => "Incognito MNK";
		public override string Description => "PvP Rotation for MNK";
		public override CombatType Type => CombatType.PvP;
		public override bool ShowStatus => true;

		protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
			.SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
			.SetInt(CombatType.PvP, "MDValue", 60000, "How much HP does the enemy have for LB:Meteodrive to be done", 1, 100000)
			.SetBool(CombatType.PvP, "GuardCancel", false, "Turn on if you want to FORCE RS to use nothing while ENEMIES in guard in PvP");


		protected override bool GeneralGCD(out IAction act)
		{
			act = null;

			#region PvP
			if (Configs.GetBool("GuardCancel") && Player.HasStatus(true, StatusID.PvP_Guard)) return false;

			if (Configs.GetBool("LBInPvP") && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("MDValue") && PvP_Phantomrush.IsEnabled && PvP_Meteodrive.IsEnabled)
			{
				if (PvP_Risingphoenix.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_Enlightenment.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_Sixsidedstar.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_Meteodrive.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_Risingphoenix.CanUse(out act, CanUseOption.MustUse)) return true;
				if (PvP_Phantomrush.CanUse(out act, CanUseOption.MustUse)) return true;
			}

			if (PvP_Sixsidedstar.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Enlightenment.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
			if (PvP_Risingphoenix.CanUse(out act, CanUseOption.MustUse) && InCombat) return true;
			if (PvP_Thunderclap.CanUse(out act, CanUseOption.MustUse) && InCombat) return true;

			if (PvP_Riddleofearth.CanUse(out act, CanUseOption.MustUse) && InCombat) return true;
			if (Player.HasStatus(true, StatusID.PvP_EarthResonance))
			{
				if (PvP_Earthsreply.CanUse(out act, CanUseOption.MustUse)) return true;
			}

			if (PvP_Phantomrush.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Demolish.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Twinsnakes.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Dragonkick.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Snappunch.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Truestrike.CanUse(out act, CanUseOption.MustUse)) return true;
			if (PvP_Bootshine.CanUse(out act, CanUseOption.MustUse)) return true;


			return false;
			#endregion
		}


	}
}