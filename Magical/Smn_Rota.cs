namespace IcWaRotations.Magical;

[RotationDesc(ActionID.SearingLight)]
[LinkDescription("https://github.com/IncognitoWater/IncognitoWaterRotations/blob/main/Magical/Smn_Rota.cs")]
public sealed class SmnRotation : SMN_Base
{
    public override string GameVersion => "6.51";

    public override string RotationName => "IncognitoWater's Summoner";

    public override string Description => "High Level Content Summoner Rotation";

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetCombo("addSwiftcast", 0, "Use Swiftcast With", "No", "Emerald", "Ruby", "All")
            .SetCombo("SummonOrder", 0, "Invocation Order", "Topaz-Emerald-Ruby", "Topaz-Ruby-Emerald", "Emerald-Topaz-Ruby")
            .SetBool("addCrimsonCyclone", true, "Use Crimson Cyclone")
            .SetBool("RadiantOnCooldown", false, "Use Radiant On Cooldown");
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
        //Spawning carbuncle + attempting to avoid unwanted try of spawning carbuncle
        if (!InBahamut && !InPhoenix && !InGaruda && !InIfrit && !InTitan && SummonCarbuncle.CanUse(out act)) return true;

        //slipstream
        if (Slipstream.CanUse(out act, CanUseOption.MustUse)) return true;
        //Crimson strike 
        if (CrimsonStrike.CanUse(out act, CanUseOption.MustUse)) return true;

        //AOE
        if (PreciousBrilliance.CanUse(out act)) return true;
        //gemshine
        if (Gemshine.CanUse(out act)) return true;

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
        if (Outburst.CanUse(out act)) return true;

        //Any ruin ( 1-2-3 ) 
        if (Ruin.CanUse(out act)) return true;
        return false;
    }

    protected override bool AttackAbility(out IAction act)
    {
        if (IsBurst && !Player.HasStatus(false, StatusID.SearingLight))
        {
            //Burst raidbuff searinglight
            if (SearingLight.CanUse(out act, CanUseOption.MustUse)) return true;
        }
        
        
        //Burst for bahamut
        if ((InBahamut && SummonBahamut.ElapsedOneChargeAfterGCD(3) || InPhoenix || (HostileTarget.IsBoss() || HostileTarget.IsDying())) && EnkindleBahamut.CanUse(out act, CanUseOption.MustUse)) return true;
        //Burst second part for bahamut
        if ((InBahamut && SummonBahamut.ElapsedOneChargeAfterGCD(3) || (HostileTarget.IsBoss() || HostileTarget.IsDying())) && DeathFlare.CanUse(out act, CanUseOption.MustUse)) return true;
        //Change rekindle timing to avoid triple weaving issue if animation are unlocked
        if (InPhoenix && SummonBahamut.ElapsedOneChargeAfterGCD(1) && Rekindle.CanUse(out act, CanUseOption.MustUse)) return true;
        //Special Titan
        if (MountainBuster.CanUse(out act, CanUseOption.MustUse)) return true;
        
        //Painflare timing for tincture and rotation
        if ((Player.HasStatus(false, StatusID.SearingLight) && InBahamut && (SummonBahamut.ElapsedOneChargeAfterGCD(4) || ((!EnergyDrain.IsCoolingDown  || EnergyDrain.ElapsedAfter(50))) && SummonBahamut.ElapsedOneChargeAfterGCD(1)) ||
            !SearingLight.EnoughLevel || (HostileTarget.IsBoss() || HostileTarget.IsDying()) ) && PainFlare.CanUse(out act)) return true;
        //fester timing for tincture and rotation
        if ((Player.HasStatus(false, StatusID.SearingLight) && InBahamut && (SummonBahamut.ElapsedOneChargeAfterGCD(4) || ((!EnergyDrain.IsCoolingDown  || EnergyDrain.ElapsedAfter(50))) && SummonBahamut.ElapsedOneChargeAfterGCD(1)) ||
            !SearingLight.EnoughLevel || (HostileTarget.IsBoss() || HostileTarget.IsDying())) && Fester.CanUse(out act)) return true;

        //energy siphon recharge
        if (AetherCharge.CurrentCharges==0 && EnergySiphon.CanUse(out act)) return true;
        //energy drain recharge
        if (AetherCharge.CurrentCharges==0 && EnergyDrain.CanUse(out act)) return true;
        

        return false;
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;
                
        // Adding tincture timing to rotations
        if(((SearingLight.IsInCooldown || Player.HasStatus(false,StatusID.SearingLight)) && InBahamut) && (UseBurstMedicine(out act))) return true;
        
        // moved swift usage on emergency to avoid unsended swift
        switch (Configs.GetCombo("addSwiftcast"))
        {
            default:
                break;
            case 1:
                if (InGaruda && Player.Level > 86)
                {
                    if(Swiftcast.CanUse(out act, CanUseOption.MustUse)) return true;
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
        if (Configs.GetBool("RadiantOnCooldown") && Player.Level < 88 && SummonBahamut.IsCoolingDown && RadiantAegis.CanUse(out act, CanUseOption.MustUse)) return true;

        return base.EmergencyAbility(nextGCD, out act);
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
