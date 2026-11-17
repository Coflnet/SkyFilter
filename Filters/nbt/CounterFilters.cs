namespace Coflnet.Sky.Filter;

[FilterDescription("Kill count recorded by an applied Book of Stats")]
public class StatsBookFilter : NBTNumberFilter
{
    protected override string PropName => "stats_book";
}

[FilterDescription("Boss tier of the item (0-4)")]
public class BossTierFilter : NBTNumberFilter
{
    protected override string PropName => "boss_tier";
}

[FilterDescription("Combat xp gained with the Champion enchantment")]
public class ChampionCombatXpFilter : NBTNumberFilter
{
    protected override string PropName => "champion_combat_xp";
}

[FilterDescription("Dungeon S runs counted by the Hecatomb enchantment")]
public class HecatombRunsFilter : NBTNumberFilter
{
    protected override string PropName => "hecatomb_s_runs";
}

[FilterDescription("Blocks mined counted by the Compact enchantment")]
public class CompactBlocksFilter : NBTNumberFilter
{
    protected override string PropName => "compact_blocks";
}

[FilterDescription("Combat xp gained with the Toxophilite enchantment")]
public class ToxophiliteCombatXpFilter : NBTNumberFilter
{
    protected override string PropName => "toxophilite_combat_xp";
}

[FilterDescription("Seconds the item was held, supports number ranges")]
public class SecondsHeldFilter : NBTNumberFilter
{
    protected override string PropName => "seconds_held";
}

[FilterDescription("Level of the potion")]
public class PotionLevelFilter : NBTNumberFilter
{
    protected override string PropName => "potion_level";
}

[FilterDescription("Type of the potion")]
public class PotionFilter : NBTFilter
{
    protected override string PropName => "potion";
}
