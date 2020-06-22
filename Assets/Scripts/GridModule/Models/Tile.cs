namespace GridModule.Models
{
    public struct Tile
    {
        public int Width;
        public int Height;

        public int Square => Width * Height;

        public override string ToString()
        {
            return $"[{Width}, {Height}]";
        }

        public string GetStringHash()
        {
            return $"{Width}X{Height}";
        }

        public bool isVertical => Height > Width;
    }
}