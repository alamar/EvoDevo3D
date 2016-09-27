color=cellType;
if (cellType==0)
{
	if (numDivisions<12)
	{
		holdingPosition=true;
		SpawnWherever().cellType=1;
	}
	else
	{
		Spill(0);
	}
}
if (cellType==1)
{
	if (sensorReaction[0]>0.03)
	{
		Cell descendant = SpawnGradient(0,true);
		descendant.cellType=2;
	}
	else
	{
		Spill(1);
	}
}
if (cellType==2)
{
	if (sensorReaction[1]>0 && EnvironmentalAccess>0)
	{
		SpawnWherever().cellType=3;
		SpawnWherever().cellType=3;
		cellType=3;
	}
}