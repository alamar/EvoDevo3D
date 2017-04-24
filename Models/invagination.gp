int LINKS = 15;

int WHEN_DONE = 70;

switch (cell.color)
{
    case 0:
        if (numDivisions<11)
        {
            SpawnWherever();
        }
        else if (age == 22)
        {
            int i = 0;
            foreach (Cell cell in surroundingCells)
            {
                LinkTo(cell);
                if (i++ == LINKS)
                {
                    break;
                }
            }
        }
        else if (age < 40)
        {
            Spill(1);

            MoveGradient(1, true, true, age < 20 ? -1.0 : (-1.0 / (double) age));
        } else if (age == 40) {
            if (cell.position.z > 12.0)
            {
                cell.color = 2;
                DeSpill(1);
                Spill(2);
            }
            else if (cell.position.z > 10.0)
            {
                cell.color = 3;
                DeSpill(1);
                Spill(3);
            }
            else
            {
                cell.color = 1;
            }
        }
        break;
    case 1:
        MoveGradient(1, true, true, age < WHEN_DONE ? -0.2 : -0.1);
        break;
    case 2:
        if (age <= WHEN_DONE)
        {
            MoveGradient(1, true, true, 1.0);
        }
        else
        {
            MoveGradient(2, true, true, -0.2);
        }
        break;
    case 3:
        MoveGradient(3, true, true, -1.0);
        if (age == WHEN_DONE) {
            cell.color = 2;
        }
        break;
}
