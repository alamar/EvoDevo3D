int MORULA_AGE = 10;
int BLASTULA_AGE = 30;
int GASTRULA_AGE = 35;

int ECTODERM = 1;
int ENDODERM = 2;
int MOUTH = 3;
int CONTROL = 4;

bool mixedLinks = false;
bool linkedControl = (cellType == CONTROL);
foreach (Cell cell in linkedCells) {
    mixedLinks |= (cell.cellType != cellType);
    linkedControl |= (cell.cellType == CONTROL);
}


if (step <= MORULA_AGE)
{
    SpawnWherever();
}
else if (step < BLASTULA_AGE)
{
    if (EnvironmentalAccess > 0.2 && sensorReaction[4] == 0) {
        Spill(CONTROL);
        cellType = CONTROL;
    }
    Spill(ECTODERM);
    MoveGradient(ECTODERM, true, true, age < 20 ? -1.0 : (-1.0 / (double) age));
    
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
else if (step == BLASTULA_AGE && cellType == 0)
{
    if (rnd < 0.45)
    {
        cellType = ENDODERM;
        Spill(ENDODERM);
        DeSpill(ECTODERM);
    }
    else
    {
        cellType = ECTODERM;
    }
}
else if (step < GASTRULA_AGE)
{
    switch (cellType)
    {
        case 1:
            MoveGradient(1, true, true, -0.2);
            break;
        case 2:
            MoveGradient(2, true, true, 1.0);
            break;
    }
} else if (step == GASTRULA_AGE) {
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
} else {
    if (linkedControl && (cellType == ECTODERM || cellType == ENDODERM))
    {
        DeSpill(ECTODERM);
        DeSpill(ENDODERM);
        Spill(MOUTH);
        cellType = MOUTH;
    }
    MoveGradient(cellType, true, true, -0.1);
}
