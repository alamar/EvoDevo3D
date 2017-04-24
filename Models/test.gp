cellType=cellType;
if (cellType==0)
{
	SpawnPolarization().cellType=1;
	polarization.Turn(0.3);
	if (sensorReaction[0]<0.1)
	{
		Spill(0);
	}
}
if (cellType==1)
{
	if (sensorReaction[0]<0.1)
	{
		cellType=0;
		Spill(0);
		polarization.Turn(1f);
	}
}