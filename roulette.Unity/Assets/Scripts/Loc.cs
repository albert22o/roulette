using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public static class Loc
{
    public const string UiTable = "UI";
    public const string ItemsTable = "Items";

    public static string Get(string key, params object[] args)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(UiTable, key, args);
    }

    public static string Color(ColorType color) => Get("color." + color);

    public static string Resolve(this LocalizedString localizedString)
    {
        return localizedString == null || localizedString.IsEmpty ? string.Empty : localizedString.GetLocalizedString();
    }
}
