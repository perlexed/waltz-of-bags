namespace GridModule.Models
{
    public struct Tile
    {
        public int Width;
        public int Height;

        public override string ToString()
        {
            return $"[{Height}, {Width}]";
        }
    }
}