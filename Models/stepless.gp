int STEM = 0;
int ECTODERM = 1;
int ENDODERM = 2;
int MOUTH = 3;
int CONTROL = 4;

int CONTROLG = 4;
int CONTROLD = 5;

double MAX_LINK_LENGTH = 4.0;

if (step == 45) {
    BreakFree();
    this.position = new Vector(rnd * 100 - 50, rnd * 100 - 50, rnd * 20 - 10);
}

foreach (Cell cell in linkedCells.Copy())
{
    if ((position - cell.position).mod > MAX_LINK_LENGTH)
    {
        UnlinkFrom(cell);
    }
}

if (cellType == STEM)
{
    radius = 1.5;
    if (sensorReaction[CONTROLG] < 0.15 && sensorReaction[ECTODERM] < 0.15)
    {
        SpawnWherever();
    }
    else
    {
        cellType = ECTODERM;
    }
    if (step == 0) {
        cellType = CONTROL;
    }
    if (EnvironmentalAccess > 0.5)
    {
        MoveToTheCrowd(true, 1);
    }
}

if (cellType == ECTODERM)
{
    if (sensorReaction[CONTROLD] > 0.1)
    {
        DeSpillAll();
        cellType = ENDODERM;
        Spill(ENDODERM);
        return;
    }
    Spill(ECTODERM);
    if (EnvironmentalAccess < 0.5)
    {
        MoveGradient(ECTODERM, true, true, -0.5);
    }
    int links = linkedCells.Count;
    BreakFree();
    foreach (Cell cell in surroundingCells)
    {
        if ((position - cell.position).mod > MAX_LINK_LENGTH) {
            continue;
        }
        LinkTo(cell);
        if (--links <= 0)
        {
            break;
        }
    }
}

if (cellType == ENDODERM) {
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
    if (!coveredByEctoderm)
    {
        MoveGradient(ECTODERM, true, true, 1.0);
    }
}

if (cellType == CONTROL && EnvironmentalAccess < 0.15) {
    MoveFromTheCrowd(false, 0.15);
}

if (cellType == CONTROL && step == 11) {
    Spill(CONTROLG);
}

if (cellType == CONTROL && step == 25) {
    DeSpillAll();
    Spill(CONTROLD);
}

if (linkedCells.Count == 0 && cellType != CONTROL)
{
    DeSpillAll();
    cellType = STEM;
}


/*int mixedLinksCount = 0;
bool linkedControl = (cellType == CONTROL);
foreach (Cell cell in linkedCells.Copy()) {
    if ((position - cell.position).mod > 5) {
        UnlinkFrom(cell);
    }
    linkedControl |= (cell.cellType == CONTROL);
    if (cell.cellType != cellType)
    {
        mixedLinksCount++;
    }
}
bool mixedLinks = mixedLinksCount > 2;*/


/*
Interject(45, () => this.position = new Vector(rnd * 100 - 50, rnd * 100 - 50, rnd * 20 - 10));

Stage(MORULA_AGE, () => SpawnWherever());

Stage(BLASTULA_AGE, () =>
{
    if (EnvironmentalAccess > 0.2 && sensorReaction[4] == 0) {
        Spill(CONTROL);
        cellType = CONTROL;
    }
    Spill(ECTODERM);
});

Stage(1, () => {
if (cellType == 0)
{
    if (state == "immigration" ? (rnd < 0.45) : (sensorReaction[CONTROL] > 0.01))
    {
        cellType = ENDODERM;
        DeSpillAll();
        Spill(ENDODERM);
    }
    else
    {
        cellType = ECTODERM;
    }
    return;
}
});

Stage(GASTRULA_AGE, () =>
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
            if (!coveredByEctoderm)
            {
                MoveGradient(ECTODERM, true, true, 1.0);
            }
            break;
    }
});

Stage(1, () => {
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
});

Stage(1, () => {
    if (mixedLinks && cellType == ECTODERM)
    {
        DeSpillAll();
        Spill(MOUTH);
        cellType = MOUTH;
    }
});

Stage(10000, () => {
    if (cellType == MOUTH)
    {
        MoveToTheCrowd(true, 0.5);
    } else {
        MoveGradient(cellType, true, true, -0.1);
    }
});
*/
