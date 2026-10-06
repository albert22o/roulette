public static class ColorTypeExtensions
{
    public static ColorMask ToMask(this ColorType color) => color switch
    {
        ColorType.Red => ColorMask.Red,
        ColorType.Black => ColorMask.Black,
        ColorType.Green => ColorMask.Green,
        ColorType.White => ColorMask.White,
        ColorType.Violet => ColorMask.Violet,
        _ => ColorMask.None
    };
}