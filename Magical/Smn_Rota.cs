namespace IcWaRotations.Magical;

[RotationDesc(ActionID.SearingLight)]
[LinkDescription("https://github.com/IncognitoWater/IncognitoWaterRotations/blob/main/Magical/Smn_Rota.cs")]
public sealed class SmnRotation : SMN_Base
{
	public override string GameVersion => "6.51";

	public override string RotationName => "IncognitoWater's SMN";

	public override string Description => "High Level Content Summoner Rotation and PvP";

	public override CombatType Type => CombatType.Both;

	protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
		.SetCombo(CombatType.PvE, "addSwiftcast", 0, "Use Swiftcast With", "No", "Emerald", "Ruby", "All")
		.SetCombo(CombatType.PvE, "SummonOrder", 0, "Invocation Order", "Topaz-Emerald-Ruby", "Topaz-Ruby-Emerald", "Emerald-Topaz-Ruby")
		.SetBool(CombatType.PvE, "addCrimsonCyclone", true, "Use Crimson Cyclone")
		.SetBool(CombatType.PvE, "RadiantOnCooldown", false, "Use Radiant On Cooldown")
		.SetBool(CombatType.PvE, "OrbWalkerAdjust", false, "Turn it on to be able to use Orbwalker with this rotation")
		.SetBool(CombatType.PvP, "UseBahamutPvP", false, "Use Bahamut in PvP")
		.SetBool(CombatType.PvP, "UsePhoenixPvP", false, "Use Phoenix in PvP")
		.SetBool(CombatType.PvP, "CrimsonCycloneInPvP", false, "Use CrimsonCyclone in PvP")
		.SetBool(CombatType.PvP, "GuardCancel", false, "Turn on if you want to FORCE RS to use nothing while enemy is in guard in PvP")
		.SetBool(CombatType.PvP, "CrimsonSpecial", false, "Turn on if you want Crimson in PvP to be executed only if the enemys has less life than the next setting \n (Need CrimsonCyclonPvpTurnedOn)")
		.SetInt(CombatType.PvP, "CrimsonSpecialValue", 20000, "How much HP does the enemy have for crimson to be done", 1, 100000);

	private float GetPlayerHealthPercent()
	{
		return (Player.CurrentHp / Player.MaxHp) * 100;
	}

	public override bool CanHealSingleSpell => false;

	[RotationDesc(ActionID.CrimsonCyclone)]
	protected override bool MoveForwardGCD(out IAction act)
	{
		//Crimson Cyclone (which makes the player move)
		if (CrimsonCyclone.CanUse(out act, CanUseOption.MustUse)) return true;
		return base.MoveForwardGCD(out act);
	}

	protected override bool GeneralGCD(out IAction act)
	{
		act = null;

		#region PvP
		if (Configs.GetBool("GuardCancel") && Player.HasStatus(true, StatusID.PvP_Guard)) return false;
		if (Player.HasStatus(true, StatusID.PvP_DreadwyrmTrance))
		{
			if (PvP_AstralImpulse.CanUse(out act, CanUseOption.MustUse)) return true;
		}
		if (Player.HasStatus(true, StatusID.PvP_FirebirdTrance))
		{
			if (PvP_FountainOfFire.CanUse(out act, CanUseOption.MustUse)) return true;
		}
		if (Configs.GetBool("CrimsonCycloneInPvP") && (Configs.GetBool("CrimsonSpecial")))
		{
			if ((HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("CrimsonSpecialValue"))
				&& PvP_CrimsonCyclone.CanUse(out act, CanUseOption.MustUse)) return true;
		}
		if (Configs.GetBool("CrimsonCycloneInPvP") && !(Configs.GetBool("CrimsonSpecial")))
		{
			if (PvP_CrimsonCyclone.CanUse(out act, CanUseOption.MustUse)) return true;
		}
		if (IsLastGCD(ActionID.PvP_CrimsonCyclone) && PvP_CrimsonStrike.CanUse(out act, CanUseOption.MustUse)) return true;
		if (PvP_Slipstream.CanUse(out act, CanUseOption.MustUse)) return true;
		if (PvP_Ruin3.CanUse(out act, CanUseOption.MustUse)) return true;
		#endregion

		#region PvE
		//Spawning carbuncle + attempting to avoid unwanted try of spawning carbuncle
		if (Configs.GetBool("OrbWalkerAdjust"))
		{
			if (!InBahamut && !InPhoenix && !InGaruda && !InIfrit && !InTitan && SummonCarbuncle.CanUse(out act, CanUseOption.IgnoreCastCheck)) return true;
		}
		else
		{
			if (!InBahamut && !InPhoenix && !InGaruda && !InIfrit && !InTitan && SummonCarbuncle.CanUse(out act)) return true;
		}

		//slipstream
		if (Configs.GetBool("OrbWalkerAdjust"))
		{
			if (Slipstream.CanUse(out act, CanUseOption.IgnoreCastCheck)) return true;
		}
		else
		{
			if (Slipstream.CanUse(out act, CanUseOption.MustUse)) return true;
		}

		//Crimson strike 
		if (CrimsonStrike.CanUse(out act, CanUseOption.MustUse)) return true;

		//AOE
		if (PreciousBrilliance.CanUse(out act)) return true;
		//gemshine
		if (InIfrit && Configs.GetBool("OrbWalkerAdjust"))
		{
			if (Gemshine.CanUse(out act, CanUseOption.IgnoreCastCheck)) return true;
		}
		else
		{
			if (Gemshine.CanUse(out act)) return true;
		}

		if (Configs.GetBool("addCrimsonCyclone") && CrimsonCyclone.CanUse(out act, CanUseOption.MustUse)) return true;

		//Summon Baha or Phoenix
		if ((Player.HasStatus(false, StatusID.SearingLight) || SearingLight.IsCoolingDown) && SummonBahamut.CanUse(out act)) return true;
		if (!SummonBahamut.EnoughLevel && HasHostilesInRange && AetherCharge.CanUse(out act)) return true;

		//Ruin4
		if (IsMoving && InIfrit
			&& !Player.HasStatus(true, StatusID.SwiftCast) && !InBahamut && !InPhoenix
			&& RuinIV.CanUse(out act, CanUseOption.MustUse)) return true;

		//Select summon order
		switch (Configs.GetCombo("SummonOrder"))
		{
		default:
			//Titan
			if (SummonTopaz.CanUse(out act)) return true;
			//Garuda
			if (SummonEmerald.CanUse(out act)) return true;
			//Ifrit
			if (SummonRuby.CanUse(out act)) return true;
			break;

		case 1:
			//Titan
			if (SummonTopaz.CanUse(out act)) return true;
			//Ifrit
			if (SummonRuby.CanUse(out act)) return true;
			//Garuda
			if (SummonEmerald.CanUse(out act)) return true;
			break;

		case 2:
			//Garuda
			if (SummonEmerald.CanUse(out act)) return true;
			//Titan
			if (SummonTopaz.CanUse(out act)) return true;
			//Ifrit
			if (SummonRuby.CanUse(out act)) return true;
			break;
		}
		if (SummonTimeEndAfterGCD() && AttunmentTimeEndAfterGCD() &&
			!Player.HasStatus(true, StatusID.SwiftCast) && !InBahamut && !InPhoenix && !InGaruda && !InTitan &&
			RuinIV.CanUse(out act, CanUseOption.MustUse)) return true;
		//Outburst
		if (Configs.GetBool("OrbWalkerAdjust"))
		{
			if (Outburst.CanUse(out act, CanUseOption.IgnoreCastCheck)) return true;
		}
		else
		{
			if (Outburst.CanUse(out act)) return true;
		}

		//Any ruin ( 1-2-3 ) 
		if (Configs.GetBool("OrbWalkerAdjust"))
		{
			if (Ruin.CanUse(out act, CanUseOption.IgnoreCastCheck)) return true;
		}
		else
		{
			if (Ruin.CanUse(out act)) return true;
		}
		return false;
		#endregion
	}

	protected override bool AttackAbility(out IAction act)
	{
		#region PvE
		if (IsBurst && !Player.HasStatus(false, StatusID.SearingLight))
		{
			//Burst raidbuff searinglight
			if (SearingLight.CanUse(out act, CanUseOption.MustUse)) return true;
		}


		//Burst for bahamut
		if ((InBahamut && SummonBahamut.ElapsedOneChargeAfterGCD(3) || InPhoenix || (HostileTarget.IsBossFromIcon() && HostileTarget.IsDying())) && EnkindleBahamut.CanUse(out act, CanUseOption.MustUse)) return true;
		//Burst second part for bahamut
		if ((InBahamut && SummonBahamut.ElapsedOneChargeAfterGCD(3) || (HostileTarget.IsBossFromIcon() && HostileTarget.IsDying())) && DeathFlare.CanUse(out act, CanUseOption.MustUse)) return true;
		//Change rekindle timing to avoid triple weaving issue if animation are unlocked
		if (InPhoenix && SummonBahamut.ElapsedOneChargeAfterGCD(1) && Rekindle.CanUse(out act, CanUseOption.MustUse)) return true;
		//Special Titan
		if (MountainBuster.CanUse(out act, CanUseOption.MustUse)) return true;

		//Painflare timing for tincture and rotation
		if ((Player.HasStatus(false, StatusID.SearingLight) && InBahamut && (SummonBahamut.ElapsedOneChargeAfterGCD(4) || ((!EnergyDrain.IsCoolingDown || EnergyDrain.ElapsedAfter(50))) && SummonBahamut.ElapsedOneChargeAfterGCD(1)) ||
			!SearingLight.EnoughLevel || (HostileTarget.IsBossFromIcon() && HostileTarget.IsDying())) && PainFlare.CanUse(out act)) return true;
		//fester timing for tincture and rotation
		if ((Player.HasStatus(false, StatusID.SearingLight) && InBahamut && (SummonBahamut.ElapsedOneChargeAfterGCD(4) || ((!EnergyDrain.IsCoolingDown || EnergyDrain.ElapsedAfter(50))) && SummonBahamut.ElapsedOneChargeAfterGCD(1)) ||
			!SearingLight.EnoughLevel || (HostileTarget.IsBossFromIcon() && HostileTarget.IsDying())) && Fester.CanUse(out act)) return true;

		//energy siphon recharge
		if (AetherCharge.CurrentCharges == 0 && EnergySiphon.CanUse(out act)) return true;
		//energy drain recharge
		if (AetherCharge.CurrentCharges == 0 && EnergyDrain.CanUse(out act)) return true;
		return false;
		#endregion
	}

	protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
	{
		act = null;

		#region PvP
		if (Configs.GetBool("GuardCancel") && Player.HasStatus(true, StatusID.PvP_Guard)) return false;

		if (GetPlayerHealthPercent() < 50 && PvP_RadiantAegis.CanUse(out act, CanUseOption.MustUse)) return true;
		if (Configs.GetBool("UseBahamutPvP") && PvP_SummonBahamut.CanUse(out act, CanUseOption.MustUse)) return true;
		if (Configs.GetBool("UsePhoenixPvP") && PvP_SummonPhoenix.CanUse(out act, CanUseOption.MustUse)) return true;
		if (Player.HasStatus(true, StatusID.PvP_DreadwyrmTrance))
		{
			if (PvP_EnkindleBahamut.CanUse(out act, CanUseOption.MustUse)) return true;
		}
		if (Player.HasStatus(true, StatusID.PvP_FirebirdTrance))
		{
			if (PvP_EnkindlePhoenix.CanUse(out act, CanUseOption.MustUse)) return true;
		}
		if (PvP_MountainBuster.CanUse(out act, CanUseOption.MustUse)) return true;
		if (PvP_Fester.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
		#endregion

		#region PvE
		// Adding tincture timing to rotations
		if (((SearingLight.IsInCooldown || Player.HasStatus(false, StatusID.SearingLight)) && InBahamut) && (UseBurstMedicine(out act))) return true;

		// moved swift usage on emergency to avoid unsended swift
		switch (Configs.GetCombo("addSwiftcast"))
		{
		default:
			break;
		case 1:
			if (InGaruda && Player.Level > 86)
			{
				if (Swiftcast.CanUse(out act, CanUseOption.MustUse)) return true;
			}
			break;
		case 2:
			if (InIfrit)
			{
				if (Swiftcast.CanUse(out act, CanUseOption.MustUse)) return true;
			}
			break;
		case 3:
			if ((InGaruda && Player.Level > 86) || InIfrit)
			{
				if (Swiftcast.CanUse(out act, CanUseOption.MustUse)) return true;
			}
			break;
		}

		if (Configs.GetBool("RadiantOnCooldown") && (RadiantAegis.CurrentCharges == 2) && SummonBahamut.IsCoolingDown && RadiantAegis.CanUse(out act, CanUseOption.MustUse)) return true;
		if (Configs.GetBool("RadiantOnCooldown") && Player.Level < 88 && SummonBahamut.IsCoolingDown && RadiantAegis.CanUse(out act, CanUseOption.MustUseEmpty)) return true;

		return base.EmergencyAbility(nextGCD, out act);
		#endregion
	}

	protected override IAction CountDownAction(float remainTime)
	{
		if (SummonCarbuncle.CanUse(out _)) return SummonCarbuncle;
		//1.5s prepull ruin 
		if (remainTime <= Ruin.CastTime + CountDownAhead
			&& Ruin.CanUse(out _)) return Ruin;
		return base.CountDownAction(remainTime);
	}
}