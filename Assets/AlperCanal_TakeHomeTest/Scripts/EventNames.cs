public static class LoadingEvents
{
    public const string LOADING_STARTED = "LOADING_STARTED";
    public const string LOADING_PROGRESS = "LOADING_PROGRESS";
    public const string LOADING_FINISHED = "LOADING_FINISHED";
}


public static class SaveEvents
{
    public const string SAVE_GAME_STATE = "SAVE_GAME_STATE";
    public const string LOADING_SAVE_COMPLETED = "LOADING_SAVE_COMPLETED";
}

public static class SelectionEvents
{
    public const string HERO_FRAME_SELECTED = "HERO_FRAME_SELECTED";
    public const string HERO_FRAME_DESELECTED = "HERO_FRAME_DESELECTED";

    public const string HERO_FRAME_CLICKED_DOWN = "HERO_FRAME_CLICKED_DOWN";
    public const string HERO_FRAME_CLICKED_UP = "HERO_FRAME_CLICKED_UP";
}

public static class HeroEvents
{
    public const string HERO_ADDED = "HERO_ADDED";
}

public static class SceneEvents
{
    public const string BATTLE_START_SIGNAL = "BATTLE_START_SIGNAL";
    public const string BATTLE_STARTED = "BATTLE_STARTED";

    public const string BATTLE_END_SIGNAL = "BATTLE_END_SIGNAL";
    public const string BATTLE_ENDED = "BATTLE_ENDED";

    public const string REWARDING_COMPLETE = "REWARDING_COMPLETE";

    public const string REWARD_MATCH_COUNT = "REWARD_MATCH_COUNT";
}

public static class FightEvents
{
    public const string ATTACK_SIGNAL_GIVEN = "ATTACK_SIGNAL_GIVEN";
    public const string DAMAGE_DONE = "DAMAGE_DONE";
    public const string ATTACK_COMPLETED = "ATTACK_COMPLETED";

    public const string HERO_DIED = "HERO_DIED";

    public const string ALL_ENEMY_DEAD = "ALL_ENEMY_DEAD";
    public const string ALL_ALLY_DEAD = "ALL_ALLY_DEAD";
}