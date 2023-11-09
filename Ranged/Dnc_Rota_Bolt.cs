using Dalamud.Game.ClientState.Objects.Types;

namespace IcWaRotations.Ranged
{
	public sealed class DncRotation : DNC_Base
	{
		public override string GameVersion => "6.51";

		public override string RotationName => "Bolt Incognito Dancer Revived";

		public override string Description => "\n\n Bolt Dancer rotation, revived and modified by Incognito\n\n Better Technical Finishes! \n\nRest is same as Default";

		public override CombatType Type => CombatType.PvE;

		protected override IAction CountDownAction(float remainTime)
		{
			IAction act;
			return remainTime <= 15.0 && (StandardStep.CanUse(out act, CanUseOption.MustUse) || ExecuteStepGCD(out act)) ? act : base.CountDownAction(remainTime);
		}

		protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
		{
			act = null;
			return TechnicalStep.ElapsedAfter(2f) && Devilment.CanUse(out act);
		}

		protected override bool AttackAbility(out IAction act)
		{
			act = null;
			if (IsDancing)
				return false;
			if (Devilment.CanUse(out act))
			{
				if (IsBurst && !TechnicalStep.EnoughLevel)
					return true;
				if (StatusHelper.HasStatus(Player, true, StatusID.TechnicalFinish))
					return true;
			}
			if (UseClosedPosition(out act) || Flourish.CanUse(out act) || FanDance3.CanUse(out act, CanUseOption.MustUse))
				return true;
			return (StatusHelper.HasStatus(Player, true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel) && (FanDance2.CanUse(out act) || FanDance.CanUse(out act)) || FanDance4.CanUse(out act, CanUseOption.MustUse) && (!TechnicalStep.EnoughLevel || !TechnicalStep.IsCoolingDown || !TechnicalStep.WillHaveOneChargeGCD());
		}

		protected override bool GeneralGCD(out IAction act)
		{
			if (!InCombat)
			{
				if (!StatusHelper.HasStatus(Player, true, StatusID.ClosedPosition1) && ClosedPosition.CanUse(out act))
					return true;
			}
			if (FinishStepGCD(out act) || ExecuteStepGCD(out act) || IsBurst && InCombat && TechnicalStep.CanUse(out act, CanUseOption.MustUse))
				return true;
			return AttackGCD(out act, (StatusHelper.HasStatus(Player, true, StatusID.Devilment) ? 1 : 0) != 0);
		}

		private bool AttackGCD(out IAction act, bool breaking)
		{
			act = null;
			return !IsDancing && ((breaking || Esprit >= 85) && SaberDance.CanUse(out act, CanUseOption.MustUse) || Tillana.CanUse(out act, CanUseOption.MustUse) || StarFallDance.CanUse(out act, CanUseOption.MustUse) || UseStandardStep(out act) || BloodShower.CanUse(out act) || FountainFall.CanUse(out act) || RisingWindmill.CanUse(out act) || ReverseCascade.CanUse(out act) || BladeShower.CanUse(out act)
				|| Windmill.CanUse(out act) || Fountain.CanUse(out act) || Cascade.CanUse(out act));
		}

		private bool UseStandardStep(out IAction act)
		{
			if (!StandardStep.CanUse(out act, CanUseOption.MustUse))
				return false;
			if (StatusHelper.WillStatusEndGCD(Player, 2U, 0.0f, true, StatusID.StandardFinish))
				return true;
			if (!HasHostilesInRange)
				return false;
			if (TechnicalStep.EnoughLevel)
			{
				if (StatusHelper.HasStatus(Player, true, StatusID.TechnicalFinish) || TechnicalStep.IsCoolingDown && TechnicalStep.WillHaveOneCharge(5f))
					return false;
			}
			return true;
		}

		private bool UseClosedPosition(out IAction act)
		{
			if (!ClosedPosition.CanUse(out act))
				return false;
			if (InCombat)
			{
				if (StatusHelper.HasStatus(Player, true, StatusID.ClosedPosition1))
				{
					foreach (BattleChara partyMember in PartyMembers)
					{
						if (StatusHelper.HasStatus(partyMember, true, StatusID.ClosedPosition2))
						{
							if (ClosedPosition.Target == partyMember)
								return true;
							break;
						}
					}
				}
			}
			act = null;
			return false;
		}

		private static bool FinishStepGCD(out IAction act)
		{
			act = null;
			if (!IsDancing)
				return false;
			if (StatusHelper.HasStatus(Player, true, StatusID.StandardStep))
			{
				if (!StatusHelper.WillStatusEnd(Player, 1f, true, StatusID.StandardStep))
				{
					if (CompletedSteps == 2)
					{
						if (StatusHelper.WillStatusEnd(Player, 1f, true, StatusID.StandardFinish))
							goto label_7;
					}
				}
				else
					goto label_7;
			}
			IAction act1;
			if (!StandardFinish.CanUse(out act1, CanUseOption.MustUse))
			{
				if (StatusHelper.HasStatus(Player, true, StatusID.TechnicalStep))
				{
					if (StatusHelper.WillStatusEnd(Player, 1f, true, StatusID.TechnicalStep))
						goto label_11;
				}
				if (!TechnicalFinish.CanUse(out act1, CanUseOption.MustUse))
					return false;
				label_11:
				act = TechnicalStep;
				return true;
			}
			label_7:
			act = StandardStep;
			return true;
		}
	}
}