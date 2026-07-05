public sealed class HealthDisplay : ScreenSurface // lists out the health of a body's limbs
{
    private const int LABEL_X_OFFSET = 1;
    private List<LimbData> _limbs = [];
    public void SetLimbs(BodyComponent body)
    {
        _limbs.Clear();
        BodyComponent.GetAllLimbs(_limbs, body.RootLimb);
        int y = 0;
        foreach (LimbData limb in _limbs)
        {
            Children.Add(new LimbLabel(Width, y, limb));
            y++;
        }
    }

    public HealthDisplay(int x, int y, int width, int height) : base(width, height)
    {
        Position = (x, y);
    }

    private sealed class LimbLabel : ScreenSurface
    {
        LimbData _limb;

        public override void Render(TimeSpan delta)
        {
            Surface.Clear();
            // display progress
            Surface.Fill(new Rectangle(0, 0, (int)(_limb.Hp / (float)_limb.MaxHp * Width), 1), Color.Transparent, Color.LimeGreen, 0, Mirror.None);

            Surface.Print(LABEL_X_OFFSET, 0, $"{_limb.Name}: {_limb.Hp} / {_limb.MaxHp}", Color.White);

            base.Render(delta);
        }

        public LimbLabel(int width, int y, LimbData limb) : base(width, 1)
        {
            Position = (0, y);
            Surface.DefaultBackground = Color.Red;
            _limb = limb;
        }
    }
}