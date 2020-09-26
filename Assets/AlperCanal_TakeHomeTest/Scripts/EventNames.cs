﻿public static class LoadingEvents
{
    public const string LOADING_STARTED = "LOADING_STARTED";
    public const string LOADING_PROGRESS = "LOADING_PROGRESS";
    public const string LOADING_FINISHED = "LOADING_FINISHED";
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
}