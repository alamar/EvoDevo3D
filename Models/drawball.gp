if (numDivisions == 0)
{
    for (double i = 0.01; i < 1.0; i += 0.0298)
    {
      double step = 0.15 * Math.Abs(0.5 - i) + 0.01;
      for (double j = 0; j < 1.0; j += step)
      {
        double z = 2.0 * i - 1.0;
        double t = 2.0 * 3.142 * j;
        double x = Math.Sqrt(1.0 - z * z) * Math.Cos(t);
        double y = Math.Sqrt(1.0 - z * z) * Math.Sin(t);
        SpawnAt(new Vector(x * 15.0, y * 15.0, z * 15.0));
        SpawnAt(new Vector(y * 15.0, z * 15.0, x * 15.0));
      }
    }
    Die();
}