using RotationSolver.Basic.Configuration;

namespace IcWaRotations.Healer
{
	[RotationDesc(ActionID.Divination)]
	public sealed class AstRotation : AST_Base
	{
		private readonly byte _aoeCountcheck = PartyMembers.Count() > 4 ? (byte)5 : (byte)3;

		public override string GameVersion => "6.51";

		public override string RotationName => "Bolt Incognito Astrologian Revived";

		public override string Description => "Bolt Astro revived and modified by Incognito";

		public override CombatType Type => CombatType.PvE;

		private static IBaseAction MicroCosmos { get; } = new BaseAction(ActionID.Microcosmos, ActionOption.Friendly);

		private static IBaseAction EarthlyDetonate { get; } = new BaseAction((ActionID)8324);

		protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
			.SetFloat(ConfigUnitType.Seconds, CombatType.PvE, "UseEarthlyStarTime", 15f, "Use Earthly Star during countdown timer.", 4f, 20f)
			.SetBool(CombatType.PvE, "BOSS_Divination", false, "Save Divination for Bosses.")
			.SetBool(CombatType.PvE, "Aspected_Spam", false, "Don't spam Aspected Benefic on Tank during Boss fights. (Better for your DPS)");

		private static IBaseAction AspectedBeneficDefense { get; } = new BaseAction(ActionID.AspectedBenefic, ActionOption.Friendly | ActionOption.Eot)
		{
			ChoiceTarget = TargetFilter.FindAttackedTarget,
			ActionCheck = (b, m) => b.IsJobCategory(JobRole.Tank),
			TargetStatus = new StatusID[1]
			{
				StatusID.AspectedBenefic
			}
		};

		private static IBaseAction LordCard { get; } = new BaseAction((ActionID)7444)
		{
			AOECount = 1
		};

		private static IBaseAction LadyCard { get; } = new BaseAction(ActionID.EnkindleBahamut | ActionID.ShieldBash)
		{
			AOECount = 1
		};

		protected override IAction CountDownAction(float remainTime)
		{
			IAction act;
			return remainTime < Malefic.CastTime + (double)CountDownAhead && Malefic.CanUse(out act, CanUseOption.IgnoreClippingCheck) || remainTime < 3.0 && UseBurstMedicine(out act) || remainTime < 4.0 && AspectedBeneficDefense.CanUse(out act, CanUseOption.IgnoreClippingCheck) || remainTime < (double)Configs.GetFloat("UseEarthlyStarTime") && EarthlyStar.CanUse(out act, CanUseOption.IgnoreClippingCheck)
				|| remainTime < 30.0 && Draw.CanUse(out act, CanUseOption.IgnoreClippingCheck)
					? act
					: base.CountDownAction(remainTime);
		}

		[RotationDesc(ActionID.CelestialIntersection, ActionID.Exaltation)]
		protected override bool DefenseSingleAbility(out IAction act) => Exaltation.CanUse(out act) || Exaltation.ElapsedAfter(3f) && CelestialIntersection.CanUse(out act, CanUseOption.EmptyOrSkipCombo);

		[RotationDesc(ActionID.Macrocosmos)]
		protected override bool DefenseAreaGCD(out IAction act)
		{
			if (Macrocosmos.CanUse(out act, aoeCount: 1))
				return true;
			act = null;
			return false;
		}

		[RotationDesc(ActionID.CollectiveUnconscious)]
		protected override bool DefenseAreaAbility(out IAction act)
		{
			if (Macrocosmos.IsCoolingDown)
			{
				if (!Player.HasStatus(true, StatusID.Macrocosmos) && CollectiveUnconscious.CanUse(out act))
					return true;
			}
			if (!Macrocosmos.EnoughLevel && CollectiveUnconscious.CanUse(out act))
				return true;
			act = null;
			return false;
		}

		protected override bool GeneralGCD(out IAction act)
		{
			if (CanUseAspected(out act) && AspectedBeneficDefense.CanUse(out act) || Gravity.CanUse(out act) || Combust.CanUse(out act) || Malefic.CanUse(out act) || Combust.CanUse(out act, CanUseOption.MustUse))
				return true;
			act = null;
			return false;
		}

		private bool CanUseAspected(out IAction act)
		{
			act = null;
			return InCombat && !HostileTarget.IsBossFromIcon() || InCombat && !Configs.GetBool("Aspected_Spam");
		}

		[RotationDesc(ActionID.AspectedHelios, ActionID.Helios)]
		protected override bool HealAreaGCD(out IAction act)
		{
			if (!Player.HasStatus(true, StatusID.Macrocosmos) && (AspectedHelios.CanUse(out act, aoeCount: _aoeCountcheck) || Helios.CanUse(out act, aoeCount: _aoeCountcheck)))
				return true;
			act = null;
			return false;
		}

		private bool CanUseDraw(out IAction act)
		{
			act = null;
			if (!InCombat || !HostileTarget.IsBossFromIcon() || !Divination.EnoughLevel)
				return true;
			return Player.HasStatus(true, StatusID.Divination) || Divination.IsCoolingDown && Divination.ElapsedAfter(115f) || Divination.IsCoolingDown && !Divination.ElapsedAfter(60f) || Divination.IsCoolingDown && Draw.CurrentCharges == 2 && !Divination.ElapsedAfter(91f);
		}

		private bool CanUsePlay(out IAction act)
		{
			act = null;
			if (!HostileTarget.IsBossFromIcon() || !Divination.EnoughLevel)
				return true;
			return Player.HasStatus(true, StatusID.Divination) || Divination.IsCoolingDown && Divination.ElapsedAfter(115f) || Divination.IsCoolingDown && !Divination.ElapsedAfter(60f) || Draw.CurrentCharges == 2 && DrawnCard != null && !Divination.ElapsedAfter(90f);
		}

		protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
		{
			if (!CombatElapsedLessGCD(4))
			{
				if (Configs.GetBool("BOSS_Divination") && HostileTarget.IsBossFromIcon())
				{
					if (Divination.CanUse(out act, CanUseOption.MustUse))
						return true;
				}
				else if (!Configs.GetBool("BOSS_Divination") && Divination.CanUse(out act, CanUseOption.MustUse))
					return true;
			}
			if (Player.CurrentMp <= 4000U && LucidDreaming.CanUse(out act) || InCombat && Divination.WillHaveOneChargeGCD(2U) && HostileTarget.IsBossFromIcon() && Lightspeed.CanUse(out act) || MinorArcana.CanUse(out act) || MinorArcana.WillHaveOneChargeGCD(1U) && MinorArcana.CanUse(out act, CanUseOption.MustUse))
				return true;
			if (Player.HasStatus(true, StatusID.Pacification | StatusID.PvP_EnAvant) && !HostileTarget.IsBossFromIcon() && LordCard.CanUse(out act, aoeCount: 2))
				return true;
			if (Player.HasStatus(true, StatusID.Pacification | StatusID.PvP_EnAvant))
			{
				if (Player.HasStatus(true, StatusID.Divination) && LordCard.CanUse(out act, aoeCount: 1))
					return true;
			}
			if (Player.HasStatus(true, StatusID.Pacification | StatusID.PvP_EnAvant) && Divination.IsInCooldown && !Divination.ElapsedAfter(60f) && LordCard.CanUse(out act, aoeCount: 1) || base.EmergencyAbility(nextGCD, out act))
				return true;
			if (PartyHealers.Count() == 1)
			{
				if (Player.HasStatus(false, StatusID.Silence) && HasHostilesInRange && EchoDrops.CanUse(out act))
					return true;
			}
			if (!InCombat)
				return false;
			if (nextGCD.IsTheSameTo(true, AspectedHelios, Helios) && (Horoscope.CanUse(out act) || NeutralSect.CanUse(out act)))
				return true;
			return nextGCD.IsTheSameTo(true, Benefic, Benefic2, AspectedBenefic) && Synastry.CanUse(out act);
		}

		protected override bool GeneralAbility(out IAction act)
		{
			if (Player.HasStatus(true, StatusID.Divination) && Astrodyne.CanUse(out act))
				return true;
			if (!IsMoving && InCombat && !StopMovingElapsedLess(3f))
			{
				if (!Player.HasStatus(true, StatusID.GiantDominance) && EarthlyStar.CanUse(out act, CanUseOption.MustUse))
					return true;
			}
			if (InCombat && CanUsePlay(out act) && PlayCard(out act) || CanUseDraw(out act) && DrawnCard == null && Draw.CanUse(out act, CanUseOption.MustUseEmpty) || Redraw.CanUse(out act))
				return true;
			act = null;
			return false;
		}

		[RotationDesc(ActionID.AspectedBenefic, ActionID.Benefic2, ActionID.Benefic)]
		protected override bool HealSingleGCD(out IAction act)
		{
			if (AspectedBenefic.CanUse(out act) && (IsMoving || AspectedBenefic.Target.GetHealthRatio() > 0.4) || Benefic2.CanUse(out act) || Benefic.CanUse(out act))
				return true;
			act = null;
			return false;
		}

		protected override bool AttackAbility(out IAction act)
		{
			act = null;
			return false;
		}

		[RotationDesc(ActionID.EssentialDignity, ActionID.CelestialIntersection, ActionID.Horoscope)]
		protected override bool HealSingleAbility(out IAction act)
		{
			if (EssentialDignity.CanUse(out act) || CelestialIntersection.CanUse(out act, CanUseOption.EmptyOrSkipCombo))
				return true;
			return Player.HasStatus(true, StatusID.Silence | StatusID.PvP_EnAvant) && LadyCard.CanUse(out act, CanUseOption.MustUse) || base.HealSingleAbility(out act);
		}

		[RotationDesc(ActionID.CelestialOpposition, ActionID.EarthlyStar, ActionID.Horoscope)]
		protected override bool HealAreaAbility(out IAction act)
		{
			if (Player.HasStatus(true, StatusID.GiantDominance) && EarthlyDetonate.CanUse(out act, CanUseOption.MustUse))
				return true;
			if (Player.HasStatus(true, StatusID.Silence | StatusID.PvP_EnAvant) && LadyCard.CanUse(out act, aoeCount: _aoeCountcheck))
				return true;
			if (Player.HasStatus(true, StatusID.Macrocosmos) && MicroCosmos.CanUse(out act) || CelestialOpposition.CanUse(out act, aoeCount: _aoeCountcheck))
				return true;
			if (Player.HasStatus(true, StatusID.HoroscopeHelios) && Horoscope.CanUse(out act, aoeCount: _aoeCountcheck))
				return true;
			act = null;
			return false;
		}
	}
}