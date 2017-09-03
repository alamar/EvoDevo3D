bool CONTROL_CELL = true, SMASH = false;
double MAX_LINK_LENGTH = 5.0;
int MAX_LINKS = 10;

int STEM = 0;
int ECTODERM = 1;
int ENDODERM = 2;
int MOUTH = 3;
int PREMOUTH = 33;
int CONTROL = 4;
int REMERGE = 6;

ProteinPenetration(ECTODERM, 0.99);
//ProteinPenetration(MOUTH, 0.99);
ProteinPenetration(REMERGE, 0.999);

movingSpeed = 0.1 + 0.2 * rnd;

radius = 1.5;

if (cellType != CONTROL)
    DeSpillAll();

if (cellType == PREMOUTH)
    cellType = MOUTH;

if (SMASH && step == 80)
{
    BreakFree();
    this.position = this.position + new Vector(rnd * 100 - 50, rnd * 100 - 50, rnd * 20 - 10);
    cellType = ECTODERM;
}

int nearCells = 0;

foreach (Cell cell in surroundingCells)
{
    if ((position - cell.position).mod < MAX_LINK_LENGTH)
        nearCells++;
}

if (cellType == ECTODERM)
{
    if (P(0.01) && sensorReaction[CONTROL] < Simulation.ALMOST_ZERO &&
        linkedCells.Count == MAX_LINKS && sensorReaction[ENDODERM] < 0.1 &&
        sensorReaction[REMERGE] < 4.0)
    {
        cellType = MOUTH;
        Spill(MOUTH);
        return;
    }

    if (sensorReaction[CONTROL] > 0.025 ||
        (P(0.2) && (sensorReaction[CONTROL] < Simulation.ALMOST_ZERO) &&
            (sensorReaction[MOUTH] > 0.1 || sensorReaction[ENDODERM] > 0.1) &&
            (sensorReaction[ENDODERM] < 13)))
    {
        cellType = ENDODERM;
        Spill(ENDODERM);
        return;
    }

    bool remerge = (nearCells < 5);
    Spill(remerge ? REMERGE : ECTODERM);
    MoveGradient(ECTODERM, true, remerge || sensorReaction[REMERGE] > 4.0, 0.25);
}

if (cellType == ENDODERM)
	{
    Vector outside = Vector.Invert(simulation.GetGradient(position, ECTODERM)).Normalize(1.0);
    bool coveredByEctoderm = false;
    Vector closest = null;
    foreach (Cell cell in surroundingCells)
    {
        Vector difference = (cell.position - position).Normalize(1.0) - outside;
        if (cell.cellType == ECTODERM)
        {
            if (difference.Length < 1.0)
            {
                coveredByEctoderm = true;
                break;
            }
            else if (difference.Length < 2.0)
            {
                closest = difference;
            }
        }
    }
    if (!coveredByEctoderm)
    {
        if (closest != null)
            Move(closest, 0.3, true);
        else
            MoveGradient(ECTODERM, true, true, 0.05);
    }
    Spill(ENDODERM);
}


if (sensorReaction[REMERGE] < 4.0 && P(1.0 / (2.0 * (1.0 + linkedCells.Count))) && linkedCells.Count < MAX_LINKS && sensorReaction[ECTODERM] > Simulation.ALMOST_ZERO)
{
    foreach (Cell cell in surroundingCells)
    {
        if (!linkedCells.Contains(cell) && (cellType == cell.cellType || cellType == CONTROL || cell.cellType == CONTROL))
        {
            LinkTo(cell);

            break;
        }
    }
}

if (cellType == MOUTH) {
    Spill(MOUTH);

    //MoveGradient(MOUTH, true, true, 0.3);
}

if ((sensorReaction[MOUTH] > Simulation.ALMOST_ZERO || sensorReaction[CONTROL] > Simulation.ALMOST_ZERO) && cellType != MOUTH) {
    bool linkedEcto = false, linkedEndo = false, linkedMouth = false;

    foreach (Cell cell in linkedCells) {
        if (cell.cellType == ECTODERM)
            linkedEcto = true;
        if (cell.cellType == ENDODERM)
            linkedEndo = true;
        if (cell.cellType == MOUTH)
            linkedMouth = true;
    }

    if (linkedEcto && linkedEndo) {
        if ((linkedMouth || sensorReaction[CONTROL] > Simulation.ALMOST_ZERO)
            && linkedEcto && linkedEndo)
        {
            cellType = PREMOUTH;
            return;
        } else {
            foreach (Cell cell in linkedCells.Copy()) {
                if (cell.cellType != cellType) {
                    UnlinkFrom(cell);
                }
            }
        }
    }
}

if (cellType == CONTROL) {
    if (EnvironmentalAccess < 0.15)
        MoveFromTheCrowd(false, 0.15);

    if (linkedCells.Count >= 7 && sensorReaction[ENDODERM] < Simulation.ALMOST_ZERO)
        Spill(CONTROL);
}

if (cellType == STEM) {
    if (sensorReaction[STEM] > 200 || sensorReaction[ECTODERM] > Simulation.ALMOST_ZERO)
        cellType = ECTODERM;
    else if (P(0.1))
        SpawnWherever();

    Spill(STEM);
}

if (CONTROL_CELL && step == 0) {
    SpawnWherever();
    cellType = CONTROL;
}
