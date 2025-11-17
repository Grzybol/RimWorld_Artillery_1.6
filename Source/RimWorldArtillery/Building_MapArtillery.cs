using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorldArtillery;

public class Building_MapArtillery : Building
{
    private const int WorldRangeTiles = 12;

    private CompPowerTrader? _powerComp;
    private CompRefuelable? _refuelable;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        _powerComp = GetComp<CompPowerTrader>();
        _refuelable = GetComp<CompRefuelable>();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (Faction == null || !Faction.IsPlayer)
        {
            yield break;
        }

        var command = new Command_Action
        {
            defaultLabel = "CommandFireMapArtilleryLabel".Translate(),
            defaultDesc = "CommandFireMapArtilleryDesc".Translate(WorldRangeTiles),
            icon = TexCommand.Attack,
            action = BeginTargeting
        };

        if (_powerComp != null && !_powerComp.PowerOn)
        {
            command.Disable("CommandFireMapArtilleryNoPower".Translate());
        }
        else if (_refuelable != null && !_refuelable.HasFuel)
        {
            command.Disable("CommandFireMapArtilleryNoAmmo".Translate());
        }

        yield return command;
    }

    private void BeginTargeting()
    {
        Find.WorldTargeter.BeginTargeting(TryStrikeTarget, canTargetTiles: false, mouseAttachment: TexCommand.Attack, closeWorldTabWhenFinished: true);
    }

    private bool TryStrikeTarget(GlobalTargetInfo target)
    {
        if (!target.IsValid)
        {
            Messages.Message("CommandFireMapArtilleryInvalidTarget".Translate(), MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }

        if (target.WorldObject is not Settlement settlement)
        {
            Messages.Message("CommandFireMapArtilleryMustTargetSettlement".Translate(), MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }

        if (settlement.Faction == Faction.OfPlayer)
        {
            Messages.Message("CommandFireMapArtilleryCannotTargetColony".Translate(), MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }

        int distance = Find.WorldGrid.TraversalDistanceBetween(Map.Tile, settlement.Tile, passImpassable: true, int.MaxValue);
        if (distance < 0 || distance > WorldRangeTiles)
        {
            Messages.Message("CommandFireMapArtilleryOutOfRange".Translate(WorldRangeTiles), MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }

        DoStrike(settlement);
        return true;
    }

    private void DoStrike(Settlement settlement)
    {
        _refuelable?.ConsumeFuel(1f);
        if (Map != null)
        {
            GenExplosion.DoExplosion(Position, Map, 6f, DamageDefOf.Bomb, instigator: this);
        }

        string letterLabel = "LetterLabelMapArtilleryStrike".Translate(settlement.LabelCap);
        string letterText = "LetterTextMapArtilleryStrike".Translate(settlement.LabelCap);
        Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.PositiveEvent, settlement);

        settlement.Destroy();
    }
}
