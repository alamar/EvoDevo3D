
Spill(1);
if (numDivisions == 0)
{
    cell.radius = 10;
}
if (numDivisions<10)
{
    cell.radius = cell.radius / 1.26;
    SpawnGradient(1, false, 1);
} else {
    MoveGradient(1, true, true, -1.0);
}
