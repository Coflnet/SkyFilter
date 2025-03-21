namespace Coflnet.Sky.Filter;

[FilterDescription("Dugneon skill requirement filter, ie floor dropped")]
public class DungeonSkillReqFilter : NBTFilter
{
    protected override string PropName => "dungeon_skill_req";
}