namespace Coflnet.Sky.Filter;
[FilterDescription("Upgrade module on a drill")]
public class DrillPartUpgradeModuleFilter : NBTItemFilter
{
    protected override string PropName => "upgrade_module.id";
    protected override System.Collections.Generic.IEnumerable<string> AlternatePropNames => new[] { "drill_part_upgrade_module" };
}

