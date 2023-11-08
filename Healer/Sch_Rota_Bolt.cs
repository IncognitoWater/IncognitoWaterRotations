using Dalamud.Game.ClientState.Objects.Types;

namespace IcWaRotations.Healer
{
  [RotationDesc(ActionID.ChainStratagem)]
  public sealed class SchRotation : SCH_Base
  {
    private readonly bool _cantSacredSoil = !HasAetherflow || SacredSoil.IsCoolingDown || SacredSoil.ElapsedAfter(2f);

    public override string GameVersion => "6.51";

    public override string RotationName => "Bolt Incognito Scholar Revived";

    public override string Description => "\n\n Bolt Scholar rotation,revived and modified by Incognito\n\nWill only waste a GCD on Succor to heal/mitigate as a last resort! \r\nAdlo -> Deployment Tactics will be used as a new area defense tool. \r\nProtraction will be used for Tank busters";
    
    public override CombatType Type => CombatType.PvE;

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration().SetBool(CombatType.PvE,"GCDHeal", false, "Use spells with cast times to heal.");
    

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
      if (IsLastGCD(true, Adloquium) && PartyMembers.Count() > 1 && DeploymentTactics.CanUse(out act))
        return true;
      if (nextGCD.IsTheSameTo(true, Succor, Adloquium) && Recitation.CanUse(out act))
        return true;
      foreach (BattleChara partyMember in PartyMembers)
      {
        if (partyMember.GetHealthRatio() >= 0.9)
        {
          if (partyMember.HasStatus(true, StatusID.Aetherpact))
          {
            act = Aetherpact;
            return true;
          }
        }
      }
      return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction act) => SummonEos.CanUse(out act) || Bio.CanUse(out act) || ArtOfWar.CanUse(out act) || Ruin.CanUse(out act) || Ruin2.CanUse(out act) || Bio.CanUse(out act, CanUseOption.MustUse);

    [RotationDesc(ActionID.Adloquium, ActionID.Physick)]
    protected override bool HealSingleGCD(out IAction act) => Adloquium.CanUse(out act) || Physick.CanUse(out act);

    [RotationDesc(ActionID.Aetherpact, ActionID.Excogitation, ActionID.Lustrate, ActionID.Aetherpact)]
    protected override bool HealSingleAbility(out IAction act)
    {
      bool flag = PartyMembers.Any((Func<BattleChara, bool>) (p => p.HasStatus(true, StatusID.Aetherpact)));
      return Aetherpact.CanUse(out act) && FairyGauge >= 70 && !flag || Excogitation.CanUse(out act) || Lustrate.CanUse(out act) || Aetherpact.CanUse(out act) && !flag || base.HealSingleAbility(out act);
    }

    [RotationDesc(ActionID.Excogitation, ActionID.Protraction)]
    protected override bool DefenseSingleAbility(out IAction act) => Protraction.CanUse(out act) || Protraction.ElapsedAfter(5f) && Excogitation.CanUse(out act) || Protraction.ElapsedAfter(5f) && Excogitation.ElapsedAfter(5f) && SacredSoil.CanUse(out act);

    [RotationDesc(ActionID.Succor)]
    protected override bool HealAreaGCD(out IAction act)
    {
      if (Succor.CanUse(out act))
        return true;
      act = null;
      return false;
    }

    [RotationDesc(ActionID.SummonSeraph, ActionID.Consolation, ActionID.WhisperingDawn, ActionID.Indomitability)]
    protected override bool HealAreaAbility(out IAction act)
    {
      if ((WhisperingDawn.ElapsedOneChargeAfterGCD(1U) || FeyIllumination.ElapsedOneChargeAfterGCD(1U) || FeyBlessing.ElapsedOneChargeAfterGCD(1U)) && SummonSeraph.CanUse(out act) || Consolation.CanUse(out act, CanUseOption.EmptyOrSkipCombo) || FeyBlessing.CanUse(out act) || !FeyBlessing.CanUse(out act) && WhisperingDawn.CanUse(out act) || Indomitability.CanUse(out act))
        return true;
      act = null;
      return false;
    }

    [RotationDesc(ActionID.Succor)]
    protected override bool DefenseAreaGCD(out IAction act)
    {
      if (SummonSeraph.ElapsedAfter(10f) && _cantSacredSoil && Expedient.IsCoolingDown && FeyIllumination.IsCoolingDown && (!DeploymentTactics.IsCoolingDown && PartyMembers.Count() > 1 && Adloquium.CanUse(out act) || Succor.CanUse(out act)))
        return true;
      act = null;
      return false;
    }

    [RotationDesc(ActionID.FeyIllumination, ActionID.Expedient, ActionID.SummonSeraph, ActionID.Consolation, ActionID.SacredSoil, ActionID.DeploymentTactics)]
    protected override bool DefenseAreaAbility(out IAction act) => HasAetherflow && SacredSoil.CanUse(out act, CanUseOption.MustUse) || _cantSacredSoil && FeyIllumination.CanUse(out act) || _cantSacredSoil && FeyIllumination.ElapsedAfter(3f) && Expedient.CanUse(out act) || (WhisperingDawn.ElapsedOneChargeAfterGCD(1U) || FeyIllumination.ElapsedOneChargeAfterGCD(1U) || FeyBlessing.ElapsedOneChargeAfterGCD(1U)) && SummonSeraph.CanUse(out act) || Consolation.CanUse(out act, CanUseOption.EmptyOrSkipCombo);

    protected override bool AttackAbility(out IAction act)
    {
      if (IsBurst && ChainStratagem.CanUse(out act) || (Dissipation.EnoughLevel && Dissipation.WillHaveOneChargeGCD(3U) && Dissipation.IsEnabled || Aetherflow.WillHaveOneChargeGCD(3U)) && EnergyDrain.CanUse(out act, CanUseOption.EmptyOrSkipCombo) || HostileTarget.IsBossFromIcon() && Dissipation.CanUse(out act) || Aetherflow.CanUse(out act))
        return true;
      act = null;
      return false;
    }

    protected override IAction CountDownAction(float remainTime)
    {
      IAction act;
      return remainTime < Ruin.CastTime + (double) CountDownAhead && Ruin.CanUse(out act) || remainTime < 3.0 && UseBurstMedicine(out act) ? act : base.CountDownAction(remainTime);
    }
  }
}
