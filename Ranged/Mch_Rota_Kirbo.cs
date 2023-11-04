namespace IcWaRotations.Ranged;

[RotationDesc(ActionID.Wildfire)]
[LinkDescription("https://i.imgur.com/vekKW2k.jpg", "Delayed Tools")]
public class MchRotationKirbo : MCH_Base
{

	public override string GameVersion => "6.51";

	public override string RotationName => "Kirbo's Machinist Delayed Tools";

	public override string Description => "Kirbo's Machinist, just updated some things code wise, can only do delayed tools. \n Should be optimised for Boss Level 90 content with 2.5 GCD.";

	private bool InBurst { get; set; }


	public override bool ShowStatus => true;

	private int Openerstep { get; set; }


	private bool OpenerHasFinished { get; set; }


	private bool OpenerHasFailed { get; set; }


	private bool OpenerActionsAvailable { get; set; }


	private bool OpenerInProgress { get; set; }


	private bool SafeToUseWildfire { get; } = false;


	private bool WillhaveTool { get; set; }

	private bool Flag { get; set; }


	protected override IRotationConfigSet CreateConfiguration()
	{
		return base.CreateConfiguration()
			.SetBool("BatteryStuck", false, "Battery overcap protection\n(Will try and use Rook AutoTurret if Battery is at 100 and next skill increases Battery)")
			.SetBool("HeatStuck", false, "Heat overcap protection\n(Will try and use HyperCharge if Heat is at 100 and next skill increases Heat)")
			.SetBool("DumpSkills", true, "Dump Skills when Target is dying\n(Will try and spend remaining resources before boss dies)");
	}

	protected override IAction CountDownAction(float remainTime)
	{
		if (OpenerActionsAvailable)
		{
			if (remainTime <= SplitShot.AnimationLockTime && SplitShot.CanUse(out _))
			{
				OpenerInProgress = true;
				return SplitShot;
			}
			IAction act1;
			if (remainTime <= SplitShot.AnimationLockTime + TinctureOfDexterity8.AnimationLockTime + 0.2 && UseBurstMedicine(out act1, false))
			{
				return act1;
			}
		}
		if (Player.Level < 90)
		{
			if (AirAnchor.EnoughLevel && remainTime <= 0.6 + CountDownAhead && AirAnchor.CanUse(out _))
			{
				return AirAnchor;
			}
			if (!AirAnchor.EnoughLevel && Drill.EnoughLevel && remainTime <= 0.6 + CountDownAhead && Drill.CanUse(out _))
			{
				return Drill;
			}
			if (!AirAnchor.EnoughLevel && !Drill.EnoughLevel && HotShot.EnoughLevel && remainTime <= 0.6 + CountDownAhead && HotShot.CanUse(out _))
			{
				return HotShot;
			}
			if (!AirAnchor.EnoughLevel && !Drill.EnoughLevel && !HotShot.EnoughLevel && remainTime <= 0.6 + CountDownAhead && CleanShot.CanUse(out _))
			{
				return CleanShot;
			}
			if (remainTime < 5f && Reassemble.CurrentCharges > 0)
			{
				return Reassemble;
			}
		}
		return base.CountDownAction(remainTime);
	}

	private bool Opener(out IAction act)
	{
		act = default(IAction);
		byte OverHeatStacks = StatusHelper.StatusStack(Player, true, (StatusID)2688);
		while (OpenerInProgress && (!OpenerHasFinished || !OpenerHasFailed))
		{
			if (TimeSinceLastAction.TotalSeconds > 3.0 && !Flag)
			{
				OpenerHasFailed = true;
				OpenerInProgress = false;
				Openerstep = 0;
				// PluginLog.Warning("Opener Failed Reason: 'Time Since Last Action more then 3 seconds'", Array.Empty<object>());
				// PluginLog.Debug("openerstep is now: {Openerstep}", Array.Empty<object>());
				// PluginLog.Debug("opener is no longer in progress", Array.Empty<object>());
				Flag = true;
			}
			if (Player.IsDead && !Flag)
			{
				OpenerHasFailed = true;
				OpenerInProgress = false;
				Openerstep = 0;
				// PluginLog.Warning("Opener Failed Reason: 'You died'", Array.Empty<object>());
				// PluginLog.Debug($"openerstep is now: {Openerstep}", Array.Empty<object>());
				// PluginLog.Debug("opener is no longer in progress", Array.Empty<object>());
				Flag = true;
			}
			switch (Openerstep)
			{
			case 0:
				return OpenerStep(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, (CanUseOption)1));
			case 1:
				return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, (CanUseOption)3));
			case 2:
				return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, (CanUseOption)3));
			case 3:
				return OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, (CanUseOption)1));
			case 4:
				return OpenerStep(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, (CanUseOption)1));
			case 5:
				return OpenerStep(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, (CanUseOption)1));
			case 6:
				return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, (CanUseOption)3));
			case 7:
				return OpenerStep(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, (CanUseOption)1));
			case 8:
				return OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, (CanUseOption)3));
			case 9:
				return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, (CanUseOption)3));
			case 10:
				return OpenerStep(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, (CanUseOption)1));
			case 11:
				return OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, (CanUseOption)3));
			case 12:
				return OpenerStep(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, (CanUseOption)17));
			case 13:
				return OpenerStep(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, (CanUseOption)1));
			case 14:
				return OpenerStep(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, (CanUseOption)1));
			case 15:
				return OpenerStep(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, (CanUseOption)51));
			case 16:
				return OpenerStep(IsLastGCD(false, HeatBlast) && OverHeatStacks == 4, HeatBlast.CanUse(out act, (CanUseOption)1));
			case 17:
				return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, (CanUseOption)3));
			case 18:
				return OpenerStep(IsLastGCD(false, HeatBlast) && OverHeatStacks == 3, HeatBlast.CanUse(out act, (CanUseOption)1));
			case 19:
				return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, (CanUseOption)3));
			case 20:
				return OpenerStep(IsLastGCD(false, HeatBlast) && OverHeatStacks == 2, HeatBlast.CanUse(out act, (CanUseOption)1));
			case 21:
				return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, (CanUseOption)3));
			case 22:
				return OpenerStep(IsLastGCD(false, HeatBlast) && OverHeatStacks == 1, HeatBlast.CanUse(out act, (CanUseOption)1));
			case 23:
				return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, (CanUseOption)3));
			case 24:
				return OpenerStep(IsLastGCD(false, HeatBlast) && OverHeatStacks == 0, HeatBlast.CanUse(out act, (CanUseOption)1));
			case 25:
				return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, (CanUseOption)3));
			case 26:
				return OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, (CanUseOption)1));
			case 27:
				OpenerHasFinished = true;
				OpenerInProgress = false;
				// PluginLog.Debug("Succesfully completed Opener: 'Delayed Tools'", Array.Empty<object>());
				break;
			}
		}
		act = null;
		return false;
	}

	private bool OpenerStep(bool condition, bool result)
	{
		if (condition)
		{
			Openerstep++;
			return false;
		}
		return result;
	}

	protected override bool GeneralGCD(out IAction act)
	{
		if (OpenerInProgress)
		{
			return Opener(out act);
		}
		if (!OpenerInProgress || OpenerHasFailed || OpenerHasFinished)
		{
			if (AutoCrossbow.CanUse(out act, (CanUseOption)1, 3) && ObjectHelper.DistanceToPlayer(HostileTarget) <= 12f)
			{
				return true;
			}
			if (HeatBlast.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (ObjectHelper.GetHealthRatio(HostileTarget) > 0.6 && BioBlaster.CanUse(out act, (CanUseOption)1, 3) && ObjectHelper.DistanceToPlayer(HostileTarget) <= 12f)
			{
				return true;
			}
			if (Drill.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (AirAnchor.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (!AirAnchor.EnoughLevel && HotShot.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (ChainSaw.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (SpreadShot.CanUse(out act, (CanUseOption)1, 3))
			{
				return true;
			}
			if (CleanShot.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (SlugShot.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (SplitShot.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
		}
		act = null;
		return false;
	}


	protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
	{
		TerritoryContentType Content = TerritoryContentType;
		bool Dungeon = (int)Content == 2;
		bool Roulette = (int)Content == 1;
		bool Deepdungeon = (int)Content == 21;
		bool VCDungeonFinder = (int)Content == 30;
		bool FATEs = (int)Content == 8;
		bool Eureka = (int)Content == 26;
		UseBurstMedicine(out act, false);
		if ((!TinctureOfDexterity8.IsCoolingDown || !TinctureOfDexterity7.IsCoolingDown || !TinctureOfDexterity6.IsCoolingDown) && !StatusHelper.HasStatus(Player, false, (StatusID)1946) && Wildfire.WillHaveOneCharge(20f) && !InBurst && UseBurstMedicine(out act))
		{
			return true;
		}
		if (OpenerInProgress && !OpenerHasFailed && !OpenerHasFinished)
		{
			return Opener(out act);
		}
		if (Configs.GetBool("BatteryStuck") && !OpenerInProgress && Battery == 100 && RookAutoturret.CanUse(out act, (CanUseOption)3) && (nextGCD == ChainSaw || nextGCD == AirAnchor || nextGCD == CleanShot))
		{
			return true;
		}
		if (Configs.GetBool("HeatStuck") && !OpenerInProgress && Heat == 100 && Hypercharge.CanUse(out act, (CanUseOption)3) && (nextGCD == SplitShot || nextGCD == SlugShot || nextGCD == CleanShot))
		{
			return true;
		}
		if (Configs.GetBool("DumpSkills") && HostileTarget.IsDying() && HostileTarget.IsBoss())
		{
			if (!StatusHelper.HasStatus(Player, false, (StatusID)851) && Reassemble.CanUse(out act, (CanUseOption)2) && Reassemble.CurrentCharges > 0 && (nextGCD == ChainSaw || nextGCD == AirAnchor || nextGCD == Drill))
			{
				return true;
			}
			if (BarrelStabilizer.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (AirAnchor.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (ChainSaw.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (Drill.CanUse(out act, (CanUseOption)1))
			{
				return true;
			}
			if (RookAutoturret.CanUse(out act, (CanUseOption)1) && Battery >= 50)
			{
				return true;
			}
			if (Hypercharge.CanUse(out act) && !WillhaveTool && Heat >= 50)
			{
				return true;
			}
			if (ObjectHelper.GetHealthRatio(HostileTarget) < 0.03 && nextGCD == CleanShot && Reassemble.CurrentCharges > 0 && Reassemble.CanUse(out act, (CanUseOption)35))
			{
				return true;
			}
			if (ObjectHelper.GetHealthRatio(HostileTarget) < 0.03 && RookAutoturret.ElapsedAfter(5f) && QueenOverdrive.CanUse(out act))
			{
				return true;
			}
			if (ObjectHelper.GetHealthRatio(HostileTarget) < 0.02 && (StatusHelper.HasStatus(Player, false, (StatusID)1946) || InBurst) && Wildfire.ElapsedAfter(5f) && Detonator.CanUse(out act))
			{
				return true;
			}
		}
		if ((!OpenerInProgress || OpenerHasFailed || OpenerHasFinished) && Player.Level >= 90)
		{
			if (Wildfire.CanUse(out act, (CanUseOption)16) && nextGCD == ChainSaw && Heat >= 50)
			{
				return true;
			}
			if (BarrelStabilizer.CanUse(out act, (CanUseOption)3))
			{
				if (Wildfire.IsCoolingDown && IsLastGCD((ActionID)16498))
				{
					return true;
				}
				return true;
			}
			if (Reassemble.CanUse(out act, (CanUseOption)3) && !StatusHelper.HasStatus(Player, true, (StatusID)851))
			{
				if (IActionHelper.IsTheSameTo(nextGCD, true, ChainSaw))
				{
					return true;
				}
				if ((IActionHelper.IsTheSameTo(nextGCD, true, AirAnchor) || IActionHelper.IsTheSameTo(nextGCD, true, Drill)) && !Wildfire.WillHaveOneCharge(55f))
				{
					return true;
				}
			}
			if (RookAutoturret.CanUse(out act, (CanUseOption)16) && HostileTarget.IsTargetable && InCombat)
			{
				if (CombatElapsedLess(60f) && !CombatElapsedLess(45f) && Battery >= 50)
				{
					return true;
				}
				if (Wildfire.IsCoolingDown && Wildfire.ElapsedAfter(105f) && Battery == 100 && (nextGCD == AirAnchor || nextGCD == CleanShot))
				{
					return true;
				}
				if (Battery >= 90 && !Wildfire.ElapsedAfter(70f))
				{
					return true;
				}
				if (Battery >= 80 && !Wildfire.ElapsedAfter(77.5f) && IsLastGCD((ActionID)16500))
				{
					return true;
				}
			}
			if (Hypercharge.CanUse(out act) && !WillhaveTool)
			{
				if (InBurst && IsLastGCD((ActionID)25788))
				{
					return true;
				}
				if (Heat >= 100 && Wildfire.WillHaveOneCharge(10f))
				{
					return true;
				}
				if (Heat >= 90 && Wildfire.WillHaveOneCharge(40f))
				{
					return true;
				}
				if (Heat >= 50 && !Wildfire.WillHaveOneCharge(30f))
				{
					return true;
				}
			}
		}
		if (Deepdungeon || Eureka || Roulette || Dungeon || VCDungeonFinder || FATEs || Player.Level < 90)
		{
			if (Wildfire.CanUse(out act) && HostileTarget.IsBoss() && SafeToUseWildfire)
			{
				return true;
			}
			if (Reassemble.CurrentCharges > 0 && Reassemble.CanUse(out act, (CanUseOption)3))
			{
				if (ChainSaw.EnoughLevel && (nextGCD == ChainSaw || nextGCD == Drill || nextGCD == AirAnchor))
				{
					return true;
				}
				if (!Drill.EnoughLevel && nextGCD == CleanShot)
				{
					return true;
				}
			}
			if (BarrelStabilizer.CanUse(out act) && HostileTarget.IsTargetable && InCombat)
			{
				return true;
			}
			if (Hypercharge.CanUse(out act) && InCombat && HostileTarget.IsTargetable)
			{
				if (ObjectHelper.GetHealthRatio(HostileTarget) > 0.25)
				{
					return true;
				}
				if (HostileTarget.IsBoss())
				{
					return true;
				}
			}
			if (RookAutoturret.CanUse(out act) && HostileTarget.IsTargetable && InCombat)
			{
				if (!HostileTarget.IsBoss() && CombatElapsedLess(30f))
				{
					return true;
				}
				if (HostileTarget.IsBoss())
				{
					return true;
				}
			}
		}
		act = null;
		return false;
	}

	protected override bool AttackAbility(out IAction act)
	{
		if (OpenerInProgress)
		{
			return Opener(out act);
		}
		if (GaussRound.CurrentCharges >= Ricochet.CurrentCharges)
		{
			if (GaussRound.CanUse(out act, (CanUseOption)3))
			{
				return true;
			}
		}
		else if (Ricochet.CanUse(out act, (CanUseOption)19))
		{
			return true;
		}
		act = null;
		return false;
	}

	protected override void UpdateInfo()
	{
		HandleOpenerAvailability();
		ToolKitCheck();
		StateOfOpener();
	}

	private void ToolKitCheck()
	{
		bool WillHaveDrill = Drill.WillHaveOneCharge(5f);
		bool WillHaveAirAnchor = AirAnchor.WillHaveOneCharge(5f);
		bool WillHaveChainSaw = ChainSaw.WillHaveOneCharge(5f);
		if (Player.Level >= 90)
		{
			WillhaveTool = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;
		}

		InBurst = StatusHelper.HasStatus(Player, false, (StatusID)1946);
	}

	public void StateOfOpener()
	{
		if (Player.IsDead)
		{
			OpenerHasFailed = false;
			OpenerHasFinished = false;
			Openerstep = 0;
		}
		if (!InCombat)
		{
			OpenerHasFailed = false;
			OpenerHasFinished = false;
			Openerstep = 0;
		}
	}

	
	public void HandleOpenerAvailability()
	{
		bool Lvl90 = Player.Level >= 90;
		bool HasChainSaw = !ChainSaw.IsCoolingDown;
		bool HasBarrelStabilizer = !BarrelStabilizer.IsCoolingDown;
		bool HasRicochet = Ricochet.CurrentCharges == 3;
		bool HasWildfire = !Wildfire.IsCoolingDown;
		bool HasGaussRound = GaussRound.CurrentCharges == 3;
		bool ReassembleOneCharge = Reassemble.CurrentCharges >= 1;
		bool NoHeat = Heat == 0;
		bool NoBattery = Battery == 0;
		bool Openerstep0 = Openerstep == 0;
		OpenerActionsAvailable = ReassembleOneCharge && HasChainSaw && HasBarrelStabilizer && HasRicochet && HasWildfire && HasGaussRound && Lvl90 && NoBattery && NoHeat && Openerstep0;
	}

}