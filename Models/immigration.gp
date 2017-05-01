int WHEN_DONE = 35;

bool mixedLinks = false;
bool linkedControl = (cellType == 4);
foreach (Cell cell in linkedCells) {
    mixedLinks |= (cell.cellType != cellType);
    linkedControl |= (cell.cellType == 4);
}


if (step < 11)
{
    SpawnWherever();
}
else if (step < 30)
{
    if (step == 11) {
        if (EnvironmentalAccess > 0.2 && sensorReaction[4] == 0) {
            Spill(4);
            cellType = 4;
        }
    }
    Spill(1);
    MoveGradient(1, true, true, age < 20 ? -1.0 : (-1.0 / (double) age));
    
    int links = 0;
    BreakFree();
    foreach (Cell cell in surroundingCells)
    {
        LinkTo(cell);
        if (links++ == ((step - 10) / 2))
        {
            break;
        }
    }
}
else if (step == 30 && cellType == 0)
{
    if (rnd < 0.45)
    {
        cellType = 2;
        DeSpill(1);
    }
    else
    {
        cellType = 1;
    }
}
else if (step < WHEN_DONE) {
    switch (cellType)
    {
        case 1:
            MoveGradient(1, true, true, age < WHEN_DONE ? -0.2 : -0.1);
            break;
        case 2:
            DeSpill(1);
            Spill(2);
            if (step <= WHEN_DONE)
            {
                Spill(2);
                MoveGradient(2, true, true, 1.0);
            }
            else if (step == WHEN_DONE)
            {
                int links = 0;
                BreakFree();
                foreach (Cell cell in surroundingCells)
                {
                    if (cell.cellType == 2) 
                    {
                        LinkTo(cell);
                        if (links++ == 10)
                        {
                            break;
                        }
                    }
                }
            } else {
                MoveGradient(2, true, true, -0.1);
            }
            break;
    }
} else if (step == WHEN_DONE) {
    if (!linkedControl)
    {
        int links = 0;
        BreakFree();
        foreach (Cell cell in surroundingCells)
        {
            if (cell.cellType == cellType) 
            {
                LinkTo(cell);
                if (links++ == 10)
                {
                    break;
                }
            }
        }
    }
} else if (step == (WHEN_DONE + 1)) {
    if (linkedControl && cellType != 4)
    {
        DeSpill(1);
        DeSpill(2);
        Spill(3);
        cellType = 3;
    }
} else {
    MoveGradient(cellType, true, true, -0.1);
}
