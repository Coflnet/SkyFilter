namespace Coflnet.Sky.Filter;
[FilterDescription("Engine of the drill")]
public class DrillPartEngineFilter : NBTItemFilter
{
    protected override string PropName => "engine.id";
    protected override System.Collections.Generic.IEnumerable<string> AlternatePropNames => new[] { "drill_part_engine" };
}

