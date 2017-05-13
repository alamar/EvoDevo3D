int MORULA_AGE = 10;
int BLASTULA_AGE = 30;
int GASTRULA_AGE = 50;

int ECTODERM = 1;
int ENDODERM = 2;
int MOUTH = 3;
int CONTROL = 4;

int mixedLinksCount = 0;
bool linkedControl = (cellType == CONTROL);
foreach (Cell cell in linkedCells) {
    linkedControl |= (cell.cellType == CONTROL);
    if (cell.cellType != cellType)
    {
        mixedLinksCount++;
    }
}
bool mixedLinks = mixedLinksCount > 2;

if (state == "default")
{
    state = rnd > 0.5 ? "immigration" : "invagination";
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
    if (state == "immigration" ? (rnd < 0.45) : (sensorReaction[CONTROL] > 0.01))
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
            MoveGradient(ECTODERM, true, true, -0.2);
            break;
        case 2:
            Vector outside = Vector.Invert(simulation.GetGradient(position, ECTODERM)).Normalize(1.0);
            bool coveredByEctoderm = false;
            foreach (Cell cell in surroundingCells)
            {
                Vector difference = (cell.position - position).Normalize(1.0) - outside;
                if (cell.cellType == ECTODERM && difference.Length < 1.0)
                {
                     coveredByEctoderm = true;
                     break;
                }
            }
            if (!coveredByEctoderm) {
                MoveGradient(ECTODERM, true, true, 1.0);
            }
            break;
    }
} else if (step == GASTRULA_AGE) {
    if (state == "immigration" && !linkedControl)
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
} else if (step == (GASTRULA_AGE + 1)) {
    if (mixedLinks && cellType == ECTODERM)
    {
        DeSpill(ECTODERM);
        DeSpill(ENDODERM);
        Spill(MOUTH);
        cellType = MOUTH;
    }
} else {
    if (cellType == MOUTH)
    {
        MoveToTheCrowd(true, 0.5);
    } else {
        MoveGradient(cellType, true, true, -0.1);
    }
}
