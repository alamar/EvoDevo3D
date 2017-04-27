int WHEN_DONE = 70;

bool mixedLinks = false;
foreach (Cell cell in linkedCells) {
    mixedLinks |= (cell.cellType != cellType);
}

switch (cellType)
{
    case 0:
        if (step == 0) {
            radius = 12.7;
        }
        if (step < 11)
        {
            radius *= 0.8;
            SpawnWherever();
        }
        else if (step < 40)
        {
            Spill(1);

            MoveGradient(1, true, true, age < 20 ? -1.0 : (-1.0 / (double) age));
            if (step <= 22)
            {
                int i = 0;
                BreakFree();
                foreach (Cell cell in surroundingCells)
                {
                    LinkTo(cell);
                    if (i++ == (step - 10))
                    {
                        break;
                    }
                }
            }
        } else if (step == 40) {
            if (position.z < -12.0)
            {
                cellType = 2;
                DeSpill(1);
            }
            else
            {
                cellType = 1;
            }
        }
        break;
    case 1:
        /*if (mixedLinks) {
            DeSpill(1);
            Spill(4);
            MoveGradient(4, true, true, 0.8);
            break;
        }*/
        MoveGradient(1, true, true, age < WHEN_DONE ? -0.2 : -0.1);
        break;
    case 2:
        if (step <= WHEN_DONE)
        {
            if (mixedLinks) {
                DeSpill(2);
                Spill(3);
                MoveGradient(3, true, true, -1.0);
                break;
            }
            DeSpill(3);
            Spill(2);
            MoveGradient(1, true, true, 1.0);
        }
        else
        {
            MoveGradient(2, true, true, -0.2);
        }
        break;
}
