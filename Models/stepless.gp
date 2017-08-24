int STEM = 0;
int ECTODERM = 1;
int ENDODERM = 2;
int MOUTH = 3;
int CONTROL = 4;

int CONTROLG = 4;
int CONTROLD = 5;

int REMERGE = 6;

ProteinPenetration(ECTODERM, 0.99);
ProteinPenetration(REMERGE, 0.999);

movingSpeed = 0.1 + 0.2 * rnd;

double MAX_LINK_LENGTH = 5.0;
int MAX_LINKS = 10;

if (step == 80) {
    BreakFree();
    this.position = new Vector(rnd * 100 - 50, rnd * 100 - 50, rnd * 20 - 10);    cellType = ECTODERM;
}

foreach (Cell cell in linkedCells.Copy())
{
    if ((position - cell.position).mod > MAX_LINK_LENGTH)
    {
        UnlinkFrom(cell);
    }
}

int nearCells = 0;

foreach (Cell cell in surroundingCells)
{
    if ((position - cell.position).mod < MAX_LINK_LENGTH)
    {
        nearCells++;
    }
}


if (cellType == STEM)
{
    radius = 1.5;
    if (P(sensorReaction[CONTROLG]) || P(sensorReaction[ECTODERM]))
    {
        cellType = ECTODERM;
    }
    else if (P(1.0 - EnvironmentalAccess))
    {
        Move(position, -0.1, true);
    }
}

if (cellType == ECTODERM)
{
    if (P(sensorReaction[CONTROLD]) || (sensorReaction[CONTROLD] < Simulation.ALMOST_ZERO && linkedCells.Count == MAX_LINKS && sensorReaction[ENDODERM] < 0.5 && sensorReaction[REMERGE] < Simulation.ALMOST_ZERO))
    {
        DeSpillAll();
        cellType = ENDODERM;
        Spill(ENDODERM);
        return;
    }

    DeSpillAll();
    Spill(nearCells < 5 ? REMERGE : ECTODERM);
    MoveGradient(ECTODERM, true, nearCells < 5 || sensorReaction[REMERGE] > 4.0, 0.25);
    if (P(0.2) && sensorReaction[REMERGE] < Simulation.ALMOST_ZERO)
    {
        int links = Math.Min(linkedCells.Count + 1, MAX_LINKS);
        BreakFree();
        foreach (Cell cell in surroundingCells)
        {
            if ((position - cell.position).mod > MAX_LINK_LENGTH)
            {
                continue;
            }
            LinkTo(cell);
            if (--links <= 0)
            {
                break;
            }
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
        MoveGradient(ECTODERM, true, true, 0.8);
    }
}

if (cellType == CONTROL && EnvironmentalAccess < 0.15) {
    MoveFromTheCrowd(false, 0.15);
}

if (cellType == CONTROL && sensorReaction[STEM] > 100) {
    Spill(CONTROLG);
}

if (cellType == CONTROL && step == 140) {
    DeSpillAll();
    Spill(CONTROLD);
}

if (cellType == STEM && P(0.1)) {
    SpawnWherever();
    DeSpillAll();
    Spill(STEM);
}

if (step == 0) {
    SpawnWherever();
    cellType = CONTROL;
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
