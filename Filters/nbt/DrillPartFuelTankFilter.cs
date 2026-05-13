namespace Coflnet.Sky.Filter;
[FilterDescription("Fuel tank of the drill")]
public class DrillPartFuelTankFilter : NBTItemFilter
{
    protected override string PropName => "fuel_tank.id";
    protected override System.Collections.Generic.IEnumerable<string> AlternatePropNames => new[] { "drill_part_fuel_tank" };
}

