public enum ItemType
{
    NONE,
    EQUIP = 1,
    USABLE = 2,
    MATERIAL = 3,
    QUEST = 4,
    COUNT
}

public enum ItemSubType 
{
    EQUIP_START = 1,
    WEAPON = EQUIP_START,
    ARMOR = 2,
    EQUIP_END,

    USABLE_START = 1,
    ADD_HP = USABLE_START,
    ADD_MP = 2,
    USABLE_END,

    MATERIAL_START = 1,
    MATERIAL_COMBINE = MATERIAL_START,
    MATERIAL_END,

    QUEST_START = 1,
    QUEST_ONE = QUEST_START,
    QUEST_TWO = 2,
    QUEST_END
}