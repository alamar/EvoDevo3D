if (cellType==0)
{
	cellType=1;
	if (sensorReaction[0]<0.001)
	{
		Spill(0);
	}
	if (EnvironmentalAccess>0.2)
	{
		SpawnGradient(0,false,0).cellType=1;
	}
}
if (cellType==1)
{
	cellType=2;
	resilience=0.42;
	if (EnvironmentalAccess>0.5 && sensorReaction[0]>0.03)
	{
		SpawnGradient(0,false,0).cellType=1;
	}
	if (sensorReaction[0]<=0.03)
	{
		Spill(2);
	}
	if (EnvironmentalAccess>0&&EnvironmentalAccess<0.6&&sensorReaction[2]>0)
	{
		SpawnWherever().cellType=2;
		SpawnGradient(2,true);
	}
}
if (cellType==2)
{
	cellType=3;
	resilience=0.6;
	if (EnvironmentalAccess>0)
	{
		SpawnGradient(0,false,0.5);
	}
}